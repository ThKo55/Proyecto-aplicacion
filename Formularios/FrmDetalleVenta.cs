using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Negocio;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;

namespace AorusMarket.Formularios
{
    public partial class FrmDetalleVenta : Form
    {
        private int idVenta;
        private string fecha, cliente, cajero, metodoPago;
        private decimal total;

        private DataGridView dgvDetalle;
        private CyberButton btnDescargarPDF;
        private VentaNegocio _ventaNegocio = new VentaNegocio();

        public FrmDetalleVenta(int idVenta, string fecha, string cliente, string cajero, decimal total, string metodoPago)
        {
            this.idVenta = idVenta;
            this.fecha = fecha;
            this.cliente = cliente;
            this.cajero = cajero;
            this.total = total;
            this.metodoPago = metodoPago;

            this.ClientSize = new Size(600, 480);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Detalle del Ticket";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo($"DETALLE DE VENTA N° {idVenta}", new Point(20, 20)));

            Label lblInfo = new Label
            {
                Text = $"📅 Fecha: {fecha}  |  👤 Cliente: {cliente}\n💼 Cajero: {cajero}  |  💳 Pago: {metodoPago}",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10.5F),
                Location = new Point(20, 70),
                AutoSize = true
            };
            this.Controls.Add(lblInfo);

            dgvDetalle = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(540, 260),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            EstiloApp.EstilizarGrid(dgvDetalle);
            this.Controls.Add(dgvDetalle);

            Label lblTotal = new Label
            {
                Text = $"TOTAL: {total:C2}",
                ForeColor = EstiloApp.Verde,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 400),
                AutoSize = true
            };
            this.Controls.Add(lblTotal);

            btnDescargarPDF = EstiloApp.CrearBoton("DESCARGAR PDF", new Point(340, 395), 220, EstiloApp.RojoNeon);
            btnDescargarPDF.Click += BtnDescargarPDF_Click;
            this.Controls.Add(btnDescargarPDF);

            CargarDetalles();
        }

        private void CargarDetalles()
        {
            // Reutiliza tu método actual de VentaNegocio
            var detalles = _ventaNegocio.ObtenerDetallesDeVenta(idVenta);
            dgvDetalle.DataSource = detalles;

            if (dgvDetalle.Columns.Count > 0)
            {
                dgvDetalle.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C2";
                dgvDetalle.Columns["SubTotal"].DefaultCellStyle.Format = "C2";
                dgvDetalle.Columns["NombreProducto"].HeaderText = "Producto";
                dgvDetalle.Columns["PrecioUnitario"].HeaderText = "Precio Unit.";
            }

            dgvDetalle.ClearSelection();
        }

        private void BtnDescargarPDF_Click(object sender, EventArgs e)
        {
            var detallesBD = _ventaNegocio.ObtenerDetallesDeVenta(idVenta);
            GeneradorFactura.GenerarPDF(idVenta, fecha, cliente, cajero, total, metodoPago, detallesBD, true);
        }
    }
}