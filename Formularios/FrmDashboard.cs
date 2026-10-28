using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmDashboard : Form
    {
        private ComboBox cmbSucursal;
        private DateTimePicker dtDesde, dtHasta;
        private CyberButton btnFiltrar;

        private System.Windows.Forms.Label lblTotalVendido, lblCantidadVentas, lblProductoTop;
        private DataGridView dgvVentasRecientes;

        private DashboardNegocio _dashboardNegocio = new DashboardNegocio();

        public FrmDashboard()
        {
            InitializeComponent();

            // APLICAMOS FONDO SOBRIO (Gris muy oscuro, casi negro)
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Dashboard - Panel de Administración";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("DASHBOARD", new Point(30, 15)));

            int x = 30, y = 80;

            // Filtros
            System.Windows.Forms.Label lblSucursal = EstiloApp.CrearLabel("SUCURSAL", new Point(x, y));
            lblSucursal.ForeColor = Color.LightGray; // Texto más sobrio
            this.Controls.Add(lblSucursal);

            cmbSucursal = EstiloApp.CrearComboBox(new Point(x, y + 20), 220);
            CargarComboSucursales();
            this.Controls.Add(cmbSucursal);

            System.Windows.Forms.Label lblDesde = EstiloApp.CrearLabel("DESDE", new Point(x + 240, y));
            lblDesde.ForeColor = Color.LightGray;
            this.Controls.Add(lblDesde);

            dtDesde = new DateTimePicker
            {
                Location = new Point(x + 240, y + 20),
                Size = new Size(160, 28),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-30)
            };
            this.Controls.Add(dtDesde);

            System.Windows.Forms.Label lblHasta = EstiloApp.CrearLabel("HASTA", new Point(x + 420, y));
            lblHasta.ForeColor = Color.LightGray;
            this.Controls.Add(lblHasta);

            dtHasta = new DateTimePicker
            {
                Location = new Point(x + 420, y + 20),
                Size = new Size(160, 28),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            this.Controls.Add(dtHasta);

            // Botón conservando tu Rojo Neon
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
            System.Windows.Forms.Label lblVentas = EstiloApp.CrearLabel("VENTAS RECIENTES", new Point(x, y));
            lblVentas.ForeColor = Color.LightGray;
            this.Controls.Add(lblVentas);

            dgvVentasRecientes = new DataGridView
            {
                Location = new Point(x, y + 25),
                Size = new Size(1040, this.ClientSize.Height - y - 55),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvVentasRecientes.Columns.Add("IdVenta", "Id");
            dgvVentasRecientes.Columns.Add("Fecha", "Fecha");
            dgvVentasRecientes.Columns.Add("Sucursal", "Sucursal");
            dgvVentasRecientes.Columns.Add("Cliente", "Cliente");
            dgvVentasRecientes.Columns.Add("Total", "Total");
            dgvVentasRecientes.Columns.Add("MetodoPago", "Método de Pago");
            dgvVentasRecientes.Columns.Add("Estado", "Estado");

            // Aplicamos el diseño sobrio directamente a la grilla
            AplicarDisenoSobrioGrid(dgvVentasRecientes);
            this.Controls.Add(dgvVentasRecientes);

            EjecutarFiltros();
        }

        // Diseño Sobrio para las Tarjetas
        private System.Windows.Forms.Panel CrearTarjeta(string titulo, string valorInicial, Point ubicacion, out System.Windows.Forms.Label lblValor)
        {
            System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel
            {
                Location = ubicacion,
                Size = new Size(240, 100),
                BackColor = Color.FromArgb(28, 28, 28) // Gris oscuro mate (sobrio)
            };

            // Borde lateral rojo sutil para mantener la identidad visual de la marca
            System.Windows.Forms.Panel bordeRojo = new System.Windows.Forms.Panel
            {
                Location = new Point(0, 0),
                Size = new Size(4, 100),
                BackColor = EstiloApp.RojoNeon
            };
            panel.Controls.Add(bordeRojo);

            System.Windows.Forms.Label lblTitulo = new System.Windows.Forms.Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.Gray, // Título apagado para no saturar
                AutoSize = true,
                Location = new Point(15, 15)
            };

            lblValor = new System.Windows.Forms.Label
            {
                Text = valorInicial,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = EstiloApp.RojoNeon, // El valor destaca en rojo
                AutoSize = true,
                Location = new Point(15, 45)
            };

            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblValor);
            return panel;
        }

        // MAGIA VISUAL: Modificamos la grilla para que sea plana, oscura y elegante
        private void AplicarDisenoSobrioGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.FromArgb(18, 18, 18); // Mismo fondo del form
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(40, 40, 40); // Líneas muy sutiles
            grid.RowHeadersVisible = false;

            DataGridViewCellStyle estiloCabecera = new DataGridViewCellStyle();
            estiloCabecera.BackColor = Color.FromArgb(28, 28, 28); // Gris oscuro
            estiloCabecera.ForeColor = Color.LightGray;
            estiloCabecera.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            estiloCabecera.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloCabecera.SelectionBackColor = Color.FromArgb(28, 28, 28);
            grid.ColumnHeadersDefaultCellStyle = estiloCabecera;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;

            DataGridViewCellStyle estiloFila = new DataGridViewCellStyle();
            estiloFila.BackColor = Color.FromArgb(18, 18, 18);
            estiloFila.ForeColor = Color.WhiteSmoke;
            estiloFila.Font = new Font("Segoe UI", 10F);
            estiloFila.SelectionBackColor = EstiloApp.RojoNeon; // Rojo al seleccionar
            estiloFila.SelectionForeColor = Color.White;
            grid.DefaultCellStyle = estiloFila;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 22, 22);
            grid.RowTemplate.Height = 35;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void CargarComboSucursales()
        {
            var sucursales = _dashboardNegocio.ObtenerSucursalesCombo();
            sucursales.Insert(0, new SucursalCombo() { IdSucursal = 0, Nombre = "Todas las sucursales" });

            cmbSucursal.DataSource = sucursales;
            cmbSucursal.DisplayMember = "Nombre";
            cmbSucursal.ValueMember = "IdSucursal";
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            EjecutarFiltros();
        }

        private void EjecutarFiltros()
        {
            int idSucursal = Convert.ToInt32(cmbSucursal.SelectedValue);
            DateTime desde = dtDesde.Value;
            DateTime hasta = dtHasta.Value;

            string mensajeError;

            MetricasDashboard metricas = _dashboardNegocio.ObtenerMetricas(idSucursal, desde, hasta, out mensajeError);

            if (!string.IsNullOrEmpty(mensajeError))
            {
                MessageBox.Show(mensajeError, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblTotalVendido.Text = metricas.TotalVendido.ToString("C2");
            lblCantidadVentas.Text = metricas.CantidadVentas.ToString();
            lblProductoTop.Text = string.IsNullOrEmpty(metricas.ProductoMasVendido) ? "-" : metricas.ProductoMasVendido;

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