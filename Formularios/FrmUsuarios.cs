using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls; // IMPORTANTE: Agregado para que reconozca los nuevos controles

namespace AorusMarket.Formularios
{
    public partial class FrmUsuarios : Form
    {
        // 1. Cambiamos TextBox por CyberTextBox y Button por CyberButton
        private CyberTextBox txtNombre, txtApellido, txtEmail, txtPassword;
        private ComboBox cmbPerfil, cmbSucursal;
        private DataGridView dgvUsuarios;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        public FrmUsuarios()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Usuarios";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("USUARIOS", new Point(30, 20)));

            int x = 30, y = 90;

            this.Controls.Add(EstiloApp.CrearLabel("NOMBRE", new Point(x, y)));
            txtNombre = EstiloApp.CrearTextBox(new Point(x, y + 20), 220);
            this.Controls.Add(txtNombre);

            this.Controls.Add(EstiloApp.CrearLabel("APELLIDO", new Point(x + 240, y)));
            txtApellido = EstiloApp.CrearTextBox(new Point(x + 240, y + 20), 220);
            this.Controls.Add(txtApellido);

            this.Controls.Add(EstiloApp.CrearLabel("EMAIL", new Point(x + 480, y)));
            txtEmail = EstiloApp.CrearTextBox(new Point(x + 480, y + 20), 220);
            this.Controls.Add(txtEmail);

            y += 60;
            this.Controls.Add(EstiloApp.CrearLabel("CONTRASEÑA", new Point(x, y)));
            txtPassword = EstiloApp.CrearTextBox(new Point(x, y + 20), 220, true); // Pasamos true para ocultar caracteres
            this.Controls.Add(txtPassword);

            this.Controls.Add(EstiloApp.CrearLabel("PERFIL", new Point(x + 240, y)));
            cmbPerfil = EstiloApp.CrearComboBox(new Point(x + 240, y + 20), 220);
            cmbPerfil.Items.AddRange(new object[] { "Administrador", "Cajero", "Gestor de Stock" });
            this.Controls.Add(cmbPerfil);

            this.Controls.Add(EstiloApp.CrearLabel("SUCURSAL", new Point(x + 480, y)));
            cmbSucursal = EstiloApp.CrearComboBox(new Point(x + 480, y + 20), 220);
            // TODO: cargar sucursales reales desde la base
            this.Controls.Add(cmbSucursal);

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
            dgvUsuarios = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(890, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvUsuarios);
            dgvUsuarios.Columns.Add("IdUsuario", "Id");
            dgvUsuarios.Columns.Add("Nombre", "Nombre");
            dgvUsuarios.Columns.Add("Apellido", "Apellido");
            dgvUsuarios.Columns.Add("Email", "Email");
            dgvUsuarios.Columns.Add("Perfil", "Perfil");
            dgvUsuarios.Columns.Add("Sucursal", "Sucursal");
            dgvUsuarios.Columns["IdUsuario"].Visible = false;
            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;
            this.Controls.Add(dgvUsuarios);
        }

        private void DgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;
            var fila = dgvUsuarios.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdUsuario"].Value ?? 0);
            txtNombre.TextButton = fila.Cells["Nombre"].Value?.ToString();
            txtApellido.TextButton = fila.Cells["Apellido"].Value?.ToString();
            txtEmail.TextButton = fila.Cells["Email"].Value?.ToString();
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            txtNombre.TextButton = "";
            txtApellido.TextButton = "";
            txtEmail.TextButton = "";
            txtPassword.TextButton = "";
            cmbPerfil.SelectedIndex = -1;
            cmbSucursal.SelectedIndex = -1;
            dgvUsuarios.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.TextButton) || string.IsNullOrWhiteSpace(txtApellido.TextButton) ||
                string.IsNullOrWhiteSpace(txtEmail.TextButton) || cmbPerfil.SelectedIndex == -1)
            {
                MessageBox.Show("Debe completar todos los campos obligatorios", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Usuario guardado (falta conectar la base de datos)", "AorusMarket");
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un usuario de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var resp = MessageBox.Show("¿Seguro que desea eliminar este usuario?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (resp == DialogResult.Yes)
            {
                // TODO: eliminar en la base de datos (UsuarioDAL)
                MessageBox.Show("Usuario eliminado (falta conectar la base de datos)", "AorusMarket");
                LimpiarCampos();
            }
        }
    }
}