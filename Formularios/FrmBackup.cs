using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Entidades;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmBackup : Form
    {
        private DateTimePicker dtpFechaProgramada;
        private CyberButton btnSolicitar, btnAprobar;
        private DataGridView dgvPendientes;

        // Instancias a la capa de Negocio
        private BackupNegocio _backupNegocio = new BackupNegocio();
        private UsuarioNegocio _usuarioNegocio = new UsuarioNegocio(); // Para validar al aprobador

        public FrmBackup()
        {
            InitializeComponent();
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Gestión de Backups";

            ConstruirInterfaz();
            CargarPendientes();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("SISTEMA DE BACKUP Y RESTORE", new Point(30, 20)));

            // --- 1. SECCIÓN: SOLICITAR BACKUP ---
            this.Controls.Add(EstiloApp.CrearLabel("NUEVA SOLICITUD DE BACKUP", new Point(30, 80)));

            this.Controls.Add(EstiloApp.CrearLabel("Fecha y Hora Programada:", new Point(30, 110)));

            dtpFechaProgramada = new DateTimePicker
            {
                Location = new Point(220, 105),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 11F),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm", // Permite elegir fecha y hora
                MinDate = DateTime.Now // No permite fechas en el pasado
            };
            this.Controls.Add(dtpFechaProgramada);

            btnSolicitar = EstiloApp.CrearBoton("SOLICITAR BACKUP", new Point(490, 100), 200, EstiloApp.Verde);
            btnSolicitar.Click += BtnSolicitar_Click;
            this.Controls.Add(btnSolicitar);

            // --- 2. SECCIÓN: HISTORIAL DE PENDIENTES ---
            this.Controls.Add(EstiloApp.CrearLabel("SOLICITUDES PENDIENTES DE APROBACIÓN", new Point(30, 170)));

            dgvPendientes = new DataGridView
            {
                Location = new Point(30, 200),
                Size = new Size(800, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            EstiloApp.EstilizarGrid(dgvPendientes);
            this.Controls.Add(dgvPendientes);

            // --- 3. SECCIÓN: APROBAR ---
            btnAprobar = EstiloApp.CrearBoton("APROBAR Y EJECUTAR BACKUP", new Point(30, 420), 300, EstiloApp.RojoNeon);
            btnAprobar.Click += BtnAprobar_Click;
            this.Controls.Add(btnAprobar);
        }

        private void CargarPendientes()
        {
            try
            {
                // Llamamos al SP_ListarBackupsPendientes que creamos en la base de datos
                DataTable tabla = _backupNegocio.ListarPendientes();
                dgvPendientes.DataSource = tabla;

                // Ocultamos el ID para que quede más limpio
                if (dgvPendientes.Columns.Contains("Id"))
                    dgvPendientes.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pendientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSolicitar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validación básica de roles (Opcional: Solo Admins o Gerentes pueden pedir)
                if (SesionActual.IdPerfil == 2) // Asumiendo que 2 es Cajero
                {
                    MessageBox.Show("Los cajeros no tienen permiso para solicitar Backups.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                BackupSolicitud nuevaSolicitud = new BackupSolicitud
                {
                    SolicitanteId = SesionActual.IdUsuario,
                    FechaProgramada = dtpFechaProgramada.Value
                };

                bool exito = _backupNegocio.Solicitar(nuevaSolicitud);

                if (exito)
                {
                    MessageBox.Show("Solicitud de Backup registrada con éxito. Esperando aprobación cruzada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarPendientes(); // Refrescar la grilla
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al Solicitar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAprobar_Click(object sender, EventArgs e)
        {
            if (dgvPendientes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una solicitud pendiente de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extraemos los datos de la fila seleccionada
            int idSolicitud = Convert.ToInt32(dgvPendientes.CurrentRow.Cells["Id"].Value);
            string nombreSolicitante = dgvPendientes.CurrentRow.Cells["Solicitante"].Value.ToString();

            // 1. Mostrar advertencia
            DialogResult diag = MessageBox.Show($"¿Desea aprobar la solicitud de backup de {nombreSolicitante}?\n\nSe requerirán las credenciales de un administrador distinto al solicitante.", "Autorización Requerida", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (diag == DialogResult.No) return;

            // 2. Pedir credenciales cruzadas mediante un formulario emergente temporal
            Usuario aprobador = PedirCredencialesAprobador();

            if (aprobador == null)
            {
                MessageBox.Show("Aprobación cancelada o credenciales incorrectas.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Mandamos a ejecutar
            try
            {
                // Ruta para el contenedor de Docker
                string rutaArchivo = $"/var/opt/mssql/data/AorusMarket_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";

                // El negocio validará que el SolicitanteId != AprobadorId
                bool exito = _backupNegocio.Aprobar(idSolicitud, SesionActual.IdUsuario, aprobador.IdUsuario, rutaArchivo);

                if (exito)
                {
                    MessageBox.Show($"¡Backup aprobado por {aprobador.Nombre} y ejecutado correctamente!\nGuardado en: {rutaArchivo}", "Backup Completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarPendientes(); // Refrescar grilla
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la ejecución: " + ex.Message, "Error de Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método auxiliar que crea una ventanita para pedir el Usuario y Contraseña
        private Usuario PedirCredencialesAprobador()
        {
            Usuario aprobador = null;

            using (Form frmLogin = new Form())
            {
                frmLogin.Width = 350;
                frmLogin.Height = 250;
                frmLogin.Text = "Credenciales Cruzadas";
                frmLogin.StartPosition = FormStartPosition.CenterParent;
                frmLogin.FormBorderStyle = FormBorderStyle.FixedDialog;
                frmLogin.MaximizeBox = false;
                frmLogin.MinimizeBox = false;
                frmLogin.BackColor = Color.White;

                Label lblCorreo = new Label { Text = "Correo del Aprobador:", Left = 20, Top = 20, Width = 200 };
                TextBox txtCorreo = new TextBox { Left = 20, Top = 45, Width = 290 };

                Label lblClave = new Label { Text = "Contraseña:", Left = 20, Top = 80, Width = 200 };
                TextBox txtClave = new TextBox { Left = 20, Top = 105, Width = 290, PasswordChar = '*' };

                System.Windows.Forms.Button btnConfirmar = new System.Windows.Forms.Button { Text = "Validar", Left = 120, Top = 150, Width = 100, DialogResult = DialogResult.OK, BackColor = Color.LightGreen };

                frmLogin.Controls.Add(lblCorreo);
                frmLogin.Controls.Add(txtCorreo);
                frmLogin.Controls.Add(lblClave);
                frmLogin.Controls.Add(txtClave);
                frmLogin.Controls.Add(btnConfirmar);
                frmLogin.AcceptButton = btnConfirmar;

                if (frmLogin.ShowDialog() == DialogResult.OK)
                {
                    string mensajeLogin = "";
                    // Agregamos el parámetro "out mensajeLogin" que exigía tu arquitectura
                    aprobador = _usuarioNegocio.Login(txtCorreo.Text, txtClave.Text, out mensajeLogin);

                    if (aprobador == null)
                    {
                        MessageBox.Show(mensajeLogin, "Credenciales Incorrectas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return aprobador;
        }
    }
}
