using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades; // Importamos Entidades
using AorusMarket.Negocio;   // Importamos Negocio

namespace AorusMarket.Formularios
{
    public partial class FrmDashboard : Form
    {
        private ComboBox cmbSucursal;
        private DateTimePicker dtDesde, dtHasta;
        private CyberButton btnFiltrar;

        // ESPECIFICAMOS que son Labels de Windows Forms para evitar ambigüedad con ReaLTaiizor
        private System.Windows.Forms.Label lblTotalVendido, lblCantidadVentas, lblProductoTop;
        private DataGridView dgvVentasRecientes;

        // Instanciamos el Cerebro del Dashboard
        private DashboardNegocio _dashboardNegocio = new DashboardNegocio();

        public FrmDashboard()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Dashboard - Panel de Administración";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("DASHBOARD", new Point(30, 15)));

            int x = 30, y = 80;
            this.Controls.Add(EstiloApp.CrearLabel("SUCURSAL", new Point(x, y)));
            cmbSucursal = EstiloApp.CrearComboBox(new Point(x, y + 20), 220);

            // Llenamos el ComboBox de Sucursales desde la Base de Datos
            CargarComboSucursales();

            this.Controls.Add(cmbSucursal);

            this.Controls.Add(EstiloApp.CrearLabel("DESDE", new Point(x + 240, y)));
            dtDesde = new DateTimePicker
            {
                Location = new Point(x + 240, y + 20),
                Size = new Size(160, 28),
                Format = DateTimePickerFormat.Short,
                // Por defecto, restamos 30 días para mostrar el último mes
                Value = DateTime.Now.AddDays(-30)
            };
            this.Controls.Add(dtDesde);

            this.Controls.Add(EstiloApp.CrearLabel("HASTA", new Point(x + 420, y)));
            dtHasta = new DateTimePicker
            {
                Location = new Point(x + 420, y + 20),
                Size = new Size(160, 28),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now // Hasta hoy
            };
            this.Controls.Add(dtHasta);

            btnFiltrar = EstiloApp.CrearBoton("FILTRAR", new Point(x + 600, y + 18), 150, EstiloApp.RojoNeon);
            btnFiltrar.Click += BtnFiltrar_Click;
            this.Controls.Add(btnFiltrar);

            // ---------- Tarjetas de métricas ----------
            y += 80;
            System.Windows.Forms.Panel tarjetaTotal = CrearTarjeta("TOTAL VENDIDO", "$0.00", new Point(x, y), out lblTotalVendido);
            System.Windows.Forms.Panel tarjetaCantidad = CrearTarjeta("VENTAS REALIZADAS", "0", new Point(x + 260, y), out lblCantidadVentas);
            System.Windows.Forms.Panel tarjetaTop = CrearTarjeta("PRODUCTO MÁS VENDIDO", "-", new Point(x + 520, y), out lblProductoTop);

            this.Controls.Add(tarjetaTotal);
            this.Controls.Add(tarjetaCantidad);
            this.Controls.Add(tarjetaTop);

            // ---------- Grid de ventas recientes ----------
            y += 140;
            this.Controls.Add(EstiloApp.CrearLabel("VENTAS RECIENTES", new Point(x, y)));
            dgvVentasRecientes = new DataGridView
            {
                Location = new Point(x, y + 25),
                Size = new Size(1040, this.ClientSize.Height - y - 55),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvVentasRecientes);
            dgvVentasRecientes.Columns.Add("IdVenta", "Id");
            dgvVentasRecientes.Columns.Add("Fecha", "Fecha");
            dgvVentasRecientes.Columns.Add("Sucursal", "Sucursal");
            dgvVentasRecientes.Columns.Add("Cliente", "Cliente");
            dgvVentasRecientes.Columns.Add("Total", "Total");
            dgvVentasRecientes.Columns.Add("MetodoPago", "Método de Pago");
            dgvVentasRecientes.Columns.Add("Estado", "Estado");
            this.Controls.Add(dgvVentasRecientes);

            // Cargar datos por primera vez al abrir el Dashboard
            EjecutarFiltros();
        }

        // ESPECIFICAMOS System.Windows.Forms.Panel y System.Windows.Forms.Label en la firma del método
        private System.Windows.Forms.Panel CrearTarjeta(string titulo, string valorInicial, Point ubicacion, out System.Windows.Forms.Label lblValor)
        {
            System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel
            {
                Location = ubicacion,
                Size = new Size(240, 100),
                BackColor = EstiloApp.FondoPanel
            };
            System.Windows.Forms.Label lblTitulo = new System.Windows.Forms.Label
            {
                Text = titulo,
                Font = EstiloApp.FuenteLabel,
                ForeColor = EstiloApp.Gris,
                AutoSize = true,
                Location = new Point(15, 15)
            };
            lblValor = new System.Windows.Forms.Label
            {
                Text = valorInicial,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = EstiloApp.RojoNeon,
                AutoSize = true,
                Location = new Point(15, 40)
            };
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblValor);
            return panel;
        }

        // METODO NUEVO: Consulta la BD para llenar el combo de sucursales
        private void CargarComboSucursales()
        {
            var sucursales = _dashboardNegocio.ObtenerSucursalesCombo();

            // Insertamos la opción comodín "0" al principio de la lista
            sucursales.Insert(0, new SucursalCombo() { IdSucursal = 0, Nombre = "Todas las sucursales" });

            cmbSucursal.DataSource = sucursales;
            cmbSucursal.DisplayMember = "Nombre";    // Lo que ve el usuario
            cmbSucursal.ValueMember = "IdSucursal";  // El ID real oculto
              
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            EjecutarFiltros();
        }

        // METODO NUEVO: Orquesta la consulta a la BD y pinta los resultados en las tarjetas y la grilla
        private void EjecutarFiltros()
        {
            // 1. Obtener valores de los filtros
            int idSucursal = Convert.ToInt32(cmbSucursal.SelectedValue);
            DateTime desde = dtDesde.Value;
            DateTime hasta = dtHasta.Value;

            string mensajeError;

            // 2. Pedir Métricas al Negocio
            MetricasDashboard metricas = _dashboardNegocio.ObtenerMetricas(idSucursal, desde, hasta, out mensajeError);

            if (!string.IsNullOrEmpty(mensajeError))
            {
                MessageBox.Show(mensajeError, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Actualizar Tarjetas (Formateando el número como moneda y separador de miles)
            lblTotalVendido.Text = metricas.TotalVendido.ToString("C2");
            lblCantidadVentas.Text = metricas.CantidadVentas.ToString();
            lblProductoTop.Text = string.IsNullOrEmpty(metricas.ProductoMasVendido) ? "-" : metricas.ProductoMasVendido;

            // 4. Pedir Lista de Ventas al Negocio y pintar la Grilla
            dgvVentasRecientes.Rows.Clear();
            var listaVentas = _dashboardNegocio.ObtenerVentasRecientes(idSucursal, desde, hasta);

            foreach (var v in listaVentas)
            {
                dgvVentasRecientes.Rows.Add(
                    v.IdVenta,
                    v.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    v.Sucursal,
                    v.Cliente,
                    v.Total.ToString("C2"),
                    v.MetodoPago,
                    v.Estado
                );
            }
            dgvVentasRecientes.ClearSelection();
        }
    }
}