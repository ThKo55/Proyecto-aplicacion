using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmClientes : Form
    {
        private CyberTextBox txtDni, txtCuil, txtNombre, txtApellido, txtEmail, txtTelefono, txtCalle, txtAltura, txtBuscarDni;
        private DateTimePicker dtFechaNacimiento;
        private DataGridView dgvClientes;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnBuscar;

        private int idSeleccionado = 0;
        private int idDireccionSeleccionada = 0;
        private ClienteNegocio _clienteNegocio = new ClienteNegocio();

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

            this.Controls.Add(EstiloApp.CrearLabel("BUSCAR POR DNI:", new Point(600, 20)));
            txtBuscarDni = EstiloApp.CrearTextBox(new Point(600, 40), 200);
            this.Controls.Add(txtBuscarDni);
            btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(810, 30), 100, EstiloApp.Verde);
            btnBuscar.Click += (s, e) => CargarGrilla(txtBuscarDni.TextButton);
            this.Controls.Add(btnBuscar);

            int x = 30, y = 80;

            // FILA 1 (Arreglado el eje Y)
            this.Controls.Add(EstiloApp.CrearLabel("DNI", new Point(x, y)));
            txtDni = EstiloApp.CrearTextBox(new Point(x, y + 20), 150);
            this.Controls.Add(txtDni);

            this.Controls.Add(EstiloApp.CrearLabel("NOMBRE", new Point(x + 170, y)));
            txtNombre = EstiloApp.CrearTextBox(new Point(x + 170, y + 20), 220);
            this.Controls.Add(txtNombre);

            this.Controls.Add(EstiloApp.CrearLabel("APELLIDO", new Point(x + 410, y)));
            txtApellido = EstiloApp.CrearTextBox(new Point(x + 410, y + 20), 220);
            this.Controls.Add(txtApellido);

            y += 60;
            // FILA 2
            this.Controls.Add(EstiloApp.CrearLabel("EMAIL", new Point(x, y)));
            txtEmail = EstiloApp.CrearTextBox(new Point(x, y + 20), 220);
            this.Controls.Add(txtEmail);

            this.Controls.Add(EstiloApp.CrearLabel("TELÉFONO", new Point(x + 240, y)));
            txtTelefono = EstiloApp.CrearTextBox(new Point(x + 240, y + 20), 180);
            this.Controls.Add(txtTelefono);

            this.Controls.Add(EstiloApp.CrearLabel("FECHA DE NACIMIENTO", new Point(x + 440, y)));
            dtFechaNacimiento = new DateTimePicker { Location = new Point(x + 440, y + 20), Size = new Size(190, 28), Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10F) };
            this.Controls.Add(dtFechaNacimiento);

            y += 60;
            // FILA 3
            this.Controls.Add(EstiloApp.CrearLabel("CALLE", new Point(x, y)));
            txtCalle = EstiloApp.CrearTextBox(new Point(x, y + 20), 220);
            this.Controls.Add(txtCalle);

            this.Controls.Add(EstiloApp.CrearLabel("ALTURA", new Point(x + 240, y)));
            txtAltura = EstiloApp.CrearTextBox(new Point(x + 240, y + 20), 100);
            this.Controls.Add(txtAltura);

            this.Controls.Add(EstiloApp.CrearLabel("CUIL / CUIT", new Point(x + 360, y)));
            txtCuil = EstiloApp.CrearTextBox(new Point(x + 360, y + 20), 270);
            this.Controls.Add(txtCuil);

            y += 60;
            // BOTONES
            btnNuevo = EstiloApp.CrearBoton("NUEVO", new Point(x, y + 10), 120, EstiloApp.Gris);
            btnGuardar = EstiloApp.CrearBoton("GUARDAR", new Point(x + 130, y + 10), 120, EstiloApp.Verde);
            btnEliminar = EstiloApp.CrearBoton("ELIMINAR", new Point(x + 260, y + 10), 120, EstiloApp.RojoNeon);

            btnNuevo.Click += (s, e) => LimpiarCampos();
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            this.Controls.Add(btnNuevo); this.Controls.Add(btnGuardar); this.Controls.Add(btnEliminar);
            // Botón que solo vacía las cajitas de texto en la pantalla
            CyberButton btnLimpiar = EstiloApp.CrearBoton("LIMPIAR", new Point(x + 390, y + 10), 120, EstiloApp.Gris);
            btnLimpiar.Click += (s, e) => LimpiarCampos();
            this.Controls.Add(btnLimpiar);

            y += 60;
            // GRILLA
            dgvClientes = new DataGridView { Location = new Point(x, y), Size = new Size(1000, this.ClientSize.Height - y - 30), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            EstiloApp.EstilizarGrid(dgvClientes);
            dgvClientes.Columns.Add("IdCliente", "Id"); dgvClientes.Columns.Add("Dni", "DNI"); dgvClientes.Columns.Add("Nombre", "Nombre"); dgvClientes.Columns.Add("Apellido", "Apellido"); dgvClientes.Columns.Add("Email", "Email"); dgvClientes.Columns.Add("Telefono", "Teléfono"); dgvClientes.Columns.Add("Calle", "Calle"); dgvClientes.Columns.Add("Altura", "Altura"); dgvClientes.Columns.Add("FechaNacimiento", "F. Nac"); dgvClientes.Columns.Add("CuilCuit", "CUIL/CUIT"); dgvClientes.Columns.Add("IdDireccion", "IdDireccion");
            dgvClientes.Columns["IdCliente"].Visible = false; dgvClientes.Columns["IdDireccion"].Visible = false;
            dgvClientes.SelectionChanged += DgvClientes_SelectionChanged;
            this.Controls.Add(dgvClientes);

            CargarGrilla();
        }

        private void CargarGrilla(string filtro = "")
        {
            // 1. APAGAMOS el evento
            dgvClientes.SelectionChanged -= DgvClientes_SelectionChanged;

            dgvClientes.Rows.Clear();
            var lista = _clienteNegocio.Listar(filtro);
            foreach (var item in lista)
            {
                dgvClientes.Rows.Add(item.IdCliente, item.Dni, item.Nombre, item.Apellido, item.Email, item.Telefono, item.Calle, item.Altura, item.FechaNacimiento.HasValue ? item.FechaNacimiento.Value.ToString("dd/MM/yyyy") : "", item.CuilCuit, item.IdDireccion);
            }

            // 2. Limpiamos la selección rebelde
            dgvClientes.ClearSelection();

            // 3. VOLVEMOS A PRENDER el evento
            dgvClientes.SelectionChanged += DgvClientes_SelectionChanged;
        }
        private void DgvClientes_SelectionChanged(object sender, EventArgs e) { if (dgvClientes.CurrentRow == null) return; var fila = dgvClientes.CurrentRow; idSeleccionado = Convert.ToInt32(fila.Cells["IdCliente"].Value ?? 0); idDireccionSeleccionada = Convert.ToInt32(fila.Cells["IdDireccion"].Value ?? 0); txtDni.TextButton = fila.Cells["Dni"].Value?.ToString(); txtNombre.TextButton = fila.Cells["Nombre"].Value?.ToString(); txtApellido.TextButton = fila.Cells["Apellido"].Value?.ToString(); txtEmail.TextButton = fila.Cells["Email"].Value?.ToString(); txtTelefono.TextButton = fila.Cells["Telefono"].Value?.ToString(); txtCalle.TextButton = fila.Cells["Calle"].Value?.ToString(); txtAltura.TextButton = fila.Cells["Altura"].Value?.ToString(); txtCuil.TextButton = fila.Cells["CuilCuit"].Value?.ToString(); if (DateTime.TryParse(fila.Cells["FechaNacimiento"].Value?.ToString(), out DateTime fechaNac)) dtFechaNacimiento.Value = fechaNac; else dtFechaNacimiento.Value = DateTime.Now; }
        private void LimpiarCampos() { idSeleccionado = 0; idDireccionSeleccionada = 0; txtDni.TextButton = ""; txtNombre.TextButton = ""; txtApellido.TextButton = ""; txtEmail.TextButton = ""; txtTelefono.TextButton = ""; txtCalle.TextButton = ""; txtAltura.TextButton = ""; txtCuil.TextButton = ""; txtBuscarDni.TextButton = ""; dtFechaNacimiento.Value = DateTime.Now; dgvClientes.ClearSelection(); CargarGrilla(); }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Cliente nuevo = new Cliente() { IdCliente = idSeleccionado, IdDireccion = idDireccionSeleccionada, Dni = txtDni.TextButton, CuilCuit = txtCuil.TextButton, Nombre = txtNombre.TextButton, Apellido = txtApellido.TextButton, Email = txtEmail.TextButton, Telefono = txtTelefono.TextButton, Calle = txtCalle.TextButton, Altura = txtAltura.TextButton, FechaNacimiento = dtFechaNacimiento.Value.Date };
            if (_clienteNegocio.Guardar(nuevo, out string mensaje)) { MessageBox.Show("Cliente guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information); LimpiarCampos(); } else MessageBox.Show(mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void BtnEliminar_Click(object sender, EventArgs e) { if (idSeleccionado == 0) return; if (MessageBox.Show("¿Seguro que desea eliminar?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes) { if (_clienteNegocio.Eliminar(idSeleccionado, out _)) LimpiarCampos(); } }
    }
}