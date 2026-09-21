using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls; // IMPORTANTE: Mantenido para reconocer CyberTextBox
using AorusMarket.Entidades; // Acceso a los moldes
using AorusMarket.Negocio;   // Acceso a las validaciones y lógica

namespace AorusMarket.Formularios
{
    public partial class FrmSucursales : Form
    {
        private CyberTextBox txtNombre, txtDireccion, txtTelefono;
        private DataGridView dgvSucursales;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        // Instanciamos el "cerebro" (capa de negocio)
        private SucursalNegocio _sucursalNegocio = new SucursalNegocio();

        public FrmSucursales()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Sucursales";
            ConstruirInterfaz(); // Interfaz visual original
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

            // LLAMADO NUEVO: Trae los datos reales de la BD al abrir la ventana
            CargarGrilla();
        }

        // METODO NUEVO: Consulta a través de la capa de Negocio y pinta la grilla
        private void CargarGrilla()
        {
            dgvSucursales.Rows.Clear(); // Limpia la tabla visual
            var listaSucursales = _sucursalNegocio.Listar(); // Pide la data real a SQL

            foreach (var item in listaSucursales)
            {
                // Llena las celdas en el mismo orden que agregamos las columnas arriba
                dgvSucursales.Rows.Add(item.IdSucursal, item.Nombre, item.Direccion, item.Telefono);
            }
            dgvSucursales.ClearSelection();
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
            // 1. Armamos un "paquete" (Objeto Sucursal) con lo que tipeó el usuario
            Sucursal nuevaSucursal = new Sucursal()
            {
                IdSucursal = idSeleccionado, // 0 = Crea uno nuevo / > 0 = Edita el seleccionado
                Nombre = txtNombre.TextButton,
                Direccion = txtDireccion.TextButton,
                Telefono = txtTelefono.TextButton
            };

            // 2. Se lo enviamos al "Cerebro" (Negocio) para que valide y guarde
            string mensaje;
            bool resultado = _sucursalNegocio.Guardar(nuevaSucursal, out mensaje);

            // 3. Revisamos qué nos respondió la capa de negocio
            if (resultado)
            {
                MessageBox.Show("Sucursal guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrilla(); // Volvemos a pedir los datos a la BD
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
                MessageBox.Show("Seleccione una sucursal de la tabla", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show("¿Seguro que desea eliminar esta sucursal?", "Confirmar Eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (resp == DialogResult.Yes)
            {
                // 1. Enviamos solo el ID al Negocio para procesar la baja lógica
                string mensaje;
                bool resultado = _sucursalNegocio.Eliminar(idSeleccionado, out mensaje);

                // 2. Evaluamos la respuesta
                if (resultado)
                {
                    MessageBox.Show("Sucursal eliminada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrilla(); // Actualiza la grilla
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}