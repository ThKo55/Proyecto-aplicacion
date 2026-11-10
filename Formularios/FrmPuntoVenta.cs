using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmPuntoVenta : Form
    {
        private CyberTextBox txtBuscar;
        private DataGridView dgvBusqueda;
        private DataGridView dgvCarrito;
        private ComboBox cmbCliente, cmbMetodoPago;
        private Label lblTotal;
        private CyberButton btnAgregar, btnQuitar, btnConfirmar, btnCancelar;

        private StockNegocio _stockNegocio = new StockNegocio();
        private VentaNegocio _ventaNegocio = new VentaNegocio();

        private decimal totalAcumulado = 0;

        public FrmPuntoVenta()
        {
            InitializeComponent();

            // CORRECCIÓN: Evita el desborde al maximizar
            this.ClientSize = new Size(1150, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Punto de Venta";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("PUNTO DE VENTA", new Point(30, 20)));

            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                lbl.ForeColor = Color.LightGray;
                this.Controls.Add(lbl);
            }

            int leftX = 30;

            AddLabelSobrio("BUSCAR PRODUCTO (nombre)", new Point(leftX, 70));

            txtBuscar = EstiloApp.CrearTextBox(new Point(leftX, 90), 200);

            // CORRECCIÓN: Búsqueda ágil con tecla ENTER
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnBuscar_Click(s, e); };

            this.Controls.Add(txtBuscar);

            CyberButton btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(leftX + 210, 89), 100, EstiloApp.Gris);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            btnAgregar = EstiloApp.CrearBoton("AGREGAR AL CARRITO", new Point(leftX + 320, 89), 180, EstiloApp.Verde);
            btnAgregar.Click += BtnAgregar_Click;
            this.Controls.Add(btnAgregar);

            dgvBusqueda = new DataGridView
            {
                Location = new Point(leftX, 140),
                Size = new Size(470, this.ClientSize.Height - 170),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };

            dgvBusqueda.Columns.Add("IdProducto", "IdProducto");
            dgvBusqueda.Columns.Add("Producto", "Producto");
            dgvBusqueda.Columns.Add("Precio", "Precio");
            dgvBusqueda.Columns.Add("Disponible", "Stock Disp.");

            var colCantidad = new DataGridViewTextBoxColumn { Name = "Cantidad", HeaderText = "Cant. a Vender" };
            dgvBusqueda.Columns.Add(colCantidad);
            dgvBusqueda.Columns["IdProducto"].Visible = false;

            foreach (DataGridViewColumn c in dgvBusqueda.Columns)
                if (c.Name != "Cantidad") c.ReadOnly = true;

            EstiloApp.EstilizarGrid(dgvBusqueda);
            this.Controls.Add(dgvBusqueda);

            int rightX = 530;
            int rightWidth = this.ClientSize.Width - rightX - 30;

            AddLabelSobrio("CARRITO DE VENTA", new Point(rightX, 70));

            btnQuitar = EstiloApp.CrearBoton("QUITAR ITEM", new Point(this.ClientSize.Width - 180, 65), 150, EstiloApp.RojoNeon);
            btnQuitar.Click += BtnQuitar_Click;
            btnQuitar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Controls.Add(btnQuitar);

            dgvCarrito = new DataGridView
            {
                Location = new Point(rightX, 110),
                Size = new Size(rightWidth, this.ClientSize.Height - 310),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvCarrito.Columns.Add("IdProducto", "IdProducto");
            dgvCarrito.Columns.Add("Producto", "Producto");
            dgvCarrito.Columns.Add("Cantidad", "Cantidad");
            dgvCarrito.Columns.Add("PrecioUnitario", "Precio Unit.");
            dgvCarrito.Columns.Add("Subtotal", "Subtotal");
            dgvCarrito.Columns["IdProducto"].Visible = false;

            EstiloApp.EstilizarGrid(dgvCarrito);
            this.Controls.Add(dgvCarrito);

            int checkoutY = this.ClientSize.Height - 170;

            AddLabelSobrio("CLIENTE (opcional)", new Point(rightX, checkoutY));
            cmbCliente = EstiloApp.CrearComboBox(new Point(rightX, checkoutY + 20), 240);
            CargarComboClientes();
            cmbCliente.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(cmbCliente);

            Label lblMetodo = EstiloApp.CrearLabel("MÉTODO DE PAGO", new Point(rightX + 260, checkoutY));
            lblMetodo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(lblMetodo);

            cmbMetodoPago = EstiloApp.CrearComboBox(new Point(rightX + 260, checkoutY + 20), 250);
            cmbMetodoPago.Items.AddRange(new object[] { "Efectivo", "Débito", "Crédito", "Transferencia" });
            cmbMetodoPago.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(cmbMetodoPago);

            lblTotal = new Label
            {
                Text = "TOTAL: $0.00",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(rightX, checkoutY + 70),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(lblTotal);

            btnConfirmar = EstiloApp.CrearBoton("CONFIRMAR VENTA", new Point(rightX, checkoutY + 120), 250, EstiloApp.Verde);
            btnConfirmar.Click += BtnConfirmar_Click;
            btnConfirmar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(btnConfirmar);

            btnCancelar = EstiloApp.CrearBoton("CANCELAR", new Point(rightX + 270, checkoutY + 120), 240, EstiloApp.Gris);
            btnCancelar.Click += (s, e) => LimpiarVenta();
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(btnCancelar);
        }

        private void CargarComboClientes()
        {
            var clientes = _ventaNegocio.ObtenerClientesCombo();
            clientes.Insert(0, new Cliente() { IdCliente = 0, Dni = "00000000", Nombre = "Consumidor", Apellido = "Final" });

            cmbCliente.DataSource = clientes;
            cmbCliente.DisplayMember = "InfoBusqueda";
            cmbCliente.ValueMember = "IdCliente";

            cmbCliente.DropDownStyle = ComboBoxStyle.DropDown;
            cmbCliente.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbCliente.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.TextButton))
            {
                MessageBox.Show("Escriba el nombre del producto a buscar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            dgvBusqueda.Rows.Clear();
            var resultados = _ventaNegocio.BuscarProducto(txtBuscar.TextButton, SesionActual.IdSucursal);

            foreach (var r in resultados)
            {
                dgvBusqueda.Rows.Add(r.IdProducto, r.Nombre, r.Precio, r.StockDisponible, "");
            }
            dgvBusqueda.ClearSelection();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvBusqueda.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto de la búsqueda", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fila = dgvBusqueda.CurrentRow;
            string cantIngresadaStr = fila.Cells["Cantidad"].Value?.ToString();

            if (!int.TryParse(cantIngresadaStr, out int cantIngresada) || cantIngresada <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida mayor a 0 en la grilla.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idProd = Convert.ToInt32(fila.Cells["IdProducto"].Value);
            int stockDispGrilla = Convert.ToInt32(fila.Cells["Disponible"].Value);

            int cantidadYaEnCarrito = 0;
            foreach (DataGridViewRow row in dgvCarrito.Rows)
            {
                if (Convert.ToInt32(row.Cells["IdProducto"].Value) == idProd)
                {
                    cantidadYaEnCarrito += Convert.ToInt32(row.Cells["Cantidad"].Value);
                }
            }

            int cantidadTotalDeseada = cantIngresada + cantidadYaEnCarrito;

            if (cantidadTotalDeseada > stockDispGrilla)
            {
                MessageBox.Show($"No hay stock suficiente.\nTienes {stockDispGrilla} disponibles y estás intentando vender un total de {cantidadTotalDeseada}.", "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_stockNegocio.HayStockSuficiente(idProd, SesionActual.IdSucursal, cantidadTotalDeseada))
            {
                MessageBox.Show("El stock en la base de datos ya no es suficiente (quizás otra caja acaba de venderlo).", "Error de Stock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string nombre = fila.Cells["Producto"].Value.ToString();
            decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
            decimal subtotalAgregar = precio * cantIngresada;

            // CORRECCIÓN: En lugar de crear otra fila, actualizamos la cantidad si el producto ya está en el carrito
            bool existe = false;
            foreach (DataGridViewRow row in dgvCarrito.Rows)
            {
                if (Convert.ToInt32(row.Cells["IdProducto"].Value) == idProd)
                {
                    row.Cells["Cantidad"].Value = cantidadTotalDeseada;
                    row.Cells["Subtotal"].Value = cantidadTotalDeseada * precio;
                    existe = true;
                    break;
                }
            }

            if (!existe)
            {
                dgvCarrito.Rows.Add(idProd, nombre, cantIngresada, precio, subtotalAgregar);
            }

            fila.Cells["Cantidad"].Value = "";
            RecalcularTotal();

            if (_stockNegocio.StockEsCritico(idProd, SesionActual.IdSucursal))
            {
                MessageBox.Show("¡ALERTA! El stock de este producto ha llegado a su nivel mínimo. Considere reponer.", "Stock Bajo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow != null)
            {
                dgvCarrito.Rows.Remove(dgvCarrito.CurrentRow);
                RecalcularTotal();
            }
        }

        private void RecalcularTotal()
        {
            totalAcumulado = 0;
            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                if (fila.Cells["Subtotal"].Value != null)
                {
                    totalAcumulado += Convert.ToDecimal(fila.Cells["Subtotal"].Value);
                }
            }
            lblTotal.Text = $"TOTAL: {totalAcumulado:C2}";
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.Rows.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbMetodoPago.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un método de pago.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogo = MessageBox.Show(
                $"¿Confirmar venta por un total de {totalAcumulado:C2}?\n\nVerifique el vuelto antes de continuar.",
                "Doble Validación de Venta",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogo == DialogResult.No) return;

            if (totalAcumulado > 50000)
            {
                DialogResult dialogoGerente = MessageBox.Show(
                    "Esta venta supera el límite de $50,000. ¿Desea continuar de todas formas?",
                    "Autorización Requerida",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogoGerente == DialogResult.No) return;
            }

            Venta nuevaVenta = new Venta()
            {
                Total = totalAcumulado,
                MetodoPago = cmbMetodoPago.SelectedItem?.ToString(),
                IdUsuario = SesionActual.IdUsuario,
                IdSucursal = SesionActual.IdSucursal,
                IdCliente = Convert.ToInt32(cmbCliente.SelectedValue) == 0 ? (int?)null : Convert.ToInt32(cmbCliente.SelectedValue)
            };

            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                nuevaVenta.Detalles.Add(new DetalleVenta()
                {
                    IdProducto = Convert.ToInt32(fila.Cells["IdProducto"].Value),
                    Cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value),
                    PrecioUnitario = Convert.ToDecimal(fila.Cells["PrecioUnitario"].Value)
                });
            }

            string mensaje;
            bool resultado = _ventaNegocio.ConfirmarVenta(nuevaVenta, out mensaje);

            if (resultado)
            {
                MessageBox.Show("¡Venta procesada con éxito!", "AorusMarket", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var resp = MessageBox.Show("¿Desea generar y descargar la Factura en PDF ahora?", "Factura", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resp == DialogResult.Yes)
                {
                    List<DetalleHistorial> detallesPdf = new List<DetalleHistorial>();
                    foreach (DataGridViewRow row in dgvCarrito.Rows)
                    {
                        detallesPdf.Add(new DetalleHistorial()
                        {
                            NombreProducto = row.Cells["Producto"].Value.ToString(),
                            Cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value),
                            PrecioUnitario = Convert.ToDecimal(row.Cells["PrecioUnitario"].Value),
                            SubTotal = Convert.ToDecimal(row.Cells["Subtotal"].Value)
                        });
                    }

                    string nombreCliente = cmbCliente.Text.Contains("-") ? cmbCliente.Text.Split('-')[1].Trim() : "Consumidor Final";

                    // CORRECCIÓN: El ticket ahora lleva el Nombre Real del cajero y el Nro de Ticket correcto
                    string nombreCajero = SesionActual.NombreCompleto;

                    GeneradorFactura.GenerarPDF(nuevaVenta.IdVenta, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), nombreCliente, nombreCajero, nuevaVenta.Total, nuevaVenta.MetodoPago, detallesPdf, true);
                }

                LimpiarVenta();
            }
            else
            {
                MessageBox.Show(mensaje, "Error en Venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarVenta()
        {
            dgvCarrito.Rows.Clear();
            dgvBusqueda.Rows.Clear();
            cmbCliente.SelectedIndex = 0;
            cmbMetodoPago.SelectedIndex = -1;
            EstiloApp.EscribirTexto(txtBuscar, "");
            RecalcularTotal();
        }
    }
}