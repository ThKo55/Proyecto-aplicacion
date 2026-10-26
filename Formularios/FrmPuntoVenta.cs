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

        // Instancias a la capa de Negocio
        private StockNegocio _stockNegocio = new StockNegocio();
        private VentaNegocio _ventaNegocio = new VentaNegocio();

        private decimal totalAcumulado = 0;

        public FrmPuntoVenta()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Punto de Venta";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("PUNTO DE VENTA", new Point(30, 15)));

            // ---------- Búsqueda de productos ----------
            this.Controls.Add(EstiloApp.CrearLabel("BUSCAR PRODUCTO (nombre)", new Point(30, 70)));

            txtBuscar = EstiloApp.CrearTextBox(new Point(30, 90), 300);
            this.Controls.Add(txtBuscar);

            CyberButton btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(340, 89), 100, EstiloApp.Gris);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            btnAgregar = EstiloApp.CrearBoton("AGREGAR AL CARRITO", new Point(450, 89), 220, EstiloApp.Verde);
            btnAgregar.Click += BtnAgregar_Click;
            this.Controls.Add(btnAgregar);

            dgvBusqueda = new DataGridView
            {
                Location = new Point(30, 130),
                Size = new Size(1040, 180),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            EstiloApp.EstilizarGrid(dgvBusqueda);
            dgvBusqueda.ReadOnly = false;

            dgvBusqueda.Columns.Add("IdProducto", "IdProducto");
            dgvBusqueda.Columns.Add("Producto", "Producto");
            dgvBusqueda.Columns.Add("Precio", "Precio");
            dgvBusqueda.Columns.Add("Disponible", "Stock Disp.");

            var colCantidad = new DataGridViewTextBoxColumn { Name = "Cantidad", HeaderText = "Cant. a Vender" };
            dgvBusqueda.Columns.Add(colCantidad);
            dgvBusqueda.Columns["IdProducto"].Visible = false;

            foreach (DataGridViewColumn c in dgvBusqueda.Columns)
                if (c.Name != "Cantidad") c.ReadOnly = true;

            this.Controls.Add(dgvBusqueda);

            // ---------- Carrito ----------
            this.Controls.Add(EstiloApp.CrearLabel("CARRITO DE VENTA", new Point(30, 325)));
            dgvCarrito = new DataGridView
            {
                Location = new Point(30, 350),
                Size = new Size(1040, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            EstiloApp.EstilizarGrid(dgvCarrito);
            dgvCarrito.Columns.Add("IdProducto", "IdProducto");
            dgvCarrito.Columns.Add("Producto", "Producto");
            dgvCarrito.Columns.Add("Cantidad", "Cantidad");
            dgvCarrito.Columns.Add("PrecioUnitario", "Precio Unit.");
            dgvCarrito.Columns.Add("Subtotal", "Subtotal");
            dgvCarrito.Columns["IdProducto"].Visible = false;
            this.Controls.Add(dgvCarrito);

            btnQuitar = EstiloApp.CrearBoton("QUITAR ITEM", new Point(30, 560), 180, EstiloApp.RojoNeon);
            btnQuitar.Click += BtnQuitar_Click;
            this.Controls.Add(btnQuitar);

            // ---------- Pie ----------
            this.Controls.Add(EstiloApp.CrearLabel("CLIENTE (opcional)", new Point(230, 565)));
            cmbCliente = EstiloApp.CrearComboBox(new Point(230, 585), 220);
            CargarComboClientes();
            this.Controls.Add(cmbCliente);

            this.Controls.Add(EstiloApp.CrearLabel("MÉTODO DE PAGO", new Point(470, 565)));
            cmbMetodoPago = EstiloApp.CrearComboBox(new Point(470, 585), 200);
            cmbMetodoPago.Items.AddRange(new object[] { "Efectivo", "Débito", "Crédito", "Transferencia" });
            this.Controls.Add(cmbMetodoPago);

            lblTotal = new Label
            {
                Text = "TOTAL: $0.00",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = EstiloApp.RojoNeon,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(700, 578)
            };
            this.Controls.Add(lblTotal);

            btnConfirmar = EstiloApp.CrearBoton("CONFIRMAR VENTA", new Point(30, 630), 220, EstiloApp.Verde);
            btnConfirmar.Click += BtnConfirmar_Click;
            this.Controls.Add(btnConfirmar);

            btnCancelar = EstiloApp.CrearBoton("CANCELAR VENTA", new Point(260, 630), 220, EstiloApp.Gris);
            btnCancelar.Click += (s, e) => LimpiarVenta();
            this.Controls.Add(btnCancelar);
        }

        private void CargarComboClientes()
        {
            var clientes = _ventaNegocio.ObtenerClientesCombo();

            // Agregamos la opción por defecto con un DNI genérico
            clientes.Insert(0, new Cliente() { IdCliente = 0, Dni = "00000000", Nombre = "Consumidor", Apellido = "Final" });

            cmbCliente.DataSource = clientes;
            cmbCliente.DisplayMember = "InfoBusqueda"; // Muestra "DNI - Nombre Apellido"
            cmbCliente.ValueMember = "IdCliente";

            // LA SOLUCIÓN: Habilitar la escritura en la cajita
            cmbCliente.DropDownStyle = ComboBoxStyle.DropDown;

            // ACTIVAR EL BUSCADOR INTELIGENTE POR DNI
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

            // 1. Validar que escribió un número mayor a 0
            if (!int.TryParse(cantIngresadaStr, out int cantIngresada) || cantIngresada <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida mayor a 0 en la grilla.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idProd = Convert.ToInt32(fila.Cells["IdProducto"].Value);
            int stockDispGrilla = Convert.ToInt32(fila.Cells["Disponible"].Value);

            // 2. NUEVO: Calcular cuántos de este producto YA metió al carrito antes
            int cantidadYaEnCarrito = 0;
            foreach (DataGridViewRow row in dgvCarrito.Rows)
            {
                if (Convert.ToInt32(row.Cells["IdProducto"].Value) == idProd)
                {
                    cantidadYaEnCarrito += Convert.ToInt32(row.Cells["Cantidad"].Value);
                }
            }

            // Sumamos lo que quiere llevar AHORA + lo que YA TENÍA en el carrito
            int cantidadTotalDeseada = cantIngresada + cantidadYaEnCarrito;

            // 3. Validación Rápida (Contra lo que dice la grilla de la sucursal actual)
            if (cantidadTotalDeseada > stockDispGrilla)
            {
                MessageBox.Show($"No hay stock suficiente.\nTienes {stockDispGrilla} disponibles y estás intentando vender un total de {cantidadTotalDeseada} (incluyendo los que ya pusiste en el carrito).", "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Validación de Seguridad en BD (para evitar problemas si otro cajero vendió al mismo tiempo)
            if (!_stockNegocio.HayStockSuficiente(idProd, SesionActual.IdSucursal, cantidadTotalDeseada))
            {
                MessageBox.Show("El stock en la base de datos ya no es suficiente (quizás otra caja acaba de venderlo).", "Error de Stock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 5. Todo OK -> Mandar al carrito
            string nombre = fila.Cells["Producto"].Value.ToString();
            decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
            decimal subtotal = precio * cantIngresada;

            dgvCarrito.Rows.Add(idProd, nombre, cantIngresada, precio, subtotal);

            // Limpiamos la celda de cantidad
            fila.Cells["Cantidad"].Value = "";
            RecalcularTotal();

            // 6. Alerta visual de stock bajo
            if (_stockNegocio.StockEsCritico(idProd))
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

            // --- INICIO DOBLE VALIDACIÓN ---
            DialogResult dialogo = MessageBox.Show(
                $"¿Confirmar venta por un total de {totalAcumulado:C2}?\n\nVerifique el vuelto antes de continuar.",
                "Doble Validación de Venta",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogo == DialogResult.No)
            {
                return; // Si el cajero dice que No, se corta la ejecución y no cobra
            }

            // Opcional: Validación de clave de gerente si el monto es mayor a 50.000
            if (totalAcumulado > 50000)
            {
                DialogResult dialogoGerente = MessageBox.Show(
                    "Esta venta supera el límite de $50,000. ¿Desea continuar de todas formas?",
                    "Autorización Requerida",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogoGerente == DialogResult.No) return;
            }
            // --- FIN DOBLE VALIDACIÓN ---

            // 1. Empaquetamos la cabecera
            Venta nuevaVenta = new Venta()
            {
                Total = totalAcumulado,
                MetodoPago = cmbMetodoPago.SelectedItem?.ToString(),
                IdUsuario = SesionActual.IdUsuario,
                IdSucursal = SesionActual.IdSucursal,
                IdCliente = Convert.ToInt32(cmbCliente.SelectedValue) == 0 ? (int?)null : Convert.ToInt32(cmbCliente.SelectedValue)
            };

            // 2. Empaquetamos el carrito (los detalles)
            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                nuevaVenta.Detalles.Add(new DetalleVenta()
                {
                    IdProducto = Convert.ToInt32(fila.Cells["IdProducto"].Value),
                    Cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value),
                    PrecioUnitario = Convert.ToDecimal(fila.Cells["PrecioUnitario"].Value)
                });
            }

            // 3. Enviar el paquete gigante al Cerebro
            string mensaje;
            bool resultado = _ventaNegocio.ConfirmarVenta(nuevaVenta, out mensaje);

            // 4. Evaluar resultado y generar PDF
            if (resultado)
            {
                MessageBox.Show("¡Venta procesada con éxito!", "AorusMarket", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // --- GENERACIÓN DEL PDF ---
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

                    // Parseamos el nombre del cliente
                    string nombreCliente = cmbCliente.Text.Contains("-") ? cmbCliente.Text.Split('-')[1].Trim() : "Consumidor Final";
                    string nombreCajero = "Cajero " + SesionActual.IdUsuario.ToString();

                    // Llamamos a tu clase para crear y abrir el PDF (usamos ID de venta 0 temporal porque se acaba de generar)
                    GeneradorFactura.GenerarPDF(0, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), nombreCliente, nombreCajero, nuevaVenta.Total, nuevaVenta.MetodoPago, detallesPdf, true);
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
            txtBuscar.TextButton = "";
            RecalcularTotal();
        }
    }
}