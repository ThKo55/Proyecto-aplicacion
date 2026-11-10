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
    public partial class FrmStock : Form
    {
        private ComboBox cmbProducto, cmbSucursal;
        private CyberTextBox txtCantidad, txtPrecio, txtStockMinimo, txtBuscar;
        private DataGridView dgvStock;
        private CyberButton btnGuardar, btnEliminar, btnLimpiar, btnBuscar;

        private int idSeleccionado = 0;
        private StockNegocio _stockNegocio = new StockNegocio();

        public FrmStock()
        {
            InitializeComponent();
            this.ClientSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Stock por Sucursal";

            ConstruirInterfaz();
            CargarCombos();
            CargarGrillaStock();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("STOCK", new Point(30, 20)));

            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                this.Controls.Add(lbl);
            }

            AddLabelSobrio("BUSCAR PRODUCTO:", new Point(this.ClientSize.Width - 340, 20));
            txtBuscar = EstiloApp.CrearTextBox(new Point(this.ClientSize.Width - 340, 40), 200);
            txtBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnBuscar_Click(s, e); };
            this.Controls.Add(txtBuscar);

            btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(this.ClientSize.Width - 130, 30), 100, EstiloApp.Gris);
            btnBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            int x = 30, y = 90;

            AddLabelSobrio("PRODUCTO", new Point(x, y));
            cmbProducto = EstiloApp.CrearComboBox(new Point(x, y + 20), 220);
            this.Controls.Add(cmbProducto);

            AddLabelSobrio("SUCURSAL", new Point(x + 240, y));
            cmbSucursal = EstiloApp.CrearComboBox(new Point(x + 240, y + 20), 200);
            this.Controls.Add(cmbSucursal);

            AddLabelSobrio("CANTIDAD", new Point(x + 460, y));
            txtCantidad = EstiloApp.CrearTextBox(new Point(x + 460, y + 20), 100);
            this.Controls.Add(txtCantidad);

            AddLabelSobrio("PRECIO", new Point(x + 580, y));
            txtPrecio = EstiloApp.CrearTextBox(new Point(x + 580, y + 20), 100);
            this.Controls.Add(txtPrecio);

            AddLabelSobrio("STOCK MÍNIMO", new Point(x + 700, y));
            txtStockMinimo = EstiloApp.CrearTextBox(new Point(x + 700, y + 20), 120);
            this.Controls.Add(txtStockMinimo);

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
            dgvStock = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvStock.Columns.Add("IdStock", "Id");
            dgvStock.Columns.Add("Producto", "Producto");
            dgvStock.Columns.Add("Sucursal", "Sucursal");
            dgvStock.Columns.Add("Cantidad", "Cantidad");
            dgvStock.Columns.Add("Precio", "Precio");
            dgvStock.Columns.Add("StockMinimo", "Stock Mín.");
            dgvStock.Columns.Add("IdProducto", "IdProducto");
            dgvStock.Columns.Add("IdSucursal", "IdSucursal");

            dgvStock.Columns["IdStock"].Visible = false;
            dgvStock.Columns["IdProducto"].Visible = false;
            dgvStock.Columns["IdSucursal"].Visible = false;

            dgvStock.CellFormatting += DgvStock_CellFormatting;
            dgvStock.SelectionChanged += DgvStock_SelectionChanged;

            EstiloApp.EstilizarGrid(dgvStock);
            this.Controls.Add(dgvStock);
        }

        private void CargarCombos()
        {
            cmbProducto.DataSource = _stockNegocio.ObtenerProductos();
            cmbProducto.DisplayMember = "Nombre";
            cmbProducto.ValueMember = "Id";
            cmbProducto.SelectedIndex = -1;

            cmbSucursal.DataSource = _stockNegocio.ObtenerSucursales();
            cmbSucursal.DisplayMember = "Nombre";
            cmbSucursal.ValueMember = "Id";
            cmbSucursal.SelectedIndex = -1;
        }

        private void CargarGrillaStock(string filtro = "")
        {
            dgvStock.SelectionChanged -= DgvStock_SelectionChanged;
            dgvStock.Rows.Clear();

            var listaStock = _stockNegocio.Listar();
            if (!string.IsNullOrEmpty(filtro))
                listaStock = listaStock.Where(s => s.Producto.ToLower().Contains(filtro.ToLower())).ToList();

            foreach (var item in listaStock)
            {
                dgvStock.Rows.Add(item.IdStock, item.Producto, item.Sucursal, item.Cantidad, item.Precio, item.StockMinimo, item.IdProducto, item.IdSucursal);
            }
            dgvStock.ClearSelection();
            dgvStock.SelectionChanged += DgvStock_SelectionChanged;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            CargarGrillaStock(EstiloApp.LeerTexto(txtBuscar));
        }

        private void DgvStock_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvStock.Columns[e.ColumnIndex].Name == "Cantidad" && e.Value != null)
            {
                int cantActual = Convert.ToInt32(e.Value);
                int cantMinima = Convert.ToInt32(dgvStock.Rows[e.RowIndex].Cells["StockMinimo"].Value ?? 0);

                if (cantActual <= cantMinima)
                {
                    dgvStock.Rows[e.RowIndex].DefaultCellStyle.BackColor = EstiloApp.RojoOscuro;
                    dgvStock.Rows[e.RowIndex].DefaultCellStyle.ForeColor = EstiloApp.Blanco;
                }
            }
        }

        private void DgvStock_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow == null) return;
            var fila = dgvStock.CurrentRow;

            idSeleccionado = Convert.ToInt32(fila.Cells["IdStock"].Value ?? 0);
            cmbProducto.SelectedValue = Convert.ToInt32(fila.Cells["IdProducto"].Value ?? 0);
            cmbSucursal.SelectedValue = Convert.ToInt32(fila.Cells["IdSucursal"].Value ?? 0);

            EstiloApp.EscribirTexto(txtCantidad, fila.Cells["Cantidad"].Value?.ToString());
            EstiloApp.EscribirTexto(txtPrecio, fila.Cells["Precio"].Value?.ToString());
            EstiloApp.EscribirTexto(txtStockMinimo, fila.Cells["StockMinimo"].Value?.ToString());
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            cmbProducto.SelectedIndex = -1;
            cmbSucursal.SelectedIndex = -1;
            EstiloApp.EscribirTexto(txtCantidad, "");
            EstiloApp.EscribirTexto(txtPrecio, "");
            EstiloApp.EscribirTexto(txtStockMinimo, "");
            EstiloApp.EscribirTexto(txtBuscar, "");
            dgvStock.ClearSelection();
            CargarGrillaStock();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string strProd = cmbProducto.SelectedValue?.ToString();
            string strSuc = cmbSucursal.SelectedValue?.ToString();
            string strCant = EstiloApp.LeerTexto(txtCantidad);
            string strPrec = EstiloApp.LeerTexto(txtPrecio);
            string strMinimo = EstiloApp.LeerTexto(txtStockMinimo);

            if (_stockNegocio.Guardar(strProd, strSuc, strCant, strPrec, strMinimo, SesionActual.IdUsuario, out string mensajeError))
            {
                MessageBox.Show("¡Stock guardado con éxito en la base de datos!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show(mensajeError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0) return;

            if (MessageBox.Show("¿Seguro que desea eliminar este registro de stock?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                if (_stockNegocio.Eliminar(idSeleccionado, out string mensaje))
                {
                    MessageBox.Show("Registro eliminado correctamente", "AorusMarket", MessageBoxButtons.OK, MessageBoxIcon.Information);
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