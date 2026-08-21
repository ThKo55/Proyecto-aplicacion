using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;

namespace AorusMarket.Formularios
{
    public partial class FrmPuntoVenta : Form
    {
        private TextBox txtBuscar;
        private DataGridView dgvBusqueda;
        private DataGridView dgvCarrito;
        private ComboBox cmbCliente, cmbMetodoPago;
        private Label lblTotal;
        private Button btnAgregar, btnQuitar, btnConfirmar, btnCancelar;

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
            this.Controls.Add(EstiloApp.CrearLabel("BUSCAR PRODUCTO (nombre o código)", new Point(30, 70)));
            txtBuscar = EstiloApp.CrearTextBox(new Point(30, 90), 400);
            txtBuscar.TextChanged += TxtBuscar_TextChanged;
            this.Controls.Add(txtBuscar);

            btnAgregar = EstiloApp.CrearBoton("AGREGAR AL CARRITO", new Point(440, 89), 220, EstiloApp.Verde);
            btnAgregar.Click += BtnAgregar_Click;
            this.Controls.Add(btnAgregar);

            dgvBusqueda = new DataGridView
            {
                Location = new Point(30, 130),
                Size = new Size(1040, 180),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            EstiloApp.EstilizarGrid(dgvBusqueda);
            dgvBusqueda.ReadOnly = false; // permite editar la columna Cantidad
            dgvBusqueda.Columns.Add("IdStock", "IdStock");
            dgvBusqueda.Columns.Add("Producto", "Producto");
            dgvBusqueda.Columns.Add("Precio", "Precio");
            dgvBusqueda.Columns.Add("Disponible", "Stock Disp.");
            var colCantidad = new DataGridViewTextBoxColumn { Name = "Cantidad", HeaderText = "Cant." };
            dgvBusqueda.Columns.Add(colCantidad);
            dgvBusqueda.Columns["IdStock"].Visible = false;
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
            dgvCarrito.Columns.Add("IdStock", "IdStock");
            dgvCarrito.Columns.Add("Producto", "Producto");
            dgvCarrito.Columns.Add("Cantidad", "Cantidad");
            dgvCarrito.Columns.Add("PrecioUnitario", "Precio Unit.");
            dgvCarrito.Columns.Add("Subtotal", "Subtotal");
            dgvCarrito.Columns["IdStock"].Visible = false;
            this.Controls.Add(dgvCarrito);

            btnQuitar = EstiloApp.CrearBoton("QUITAR ITEM", new Point(30, 560), 180, EstiloApp.RojoNeon);
            btnQuitar.Click += BtnQuitar_Click;
            this.Controls.Add(btnQuitar);

            // ---------- Pie: cliente, pago, total ----------
            this.Controls.Add(EstiloApp.CrearLabel("CLIENTE (opcional)", new Point(230, 565)));
            cmbCliente = EstiloApp.CrearComboBox(new Point(230, 585), 220);
            this.Controls.Add(cmbCliente);

            this.Controls.Add(EstiloApp.CrearLabel("MÉTODO DE PAGO", new Point(470, 565)));
            cmbMetodoPago = EstiloApp.CrearComboBox(new Point(470, 585), 200);
            cmbMetodoPago.Items.AddRange(new object[] { "Efectivo", "Débito", "Crédito", "Transferencia" });
            this.Controls.Add(cmbMetodoPago);

            lblTotal = new Label
            {
                Text = "TOTAL: $0.00",
                Font = new Font("Arial Black", 16F, FontStyle.Bold),
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

        private void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            // TODO: buscar en stock_sucursal filtrando por SesionActual.IdSucursal
            // y cargar resultados en dgvBusqueda
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvBusqueda.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto de la búsqueda", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // TODO: validar stock disponible y pasar la fila al carrito
            RecalcularTotal();
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
            decimal total = 0;
            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                if (fila.Cells["Subtotal"].Value != null &&
                    decimal.TryParse(fila.Cells["Subtotal"].Value.ToString(), out decimal sub))
                    total += sub;
            }
            lblTotal.Text = $"TOTAL: ${total:0.00}";
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.Rows.Count == 0)
            {
                MessageBox.Show("El carrito está vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbMetodoPago.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un método de pago", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var resp = MessageBox.Show("¿Confirmar la venta?", "Confirmar Venta",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                // TODO: ejecutar VentaDAL.ConfirmarVenta() dentro de una transacción SQL
                MessageBox.Show("Venta confirmada (falta conectar la base de datos)", "AorusMarket");
                LimpiarVenta();
            }
        }

        private void LimpiarVenta()
        {
            dgvCarrito.Rows.Clear();
            cmbCliente.SelectedIndex = -1;
            cmbMetodoPago.SelectedIndex = -1;
            txtBuscar.Clear();
            RecalcularTotal();
        }
    }
}