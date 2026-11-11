using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
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
        private ComboBox cmbCriterioBusqueda;
        private CyberTextBox txtBusqueda;
        private CyberButton btnBuscar, btnAnular;

        private int idVentaSeleccionada = 0;
        private string estadoSeleccionado = "";

        private readonly VentaNegocio _ventaNegocio = new VentaNegocio();
        private List<VentaHistorial> listaVentasCache = new List<VentaHistorial>();

        public FrmHistorialVentas()
        {
            this.ClientSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Historial de Ventas";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("HISTORIAL DE VENTAS", new Point(30, 20)));

            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
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
                Value = DateTime.Now.AddDays(-7)
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

            btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(x + 440, y + 10), 120, EstiloApp.Gris);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            btnAnular = EstiloApp.CrearBoton("ANULAR VENTA", new Point(x + 580, y + 10), 160, EstiloApp.RojoNeon);
            btnAnular.Click += BtnAnular_Click;
            btnAnular.Visible = (SesionActual.IdPerfil == 1 || SesionActual.IdPerfil == 4);
            this.Controls.Add(btnAnular);

            y += 60;

            AddLabelSobrio("FILTRAR POR:", new Point(x, y));
            cmbCriterioBusqueda = new ComboBox
            {
                Location = new Point(x, y + 20),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCriterioBusqueda.Items.AddRange(new object[] { "Nro de Ticket", "Cliente", "Cajero" });
            cmbCriterioBusqueda.SelectedIndex = 1;
            this.Controls.Add(cmbCriterioBusqueda);

            txtBusqueda = EstiloApp.CrearTextBox(new Point(x + 170, y + 20), 410);
            txtBusqueda.TextChanged += () => TxtBusqueda_TextChanged(null, null);
            this.Controls.Add(txtBusqueda);

            y += 60;

            dgvHistorial = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvHistorial.Columns.Add("IdVenta", "N° Ticket");
            dgvHistorial.Columns.Add("Fecha", "Fecha");
            dgvHistorial.Columns.Add("Cliente", "Cliente");
            dgvHistorial.Columns.Add("Sucursal", "Sucursal");
            dgvHistorial.Columns.Add("Cajero", "Cajero");
            dgvHistorial.Columns.Add("Total", "Total ($)");
            dgvHistorial.Columns.Add("MetodoPago", "Medio de Pago");
            dgvHistorial.Columns.Add("Estado", "Estado");

            // Botón de Detalle en la Grilla
            DataGridViewButtonColumn btnDetalle = new DataGridViewButtonColumn();
            btnDetalle.Name = "ColDetalle";
            btnDetalle.HeaderText = "Acciones";
            btnDetalle.Text = "🔍 Ver Detalle";
            btnDetalle.UseColumnTextForButtonValue = true;
            dgvHistorial.Columns.Add(btnDetalle);

            dgvHistorial.SelectionChanged += DgvHistorial_SelectionChanged;
            dgvHistorial.CellContentClick += DgvHistorial_CellContentClick;

            EstiloApp.EstilizarGrid(dgvHistorial);
            this.Controls.Add(dgvHistorial);

            CargarGrillaBase();
        }

        private void CargarGrillaBase()
        {
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                MessageBox.Show("La fecha 'Desde' no puede ser mayor que 'Hasta'.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idUsuarioFiltro = (SesionActual.IdPerfil == 1 || SesionActual.IdPerfil == 4) ? 0 : SesionActual.IdUsuario;

            // Guardamos en memoria para filtrar en tiempo real sin llamar a SQL
            listaVentasCache = _ventaNegocio.ListarHistorial(dtpDesde.Value, dtpHasta.Value, idUsuarioFiltro);

            ActualizarGrillaVisible(listaVentasCache);
        }

        private void ActualizarGrillaVisible(List<VentaHistorial> lista)
        {
            dgvHistorial.SelectionChanged -= DgvHistorial_SelectionChanged;
            dgvHistorial.Rows.Clear();

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

        private void TxtBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (listaVentasCache == null) return;

            string filtro = EstiloApp.LeerTexto(txtBusqueda).ToLower().Trim();
            string criterio = cmbCriterioBusqueda.SelectedItem.ToString();

            var listaFiltrada = listaVentasCache.Where(v =>
            {
                if (string.IsNullOrEmpty(filtro)) return true;
                switch (criterio)
                {
                    case "Nro de Ticket": return v.IdVenta.ToString().Contains(filtro);
                    case "Cliente": return v.Cliente.ToLower().Contains(filtro);
                    case "Cajero": return v.Cajero.ToLower().Contains(filtro);
                    default: return true;
                }
            }).ToList();

            ActualizarGrillaVisible(listaFiltrada);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            CargarGrillaBase();
        }

        private void DgvHistorial_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHistorial.CurrentRow == null) return;
            var fila = dgvHistorial.CurrentRow;
            idVentaSeleccionada = Convert.ToInt32(fila.Cells["IdVenta"].Value);
            estadoSeleccionado = fila.Cells["Estado"].Value.ToString();
        }

        // Abre el FrmDetalleVenta al hacer clic en el botón de la grilla
        private void DgvHistorial_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvHistorial.Columns[e.ColumnIndex].Name == "ColDetalle")
            {
                var fila = dgvHistorial.Rows[e.RowIndex];
                int idVenta = Convert.ToInt32(fila.Cells["IdVenta"].Value);
                string fecha = fila.Cells["Fecha"].Value.ToString();
                string cliente = fila.Cells["Cliente"].Value.ToString();
                string cajero = fila.Cells["Cajero"].Value.ToString();
                decimal total = Convert.ToDecimal(fila.Cells["Total"].Value);
                string metodoPago = fila.Cells["MetodoPago"].Value.ToString();

                using (FrmDetalleVenta frm = new FrmDetalleVenta(idVenta, fecha, cliente, cajero, total, metodoPago))
                {
                    frm.ShowDialog();
                }
            }
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
                bool resultado = _ventaNegocio.AnularVenta(idVentaSeleccionada, estadoSeleccionado, SesionActual.IdUsuario, out mensaje);

                if (resultado)
                {
                    MessageBox.Show("Venta anulada con éxito. El stock ha sido restituido.", "Operación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarGrillaBase();
                }
                else
                {
                    MessageBox.Show(mensaje, "Error al anular", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}