using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls; // IMPORTANTE: Agregado para que reconozca los nuevos controles

namespace AorusMarket.Formularios
{
    public partial class FrmClientes : Form
    {
        // 1. Cambiamos TextBox por CyberTextBox y Button por CyberButton
        private CyberTextBox txtNombre, txtApellido, txtDni, txtTelefono, txtEmail, txtDireccion;
        private DataGridView dgvClientes;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        public FrmClientes()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Clientes";
            ConstruirInterfaz();
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
        }

        private void DgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null) return;
            var fila = dgvClientes.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdCliente"].Value ?? 0);
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
            if (string.IsNullOrWhiteSpace(txtNombre.TextButton) || string.IsNullOrWhiteSpace(txtApellido.TextButton))
            {
                MessageBox.Show("Debe completar Nombre y Apellido", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Cliente guardado (falta conectar la base de datos)", "AorusMarket");
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
                // TODO: eliminar en la base de datos (ClienteDAL)
                MessageBox.Show("Cliente eliminado (falta conectar la base de datos)", "AorusMarket");
                LimpiarCampos();
            }
        }
    }
}