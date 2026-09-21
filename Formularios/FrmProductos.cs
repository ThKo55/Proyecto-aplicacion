using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades; // Traemos los moldes
using AorusMarket.Negocio;   // Traemos los cerebros

namespace AorusMarket.Formularios
{
    public partial class FrmProductos : Form
    {
        private CyberTextBox txtNombre, txtDescripcion;
        private ComboBox cmbCategoria;
        private DataGridView dgvProductos;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        // Instanciamos los negocios necesarios
        private ProductoNegocio _productoNegocio = new ProductoNegocio();
        private CategoriaNegocio _categoriaNegocio = new CategoriaNegocio(); // Lo usamos para el ComboBox

        public FrmProductos()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Productos";
            ConstruirInterfaz(); // Interfaz visual original de tu compañero
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

            // LLAMADO NUEVO: Llenar el ComboBox con datos reales
            CargarComboCategorias();

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

            // Columna oculta extra para poder seleccionar el combo al hacer clic en la grilla
            dgvProductos.Columns.Add("IdCategoria", "IdCategoria");
            dgvProductos.Columns["IdCategoria"].Visible = false;
            dgvProductos.Columns["IdProducto"].Visible = false;

            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;
            this.Controls.Add(dgvProductos);

            // LLAMADO NUEVO: Llenar la grilla con datos reales
            CargarGrilla();
        }

        // MÉTODO NUEVO: Busca las categorías activas y llena el ComboBox
        private void CargarComboCategorias()
        {
            var listaCategorias = _categoriaNegocio.Listar();
            cmbCategoria.DataSource = listaCategorias;
            cmbCategoria.DisplayMember = "Nombre";      // Lo que ve el usuario
            cmbCategoria.ValueMember = "IdCategoria";   // El ID real que guardaremos en la BD
            cmbCategoria.SelectedIndex = -1;            // Arranca sin nada seleccionado
        }

        // MÉTODO NUEVO: Busca los productos en la BD y los pinta en la tabla
        private void CargarGrilla()
        {
            dgvProductos.Rows.Clear();
            var listaProductos = _productoNegocio.Listar();

            foreach (var item in listaProductos)
            {
                dgvProductos.Rows.Add(item.IdProducto, item.Nombre, item.Descripcion, item.NombreCategoria, item.IdCategoria);
            }
            dgvProductos.ClearSelection();
        }

        private void DgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null) return;
            var fila = dgvProductos.CurrentRow;

            idSeleccionado = Convert.ToInt32(fila.Cells["IdProducto"].Value ?? 0);
            txtNombre.TextButton = fila.Cells["Nombre"].Value?.ToString();
            txtDescripcion.TextButton = fila.Cells["Descripcion"].Value?.ToString();

            // Seleccionamos en el ComboBox la categoría correspondiente al producto
            cmbCategoria.SelectedValue = Convert.ToInt32(fila.Cells["IdCategoria"].Value ?? 0);
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
            // 1. Armamos el paquete con lo ingresado en el form
            Producto nuevoProducto = new Producto()
            {
                IdProducto = idSeleccionado, // Si es 0 es nuevo registro
                Nombre = txtNombre.TextButton,
                Descripcion = txtDescripcion.TextButton,
                IdCategoria = cmbCategoria.SelectedValue != null ? Convert.ToInt32(cmbCategoria.SelectedValue) : 0
            };

            // 2. Se lo enviamos al Cerebro
            string mensaje;
            bool resultado = _productoNegocio.Guardar(nuevoProducto, out mensaje);

            // 3. Evaluamos
            if (resultado)
            {
                MessageBox.Show("Producto guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrilla(); // Refresca la tabla
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
                MessageBox.Show("Seleccione un producto de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show("¿Seguro que desea eliminar este producto?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (resp == DialogResult.Yes)
            {
                // 1. Enviar ID al cerebro
                string mensaje;
                bool resultado = _productoNegocio.Eliminar(idSeleccionado, out mensaje);

                // 2. Evaluar
                if (resultado)
                {
                    MessageBox.Show("Producto eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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