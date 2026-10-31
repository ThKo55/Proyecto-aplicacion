using System;
using System.Drawing;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;
using WinForms = System.Windows.Forms;

namespace AorusMarket.Formularios
{
    public partial class FrmSucursales : WinForms.Form
    {
        private CyberTextBox txtNombre, txtDireccion, txtTelefono;
        private WinForms.DataGridView dgvSucursales;
        private CyberButton btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private int idSeleccionado = 0;

        private SucursalNegocio _sucursalNegocio = new SucursalNegocio();

        public FrmSucursales()
        {
            InitializeComponent();

            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Sucursales";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("SUCURSALES", new Point(30, 20)));

            void AddLabelSobrio(string texto, Point ubicacion)
            {
                WinForms.Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                lbl.ForeColor = Color.LightGray;
                this.Controls.Add(lbl);
            }

            int x = 30, y = 90;

            AddLabelSobrio("NOMBRE", new Point(x, y));
            txtNombre = EstiloApp.CrearTextBox(new Point(x, y + 20), 220);
            this.Controls.Add(txtNombre);

            AddLabelSobrio("DIRECCIÓN", new Point(x + 240, y));
            txtDireccion = EstiloApp.CrearTextBox(new Point(x + 240, y + 20), 220);
            this.Controls.Add(txtDireccion);

            AddLabelSobrio("TELÉFONO", new Point(x + 480, y));
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
            dgvSucursales = new WinForms.DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(740, this.ClientSize.Height - y - 30),
                Anchor = WinForms.AnchorStyles.Top | WinForms.AnchorStyles.Left | WinForms.AnchorStyles.Right | WinForms.AnchorStyles.Bottom
            };

            dgvSucursales.Columns.Add("IdSucursal", "Id");
            dgvSucursales.Columns.Add("Nombre", "Nombre");
            dgvSucursales.Columns.Add("Direccion", "Dirección");
            dgvSucursales.Columns.Add("Telefono", "Teléfono");
            dgvSucursales.Columns["IdSucursal"].Visible = false;
            dgvSucursales.SelectionChanged += DgvSucursales_SelectionChanged;

            AplicarDisenoSobrioGrid(dgvSucursales);
            this.Controls.Add(dgvSucursales);

            CargarGrilla();
        }

        private void AplicarDisenoSobrioGrid(WinForms.DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.FromArgb(18, 18, 18);
            grid.BorderStyle = WinForms.BorderStyle.None;
            grid.CellBorderStyle = WinForms.DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(40, 40, 40);
            grid.RowHeadersVisible = false;

            WinForms.DataGridViewCellStyle estiloCabecera = new WinForms.DataGridViewCellStyle();
            estiloCabecera.BackColor = Color.FromArgb(28, 28, 28);
            estiloCabecera.ForeColor = Color.LightGray;
            estiloCabecera.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            estiloCabecera.Alignment = WinForms.DataGridViewContentAlignment.MiddleLeft;
            estiloCabecera.SelectionBackColor = Color.FromArgb(28, 28, 28);
            grid.ColumnHeadersDefaultCellStyle = estiloCabecera;
            grid.ColumnHeadersHeightSizeMode = WinForms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;

            WinForms.DataGridViewCellStyle estiloFila = new WinForms.DataGridViewCellStyle();
            estiloFila.BackColor = Color.FromArgb(18, 18, 18);
            estiloFila.ForeColor = Color.WhiteSmoke;
            estiloFila.Font = new Font("Segoe UI", 10F);
            estiloFila.SelectionBackColor = Color.FromArgb(220, 53, 69);
            estiloFila.SelectionForeColor = Color.White;
            grid.DefaultCellStyle = estiloFila;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 22, 22);
            grid.RowTemplate.Height = 35;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = WinForms.DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = WinForms.DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void CargarGrilla()
        {
            dgvSucursales.SelectionChanged -= DgvSucursales_SelectionChanged;

            dgvSucursales.Rows.Clear();
            var listaSucursales = _sucursalNegocio.Listar();

            foreach (var item in listaSucursales)
            {
                dgvSucursales.Rows.Add(item.IdSucursal, item.Nombre, item.Direccion, item.Telefono);
            }
            dgvSucursales.ClearSelection();

            dgvSucursales.SelectionChanged += DgvSucursales_SelectionChanged;
        }

        private void DgvSucursales_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSucursales.CurrentRow == null) return;
            var fila = dgvSucursales.CurrentRow;

            idSeleccionado = Convert.ToInt32(fila.Cells["IdSucursal"].Value ?? 0);

            string valNombre = fila.Cells["Nombre"].Value?.ToString() ?? "";
            string valDireccion = fila.Cells["Direccion"].Value?.ToString() ?? "";
            string valTelefono = fila.Cells["Telefono"].Value?.ToString() ?? "";

            txtNombre.Text = valNombre;
            txtNombre.TextButton = valNombre;

            txtDireccion.Text = valDireccion;
            txtDireccion.TextButton = valDireccion;

            txtTelefono.Text = valTelefono;
            txtTelefono.TextButton = valTelefono;
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;

            txtNombre.Text = "";
            txtNombre.TextButton = "";

            txtDireccion.Text = "";
            txtDireccion.TextButton = "";

            txtTelefono.Text = "";
            txtTelefono.TextButton = "";

            dgvSucursales.ClearSelection();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string nombreIngresado = string.IsNullOrWhiteSpace(txtNombre.Text) ? txtNombre.TextButton : txtNombre.Text;
            string direccionIngresada = string.IsNullOrWhiteSpace(txtDireccion.Text) ? txtDireccion.TextButton : txtDireccion.Text;
            string telefonoIngresado = string.IsNullOrWhiteSpace(txtTelefono.Text) ? txtTelefono.TextButton : txtTelefono.Text;

            Sucursal nuevaSucursal = new Sucursal()
            {
                IdSucursal = idSeleccionado,
                Nombre = nombreIngresado?.Trim(),
                Direccion = direccionIngresada?.Trim(),
                Telefono = telefonoIngresado?.Trim()
            };

            string mensaje;
            bool resultado = _sucursalNegocio.Guardar(nuevaSucursal, out mensaje);

            if (resultado)
            {
                WinForms.MessageBox.Show("Sucursal guardada correctamente.", "Éxito", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrilla();
            }
            else
            {
                WinForms.MessageBox.Show(mensaje, "Error al guardar", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                WinForms.MessageBox.Show("Seleccione una sucursal de la tabla", "Aviso",
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
                return;
            }

            var resp = WinForms.MessageBox.Show("¿Seguro que desea eliminar esta sucursal?", "Confirmar Eliminación",
                WinForms.MessageBoxButtons.YesNo, WinForms.MessageBoxIcon.Exclamation);

            if (resp == WinForms.DialogResult.Yes)
            {
                string mensaje;
                bool resultado = _sucursalNegocio.Eliminar(idSeleccionado, out mensaje);

                if (resultado)
                {
                    WinForms.MessageBox.Show("Sucursal eliminada.", "Éxito", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrilla();
                }
                else
                {
                    WinForms.MessageBox.Show(mensaje, "Error", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
                }
            }
        }
    }
}