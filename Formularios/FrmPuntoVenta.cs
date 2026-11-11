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
        // Constantes para limpiar los mensajes de literales repetidos (S1192)
        private const string ColIdProducto = "IdProducto";
        private const string ColProducto = "Producto";
        private const string ColCantidad = "Cantidad";
        private const string ColSubtotal = "Subtotal";
        private const string ColPrecioUnit = "PrecioUnitario";
        private const string TituloAviso = "Aviso";

        private int? _idAutorizante;
        private CyberTextBox txtBuscar;
        private DataGridView dgvBusqueda;
        private DataGridView dgvCarrito;
        private ComboBox cmbCliente, cmbMetodoPago;
        private Label lblTotal;
        private CyberButton btnAgregar, btnQuitar, btnConfirmar, btnCancelar;

        // Marcados como readonly para solucionar (IDE0044)
        private readonly StockNegocio _stockNegocio = new StockNegocio();
        private readonly VentaNegocio _ventaNegocio = new VentaNegocio();

        private decimal? _montoAbonado;
        private enum Comprobante { Ninguno, Ticket, Factura, Ambos }

        private decimal totalAcumulado = 0;

        public FrmPuntoVenta()
        {
            InitializeComponent();
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

            AddLabelSobrio("BUSCAR PRODUCTO (cód. barras o nombre)", new Point(leftX, 70));

            txtBuscar = EstiloApp.CrearTextBox(new Point(leftX, 90), 200);
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnBuscar_Click(s, e); };
            this.Controls.Add(txtBuscar);

            CyberButton btnBuscarApp = EstiloApp.CrearBoton("BUSCAR", new Point(leftX + 210, 89), 100, EstiloApp.Gris);
            btnBuscarApp.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscarApp);

            btnAgregar = EstiloApp.CrearBoton("AGREGAR AL CARRITO", new Point(leftX + 320, 89), 180, EstiloApp.Verde);
            btnAgregar.Click += BtnAgregar_Click;
            this.Controls.Add(btnAgregar);

            dgvBusqueda = new DataGridView
            {
                Location = new Point(leftX, 140),
                Size = new Size(470, this.ClientSize.Height - 170),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };

            dgvBusqueda.Columns.Add(ColIdProducto, ColIdProducto);
            dgvBusqueda.Columns.Add(ColProducto, ColProducto);
            dgvBusqueda.Columns.Add("Precio", "Precio");
            dgvBusqueda.Columns.Add("Disponible", "Stock Disp.");
            dgvBusqueda.Columns.Add(new DataGridViewTextBoxColumn { Name = ColCantidad, HeaderText = "Cant. a Vender" });
            dgvBusqueda.Columns[ColIdProducto].Visible = false;

            EstiloApp.EstilizarGrid(dgvBusqueda);

            // SOLUCIÓN AL BUG MANUAL: Deshacer el ReadOnly global y habilitar edición on click
            dgvBusqueda.ReadOnly = false;
            foreach (DataGridViewColumn c in dgvBusqueda.Columns)
            {
                if (c.Name != ColCantidad) c.ReadOnly = true;
            }
            dgvBusqueda.EditMode = DataGridViewEditMode.EditOnEnter;

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

            dgvCarrito.Columns.Add(ColIdProducto, ColIdProducto);
            dgvCarrito.Columns.Add(ColProducto, ColProducto);
            dgvCarrito.Columns.Add(ColCantidad, ColCantidad);
            dgvCarrito.Columns.Add(ColPrecioUnit, "Precio Unit.");
            dgvCarrito.Columns.Add(ColSubtotal, ColSubtotal);
            dgvCarrito.Columns[ColIdProducto].Visible = false;

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
            string textoBusqueda = string.IsNullOrWhiteSpace(txtBuscar.TextButton) ? txtBuscar.Text : txtBuscar.TextButton;
            textoBusqueda = textoBusqueda?.Trim();

            if (string.IsNullOrEmpty(textoBusqueda))
            {
                MessageBox.Show("Escriba el código de barras o nombre a buscar.", TituloAviso, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            dgvBusqueda.Rows.Clear();
            var resultados = _ventaNegocio.BuscarProducto(textoBusqueda, SesionActual.IdSucursal);

            if (resultados.Count == 1 && resultados[0].CodigoBarras == textoBusqueda)
            {
                AgregarAlCarritoDirecto(resultados[0], 1);
                EstiloApp.EscribirTexto(txtBuscar, "");
                txtBuscar.Focus();
                return;
            }

            foreach (var r in resultados)
            {
                dgvBusqueda.Rows.Add(r.IdProducto, r.Nombre, r.Precio, r.StockDisponible, "1");
            }
            dgvBusqueda.ClearSelection();
        }

        private void AgregarAlCarritoDirecto(BusquedaProducto producto, int cantidadAInsertar)
        {
            int cantidadYaEnCarrito = 0;
            foreach (DataGridViewRow row in dgvCarrito.Rows)
            {
                if (Convert.ToInt32(row.Cells[ColIdProducto].Value) == producto.IdProducto)
                {
                    cantidadYaEnCarrito += Convert.ToInt32(row.Cells[ColCantidad].Value);
                }
            }

            int cantidadTotalDeseada = cantidadAInsertar + cantidadYaEnCarrito;

            if (cantidadTotalDeseada > producto.StockDisponible)
            {
                MessageBox.Show($"No hay stock suficiente. Tienes {producto.StockDisponible} disponibles.", "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_stockNegocio.HayStockSuficiente(producto.IdProducto, SesionActual.IdSucursal, cantidadTotalDeseada))
            {
                MessageBox.Show("El stock en la base de datos ya no es suficiente.", "Error de Stock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool existe = false;
            foreach (DataGridViewRow row in dgvCarrito.Rows)
            {
                if (Convert.ToInt32(row.Cells[ColIdProducto].Value) == producto.IdProducto)
                {
                    row.Cells[ColCantidad].Value = cantidadTotalDeseada;
                    row.Cells[ColSubtotal].Value = cantidadTotalDeseada * producto.Precio;
                    existe = true;
                    break;
                }
            }

            if (!existe)
            {
                dgvCarrito.Rows.Add(producto.IdProducto, producto.Nombre, cantidadAInsertar, producto.Precio, producto.Precio * cantidadAInsertar);
            }

            RecalcularTotal();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvBusqueda.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto de la búsqueda", TituloAviso, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fila = dgvBusqueda.CurrentRow;
            string cantIngresadaStr = fila.Cells[ColCantidad].Value?.ToString();

            if (!int.TryParse(cantIngresadaStr, out int cantIngresada) || cantIngresada <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida mayor a 0 en la grilla.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            BusquedaProducto bp = new BusquedaProducto()
            {
                IdProducto = Convert.ToInt32(fila.Cells[ColIdProducto].Value),
                Nombre = fila.Cells[ColProducto].Value.ToString(),
                Precio = Convert.ToDecimal(fila.Cells["Precio"].Value),
                StockDisponible = Convert.ToInt32(fila.Cells["Disponible"].Value)
            };

            AgregarAlCarritoDirecto(bp, cantIngresada);
            fila.Cells[ColCantidad].Value = "";
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
                if (fila.Cells[ColSubtotal].Value != null)
                {
                    totalAcumulado += Convert.ToDecimal(fila.Cells[ColSubtotal].Value);
                }
            }
            lblTotal.Text = $"TOTAL: {totalAcumulado:C2}";
        }

        private decimal PedirMontoEfectivo(decimal totalVenta)
        {
            System.Windows.Forms.Form prompt = new System.Windows.Forms.Form()
            {
                Width = 350,
                Height = 200,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog,
                Text = "Cobro en Efectivo",
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(18, 18, 18),
                ForeColor = Color.White,
                MaximizeBox = false,
                MinimizeBox = false
            };

            System.Windows.Forms.Label textLabel = new System.Windows.Forms.Label() { Left = 20, Top = 20, Width = 300, Text = $"Total de la venta: {totalVenta:C2}\nIngrese el monto entregado por el cliente:" };
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 20, Top = 70, Width = 290, Font = new Font("Segoe UI", 14F) };
            System.Windows.Forms.Button confirmation = new System.Windows.Forms.Button() { Text = "Calcular", Left = 210, Top = 120, Width = 100, BackColor = Color.FromArgb(40, 167, 69), FlatStyle = FlatStyle.Flat };

            decimal montoIngresado = -1;

            confirmation.Click += (sender, e) => {
                if (decimal.TryParse(textBox.Text, out montoIngresado)) { prompt.Close(); }
                else { MessageBox.Show("Ingrese un número válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };

            textBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) confirmation.PerformClick(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.ShowDialog();

            return montoIngresado;
        }

        // SOLUCIÓN A COMPLEJIDAD COGNITIVA (S3776): Se extrajo la validación inicial
        private bool ValidarPreVenta(out string metodoSeleccionado)
        {
            metodoSeleccionado = string.Empty;
            if (dgvCarrito.Rows.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", TituloAviso, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbMetodoPago.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un método de pago.", TituloAviso, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            metodoSeleccionado = cmbMetodoPago.SelectedItem.ToString();
            return true;
        }

        private bool ValidarEfectivoOConfirmar(string metodoSeleccionado)
        {


            if (metodoSeleccionado == "Efectivo")
            {
                decimal montoAbonado = PedirMontoEfectivo(totalAcumulado);
                if (montoAbonado == -1) return false;

                if (montoAbonado < totalAcumulado)
                {
                    MessageBox.Show("El monto ingresado es menor al total de la venta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                _montoAbonado = montoAbonado;
                decimal vuelto = montoAbonado - totalAcumulado;
                return MessageBox.Show($"Total a cobrar: {totalAcumulado:C2}\nAbona con: {montoAbonado:C2}\n\nSU VUELTO ES: {vuelto:C2}\n\n¿Confirmar venta?", "Verificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }

            return MessageBox.Show($"¿Confirmar venta por un total de {totalAcumulado:C2}?", "Validación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            _idAutorizante = null;
            _montoAbonado = null;

            if (!ValidarPreVenta(out string metodoSeleccionado)) return;
            if (!AutorizarVentaDeMontoAlto()) return;
            if (!ValidarEfectivoOConfirmar(metodoSeleccionado)) return;

            ProcesarCierreVenta(metodoSeleccionado);
        }

        private bool AutorizarVentaDeMontoAlto()
        {
            if (totalAcumulado <= VentaNegocio.LimiteVentaSinAutorizacion) return true;

            Usuario aprobador = DialogoAutorizacion.Solicitar(this, "Autorización de venta de monto alto",
                $"La venta ({totalAcumulado:C2}) supera el límite de {VentaNegocio.LimiteVentaSinAutorizacion:C2}.\nUn Administrador o Gerente debe autorizarla.",
                false, Perfiles.Administrador, Perfiles.Gerente);

            if (aprobador == null)
            {
                MessageBox.Show("Venta no autorizada. No se registró.", "Autorización requerida",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            _idAutorizante = aprobador.IdUsuario;
            return true;
        }

        private void ProcesarCierreVenta(string metodoSeleccionado)
        {
            Venta nuevaVenta = new Venta()
            {
                Total = totalAcumulado,
                MetodoPago = metodoSeleccionado,
                IdUsuario = SesionActual.IdUsuario,
                IdSucursal = SesionActual.IdSucursal,
                IdCliente = Convert.ToInt32(cmbCliente.SelectedValue) == 0 ? (int?)null : Convert.ToInt32(cmbCliente.SelectedValue),
                IdUsuarioAutoriza = _idAutorizante
            };

            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                nuevaVenta.Detalles.Add(new DetalleVenta()
                {
                    IdProducto = Convert.ToInt32(fila.Cells[ColIdProducto].Value),
                    Cantidad = Convert.ToInt32(fila.Cells[ColCantidad].Value),
                    PrecioUnitario = Convert.ToDecimal(fila.Cells[ColPrecioUnit].Value)
                });
            }

            if (_ventaNegocio.ConfirmarVenta(nuevaVenta, out string mensaje))
            {
                MessageBox.Show("¡Venta procesada con éxito!", "AorusMarket", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Se arman los datos antes de limpiar el carrito
                List<DetalleHistorial> detallesPdf = new List<DetalleHistorial>();
                foreach (DataGridViewRow row in dgvCarrito.Rows)
                {
                    detallesPdf.Add(new DetalleHistorial()
                    {
                        NombreProducto = row.Cells[ColProducto].Value.ToString(),
                        Cantidad = Convert.ToInt32(row.Cells[ColCantidad].Value),
                        PrecioUnitario = Convert.ToDecimal(row.Cells[ColPrecioUnit].Value),
                        SubTotal = Convert.ToDecimal(row.Cells[ColSubtotal].Value)
                    });
                }

                string nombreCliente = cmbCliente.Text.Contains("-") ? cmbCliente.Text.Split('-')[1].Trim() : "Consumidor Final";
                string fechaVenta = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                Comprobante eleccion = PedirComprobante();

                if (eleccion == Comprobante.Ticket || eleccion == Comprobante.Ambos)
                    GeneradorTicket.GenerarTicket(nuevaVenta.IdVenta, fechaVenta, nombreCliente, SesionActual.NombreCompleto,
                        SesionActual.NombreSucursal, nuevaVenta.Total, nuevaVenta.MetodoPago, detallesPdf, _montoAbonado);

                if (eleccion == Comprobante.Factura || eleccion == Comprobante.Ambos)
                    GeneradorFactura.GenerarPDF(nuevaVenta.IdVenta, fechaVenta, nombreCliente, SesionActual.NombreCompleto,
                        nuevaVenta.Total, nuevaVenta.MetodoPago, detallesPdf, true);

                LimpiarVenta();
            }
            else
            {
                MessageBox.Show(mensaje, "Error en Venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private Comprobante PedirComprobante()
        {
            Comprobante eleccion = Comprobante.Ninguno;

            using (System.Windows.Forms.Form dlg = new System.Windows.Forms.Form())
            {
                dlg.Text = "Comprobante";
                dlg.ClientSize = new Size(460, 140);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.BackColor = EstiloApp.Fondo;

                dlg.Controls.Add(new Label
                {
                    Text = "Venta registrada. ¿Qué comprobante desea generar?",
                    ForeColor = Color.WhiteSmoke,
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    Location = new Point(20, 20),
                    AutoSize = true
                });

                void AgregarBoton(string texto, int x, int ancho, Color color, Comprobante valor)
                {
                    System.Windows.Forms.Button b = new System.Windows.Forms.Button
                    {
                        Text = texto,
                        Location = new Point(x, 70),
                        Size = new Size(ancho, 38),
                        BackColor = color,
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                        Cursor = Cursors.Hand
                    };
                    b.FlatAppearance.BorderSize = 0;
                    b.Click += (s, e) => { eleccion = valor; dlg.Close(); };
                    dlg.Controls.Add(b);
                }

                AgregarBoton("TICKET", 20, 100, EstiloApp.Verde, Comprobante.Ticket);
                AgregarBoton("FACTURA", 130, 100, EstiloApp.Verde, Comprobante.Factura);
                AgregarBoton("AMBOS", 240, 100, EstiloApp.Verde, Comprobante.Ambos);
                AgregarBoton("NINGUNO", 350, 90, EstiloApp.Gris, Comprobante.Ninguno);

                dlg.ShowDialog(this);
            }
            return eleccion;
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