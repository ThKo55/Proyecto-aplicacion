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
        private CyberTextBox txtNombre, txtDescripcion, txtBuscar;
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

            // NUEVO BUSCADOR DE PRODUCTOS
            AddLabelSobrio("BUSCAR POR NOMBRE:", new Point(600, 20));
            txtBuscar = EstiloApp.CrearTextBox(new Point(600, 40), 200);
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnBuscar_Click(s, e); };
            this.Controls.Add(txtBuscar);

            btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(810, 30), 100, EstiloApp.Gris);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

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
            dgvProductos.Columns.Add("Nombre", "Nombre");
            dgvProductos.Columns.Add("Descripcion", "Descripción");
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
            {
                listaProductos = listaProductos.Where(p => p.Nombre.ToLower().Contains(filtro.ToLower())).ToList();
            }

            foreach (var item in listaProductos)
            {
                dgvProductos.Rows.Add(item.IdProducto, item.Nombre, item.Descripcion, item.NombreCategoria, item.IdCategoria);
            }
            dgvProductos.ClearSelection();

            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string filtro = EstiloApp.LeerTexto(txtBuscar);
            CargarGrilla(filtro);
        }

        private void DgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null) return;
            var fila = dgvProductos.CurrentRow;

            idSeleccionado = Convert.ToInt32(fila.Cells["IdProducto"].Value ?? 0);

            EstiloApp.EscribirTexto(txtNombre, fila.Cells["Nombre"].Value?.ToString());
            EstiloApp.EscribirTexto(txtDescripcion, fila.Cells["Descripcion"].Value?.ToString());

            cmbCategoria.SelectedValue = Convert.ToInt32(fila.Cells["IdCategoria"].Value ?? 0);
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            EstiloApp.EscribirTexto(txtNombre, "");
            EstiloApp.EscribirTexto(txtDescripcion, "");
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
                Nombre = EstiloApp.LeerTexto(txtNombre),
                Descripcion = EstiloApp.LeerTexto(txtDescripcion),
                IdCategoria = cmbCategoria.SelectedValue != null ? Convert.ToInt32(cmbCategoria.SelectedValue) : 0
            };

            string mensaje;
            bool resultado = _productoNegocio.Guardar(nuevoProducto, out mensaje);

            if (resultado)
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
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}