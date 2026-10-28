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
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        private CategoriaNegocio _categoriaNegocio = new CategoriaNegocio();

        public FrmCategorias()
        {
            InitializeComponent();

            // FONDO SOBRIO OSCURO
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Categorías";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("CATEGORÍAS", new Point(30, 20)));

            // Función local para crear labels con texto gris claro (sobrio)
            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                lbl.ForeColor = Color.LightGray;
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
                Size = new Size(740, this.ClientSize.Height - y - 30), // Corregido el error de sintaxis (- y30)
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvCategorias.Columns.Add("IdCategoria", "Id");
            dgvCategorias.Columns.Add("Nombre", "Nombre");
            dgvCategorias.Columns.Add("Descripcion", "Descripción");
            dgvCategorias.Columns["IdCategoria"].Visible = false;

            dgvCategorias.SelectionChanged += DgvCategorias_SelectionChanged;

            // APLICAMOS EL DISEÑO PREMIUM A LA GRILLA
            AplicarDisenoSobrioGrid(dgvCategorias);
            this.Controls.Add(dgvCategorias);

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
            Categoria nuevaCategoria = new Categoria()
            {
                IdCategoria = idSeleccionado,
                Nombre = txtNombre.TextButton,
                Descripcion = txtDescripcion.TextButton
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