using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmProductos : Form
    {
        private CyberTextBox txtNombre, txtDescripcion;
        private ComboBox cmbCategoria;
        private DataGridView dgvProductos;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        private ProductoNegocio _productoNegocio = new ProductoNegocio();
        private CategoriaNegocio _categoriaNegocio = new CategoriaNegocio();

        public FrmProductos()
        {
            InitializeComponent();

            // FONDO SOBRIO OSCURO
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Productos";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("PRODUCTOS", new Point(30, 20)));

            // Función local para crear labels con texto gris claro (sobrio)
            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                lbl.ForeColor = Color.LightGray;
                this.Controls.Add(lbl);
            }

            int x = 30, y = 90;

            AddLabelSobrio("NOMBRE", new Point(x, y));
            txtNombre = EstiloApp.CrearTextBox(new Point(x, y + 20), 250);
            this.Controls.Add(txtNombre);

            AddLabelSobrio("DESCRIPCIÓN", new Point(x + 270, y));
            txtDescripcion = EstiloApp.CrearTextBox(new Point(x + 270, y + 20), 250);
            this.Controls.Add(txtDescripcion);

            AddLabelSobrio("CATEGORÍA", new Point(x + 540, y));
            cmbCategoria = EstiloApp.CrearComboBox(new Point(x + 540, y + 20), 250);

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
                Size = new Size(840, this.ClientSize.Height - y - 30), // Corregido el error de sintaxis acá
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvProductos.Columns.Add("IdProducto", "Id");
            dgvProductos.Columns.Add("Nombre", "Nombre");
            dgvProductos.Columns.Add("Descripcion", "Descripción");
            dgvProductos.Columns.Add("Categoria", "Categoría");

            dgvProductos.Columns.Add("IdCategoria", "IdCategoria");
            dgvProductos.Columns["IdCategoria"].Visible = false;
            dgvProductos.Columns["IdProducto"].Visible = false;

            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;

            // APLICAMOS EL DISEÑO PREMIUM A LA GRILLA
            AplicarDisenoSobrioGrid(dgvProductos);
            this.Controls.Add(dgvProductos);

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

        private void CargarComboCategorias()
        {
            var listaCategorias = _categoriaNegocio.Listar();
            cmbCategoria.DataSource = listaCategorias;
            cmbCategoria.DisplayMember = "Nombre";
            cmbCategoria.ValueMember = "IdCategoria";
            cmbCategoria.SelectedIndex = -1;
        }

        private void CargarGrilla()
        {
            dgvProductos.SelectionChanged -= DgvProductos_SelectionChanged;

            dgvProductos.Rows.Clear();
            var listaProductos = _productoNegocio.Listar();

            foreach (var item in listaProductos)
            {
                dgvProductos.Rows.Add(item.IdProducto, item.Nombre, item.Descripcion, item.NombreCategoria, item.IdCategoria);
            }
            dgvProductos.ClearSelection();

            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;
        }

        private void DgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null) return;
            var fila = dgvProductos.CurrentRow;

            idSeleccionado = Convert.ToInt32(fila.Cells["IdProducto"].Value ?? 0);

            // CORRECCIÓN: Ahora leemos y escribimos en la propiedad .Text
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();
            txtDescripcion.Text = fila.Cells["Descripcion"].Value?.ToString();

            cmbCategoria.SelectedValue = Convert.ToInt32(fila.Cells["IdCategoria"].Value ?? 0);
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;

            // CORRECCIÓN: Limpiamos la propiedad .Text
            txtNombre.Text = "";
            txtDescripcion.Text = "";

            cmbCategoria.SelectedIndex = -1;
            dgvProductos.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Producto nuevoProducto = new Producto()
            {
                IdProducto = idSeleccionado,
                // CORRECCIÓN: Leemos de la propiedad .Text para mandar a la base de datos
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                IdCategoria = cmbCategoria.SelectedValue != null ? Convert.ToInt32(cmbCategoria.SelectedValue) : 0
            };

            string mensaje;
            bool resultado = _productoNegocio.Guardar(nuevoProducto, out mensaje);

            if (resultado)
            {
                MessageBox.Show("Producto guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione un producto de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show("¿Seguro que desea eliminar este producto?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (resp == DialogResult.Yes)
            {
                string mensaje;
                bool resultado = _productoNegocio.Eliminar(idSeleccionado, out mensaje);

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