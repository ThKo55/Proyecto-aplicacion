using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades; // Acceso a la estructura Cliente
using AorusMarket.Negocio;   // Acceso al cerebro de Clientes

namespace AorusMarket.Formularios
{
    public partial class FrmClientes : Form
    {
        private CyberTextBox txtNombre, txtApellido, txtDni, txtTelefono, txtEmail, txtDireccion;
        private DataGridView dgvClientes;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        // Instanciamos el "cerebro" (capa de negocio) para usarlo en los botones
        private ClienteNegocio _clienteNegocio = new ClienteNegocio();

        public FrmClientes()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Clientes";
            ConstruirInterfaz(); // Dejamos el diseño de tu compañero intacto
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("CLIENTES", new Point(30, 20)));

            int x = 30, y = 90;
            this.Controls.Add(EstiloApp.CrearLabel("NOMBRE", new Point(x, y)));
            txtNombre = EstiloApp.CrearTextBox(new Point(x, y + 20), 200);
            this.Controls.Add(txtNombre);

            this.Controls.Add(EstiloApp.CrearLabel("APELLIDO", new Point(x + 220, y)));
            txtApellido = EstiloApp.CrearTextBox(new Point(x + 220, y + 20), 200);
            this.Controls.Add(txtApellido);

            this.Controls.Add(EstiloApp.CrearLabel("DNI", new Point(x + 440, y)));
            txtDni = EstiloApp.CrearTextBox(new Point(x + 440, y + 20), 200);
            this.Controls.Add(txtDni);

            this.Controls.Add(EstiloApp.CrearLabel("TELÉFONO", new Point(x + 660, y)));
            txtTelefono = EstiloApp.CrearTextBox(new Point(x + 660, y + 20), 200);
            this.Controls.Add(txtTelefono);

            y += 60;
            this.Controls.Add(EstiloApp.CrearLabel("EMAIL", new Point(x, y)));
            txtEmail = EstiloApp.CrearTextBox(new Point(x, y + 20), 300);
            this.Controls.Add(txtEmail);

            this.Controls.Add(EstiloApp.CrearLabel("DIRECCIÓN", new Point(x + 320, y)));
            txtDireccion = EstiloApp.CrearTextBox(new Point(x + 320, y + 20), 300);
            this.Controls.Add(txtDireccion);

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
            dgvClientes = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(890, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvClientes);
            dgvClientes.Columns.Add("IdCliente", "Id");
            dgvClientes.Columns.Add("Nombre", "Nombre");
            dgvClientes.Columns.Add("Apellido", "Apellido");
            dgvClientes.Columns.Add("Dni", "DNI");
            dgvClientes.Columns.Add("Telefono", "Teléfono");
            dgvClientes.Columns.Add("Email", "Email");
            dgvClientes.Columns.Add("Direccion", "Dirección");
            dgvClientes.Columns["IdCliente"].Visible = false;
            dgvClientes.SelectionChanged += DgvClientes_SelectionChanged;
            this.Controls.Add(dgvClientes);

            // LLAMADO NUEVO: Trae los datos reales de la BD al abrir la ventana
            CargarGrilla();
        }

        // METODO NUEVO: Consulta la BD a través de la capa de Negocio y pinta la grilla
        private void CargarGrilla()
        {
            dgvClientes.Rows.Clear(); // Limpia la tabla visual
            var listaClientes = _clienteNegocio.Listar(); // Pide la data real

            foreach (var item in listaClientes)
            {
                // Llena las celdas en el mismo orden que agregamos las columnas arriba
                dgvClientes.Rows.Add(item.IdCliente, item.Nombre, item.Apellido, item.Dni, item.Telefono, item.Email, item.Direccion);
            }
            dgvClientes.ClearSelection();
        }

        private void DgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null) return;

            var fila = dgvClientes.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdCliente"].Value ?? 0);

            // Usamos TextButton porque así funciona el CyberTextBox de ReaLTaiizor
            txtNombre.TextButton = fila.Cells["Nombre"].Value?.ToString();
            txtApellido.TextButton = fila.Cells["Apellido"].Value?.ToString();
            txtDni.TextButton = fila.Cells["Dni"].Value?.ToString();
            txtTelefono.TextButton = fila.Cells["Telefono"].Value?.ToString();
            txtEmail.TextButton = fila.Cells["Email"].Value?.ToString();
            txtDireccion.TextButton = fila.Cells["Direccion"].Value?.ToString();
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            txtNombre.TextButton = "";
            txtApellido.TextButton = "";
            txtDni.TextButton = "";
            txtTelefono.TextButton = "";
            txtEmail.TextButton = "";
            txtDireccion.TextButton = "";
            dgvClientes.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Armamos un "paquete" (Objeto Cliente) con lo que tipeó el usuario
            Cliente nuevoCliente = new Cliente()
            {
                IdCliente = idSeleccionado, // Si es 0 se crea uno nuevo, si es > 0 edita el seleccionado
                Nombre = txtNombre.TextButton,
                Apellido = txtApellido.TextButton,
                Dni = txtDni.TextButton,
                Telefono = txtTelefono.TextButton,
                Email = txtEmail.TextButton,
                Direccion = txtDireccion.TextButton
            };

            // 2. Se lo enviamos al "Cerebro" (Negocio) para que valide y guarde
            string mensaje;
            bool resultado = _clienteNegocio.Guardar(nuevoCliente, out mensaje);

            // 3. Revisamos qué nos respondió la capa de negocio
            if (resultado)
            {
                MessageBox.Show("Cliente guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrilla(); // Volvemos a pedir los datos a la BD para que aparezca el nuevo cliente
            }
            else
            {
                MessageBox.Show(mensaje, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un cliente de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show("¿Seguro que desea eliminar este cliente?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (resp == DialogResult.Yes)
            {
                // 1. Enviamos solo el ID al Negocio para procesar la baja
                string mensaje;
                bool resultado = _clienteNegocio.Eliminar(idSeleccionado, out mensaje);

                // 2. Evaluamos la respuesta
                if (resultado)
                {
                    MessageBox.Show("Cliente eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrilla(); // Actualiza la grilla (el cliente borrado ya no aparecerá)
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}