using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades; // Importamos Entidades
using AorusMarket.Negocio;   // Importamos Negocio

namespace AorusMarket.Formularios
{
    public partial class FrmCategorias : Form
    {
        private CyberTextBox txtNombre, txtDescripcion;
        private DataGridView dgvCategorias;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        // Instanciamos la capa de negocio
        private CategoriaNegocio _categoriaNegocio = new CategoriaNegocio();

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

            // Definimos las columnas
            dgvCategorias.Columns.Add("IdCategoria", "Id");
            dgvCategorias.Columns.Add("Nombre", "Nombre");
            dgvCategorias.Columns.Add("Descripcion", "Descripción");
            dgvCategorias.Columns["IdCategoria"].Visible = false;

            dgvCategorias.SelectionChanged += DgvCategorias_SelectionChanged;
            this.Controls.Add(dgvCategorias);

            // Cargar los datos desde la BD al abrir el formulario
            CargarGrilla();
        }

        // METODO NUEVO: Consulta a la Capa de Negocio y llena la tabla
        private void CargarGrilla()
        {
            dgvCategorias.Rows.Clear();
            var listaCategorias = _categoriaNegocio.Listar();

            foreach (var item in listaCategorias)
            {
                dgvCategorias.Rows.Add(item.IdCategoria, item.Nombre, item.Descripcion);
            }
            dgvCategorias.ClearSelection();
        }

        private void DgvCategorias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow == null) return;
            var fila = dgvCategorias.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdCategoria"].Value ?? 0);

            // CyberTextBox usa TextButton en lugar de Text
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
            // 1. Armar el objeto Entidad con los datos del Formulario
            Categoria nuevaCategoria = new Categoria()
            {
                IdCategoria = idSeleccionado, // Si es 0 es nuevo, si es > 0 es edición
                Nombre = txtNombre.TextButton,
                Descripcion = txtDescripcion.TextButton
            };

            // 2. Mandar el objeto a la Capa de Negocio
            string mensaje;
            bool resultado = _categoriaNegocio.Guardar(nuevaCategoria, out mensaje);

            // 3. Evaluar el resultado y refrescar la pantalla
            if (resultado)
            {
                MessageBox.Show("Categoría guardada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrilla(); // Recarga la grilla para mostrar los cambios
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
                MessageBox.Show("Seleccione una categoría de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show("¿Seguro que desea eliminar esta categoría?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (resp == DialogResult.Yes)
            {
                // 1. Enviar el ID a la capa de Negocio
                string mensaje;
                bool resultado = _categoriaNegocio.Eliminar(idSeleccionado, out mensaje);

                // 2. Evaluar el resultado
                if (resultado)
                {
                    MessageBox.Show("Categoría eliminada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrilla(); // Recarga la grilla para que desaparezca la fila
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}