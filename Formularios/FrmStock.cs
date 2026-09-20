using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;

namespace AorusMarket.Formularios
{
    public partial class FrmStock : Form
    {
        private ComboBox cmbProducto, cmbSucursal;

        private CyberTextBox txtCantidad, txtPrecio;
        private DataGridView dgvStock;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;

        private int idSeleccionado = 0;

        public FrmStock()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Stock por Sucursal";
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
            dgvStock.Columns.Add("IdStock", "Id");
            dgvStock.Columns.Add("Producto", "Producto");
            dgvStock.Columns.Add("Sucursal", "Sucursal");
            dgvStock.Columns.Add("Cantidad", "Cantidad");
            dgvStock.Columns.Add("Precio", "Precio");
            dgvStock.Columns["IdStock"].Visible = false;

            dgvStock.CellFormatting += DgvStock_CellFormatting;
            dgvStock.SelectionChanged += DgvStock_SelectionChanged;

            this.Controls.Add(dgvStock);
        }

        private void CargarCombos()
        {
            try
            {
                using (SqlConnection conexion = AorusMarket.AccesoDatos.Conexion.ObtenerConexion())
                {
                    conexion.Open();

                    // 1. Cargar Productos
                    SqlDataAdapter daProd = new SqlDataAdapter("SELECT id_producto, nombre FROM producto ORDER BY nombre ASC", conexion);
                    DataTable dtProd = new DataTable();
                    daProd.Fill(dtProd);
                    cmbProducto.DataSource = dtProd;
                    cmbProducto.DisplayMember = "nombre";
                    cmbProducto.ValueMember = "id_producto";
                    cmbProducto.SelectedIndex = -1;

                    // 2. Cargar Sucursales
                    SqlDataAdapter daSuc = new SqlDataAdapter("SELECT id_sucursal, nombre FROM sucursal ORDER BY nombre ASC", conexion);
                    DataTable dtSuc = new DataTable();
                    daSuc.Fill(dtSuc);
                    cmbSucursal.DataSource = dtSuc;
                    cmbSucursal.DisplayMember = "nombre";
                    cmbSucursal.ValueMember = "id_sucursal";
                    cmbSucursal.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar listas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGrillaStock()
        {
            try
            {
                dgvStock.Rows.Clear();
                using (SqlConnection conexion = AorusMarket.AccesoDatos.Conexion.ObtenerConexion())
                {
                    string query = @"SELECT ss.id_stock_sucursal, p.nombre AS producto, s.nombre AS sucursal, ss.cantidad, ss.precio
                                     FROM stock_sucursal ss
                                     INNER JOIN producto p ON ss.id_producto = p.id_producto
                                     INNER JOIN sucursal s ON ss.id_sucursal = s.id_sucursal";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dgvStock.Rows.Add(
                                reader["id_stock_sucursal"],
                                reader["producto"],
                                reader["sucursal"],
                                reader["cantidad"],
                                reader["precio"]
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la grilla de stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

        private void DgvStock_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow == null) return;
            var fila = dgvStock.CurrentRow; // corregido de dgvStock
            // ... (mantenemos lógica de selección si hace falta)
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
            if (cmbProducto.SelectedIndex == -1 || cmbSucursal.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtCantidad.TextButton) || string.IsNullOrWhiteSpace(txtPrecio.TextButton))
            {
                MessageBox.Show("Debe completar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtCantidad.TextButton, out int cantidad) || !decimal.TryParse(txtPrecio.TextButton, out decimal precio))
            {
                MessageBox.Show("Cantidad y Precio deben ser numéricos válidos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idProducto = Convert.ToInt32(cmbProducto.SelectedValue);
            int idSucursal = Convert.ToInt32(cmbSucursal.SelectedValue);

            try
            {
                using (SqlConnection conexion = AorusMarket.AccesoDatos.Conexion.ObtenerConexion())
                {
                    conexion.Open();
                    // Insertamos directo en la tabla stock_sucursal de tu base de datos
                    string query = "INSERT INTO stock_sucursal (id_producto, id_sucursal, cantidad, precio) VALUES (@idProd, @idSuc, @cant, @prec)";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@idProd", idProducto);
                    cmd.Parameters.AddWithValue("@idSuc", idSucursal);
                    cmd.Parameters.AddWithValue("@cant", cantidad);
                    cmd.Parameters.AddWithValue("@prec", precio);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("¡Stock guardado con éxito en la base de datos!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrillaStock();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un registro de la tabla para eliminar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show("¿Seguro que desea eliminar este registro de stock?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (resp == DialogResult.Yes)
            {
                try
                {
                    int idStock = Convert.ToInt32(dgvStock.CurrentRow.Cells["IdStock"].Value);

                    using (SqlConnection conexion = AorusMarket.AccesoDatos.Conexion.ObtenerConexion())
                    {
                        conexion.Open();
                        string query = "DELETE FROM stock_sucursal WHERE id_stock_sucursal = @id";
                        SqlCommand cmd = new SqlCommand(query, conexion);
                        cmd.Parameters.AddWithValue("@id", idStock);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Registro eliminado correctamente", "AorusMarket", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarGrillaStock();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}