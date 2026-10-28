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
        private CyberTextBox txtDni, txtNombre, txtApellido, txtEmail, txtPassword, txtTelefono, txtCalle, txtAltura, txtBuscarDni;
        private DateTimePicker dtFechaNacimiento;
        private ComboBox cmbPerfil, cmbSucursal;
        private DataGridView dgvUsuarios;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnBuscar;

        private int idSeleccionado = 0;
        private int idDireccionSeleccionada = 0;
        private UsuarioNegocio _usuarioNegocio = new UsuarioNegocio();

        public FrmUsuarios()
        {
            InitializeComponent();

            // FONDO SOBRIO OSCURO
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Usuarios";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("USUARIOS", new Point(30, 20)));

            // Función local para crear labels con texto gris claro (sobrio)
            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                lbl.ForeColor = Color.LightGray;
                this.Controls.Add(lbl);
            }

            // BUSCADOR
            AddLabelSobrio("BUSCAR POR DNI:", new Point(600, 20));
            txtBuscarDni = EstiloApp.CrearTextBox(new Point(600, 40), 200);
            this.Controls.Add(txtBuscarDni);

            btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(810, 30), 100, EstiloApp.Verde);
            btnBuscar.Click += (s, e) => CargarGrilla(txtBuscarDni.TextButton);
            this.Controls.Add(btnBuscar);

            int x = 30, y = 80;

            // FILA 1
            AddLabelSobrio("DNI", new Point(x, y));
            txtDni = EstiloApp.CrearTextBox(new Point(x, y + 20), 150);
            this.Controls.Add(txtDni);

            AddLabelSobrio("NOMBRE", new Point(x + 170, y));
            txtNombre = EstiloApp.CrearTextBox(new Point(x + 170, y + 20), 220);
            this.Controls.Add(txtNombre);

            AddLabelSobrio("APELLIDO", new Point(x + 410, y));
            txtApellido = EstiloApp.CrearTextBox(new Point(x + 410, y + 20), 220);
            this.Controls.Add(txtApellido);

            y += 60;

            // FILA 2
            AddLabelSobrio("EMAIL", new Point(x, y));
            txtEmail = EstiloApp.CrearTextBox(new Point(x, y + 20), 220);
            this.Controls.Add(txtEmail);

            AddLabelSobrio("TELÉFONO", new Point(x + 240, y));
            txtTelefono = EstiloApp.CrearTextBox(new Point(x + 240, y + 20), 180);
            this.Controls.Add(txtTelefono);

            AddLabelSobrio("FECHA DE NACIMIENTO", new Point(x + 440, y));
            dtFechaNacimiento = new DateTimePicker { Location = new Point(x + 440, y + 20), Size = new Size(190, 28), Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10F) };
            this.Controls.Add(dtFechaNacimiento);

            y += 60;

            // FILA 3
            AddLabelSobrio("CALLE", new Point(x, y));
            txtCalle = EstiloApp.CrearTextBox(new Point(x, y + 20), 220);
            this.Controls.Add(txtCalle);

            AddLabelSobrio("ALTURA", new Point(x + 240, y));
            txtAltura = EstiloApp.CrearTextBox(new Point(x + 240, y + 20), 100);
            this.Controls.Add(txtAltura);

            AddLabelSobrio("CONTRASEÑA", new Point(x + 360, y));
            txtPassword = EstiloApp.CrearTextBox(new Point(x + 360, y + 20), 270, true);
            this.Controls.Add(txtPassword);

            y += 60;

            // FILA 4
            AddLabelSobrio("PERFIL", new Point(x, y));
            cmbPerfil = EstiloApp.CrearComboBox(new Point(x, y + 20), 200);
            this.Controls.Add(cmbPerfil);

            AddLabelSobrio("SUCURSAL", new Point(x + 220, y));
            cmbSucursal = EstiloApp.CrearComboBox(new Point(x + 220, y + 20), 200);
            this.Controls.Add(cmbSucursal);

            // BOTONES
            btnNuevo = EstiloApp.CrearBoton("NUEVO", new Point(x + 440, y + 10), 120, EstiloApp.Gris);
            btnGuardar = EstiloApp.CrearBoton("GUARDAR", new Point(x + 570, y + 10), 120, EstiloApp.Verde);
            btnEliminar = EstiloApp.CrearBoton("ELIMINAR", new Point(x + 700, y + 10), 120, EstiloApp.RojoNeon);

            btnNuevo.Click += (s, e) => LimpiarCampos();
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            this.Controls.Add(btnNuevo);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnEliminar);

            CyberButton btnLimpiar = EstiloApp.CrearBoton("LIMPIAR", new Point(x + 830, y + 10), 120, EstiloApp.Gris);
            btnLimpiar.Click += (s, e) => LimpiarCampos();
            this.Controls.Add(btnLimpiar);

            y += 60;

            // GRILLA
            dgvUsuarios = new DataGridView { Location = new Point(x, y), Size = new Size(1000, this.ClientSize.Height - y - 30), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };

            dgvUsuarios.Columns.Add("IdUsuario", "Id");
            dgvUsuarios.Columns.Add("Dni", "DNI");
            dgvUsuarios.Columns.Add("Nombre", "Nombre");
            dgvUsuarios.Columns.Add("Apellido", "Apellido");
            dgvUsuarios.Columns.Add("Email", "Email");
            dgvUsuarios.Columns.Add("Telefono", "Teléfono");
            dgvUsuarios.Columns.Add("Calle", "Calle");
            dgvUsuarios.Columns.Add("Altura", "Altura");
            dgvUsuarios.Columns.Add("FechaNacimiento", "F. Nacimiento");
            dgvUsuarios.Columns.Add("Perfil", "Perfil");
            dgvUsuarios.Columns.Add("Sucursal", "Sucursal");
            dgvUsuarios.Columns.Add("IdPerfil", "IdPerfil");
            dgvUsuarios.Columns.Add("IdSucursal", "IdSucursal");
            dgvUsuarios.Columns.Add("IdDireccion", "IdDireccion");

            dgvUsuarios.Columns["IdUsuario"].Visible = false;
            dgvUsuarios.Columns["IdPerfil"].Visible = false;
            dgvUsuarios.Columns["IdSucursal"].Visible = false;
            dgvUsuarios.Columns["IdDireccion"].Visible = false;

            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;

            // APLICAMOS EL DISEÑO PREMIUM A LA GRILLA
            AplicarDisenoSobrioGrid(dgvUsuarios);
            this.Controls.Add(dgvUsuarios);

            CargarCombos();
            CargarGrilla();
        }

        // --- MAGIA VISUAL: NUEVA GRILLA SOBRIA ---
        private void AplicarDisenoSobrioGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.FromArgb(18, 18, 18);
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(40, 40, 40);
            grid.RowHeadersVisible = false;

            DataGridViewCellStyle estiloCabecera = new DataGridViewCellStyle();
            estiloCabecera.BackColor = Color.FromArgb(28, 28, 28);
            estiloCabecera.ForeColor = Color.LightGray;
            estiloCabecera.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            estiloCabecera.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloCabecera.SelectionBackColor = Color.FromArgb(28, 28, 28);
            grid.ColumnHeadersDefaultCellStyle = estiloCabecera;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;

            DataGridViewCellStyle estiloFila = new DataGridViewCellStyle();
            estiloFila.BackColor = Color.FromArgb(18, 18, 18);
            estiloFila.ForeColor = Color.WhiteSmoke;
            estiloFila.Font = new Font("Segoe UI", 10F);
            estiloFila.SelectionBackColor = Color.FromArgb(220, 53, 69); // Rojo Aorus
            estiloFila.SelectionForeColor = Color.White;
            grid.DefaultCellStyle = estiloFila;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 22, 22);
            grid.RowTemplate.Height = 35;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        private void CargarGrilla(string filtro = "")
        {
            dgvUsuarios.SelectionChanged -= DgvUsuarios_SelectionChanged;

            dgvUsuarios.Rows.Clear();
            var listaUsuarios = _usuarioNegocio.Listar(filtro);
            foreach (var item in listaUsuarios)
            {
                dgvUsuarios.Rows.Add(item.IdUsuario, item.Dni, item.Nombre, item.Apellido, item.Email, item.Telefono, item.Calle, item.Altura, item.FechaNacimiento.HasValue ? item.FechaNacimiento.Value.ToString("dd/MM/yyyy") : "", item.NombrePerfil, item.NombreSucursal, item.IdPerfil, item.IdSucursal, item.IdDireccion);
            }

            dgvUsuarios.ClearSelection();
            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;
        }

        private void DgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;

            var fila = dgvUsuarios.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdUsuario"].Value ?? 0);
            idDireccionSeleccionada = Convert.ToInt32(fila.Cells["IdDireccion"].Value ?? 0);
            txtDni.TextButton = fila.Cells["Dni"].Value?.ToString();
            txtNombre.TextButton = fila.Cells["Nombre"].Value?.ToString();
            txtApellido.TextButton = fila.Cells["Apellido"].Value?.ToString();
            txtEmail.TextButton = fila.Cells["Email"].Value?.ToString();
            txtTelefono.TextButton = fila.Cells["Telefono"].Value?.ToString();
            txtCalle.TextButton = fila.Cells["Calle"].Value?.ToString();
            txtAltura.TextButton = fila.Cells["Altura"].Value?.ToString();
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
            idDireccionSeleccionada = 0;
            txtDni.TextButton = "";
            txtNombre.TextButton = "";
            txtApellido.TextButton = "";
            txtEmail.TextButton = "";
            txtTelefono.TextButton = "";
            txtCalle.TextButton = "";
            txtAltura.TextButton = "";
            txtPassword.TextButton = "";
            txtBuscarDni.TextButton = "";
            dtFechaNacimiento.Value = DateTime.Now;
            cmbPerfil.SelectedIndex = -1;
            cmbSucursal.SelectedIndex = -1;
            dgvUsuarios.ClearSelection();
            CargarGrilla();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Usuario nuevoUsuario = new Usuario()
            {
                IdUsuario = idSeleccionado,
                IdDireccion = idDireccionSeleccionada,
                Dni = txtDni.TextButton,
                Nombre = txtNombre.TextButton,
                Apellido = txtApellido.TextButton,
                Email = txtEmail.TextButton,
                Password = txtPassword.TextButton,
                Telefono = txtTelefono.TextButton,
                Calle = txtCalle.TextButton,
                Altura = txtAltura.TextButton,
                FechaNacimiento = dtFechaNacimiento.Value.Date,
                IdPerfil = cmbPerfil.SelectedValue != null ? Convert.ToInt32(cmbPerfil.SelectedValue) : 0,
                IdSucursal = cmbSucursal.SelectedValue != null ? Convert.ToInt32(cmbSucursal.SelectedValue) : 0
            };

            if (_usuarioNegocio.Guardar(nuevoUsuario, out string mensaje))
            {
                MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0) return;

            if (MessageBox.Show("¿Seguro que desea eliminar este usuario?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                if (_usuarioNegocio.Eliminar(idSeleccionado, out string mensaje))
                    LimpiarCampos();
                else
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}