using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Negocio;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;

namespace AorusMarket.Formularios
{
    public partial class FrmAuditoriaGeneral : Form
    {
        private DateTimePicker dtpDesde, dtpHasta;
        private ComboBox cmbEntidad;
        private DataGridView dgvAuditoria;
        private CyberButton btnBuscar;

        public FrmAuditoriaGeneral()
        {
            this.ClientSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Auditoría General del Sistema";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("AUDITORÍA GENERAL DEL SISTEMA", new Point(30, 20)));

            int x = 30, y = 80;

            this.Controls.Add(EstiloApp.CrearLabel("FECHA DESDE:", new Point(x, y)));
            dtpDesde = new DateTimePicker
            {
                Location = new Point(x, y + 20),
                Size = new Size(160, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F),
                Value = DateTime.Now.AddDays(-30)
            };
            this.Controls.Add(dtpDesde);

            this.Controls.Add(EstiloApp.CrearLabel("FECHA HASTA:", new Point(x + 190, y)));
            dtpHasta = new DateTimePicker
            {
                Location = new Point(x + 190, y + 20),
                Size = new Size(160, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F),
                Value = DateTime.Now
            };
            this.Controls.Add(dtpHasta);

            this.Controls.Add(EstiloApp.CrearLabel("ENTIDAD (Opcional):", new Point(x + 380, y)));
            cmbEntidad = new ComboBox
            {
                Location = new Point(x + 380, y + 20),
                Size = new Size(180, 28),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbEntidad.Items.AddRange(new object[]
            {
                "(Todas)", "Stock", "Venta", "Sesión", "Usuario", "Cliente",
                "Producto", "Categoría", "Sucursal", "Backup"
            });
            cmbEntidad.SelectedIndex = 0;
            this.Controls.Add(cmbEntidad);

            btnBuscar = EstiloApp.CrearBoton("BUSCAR REGISTROS", new Point(x + 590, y + 10), 200, EstiloApp.Verde);
            btnBuscar.Click += (s, e) => CargarGrilla();
            this.Controls.Add(btnBuscar);

            y += 60;
            dgvAuditoria = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvAuditoria.Columns.Add("Fecha", "Fecha y Hora");
            dgvAuditoria.Columns.Add("Usuario", "Usuario");
            dgvAuditoria.Columns.Add("Entidad", "Entidad");
            dgvAuditoria.Columns.Add("Accion", "Acción / Operación");
            dgvAuditoria.Columns.Add("Detalle", "Detalle");

            EstiloApp.EstilizarGrid(dgvAuditoria);
            dgvAuditoria.CellDoubleClick += DgvAuditoria_CellDoubleClick;
            this.Controls.Add(dgvAuditoria);

            CargarGrilla();
        }

        private void CargarGrilla()
        {
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                MessageBox.Show("La fecha 'Desde' no puede ser mayor que 'Hasta'.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string entidad = cmbEntidad.SelectedItem.ToString();
                DataTable tabla = Auditoria.ListarUnificada(dtpDesde.Value, dtpHasta.Value, entidad);

                dgvAuditoria.Rows.Clear();
                foreach (DataRow r in tabla.Rows)
                {
                    int index = dgvAuditoria.Rows.Add(
                        Convert.ToDateTime(r["Fecha"]).ToString("dd/MM/yyyy HH:mm:ss"),
                        r["Usuario"], r["Entidad"], r["Accion"], r["Detalle"]);

                    // Colorear automáticamente si es un movimiento de stock
                    string detalle = r["Detalle"].ToString();
                    if (detalle.StartsWith("+"))
                        dgvAuditoria.Rows[index].Cells["Detalle"].Style.ForeColor = Color.MediumSeaGreen;
                    else if (detalle.StartsWith("-"))
                        dgvAuditoria.Rows[index].Cells["Detalle"].Style.ForeColor = EstiloApp.RojoNeon;
                }
                dgvAuditoria.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar la auditoría: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void DgvAuditoria_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string fecha = dgvAuditoria.Rows[e.RowIndex].Cells["Fecha"].Value.ToString();
                string usuario = dgvAuditoria.Rows[e.RowIndex].Cells["Usuario"].Value.ToString();
                string accion = dgvAuditoria.Rows[e.RowIndex].Cells["Accion"].Value.ToString();
                string detalle = dgvAuditoria.Rows[e.RowIndex].Cells["Detalle"].Value.ToString();

                // Abrimos la ventana personalizada y la centramos sobre la principal
                using (FrmDetalleAuditoria modal = new FrmDetalleAuditoria(fecha, usuario, accion, detalle))
                {
                    modal.ShowDialog();
                }
            }
        }



    }
}