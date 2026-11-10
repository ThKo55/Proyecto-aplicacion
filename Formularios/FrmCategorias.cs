using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmCategorias : Form
    {
        private CyberTextBox txtNombre, txtDescripcion;
        private DataGridView dgvCategorias;
        private CyberButton btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        private CategoriaNegocio _categoriaNegocio = new CategoriaNegocio();

        public FrmCategorias()
        {
            InitializeComponent();
            this.ClientSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Categorías";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("CATEGORÍAS", new Point(30, 20)));

            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                this.Controls.Add(lbl);
            }

            int x = 30, y = 90;

            AddLabelSobrio("NOMBRE", new Point(x, y));
            txtNombre = EstiloApp.CrearTextBox(new Point(x, y + 20), 300);
            this.Controls.Add(txtNombre);

            AddLabelSobrio("DESCRIPCIÓN", new Point(x + 320, y));
            txtDescripcion = EstiloApp.CrearTextBox(new Point(x + 320, y + 20), 300);
            this.Controls.Add(txtDescripcion);

            y += 60;
            btnGuardar = EstiloApp.CrearBoton("GUARDAR", new Point(x, y), 150, EstiloApp.Verde);
            btnEliminar = EstiloApp.CrearBoton("ELIMINAR", new Point(x + 160, y), 150, EstiloApp.RojoNeon);
            btnLimpiar = EstiloApp.CrearBoton("NUEVO / LIMPIAR", new Point(x + 320, y), 150, EstiloApp.Gris);

            btnLimpiar.Click += (s, e) => LimpiarCampos();
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnEliminar);
            this.Controls.Add(btnLimpiar);

            y += 60;
            dgvCategorias = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvCategorias.Columns.Add("IdCategoria", "Id");
            dgvCategorias.Columns.Add("Nombre", "Nombre");
            dgvCategorias.Columns.Add("Descripcion", "Descripción");
            dgvCategorias.Columns["IdCategoria"].Visible = false;

            dgvCategorias.SelectionChanged += DgvCategorias_SelectionChanged;

            EstiloApp.EstilizarGrid(dgvCategorias);
            this.Controls.Add(dgvCategorias);

            CargarGrilla();
        }

        private void CargarGrilla()
        {
            dgvCategorias.SelectionChanged -= DgvCategorias_SelectionChanged;

            dgvCategorias.Rows.Clear();
            var listaCategorias = _categoriaNegocio.Listar();

            foreach (var item in listaCategorias)
            {
                dgvCategorias.Rows.Add(item.IdCategoria, item.Nombre, item.Descripcion);
            }
            dgvCategorias.ClearSelection();

            dgvCategorias.SelectionChanged += DgvCategorias_SelectionChanged;
        }

        private void DgvCategorias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow == null) return;

            var fila = dgvCategorias.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdCategoria"].Value ?? 0);

            // CORRECCIÓN: Evita el bug del texto en el CyberTextBox
            EstiloApp.EscribirTexto(txtNombre, fila.Cells["Nombre"].Value?.ToString());
            EstiloApp.EscribirTexto(txtDescripcion, fila.Cells["Descripcion"].Value?.ToString());
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            EstiloApp.EscribirTexto(txtNombre, "");
            EstiloApp.EscribirTexto(txtDescripcion, "");
            dgvCategorias.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Categoria nuevaCategoria = new Categoria()
            {
                IdCategoria = idSeleccionado,
                Nombre = EstiloApp.LeerTexto(txtNombre),
                Descripcion = EstiloApp.LeerTexto(txtDescripcion)
            };

            string mensaje;
            bool resultado = _categoriaNegocio.Guardar(nuevaCategoria, out mensaje);

            if (resultado)
            {
                MessageBox.Show("Categoría guardada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrilla();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione una categoría de la tabla", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show("¿Seguro que desea eliminar esta categoría?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (resp == DialogResult.Yes)
            {
                string mensaje;
                bool resultado = _categoriaNegocio.Eliminar(idSeleccionado, out mensaje);

                if (resultado)
                {
                    MessageBox.Show("Categoría eliminada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrilla();
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}