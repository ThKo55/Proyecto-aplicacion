using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls; // IMPORTANTE: Agregado para que reconozca los nuevos controles

namespace AorusMarket.Formularios
{
    public partial class FrmStock : Form
    {
        private ComboBox cmbProducto, cmbSucursal;

        // 1. Cambiamos TextBox por CyberTextBox y Button por CyberButton
        private CyberTextBox txtCantidad, txtPrecio;
        private DataGridView dgvStock;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;

        private int idSeleccionado = 0;

        public FrmStock()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Stock por Sucursal";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("STOCK", new Point(30, 20)));

            int x = 30, y = 90;
            this.Controls.Add(EstiloApp.CrearLabel("PRODUCTO", new Point(x, y)));
            cmbProducto = EstiloApp.CrearComboBox(new Point(x, y + 20), 250);
            this.Controls.Add(cmbProducto);

            this.Controls.Add(EstiloApp.CrearLabel("SUCURSAL", new Point(x + 270, y)));
            cmbSucursal = EstiloApp.CrearComboBox(new Point(x + 270, y + 20), 220);
            this.Controls.Add(cmbSucursal);

            this.Controls.Add(EstiloApp.CrearLabel("CANTIDAD", new Point(x + 510, y)));
            txtCantidad = EstiloApp.CrearTextBox(new Point(x + 510, y + 20), 120);
            this.Controls.Add(txtCantidad);

            this.Controls.Add(EstiloApp.CrearLabel("PRECIO", new Point(x + 650, y)));
            txtPrecio = EstiloApp.CrearTextBox(new Point(x + 650, y + 20), 120);
            this.Controls.Add(txtPrecio);

            y += 60;
            btnNuevo = EstiloApp.CrearBoton("NUEVO", new Point(x, y), 150, EstiloApp.Gris);
            btnGuardar = EstiloApp.CrearBoton("GUARDAR", new Point(x + 160, y), 150, EstiloApp.Verde);
            btnEliminar = EstiloApp.CrearBoton("ELIMINAR", new Point(x + 320, y), 150, EstiloApp.RojoNeon);
            btnLimpiar = EstiloApp.CrearBoton("LIMPIAR", new Point(x + 480, y), 150, EstiloApp.Gris);

            btnNuevo.Click += (s, e) => LimpiarCampos();
            btnLimpiar.Click += (s, e) => LimpiarCampos();
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            this.Controls.Add(btnNuevo);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnEliminar);
            this.Controls.Add(btnLimpiar);

            y += 60;
            dgvStock = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(890, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvStock);
            dgvStock.Columns.Add("IdStock", "Id");
            dgvStock.Columns.Add("Producto", "Producto");
            dgvStock.Columns.Add("Sucursal", "Sucursal");
            dgvStock.Columns.Add("Cantidad", "Cantidad");
            dgvStock.Columns.Add("Precio", "Precio");
            dgvStock.Columns["IdStock"].Visible = false;

            // Pinta de rojo la fila si la cantidad está por debajo del mínimo (RF-12)
            dgvStock.CellFormatting += DgvStock_CellFormatting;
            dgvStock.SelectionChanged += DgvStock_SelectionChanged;

            this.Controls.Add(dgvStock);
        }

        private void DgvStock_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var fila = dgvStock.Rows[e.RowIndex];
            if (fila.Cells["Cantidad"].Value != null &&
                int.TryParse(fila.Cells["Cantidad"].Value.ToString(), out int cantidad) && cantidad < 5)
            {
                fila.DefaultCellStyle.BackColor = EstiloApp.RojoOscuro;
                fila.DefaultCellStyle.ForeColor = EstiloApp.Blanco;
            }
        }

        private void DgvStock_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow == null) return;
            var fila = dgvStock.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdStock"].Value ?? 0);
            txtCantidad.TextButton = fila.Cells["Cantidad"].Value?.ToString();
            txtPrecio.TextButton = fila.Cells["Precio"].Value?.ToString();
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            cmbProducto.SelectedIndex = -1;
            cmbSucursal.SelectedIndex = -1;
            txtCantidad.TextButton = "";
            txtPrecio.TextButton = "";
            dgvStock.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedIndex == -1 || cmbSucursal.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtCantidad.TextButton) || string.IsNullOrWhiteSpace(txtPrecio.TextButton))
            {
                MessageBox.Show("Debe completar todos los campos", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtCantidad.TextButton, out _) || !decimal.TryParse(txtPrecio.TextButton, out _))
            {
                MessageBox.Show("Cantidad y Precio deben ser numéricos", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Stock guardado (falta conectar la base de datos)", "AorusMarket");
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un registro de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var resp = MessageBox.Show("¿Seguro que desea eliminar este registro de stock?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (resp == DialogResult.Yes)
            {
                // TODO: eliminar en la base de datos (StockSucursalDAL)
                MessageBox.Show("Registro eliminado (falta conectar la base de datos)", "AorusMarket");
                LimpiarCampos();
            }
        }
    }
}