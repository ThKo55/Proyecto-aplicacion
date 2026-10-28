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

            // FONDO SOBRIO OSCURO
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Auditoría de Movimientos";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("AUDITORÍA DE STOCK", new Point(30, 20)));

            // Función local para crear labels con texto gris claro (sobrio)
            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                lbl.ForeColor = Color.LightGray;
                this.Controls.Add(lbl);
            }

            int x = 30, y = 80;

            AddLabelSobrio("FECHA DESDE:", new Point(x, y));
            dtpDesde = new DateTimePicker
            {
                Location = new Point(x, y + 20),
                Size = new Size(180, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F),
                Value = DateTime.Now.AddDays(-30) // Por defecto muestra el último mes
            };
            this.Controls.Add(dtpDesde);

            AddLabelSobrio("FECHA HASTA:", new Point(x + 220, y));
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
                Size = new Size(1000, this.ClientSize.Height - y - 30), // Corregido el error de sintaxis (- y30)
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvAuditoria.Columns.Add("IdMovimiento", "ID");
            dgvAuditoria.Columns.Add("Fecha", "Fecha y Hora");
            dgvAuditoria.Columns.Add("Producto", "Producto");
            dgvAuditoria.Columns.Add("Sucursal", "Sucursal");
            dgvAuditoria.Columns.Add("Tipo", "Operación");
            dgvAuditoria.Columns.Add("Cantidad", "Variación");
            dgvAuditoria.Columns.Add("Usuario", "Usuario Responsable");

            // APLICAMOS EL DISEÑO PREMIUM A LA GRILLA
            AplicarDisenoSobrioGrid(dgvAuditoria);
            this.Controls.Add(dgvAuditoria);

            CargarGrilla();
        }

        // --- MAGIA VISUAL: NUEVA GRILLA SOBRIA ---
        private void AplicarDisenoSobrioGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.FromArgb(18, 18, 18);
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(40, 40, 40);
            grid.RowHeadersVisible = false;

            DataGridViewCellStyle estiloCabecera = new DataGridViewCellStyle();
            estiloCabecera.BackColor = Color.FromArgb(28, 28, 28);
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
            estiloFila.SelectionBackColor = Color.FromArgb(220, 53, 69); // Rojo Aorus
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
                        // Mantenemos tu estética pero usando el color directamente para evitar dependencias
                        fila.Cells["Cantidad"].Style.ForeColor = Color.FromArgb(220, 53, 69); // Rojo Aorus
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