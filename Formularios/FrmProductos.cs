using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmProductos : Form
    {
        private CyberTextBox txtCodigoBarras, txtNombre, txtMarca, txtDescripcion, txtUnidadMedida, txtBuscar;
        private ComboBox cmbCategoria;
        private DataGridView dgvProductos;
        private CyberButton btnGuardar, btnEliminar, btnLimpiar, btnBuscar;
        private int idSeleccionado = 0;

        private ProductoNegocio _productoNegocio = new ProductoNegocio();
        private CategoriaNegocio _categoriaNegocio = new CategoriaNegocio();

        public FrmProductos()
        {
            InitializeComponent();
            this.ClientSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Productos";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("PRODUCTOS", new Point(30, 20)));

            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                this.Controls.Add(lbl);
            }

            AddLabelSobrio("BUSCAR POR NOMBRE:", new Point(600, 20));
            txtBuscar = EstiloApp.CrearTextBox(new Point(600, 40), 200);
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnBuscar_Click(s, e); };
            this.Controls.Add(txtBuscar);

            btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(810, 30), 100, EstiloApp.Gris);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            int x = 30, y = 90;

            AddLabelSobrio("CÓD. BARRAS", new Point(x, y));
            txtCodigoBarras = EstiloApp.CrearTextBox(new Point(x, y + 20), 150);
            this.Controls.Add(txtCodigoBarras);

            AddLabelSobrio("NOMBRE", new Point(x + 170, y));
            txtNombre = EstiloApp.CrearTextBox(new Point(x + 170, y + 20), 220);
            this.Controls.Add(txtNombre);

            AddLabelSobrio("MARCA", new Point(x + 410, y));
            txtMarca = EstiloApp.CrearTextBox(new Point(x + 410, y + 20), 150);
            this.Controls.Add(txtMarca);

            AddLabelSobrio("CATEGORÍA", new Point(x + 580, y));
            cmbCategoria = EstiloApp.CrearComboBox(new Point(x + 580, y + 20), 200);
            CargarComboCategorias();
            this.Controls.Add(cmbCategoria);

            y += 60;
            AddLabelSobrio("DESCRIPCIÓN", new Point(x, y));
            txtDescripcion = EstiloApp.CrearTextBox(new Point(x, y + 20), 400);
            this.Controls.Add(txtDescripcion);

            AddLabelSobrio("UNIDAD MEDIDA", new Point(x + 420, y));
            txtUnidadMedida = EstiloApp.CrearTextBox(new Point(x + 420, y + 20), 150);
            this.Controls.Add(txtUnidadMedida);

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
            dgvProductos = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvProductos.Columns.Add("IdProducto", "Id");
            dgvProductos.Columns.Add("CodigoBarras", "Cód. Barras");
            dgvProductos.Columns.Add("Nombre", "Nombre");
            dgvProductos.Columns.Add("Marca", "Marca");
            dgvProductos.Columns.Add("Descripcion", "Descripción");
            dgvProductos.Columns.Add("UnidadMedida", "U. Medida");
            dgvProductos.Columns.Add("Categoria", "Categoría");
            dgvProductos.Columns.Add("IdCategoria", "IdCategoria");

            dgvProductos.Columns["IdCategoria"].Visible = false;
            dgvProductos.Columns["IdProducto"].Visible = false;

            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;

            EstiloApp.EstilizarGrid(dgvProductos);
            this.Controls.Add(dgvProductos);

            CargarGrilla();
        }

        private void CargarComboCategorias()
        {
            var listaCategorias = _categoriaNegocio.Listar();
            cmbCategoria.DataSource = listaCategorias;
            cmbCategoria.DisplayMember = "Nombre";
            cmbCategoria.ValueMember = "IdCategoria";
            cmbCategoria.SelectedIndex = -1;
        }

        private void CargarGrilla(string filtro = "")
        {
            dgvProductos.SelectionChanged -= DgvProductos_SelectionChanged;
            dgvProductos.Rows.Clear();

            var listaProductos = _productoNegocio.Listar();
            if (!string.IsNullOrEmpty(filtro))
                listaProductos = listaProductos.Where(p => p.Nombre.ToLower().Contains(filtro.ToLower())).ToList();

            foreach (var item in listaProductos)
            {
                dgvProductos.Rows.Add(item.IdProducto, item.CodigoBarras, item.Nombre, item.Marca, item.Descripcion, item.UnidadMedida, item.NombreCategoria, item.IdCategoria);
            }
            dgvProductos.ClearSelection();
            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            CargarGrilla(EstiloApp.LeerTexto(txtBuscar));
        }

        private void DgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null) return;
            var fila = dgvProductos.CurrentRow;

            idSeleccionado = Convert.ToInt32(fila.Cells["IdProducto"].Value ?? 0);
            EstiloApp.EscribirTexto(txtCodigoBarras, fila.Cells["CodigoBarras"].Value?.ToString());
            EstiloApp.EscribirTexto(txtNombre, fila.Cells["Nombre"].Value?.ToString());
            EstiloApp.EscribirTexto(txtMarca, fila.Cells["Marca"].Value?.ToString());
            EstiloApp.EscribirTexto(txtDescripcion, fila.Cells["Descripcion"].Value?.ToString());
            EstiloApp.EscribirTexto(txtUnidadMedida, fila.Cells["UnidadMedida"].Value?.ToString());

            cmbCategoria.SelectedValue = Convert.ToInt32(fila.Cells["IdCategoria"].Value ?? 0);
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            EstiloApp.EscribirTexto(txtCodigoBarras, "");
            EstiloApp.EscribirTexto(txtNombre, "");
            EstiloApp.EscribirTexto(txtMarca, "");
            EstiloApp.EscribirTexto(txtDescripcion, "");
            EstiloApp.EscribirTexto(txtUnidadMedida, "");
            EstiloApp.EscribirTexto(txtBuscar, "");
            cmbCategoria.SelectedIndex = -1;
            dgvProductos.ClearSelection();
            CargarGrilla();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Producto nuevoProducto = new Producto()
            {
                IdProducto = idSeleccionado,
                CodigoBarras = EstiloApp.LeerTexto(txtCodigoBarras),
                Nombre = EstiloApp.LeerTexto(txtNombre),
                Marca = EstiloApp.LeerTexto(txtMarca),
                Descripcion = EstiloApp.LeerTexto(txtDescripcion),
                UnidadMedida = EstiloApp.LeerTexto(txtUnidadMedida),
                IdCategoria = cmbCategoria.SelectedValue != null ? Convert.ToInt32(cmbCategoria.SelectedValue) : 0
            };

            if (_productoNegocio.Guardar(nuevoProducto, out string mensaje))
            {
                MessageBox.Show("Producto guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0) return;

            if (MessageBox.Show("¿Seguro que desea eliminar este producto?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                if (_productoNegocio.Eliminar(idSeleccionado, out string mensaje))
                {
                    MessageBox.Show("Producto eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}