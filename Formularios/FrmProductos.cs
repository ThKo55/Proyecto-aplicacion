using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls; // IMPORTANTE: Agregado para que reconozca los nuevos controles

namespace AorusMarket.Formularios
{
    public partial class FrmProductos : Form
    {
        // 1. Cambiamos TextBox por CyberTextBox y Button por CyberButton
        private CyberTextBox txtNombre, txtDescripcion;
        private ComboBox cmbCategoria;
        private DataGridView dgvProductos;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        public FrmProductos()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Productos";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("PRODUCTOS", new Point(30, 20)));

            int x = 30, y = 90;
            this.Controls.Add(EstiloApp.CrearLabel("NOMBRE", new Point(x, y)));
            txtNombre = EstiloApp.CrearTextBox(new Point(x, y + 20), 250);
            this.Controls.Add(txtNombre);

            this.Controls.Add(EstiloApp.CrearLabel("DESCRIPCIÓN", new Point(x + 270, y)));
            txtDescripcion = EstiloApp.CrearTextBox(new Point(x + 270, y + 20), 250);
            this.Controls.Add(txtDescripcion);

            this.Controls.Add(EstiloApp.CrearLabel("CATEGORÍA", new Point(x + 540, y)));
            cmbCategoria = EstiloApp.CrearComboBox(new Point(x + 540, y + 20), 250);
            // TODO: cargar categorías reales desde la base
            this.Controls.Add(cmbCategoria);

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
            dgvProductos = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(840, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvProductos);
            dgvProductos.Columns.Add("IdProducto", "Id");
            dgvProductos.Columns.Add("Nombre", "Nombre");
            dgvProductos.Columns.Add("Descripcion", "Descripción");
            dgvProductos.Columns.Add("Categoria", "Categoría");
            dgvProductos.Columns["IdProducto"].Visible = false;
            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;
            this.Controls.Add(dgvProductos);
        }

        private void DgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null) return;
            var fila = dgvProductos.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdProducto"].Value ?? 0);
            txtNombre.TextButton = fila.Cells["Nombre"].Value?.ToString();
            txtDescripcion.TextButton = fila.Cells["Descripcion"].Value?.ToString();
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            txtNombre.TextButton = "";
            txtDescripcion.TextButton = "";
            cmbCategoria.SelectedIndex = -1;
            dgvProductos.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.TextButton) || cmbCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Debe completar Nombre y Categoría", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Producto guardado (falta conectar la base de datos)", "AorusMarket");
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var resp = MessageBox.Show("¿Seguro que desea eliminar este producto?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (resp == DialogResult.Yes)
            {
                // TODO: eliminar en la base de datos (ProductoDAL)
                MessageBox.Show("Producto eliminado (falta conectar la base de datos)", "AorusMarket");
                LimpiarCampos();
            }
        }
    }
}