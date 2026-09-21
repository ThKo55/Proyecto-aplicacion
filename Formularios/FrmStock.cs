using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades; // Importamos Entidades
using AorusMarket.Negocio;   // Importamos Negocio

namespace AorusMarket.Formularios
{
    public partial class FrmStock : Form
    {
        private ComboBox cmbProducto, cmbSucursal;
        private CyberTextBox txtCantidad, txtPrecio;
        private DataGridView dgvStock;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;

        private int idSeleccionado = 0;

        // Instanciamos la capa de Negocio
        private StockNegocio _stockNegocio = new StockNegocio();

        public FrmStock()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Stock por Sucursal";

            // Construye toda la interfaz visual de tu compañero
            ConstruirInterfaz();

            // Cargamos los datos reales de la base de datos al iniciar
            CargarCombos();
            CargarGrillaStock();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("STOCK", new Point(30, 20)));

            int x = 30, y = 90;
            this.Controls.Add(EstiloApp.CrearLabel("PRODUCTO", new Point(x, y)));
            cmbProducto = EstiloApp.CrearComboBox(new Point(x, y + 20), 250);
            this.Controls.Add(cmbProducto);

            this.Controls.Add(EstiloApp.CrearLabel("SUCURSAL", new Point(x + 270, y)));
            cmbSucursal = EstiloApp.CrearComboBox(new Point(x + 270, y + 20), 220);
            this.Controls.Add(cmbSucursal);

            this.Controls.Add(EstiloApp.CrearLabel("CANTIDAD", new Point(x + 510, y)));
            txtCantidad = EstiloApp.CrearTextBox(new Point(x + 510, y + 20), 120);
            this.Controls.Add(txtCantidad);

            this.Controls.Add(EstiloApp.CrearLabel("PRECIO", new Point(x + 650, y)));
            txtPrecio = EstiloApp.CrearTextBox(new Point(x + 650, y + 20), 120);
            this.Controls.Add(txtPrecio);

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
            dgvStock = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(890, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvStock);

            // Agregamos las columnas necesarias (algunas ocultas para poder editar luego)
            dgvStock.Columns.Add("IdStock", "Id");
            dgvStock.Columns.Add("Producto", "Producto");
            dgvStock.Columns.Add("Sucursal", "Sucursal");
            dgvStock.Columns.Add("Cantidad", "Cantidad");
            dgvStock.Columns.Add("Precio", "Precio");
            dgvStock.Columns.Add("IdProducto", "IdProducto"); // Oculta
            dgvStock.Columns.Add("IdSucursal", "IdSucursal"); // Oculta

            dgvStock.Columns["IdStock"].Visible = false;
            dgvStock.Columns["IdProducto"].Visible = false;
            dgvStock.Columns["IdSucursal"].Visible = false;

            dgvStock.CellFormatting += DgvStock_CellFormatting;
            dgvStock.SelectionChanged += DgvStock_SelectionChanged;

            this.Controls.Add(dgvStock);
        }

        // METODO REFACTORIZADO: Delega la conexión a la capa de Negocio
        private void CargarCombos()
        {
            try
            {
                // Cargar Productos
                cmbProducto.DataSource = _stockNegocio.ObtenerProductos();
                cmbProducto.DisplayMember = "Nombre";
                cmbProducto.ValueMember = "Id";
                cmbProducto.SelectedIndex = -1;

                // Cargar Sucursales
                cmbSucursal.DataSource = _stockNegocio.ObtenerSucursales();
                cmbSucursal.DisplayMember = "Nombre";
                cmbSucursal.ValueMember = "Id";
                cmbSucursal.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar listas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // METODO REFACTORIZADO: Pide la lista a Negocio y la pinta en pantalla
        private void CargarGrillaStock()
        {
            try
            {
                dgvStock.Rows.Clear();
                var listaStock = _stockNegocio.Listar();

                foreach (var item in listaStock)
                {
                    dgvStock.Rows.Add(
                        item.IdStock,
                        item.Producto,
                        item.Sucursal,
                        item.Cantidad,
                        item.Precio,
                        item.IdProducto,
                        item.IdSucursal
                    );
                }
                dgvStock.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la grilla de stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ESTILO VISUAL MANTENIDO INTACTO (Pinta de rojo si stock < 5)
        private void DgvStock_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvStock.Columns[e.ColumnIndex].Name == "Cantidad" && e.Value != null)
            {
                if (int.TryParse(e.Value.ToString(), out int cantidad) && cantidad < 5)
                {
                    dgvStock.Rows[e.RowIndex].DefaultCellStyle.BackColor = EstiloApp.RojoOscuro;
                    dgvStock.Rows[e.RowIndex].DefaultCellStyle.ForeColor = EstiloApp.Blanco;
                }
            }
        }

        // Se corrigió para que al hacer clic en la grilla, los datos suban a los textbox y combos
        private void DgvStock_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow == null) return;
            var fila = dgvStock.CurrentRow;

            idSeleccionado = Convert.ToInt32(fila.Cells["IdStock"].Value ?? 0);
            cmbProducto.SelectedValue = Convert.ToInt32(fila.Cells["IdProducto"].Value ?? 0);
            cmbSucursal.SelectedValue = Convert.ToInt32(fila.Cells["IdSucursal"].Value ?? 0);
            txtCantidad.TextButton = fila.Cells["Cantidad"].Value?.ToString();
            txtPrecio.TextButton = fila.Cells["Precio"].Value?.ToString();
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            cmbProducto.SelectedIndex = -1;
            cmbSucursal.SelectedIndex = -1;
            txtCantidad.TextButton = "";
            txtPrecio.TextButton = "";
            dgvStock.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Extraemos los valores como texto
            string strProd = cmbProducto.SelectedValue?.ToString();
            string strSuc = cmbSucursal.SelectedValue?.ToString();
            string strCant = txtCantidad.TextButton;
            string strPrec = txtPrecio.TextButton;
            int idUsuarioLogueado = SesionActual.IdUsuario; // Para la tabla de auditoría (movimientos)

            // Enviamos todo al Cerebro de Negocio para que valide e inserte
            string mensajeError;
            bool resultado = _stockNegocio.Guardar(strProd, strSuc, strCant, strPrec, idUsuarioLogueado, out mensajeError);

            if (resultado)
            {
                MessageBox.Show("¡Stock guardado con éxito en la base de datos!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrillaStock();
            }
            else
            {
                MessageBox.Show(mensajeError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un registro de la tabla para eliminar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show("¿Seguro que desea eliminar este registro de stock?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (resp == DialogResult.Yes)
            {
                string mensaje;
                bool resultado = _stockNegocio.Eliminar(idSeleccionado, out mensaje);

                if (resultado)
                {
                    MessageBox.Show("Registro eliminado correctamente", "AorusMarket", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrillaStock();
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}