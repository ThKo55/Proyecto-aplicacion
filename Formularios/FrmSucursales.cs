using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls; // IMPORTANTE: Agregado para que reconozca los nuevos controles

namespace AorusMarket.Formularios
{
    public partial class FrmSucursales : Form
    {
        // 1. Cambiamos TextBox por CyberTextBox y Button por CyberButton
        private CyberTextBox txtNombre, txtDireccion, txtTelefono;
        private DataGridView dgvSucursales;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        public FrmSucursales()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Sucursales";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("SUCURSALES", new Point(30, 20)));

            int x = 30, y = 90;
            this.Controls.Add(EstiloApp.CrearLabel("NOMBRE", new Point(x, y)));
            txtNombre = EstiloApp.CrearTextBox(new Point(x, y + 20), 220);
            this.Controls.Add(txtNombre);

            this.Controls.Add(EstiloApp.CrearLabel("DIRECCIÓN", new Point(x + 240, y)));
            txtDireccion = EstiloApp.CrearTextBox(new Point(x + 240, y + 20), 220);
            this.Controls.Add(txtDireccion);

            this.Controls.Add(EstiloApp.CrearLabel("TELÉFONO", new Point(x + 480, y)));
            txtTelefono = EstiloApp.CrearTextBox(new Point(x + 480, y + 20), 220);
            this.Controls.Add(txtTelefono);

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
            dgvSucursales = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(740, this.ClientSize.Height - y - 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            EstiloApp.EstilizarGrid(dgvSucursales);
            dgvSucursales.Columns.Add("IdSucursal", "Id");
            dgvSucursales.Columns.Add("Nombre", "Nombre");
            dgvSucursales.Columns.Add("Direccion", "Dirección");
            dgvSucursales.Columns.Add("Telefono", "Teléfono");
            dgvSucursales.Columns["IdSucursal"].Visible = false;
            dgvSucursales.SelectionChanged += DgvSucursales_SelectionChanged;
            this.Controls.Add(dgvSucursales);
        }

        private void DgvSucursales_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSucursales.CurrentRow == null) return;
            var fila = dgvSucursales.CurrentRow;
            idSeleccionado = Convert.ToInt32(fila.Cells["IdSucursal"].Value ?? 0);
            txtNombre.TextButton = fila.Cells["Nombre"].Value?.ToString();
            txtDireccion.TextButton = fila.Cells["Direccion"].Value?.ToString();
            txtTelefono.TextButton = fila.Cells["Telefono"].Value?.ToString();
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            txtNombre.TextButton = "";
            txtDireccion.TextButton = "";
            txtTelefono.TextButton = "";
            dgvSucursales.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.TextButton) || string.IsNullOrWhiteSpace(txtDireccion.TextButton))
            {
                MessageBox.Show("Debe completar Nombre y Dirección", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Sucursal guardada (falta conectar la base de datos)", "AorusMarket");
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione una sucursal de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var resp = MessageBox.Show("¿Seguro que desea eliminar esta sucursal?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (resp == DialogResult.Yes)
            {
                // TODO: eliminar en la base de datos (SucursalDAL)
                MessageBox.Show("Sucursal eliminada (falta conectar la base de datos)", "AorusMarket");
                LimpiarCampos();
            }
        }
    }
}