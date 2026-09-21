using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmAuditoriaStock : Form
    {
        private DateTimePicker dtpDesde, dtpHasta;
        private DataGridView dgvAuditoria;
        private CyberButton btnBuscar;

        private StockNegocio _stockNegocio = new StockNegocio();

        public FrmAuditoriaStock()
        {
            // InitializeComponent(); // Acordate de mantener esto borrado/comentado
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Auditoría de Movimientos";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("AUDITORÍA DE STOCK", new Point(30, 20)));

            int x = 30, y = 80;

            this.Controls.Add(EstiloApp.CrearLabel("FECHA DESDE:", new Point(x, y)));
            dtpDesde = new DateTimePicker
            {
                Location = new Point(x, y + 20),
                Size = new Size(180, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F),
                Value = DateTime.Now.AddDays(-30) // Por defecto muestra el último mes
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

            btnBuscar = EstiloApp.CrearBoton("BUSCAR MOVIMIENTOS", new Point(x + 440, y + 10), 200, EstiloApp.Verde);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            y += 60;
            dgvAuditoria = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(1000, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvAuditoria);

            dgvAuditoria.Columns.Add("IdMovimiento", "ID");
            dgvAuditoria.Columns.Add("Fecha", "Fecha y Hora");
            dgvAuditoria.Columns.Add("Producto", "Producto");
            dgvAuditoria.Columns.Add("Sucursal", "Sucursal");
            dgvAuditoria.Columns.Add("Tipo", "Operación");
            dgvAuditoria.Columns.Add("Cantidad", "Variación");
            dgvAuditoria.Columns.Add("Usuario", "Usuario Responsable");

            this.Controls.Add(dgvAuditoria);

            CargarGrilla();
        }

        private void CargarGrilla()
        {
            dgvAuditoria.Rows.Clear();
            var lista = _stockNegocio.ListarAuditoria(dtpDesde.Value, dtpHasta.Value);

            foreach (var item in lista)
            {
                dgvAuditoria.Rows.Add(
                    item.IdMovimiento,
                    item.Fecha,
                    item.Producto,
                    item.Sucursal,
                    item.TipoMovimiento,
                    item.CantidadVariacion,
                    item.Usuario
                );
            }

            // Pintar de Verde los Ingresos positivos y de Rojo las salidas/ventas negativas
            foreach (DataGridViewRow fila in dgvAuditoria.Rows)
            {
                if (int.TryParse(fila.Cells["Cantidad"].Value?.ToString(), out int cantidad))
                {
                    if (cantidad > 0)
                    {
                        fila.Cells["Cantidad"].Style.ForeColor = Color.MediumSeaGreen;
                        fila.Cells["Cantidad"].Style.Font = new Font(dgvAuditoria.Font, FontStyle.Bold);
                        fila.Cells["Cantidad"].Value = "+" + cantidad; // Agregar el símbolo +
                    }
                    else if (cantidad < 0)
                    {
                        fila.Cells["Cantidad"].Style.ForeColor = EstiloApp.RojoNeon;
                        fila.Cells["Cantidad"].Style.Font = new Font(dgvAuditoria.Font, FontStyle.Bold);
                    }
                }
            }

            dgvAuditoria.ClearSelection();
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            CargarGrilla();
        }
    }
}