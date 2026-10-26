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
    public partial class FrmHistorialVentas : Form
    {
        private DateTimePicker dtpDesde, dtpHasta;
        private DataGridView dgvHistorial;
        private ComboBox cmbProductosVendidos;
        private CyberButton btnBuscar, btnAnular, btnDescargarFactura;
        private int idVentaSeleccionada = 0;
        private string estadoSeleccionado = "";

        private VentaNegocio _ventaNegocio = new VentaNegocio();

        public FrmHistorialVentas()
        {
            //InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Historial de Ventas";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("HISTORIAL DE VENTAS", new Point(30, 20)));

            int x = 30, y = 80;

            this.Controls.Add(EstiloApp.CrearLabel("FECHA DESDE:", new Point(x, y)));
            dtpDesde = new DateTimePicker
            {
                Location = new Point(x, y + 20),
                Size = new Size(180, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F),
                Value = DateTime.Now.AddDays(-7)
            };
            this.Controls.Add(dtpDesde);

            this.Controls.Add(EstiloApp.CrearLabel("FECHA HASTA:", new Point(x + 220, y)));
            dtpHasta = new DateTimePicker
            {
                Location = new Point(x + 220, y + 20),
                Size = new Size(180, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F),
                Value = DateTime.Now
            };
            this.Controls.Add(dtpHasta);

            btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(x + 440, y + 10), 120, EstiloApp.Gris);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            btnAnular = EstiloApp.CrearBoton("ANULAR VENTA", new Point(x + 580, y + 10), 160, EstiloApp.RojoNeon);
            btnAnular.Click += BtnAnular_Click;
            this.Controls.Add(btnAnular);

            y += 60;

            // CREAMOS EL DESPLEGABLE DE PRODUCTOS
            this.Controls.Add(EstiloApp.CrearLabel("PRODUCTOS DE LA VENTA SELECCIONADA:", new Point(x, y)));
            cmbProductosVendidos = new ComboBox
            {
                Location = new Point(x, y + 20),
                Size = new Size(580, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbProductosVendidos);

            // NUEVO BOTÓN PARA DESCARGAR EL PDF (Al lado del desplegable)
            btnDescargarFactura = EstiloApp.CrearBoton("DESCARGAR PDF", new Point(x + 600, y + 15), 150, EstiloApp.Gris);
            btnDescargarFactura.Click += BtnDescargarFactura_Click;
            this.Controls.Add(btnDescargarFactura);

            y += 60;

            dgvHistorial = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(1000, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvHistorial);

            dgvHistorial.Columns.Add("IdVenta", "N° Ticket");
            dgvHistorial.Columns.Add("Fecha", "Fecha");
            dgvHistorial.Columns.Add("Cliente", "Cliente");
            dgvHistorial.Columns.Add("Sucursal", "Sucursal");
            dgvHistorial.Columns.Add("Cajero", "Cajero");
            dgvHistorial.Columns.Add("Total", "Total ($)");
            dgvHistorial.Columns.Add("MetodoPago", "Medio de Pago");
            dgvHistorial.Columns.Add("Estado", "Estado");

            dgvHistorial.SelectionChanged += DgvHistorial_SelectionChanged;

            this.Controls.Add(dgvHistorial);

            CargarGrilla();
        }

        private void CargarGrilla()
        {
            dgvHistorial.SelectionChanged -= DgvHistorial_SelectionChanged;
            dgvHistorial.Rows.Clear();
            cmbProductosVendidos.Items.Clear();

            int idUsuarioFiltro = Utilidades.SesionActual.IdPerfil == 1 ? 0 : Utilidades.SesionActual.IdUsuario;

            var lista = _ventaNegocio.ListarHistorial(dtpDesde.Value, dtpHasta.Value, idUsuarioFiltro);

            foreach (var item in lista)
            {
                dgvHistorial.Rows.Add(
                    item.IdVenta, item.Fecha, item.Cliente, item.Sucursal,
                    item.Cajero, item.Total, item.MetodoPago, item.Estado
                );
            }

            foreach (DataGridViewRow fila in dgvHistorial.Rows)
            {
                if (fila.Cells["Estado"].Value.ToString() == "Anulada")
                {
                    fila.DefaultCellStyle.ForeColor = Color.IndianRed;
                    fila.DefaultCellStyle.Font = new Font(dgvHistorial.Font, FontStyle.Strikeout);
                }
            }

            dgvHistorial.ClearSelection();
            idVentaSeleccionada = 0;
            dgvHistorial.SelectionChanged += DgvHistorial_SelectionChanged;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        private void DgvHistorial_SelectionChanged(object sender, EventArgs e)
        {
            cmbProductosVendidos.Items.Clear();

            if (dgvHistorial.CurrentRow == null) return;

            var fila = dgvHistorial.CurrentRow;
            idVentaSeleccionada = Convert.ToInt32(fila.Cells["IdVenta"].Value);
            estadoSeleccionado = fila.Cells["Estado"].Value.ToString();

            var detalles = _ventaNegocio.ObtenerDetallesDeVenta(idVentaSeleccionada);

            foreach (var det in detalles)
            {
                string textoCombo = $"{det.Cantidad}x {det.NombreProducto} | Unitario: ${det.PrecioUnitario} | Subtotal: ${det.SubTotal}";
                cmbProductosVendidos.Items.Add(textoCombo);
            }

            if (cmbProductosVendidos.Items.Count > 0)
                cmbProductosVendidos.SelectedIndex = 0;
            else
                cmbProductosVendidos.Items.Add("No hay detalles registrados.");
        }

        private void BtnDescargarFactura_Click(object sender, EventArgs e)
        {
            if (idVentaSeleccionada == 0)
            {
                MessageBox.Show("Seleccione una venta de la grilla para descargar su factura.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fila = dgvHistorial.CurrentRow;
            string fecha = fila.Cells["Fecha"].Value.ToString();
            string cliente = fila.Cells["Cliente"].Value.ToString();
            string cajero = fila.Cells["Cajero"].Value.ToString();
            decimal total = Convert.ToDecimal(fila.Cells["Total"].Value);
            string metodoPago = fila.Cells["MetodoPago"].Value.ToString();

            // Buscamos los productos exactos desde la base de datos
            var detallesBD = _ventaNegocio.ObtenerDetallesDeVenta(idVentaSeleccionada);

            // Generamos el PDF (le pasamos true al final para que lo abra ni bien lo guarda)
            Utilidades.GeneradorFactura.GenerarPDF(idVentaSeleccionada, fecha, cliente, cajero, total, metodoPago, detallesBD, true);
        }

        private void BtnAnular_Click(object sender, EventArgs e)
        {
            if (idVentaSeleccionada == 0)
            {
                MessageBox.Show("Seleccione una venta de la lista para anular.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show($"¿Está seguro que desea ANULAR el Ticket N° {idVentaSeleccionada}?\n\nEsta acción devolverá los productos al stock permanentemente.",
                "Alerta de Seguridad", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            if (resp == DialogResult.Yes)
            {
                string mensaje;
                bool resultado = _ventaNegocio.AnularVenta(idVentaSeleccionada, estadoSeleccionado, Utilidades.SesionActual.IdUsuario, out mensaje);

                if (resultado)
                {
                    MessageBox.Show("Venta anulada con éxito. El stock ha sido restituido.", "Operación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarGrilla();
                }
                else
                {
                    MessageBox.Show(mensaje, "Error al anular", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}