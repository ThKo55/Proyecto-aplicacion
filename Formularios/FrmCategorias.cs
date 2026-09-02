using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls; // IMPORTANTE: Agregado para que reconozca los nuevos controles

namespace AorusMarket.Formularios
{
    public partial class FrmCategorias : Form
    {
        // 1. Cambiamos TextBox por CyberTextBox y Button por CyberButton
        private CyberTextBox txtNombre, txtDescripcion;
        private DataGridView dgvCategorias;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        public FrmCategorias()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Categorías";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("CATEGORÍAS", new Point(30, 20)));

            int x = 30, y = 90;
            this.Controls.Add(EstiloApp.CrearLabel("NOMBRE", new Point(x, y)));
            txtNombre = EstiloApp.CrearTextBox(new Point(x, y + 20), 300);
            this.Controls.Add(txtNombre);

            this.Controls.Add(EstiloApp.CrearLabel("DESCRIPCIÓN", new Point(x + 320, y)));
            txtDescripcion = EstiloApp.CrearTextBox(new Point(x + 320, y + 20), 300);
            this.Controls.Add(txtDescripcion);

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
            dgvCategorias = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(740, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvCategorias);
            dgvCategorias.Columns.Add("IdCategoria", "Id");
            dgvCategorias.Columns.Add("Nombre", "Nombre");
            dgvCategorias.Columns.Add("Descripcion", "Descripción");
            dgvCategorias.Columns["IdCategoria"].Visible = false;
            dgvCategorias.SelectionChanged += DgvCategorias_SelectionChanged;
            this.Controls.Add(dgvCategorias);
        }

        private void DgvCategorias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow == null) return;
            var fila = dgvCategorias.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdCategoria"].Value ?? 0);
            txtNombre.TextButton = fila.Cells["Nombre"].Value?.ToString();
            txtDescripcion.TextButton = fila.Cells["Descripcion"].Value?.ToString();
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            txtNombre.TextButton = "";
            txtDescripcion.TextButton = "";
            dgvCategorias.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.TextButton))
            {
                MessageBox.Show("Debe completar el nombre de la categoría", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Categoría guardada (falta conectar la base de datos)", "AorusMarket");
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione una categoría de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var resp = MessageBox.Show("¿Seguro que desea eliminar esta categoría?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (resp == DialogResult.Yes)
            {
                // TODO: eliminar en la base de datos (CategoriaDAL)
                MessageBox.Show("Categoría eliminada (falta conectar la base de datos)", "AorusMarket");
                LimpiarCampos();
            }
        }
    }
}