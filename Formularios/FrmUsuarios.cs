using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmUsuarios : Form
    {
        // 1. Agregamos los nuevos controles a las variables privadas
        private CyberTextBox txtNombre, txtApellido, txtEmail, txtPassword, txtTelefono, txtDireccion;
        private DateTimePicker dtFechaNacimiento;
        private ComboBox cmbPerfil, cmbSucursal;
        private DataGridView dgvUsuarios;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        private UsuarioNegocio _usuarioNegocio = new UsuarioNegocio();

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

            int x = 30, y = 80;

            // FILA 1
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
            // FILA 2 (Nuevos campos)
            this.Controls.Add(EstiloApp.CrearLabel("TELÉFONO", new Point(x, y)));
            txtTelefono = EstiloApp.CrearTextBox(new Point(x, y + 20), 220);
            this.Controls.Add(txtTelefono);

            this.Controls.Add(EstiloApp.CrearLabel("DIRECCIÓN", new Point(x + 240, y)));
            txtDireccion = EstiloApp.CrearTextBox(new Point(x + 240, y + 20), 220);
            this.Controls.Add(txtDireccion);

            this.Controls.Add(EstiloApp.CrearLabel("FECHA DE NACIMIENTO", new Point(x + 480, y)));
            dtFechaNacimiento = new DateTimePicker
            {
                Location = new Point(x + 480, y + 20),
                Size = new Size(220, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(dtFechaNacimiento);

            y += 60;
            // FILA 3
            this.Controls.Add(EstiloApp.CrearLabel("CONTRASEÑA", new Point(x, y)));
            txtPassword = EstiloApp.CrearTextBox(new Point(x, y + 20), 220, true);
            this.Controls.Add(txtPassword);

            this.Controls.Add(EstiloApp.CrearLabel("PERFIL", new Point(x + 240, y)));
            cmbPerfil = EstiloApp.CrearComboBox(new Point(x + 240, y + 20), 220);
            this.Controls.Add(cmbPerfil);

            this.Controls.Add(EstiloApp.CrearLabel("SUCURSAL", new Point(x + 480, y)));
            cmbSucursal = EstiloApp.CrearComboBox(new Point(x + 480, y + 20), 220);
            this.Controls.Add(cmbSucursal);

            y += 60;
            // BOTONES
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
            // GRILLA
            dgvUsuarios = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(1000, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvUsuarios);
            dgvUsuarios.Columns.Add("IdUsuario", "Id");
            dgvUsuarios.Columns.Add("Nombre", "Nombre");
            dgvUsuarios.Columns.Add("Apellido", "Apellido");
            dgvUsuarios.Columns.Add("Email", "Email");
            dgvUsuarios.Columns.Add("Telefono", "Teléfono");
            dgvUsuarios.Columns.Add("Direccion", "Dirección");
            dgvUsuarios.Columns.Add("FechaNacimiento", "F. Nacimiento");
            dgvUsuarios.Columns.Add("Perfil", "Perfil");
            dgvUsuarios.Columns.Add("Sucursal", "Sucursal");

            dgvUsuarios.Columns.Add("IdPerfil", "IdPerfil");
            dgvUsuarios.Columns.Add("IdSucursal", "IdSucursal");

            dgvUsuarios.Columns["IdUsuario"].Visible = false;
            dgvUsuarios.Columns["IdPerfil"].Visible = false;
            dgvUsuarios.Columns["IdSucursal"].Visible = false;

            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;
            this.Controls.Add(dgvUsuarios);

            CargarCombos();
            CargarGrilla();
        }

        private void CargarCombos()
        {
            cmbPerfil.DataSource = _usuarioNegocio.ObtenerPerfiles();
            cmbPerfil.DisplayMember = "Nombre";
            cmbPerfil.ValueMember = "Id";
            cmbPerfil.SelectedIndex = -1;

            cmbSucursal.DataSource = _usuarioNegocio.ObtenerSucursales();
            cmbSucursal.DisplayMember = "Nombre";
            cmbSucursal.ValueMember = "Id";
            cmbSucursal.SelectedIndex = -1;
        }

        private void CargarGrilla()
        {
            dgvUsuarios.Rows.Clear();
            var listaUsuarios = _usuarioNegocio.Listar();

            foreach (var item in listaUsuarios)
            {
                dgvUsuarios.Rows.Add(
                    item.IdUsuario,
                    item.Nombre,
                    item.Apellido,
                    item.Email,
                    item.Telefono,
                    item.Direccion,
                    item.FechaNacimiento.HasValue ? item.FechaNacimiento.Value.ToString("dd/MM/yyyy") : "",
                    item.NombrePerfil,
                    item.NombreSucursal,
                    item.IdPerfil,
                    item.IdSucursal
                );
            }
            dgvUsuarios.ClearSelection();
        }

        private void DgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;
            var fila = dgvUsuarios.CurrentRow;

            idSeleccionado = Convert.ToInt32(fila.Cells["IdUsuario"].Value ?? 0);
            txtNombre.TextButton = fila.Cells["Nombre"].Value?.ToString();
            txtApellido.TextButton = fila.Cells["Apellido"].Value?.ToString();
            txtEmail.TextButton = fila.Cells["Email"].Value?.ToString();
            txtTelefono.TextButton = fila.Cells["Telefono"].Value?.ToString();
            txtDireccion.TextButton = fila.Cells["Direccion"].Value?.ToString();
            txtPassword.TextButton = "";

            if (DateTime.TryParse(fila.Cells["FechaNacimiento"].Value?.ToString(), out DateTime fechaNac))
            {
                dtFechaNacimiento.Value = fechaNac;
            }
            else
            {
                dtFechaNacimiento.Value = DateTime.Now;
            }

            cmbPerfil.SelectedValue = Convert.ToInt32(fila.Cells["IdPerfil"].Value ?? 0);
            cmbSucursal.SelectedValue = Convert.ToInt32(fila.Cells["IdSucursal"].Value ?? 0);
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            txtNombre.TextButton = "";
            txtApellido.TextButton = "";
            txtEmail.TextButton = "";
            txtTelefono.TextButton = "";
            txtDireccion.TextButton = "";
            txtPassword.TextButton = "";
            dtFechaNacimiento.Value = DateTime.Now;
            cmbPerfil.SelectedIndex = -1;
            cmbSucursal.SelectedIndex = -1;
            dgvUsuarios.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Usuario nuevoUsuario = new Usuario()
            {
                IdUsuario = idSeleccionado,
                Nombre = txtNombre.TextButton,
                Apellido = txtApellido.TextButton,
                Email = txtEmail.TextButton,
                Password = txtPassword.TextButton,
                Telefono = txtTelefono.TextButton,
                Direccion = txtDireccion.TextButton,
                FechaNacimiento = dtFechaNacimiento.Value.Date,
                IdPerfil = cmbPerfil.SelectedValue != null ? Convert.ToInt32(cmbPerfil.SelectedValue) : 0,
                IdSucursal = cmbSucursal.SelectedValue != null ? Convert.ToInt32(cmbSucursal.SelectedValue) : 0
            };

            string mensaje;
            bool resultado = _usuarioNegocio.Guardar(nuevoUsuario, out mensaje);

            if (resultado)
            {
                MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrilla();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                string mensaje;
                bool resultado = _usuarioNegocio.Eliminar(idSeleccionado, out mensaje);

                if (resultado)
                {
                    MessageBox.Show("Usuario eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrilla();
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}