using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Negocio;
using AorusMarket.Utilidades;

namespace AorusMarket.Formularios
{
    public partial class FrmAuditoriaAcciones : Form
    {
        private DateTimePicker dtpDesde, dtpHasta;
        private ComboBox cmbEntidad;
        private DataGridView dgv;

        public FrmAuditoriaAcciones()
        {
            this.ClientSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Auditoría de Acciones";
            this.StartPosition = FormStartPosition.CenterParent;
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("AUDITORÍA DE ACCIONES DEL SISTEMA", new Point(30, 20)));

            int x = 30, y = 80;

            this.Controls.Add(EstiloApp.CrearLabel("FECHA DESDE:", new Point(x, y)));
            dtpDesde = new DateTimePicker
            {
                Location = new Point(x, y + 20),
                Size = new Size(160, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F),
                Value = DateTime.Now.AddDays(-7)
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

            this.Controls.Add(EstiloApp.CrearLabel("ENTIDAD:", new Point(x + 380, y)));
            cmbEntidad = new ComboBox
            {
                Location = new Point(x + 380, y + 20),
                Size = new Size(180, 28),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbEntidad.Items.AddRange(new object[]
            {
                "(Todas)", "Sesión", "Venta", "Usuario", "Cliente",
                "Producto", "Categoría", "Sucursal", "Stock", "Backup"
            });
            cmbEntidad.SelectedIndex = 0;
            this.Controls.Add(cmbEntidad);

            var btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(x + 590, y + 10), 140, EstiloApp.Verde);
            btnBuscar.Click += (s, e) => CargarGrilla();
            this.Controls.Add(btnBuscar);

            y += 60;
            dgv = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            dgv.Columns.Add("Fecha", "Fecha y Hora");
            dgv.Columns.Add("Usuario", "Usuario");
            dgv.Columns.Add("Accion", "Acción");
            dgv.Columns.Add("Entidad", "Entidad");
            dgv.Columns.Add("Detalle", "Detalle");

            EstiloApp.EstilizarGrid(dgv);
            this.Controls.Add(dgv);

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
                string entidad = cmbEntidad.SelectedIndex <= 0 ? null : cmbEntidad.SelectedItem.ToString();
                DataTable tabla = Auditoria.Listar(dtpDesde.Value, dtpHasta.Value, entidad);

                dgv.Rows.Clear();
                foreach (DataRow r in tabla.Rows)
                {
                    dgv.Rows.Add(
                        Convert.ToDateTime(r["Fecha"]).ToString("dd/MM/yyyy HH:mm:ss"),
                        r["Usuario"], r["Accion"], r["Entidad"], r["Detalle"]);
                }
                dgv.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar la auditoría: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}