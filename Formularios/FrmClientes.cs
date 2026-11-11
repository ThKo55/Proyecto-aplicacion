using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;
using System.Globalization;

namespace AorusMarket.Formularios
{
    public partial class FrmClientes : Form
    {
        private CyberTextBox txtDni, txtCuil, txtNombre, txtApellido, txtEmail, txtTelefono, txtCalle, txtAltura, txtBuscarDni;
        private DateTimePicker dtFechaNacimiento;
        private DataGridView dgvClientes;
        private CyberButton btnGuardar, btnEliminar, btnBuscar, btnLimpiar;
        private const string ColumnaDireccion = "IdDireccion";
        private int idSeleccionado = 0;
        private int idDireccionSeleccionada = 0;
        private  readonly ClienteNegocio _clienteNegocio = new ClienteNegocio();

        public FrmClientes()
        {
            InitializeComponent();
            this.ClientSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Clientes";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("CLIENTES", new Point(30, 20)));

            void AddLabelSobrio(string texto, Point ubicacion)
            {
                Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                this.Controls.Add(lbl);
            }

            // BUSCADOR ANCLADO A LA DERECHA
            AddLabelSobrio("BUSCAR POR DNI:", new Point(this.ClientSize.Width - 340, 20));
            txtBuscarDni = EstiloApp.CrearTextBox(new Point(this.ClientSize.Width - 340, 40), 200);
            txtBuscarDni.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtBuscarDni.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) CargarGrilla(EstiloApp.LeerTexto(txtBuscarDni)); };
            this.Controls.Add(txtBuscarDni);

            btnBuscar = EstiloApp.CrearBoton("BUSCAR", new Point(this.ClientSize.Width - 130, 30), 100, EstiloApp.Verde);
            btnBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBuscar.Click += (s, e) => CargarGrilla(EstiloApp.LeerTexto(txtBuscarDni));
            this.Controls.Add(btnBuscar);

            int x = 30, y = 80;

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
            AddLabelSobrio("CALLE", new Point(x, y));
            txtCalle = EstiloApp.CrearTextBox(new Point(x, y + 20), 220);
            this.Controls.Add(txtCalle);

            AddLabelSobrio("ALTURA", new Point(x + 240, y));
            txtAltura = EstiloApp.CrearTextBox(new Point(x + 240, y + 20), 100);
            this.Controls.Add(txtAltura);

            AddLabelSobrio("CUIL / CUIT", new Point(x + 360, y));
            txtCuil = EstiloApp.CrearTextBox(new Point(x + 360, y + 20), 270);
            this.Controls.Add(txtCuil);

            y += 60;
            btnGuardar = EstiloApp.CrearBoton("GUARDAR", new Point(x, y + 10), 120, EstiloApp.Verde);
            btnEliminar = EstiloApp.CrearBoton("ELIMINAR", new Point(x + 130, y + 10), 120, EstiloApp.RojoNeon);
            btnLimpiar = EstiloApp.CrearBoton("NUEVO / LIMPIAR", new Point(x + 260, y + 10), 140, EstiloApp.Gris);

            btnLimpiar.Click += (s, e) => LimpiarCampos();
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnEliminar);
            this.Controls.Add(btnLimpiar);

            y += 60;
            dgvClientes = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            dgvClientes.Columns.Add("IdCliente", "Id");
            dgvClientes.Columns.Add("Dni", "DNI");
            dgvClientes.Columns.Add("Nombre", "Nombre");
            dgvClientes.Columns.Add("Apellido", "Apellido");
            dgvClientes.Columns.Add("Email", "Email");
            dgvClientes.Columns.Add("Telefono", "Teléfono");
            dgvClientes.Columns.Add("Calle", "Calle");
            dgvClientes.Columns.Add("Altura", "Altura");
            dgvClientes.Columns.Add("FechaNacimiento", "F. Nac");
            dgvClientes.Columns.Add("CuilCuit", "CUIL/CUIT");
            dgvClientes.Columns.Add(ColumnaDireccion, "IdDireccion");

            dgvClientes.Columns["IdCliente"].Visible = false;
            dgvClientes.Columns[ColumnaDireccion].Visible = false;

            dgvClientes.SelectionChanged += DgvClientes_SelectionChanged;

            EstiloApp.EstilizarGrid(dgvClientes);
            this.Controls.Add(dgvClientes);

            CargarGrilla();
        }

        private void CargarGrilla(string filtro = "")
        {
            dgvClientes.SelectionChanged -= DgvClientes_SelectionChanged;
            dgvClientes.Rows.Clear();

            var lista = _clienteNegocio.Listar(filtro);
            foreach (var item in lista)
            {
                dgvClientes.Rows.Add(item.IdCliente, item.Dni, item.Nombre, item.Apellido, item.Email, item.Telefono, item.Calle, item.Altura, item.FechaNacimiento.HasValue ? item.FechaNacimiento.Value.ToString("dd/MM/yyyy") : "", item.CuilCuit, item.IdDireccion);
            }

            dgvClientes.ClearSelection();
            dgvClientes.SelectionChanged += DgvClientes_SelectionChanged;
        }

        private void DgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null) return;

            var fila = dgvClientes.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdCliente"].Value ?? 0);
            idDireccionSeleccionada = Convert.ToInt32(fila.Cells["IdDireccion"].Value ?? 0);

            EstiloApp.EscribirTexto(txtDni, fila.Cells["Dni"].Value?.ToString());
            EstiloApp.EscribirTexto(txtNombre, fila.Cells["Nombre"].Value?.ToString());
            EstiloApp.EscribirTexto(txtApellido, fila.Cells["Apellido"].Value?.ToString());
            EstiloApp.EscribirTexto(txtEmail, fila.Cells["Email"].Value?.ToString());
            EstiloApp.EscribirTexto(txtTelefono, fila.Cells["Telefono"].Value?.ToString());
            EstiloApp.EscribirTexto(txtCalle, fila.Cells["Calle"].Value?.ToString());
            EstiloApp.EscribirTexto(txtAltura, fila.Cells["Altura"].Value?.ToString());
            EstiloApp.EscribirTexto(txtCuil, fila.Cells["CuilCuit"].Value?.ToString());

            if (DateTime.TryParse(fila.Cells["FechaNacimiento"].Value?.ToString(), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime fechaNac))
                dtFechaNacimiento.Value = fechaNac;
            else
                dtFechaNacimiento.Value = DateTime.Now;
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            idDireccionSeleccionada = 0;
            EstiloApp.EscribirTexto(txtDni, "");
            EstiloApp.EscribirTexto(txtNombre, "");
            EstiloApp.EscribirTexto(txtApellido, "");
            EstiloApp.EscribirTexto(txtEmail, "");
            EstiloApp.EscribirTexto(txtTelefono, "");
            EstiloApp.EscribirTexto(txtCalle, "");
            EstiloApp.EscribirTexto(txtAltura, "");
            EstiloApp.EscribirTexto(txtCuil, "");
            EstiloApp.EscribirTexto(txtBuscarDni, "");
            dtFechaNacimiento.Value = DateTime.Now;
            dgvClientes.ClearSelection();
            CargarGrilla();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Cliente nuevo = new Cliente()
            {
                IdCliente = idSeleccionado,
                IdDireccion = idDireccionSeleccionada,
                Dni = EstiloApp.LeerTexto(txtDni),
                CuilCuit = EstiloApp.LeerTexto(txtCuil),
                Nombre = EstiloApp.LeerTexto(txtNombre),
                Apellido = EstiloApp.LeerTexto(txtApellido),
                Email = EstiloApp.LeerTexto(txtEmail),
                Telefono = EstiloApp.LeerTexto(txtTelefono),
                Calle = EstiloApp.LeerTexto(txtCalle),
                Altura = EstiloApp.LeerTexto(txtAltura),
                FechaNacimiento = dtFechaNacimiento.Value.Date
            };

            if (_clienteNegocio.Guardar(nuevo, out string mensaje))
            {
                MessageBox.Show("Cliente guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show(mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0) return;

            if (MessageBox.Show("¿Seguro que desea eliminar?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes && _clienteNegocio.Eliminar(idSeleccionado, out _))
            {
                    LimpiarCampos();
            }
        }
    }
}