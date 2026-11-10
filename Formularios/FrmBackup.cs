using AorusMarket.Entidades;
using AorusMarket.Negocio;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using System;
using System.Drawing;
using System.IO;
using WinForms = System.Windows.Forms;
using System.Threading.Tasks;

namespace AorusMarket.Formularios
{
    public partial class FrmBackup : WinForms.Form
    {
        private WinForms.ComboBox cmbDestino;
        private CyberTextBox txtRutaDestino;
        private CyberButton btnExaminar, btnCrearBackup, btnRestaurarBackup;
        private WinForms.CheckBox chkAutomatico;
        private WinForms.DateTimePicker dtpHoraAuto;
        private WinForms.DataGridView dgvBackupsLocales;

        private ConfigBackup _config;
        private CyberButton btnGuardarConfig;
        private const bool RequierePerfilDistinto = true; // true = Admin <-> Gerente; false = cualquier Admin/Gerente distinto
        private BackupNegocio _backupNegocio = new BackupNegocio();

        public FrmBackup()
        {
            InitializeComponent();

            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Backups y Seguridad";
            this.MinimumSize = new Size(950, 600);
            ConstruirInterfaz();
            CargarConfiguracion();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("RESPALDO Y RECUPERACIÓN DE DATOS", new Point(30, 20)));

            void AddLabelSobrio(string texto, Point ubicacion)
            {
                WinForms.Label lbl = EstiloApp.CrearLabel(texto, ubicacion);
                lbl.ForeColor = Color.LightGray;
                this.Controls.Add(lbl);
            }

            int margenX = 30;
            int y = 75;

            // FILA 1: DESTINO, RUTA Y BOTÓN EXPLORAR (Adaptable al ancho)
            AddLabelSobrio("DESTINO:", new Point(margenX, y));
            cmbDestino = new WinForms.ComboBox
            {
                Location = new Point(margenX, y + 22),
                Size = new Size(200, 28),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = WinForms.ComboBoxStyle.DropDownList
            };
            cmbDestino.Items.AddRange(new object[] { "Carpeta Interna / PC", "Google Drive (Nube)", "Servidor / Red Local (NAS)" });
            cmbDestino.SelectedIndex = 0;
            this.Controls.Add(cmbDestino);

            int xRuta = margenX + 220;
            AddLabelSobrio("RUTA DE ALMACENAMIENTO:", new Point(xRuta, y));

            // Ancho responsivo para la caja de texto
            int anchoRuta = Math.Max(260, this.ClientSize.Width - xRuta - 160);
            txtRutaDestino = EstiloApp.CrearTextBox(new Point(xRuta, y + 20), anchoRuta);
            txtRutaDestino.Anchor = WinForms.AnchorStyles.Top | WinForms.AnchorStyles.Left | WinForms.AnchorStyles.Right;
            txtRutaDestino.Text = @"C:\AorusMarket\Backups\";
            txtRutaDestino.TextButton = @"C:\AorusMarket\Backups\";
            this.Controls.Add(txtRutaDestino);

            btnExaminar = EstiloApp.CrearBoton("EXPLORAR", new Point(this.ClientSize.Width - margenX - 110, y + 19), 110, EstiloApp.Gris);
            btnExaminar.Anchor = WinForms.AnchorStyles.Top | WinForms.AnchorStyles.Right;
            btnExaminar.Click += BtnExaminar_Click;
            this.Controls.Add(btnExaminar);

            y += 65;

            // FILA 2: AUTOMATIZACIÓN Y ACCIÓN INMEDIATA
            chkAutomatico = new WinForms.CheckBox
            {
                Text = "Activar Respaldo Automático Diario a las:",
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Location = new Point(margenX, y + 8),
                AutoSize = true,
                Checked = false
            };
            this.Controls.Add(chkAutomatico);

            dtpHoraAuto = new WinForms.DateTimePicker
            {
                Format = WinForms.DateTimePickerFormat.Time,
                ShowUpDown = true,
                Location = new Point(margenX + 320, y + 6),
                Size = new Size(90, 26),
                Value = DateTime.Today.AddHours(1)
            };
            this.Controls.Add(dtpHoraAuto);

            btnCrearBackup = EstiloApp.CrearBoton("GENERAR BACKUP AHORA", new Point(margenX + 430, y), 220, EstiloApp.Verde);
            btnCrearBackup.Click += BtnCrearBackup_Click;
            this.Controls.Add(btnCrearBackup);

            btnGuardarConfig = EstiloApp.CrearBoton("GUARDAR CONFIG.", new Point(margenX + 670, y), 180, EstiloApp.Gris);
            btnGuardarConfig.Click += BtnGuardarConfig_Click;
            this.Controls.Add(btnGuardarConfig);

            y += 60;

            // FILA 3: HISTORIAL DE BACKUPS (GRILLA RESPONSIVA)
            AddLabelSobrio("HISTORIAL DE RESPALDOS DISPONIBLES (Conserva los últimos 5)", new Point(margenX, y));
            y += 25;

            // Cálculo dinámico para la grilla: reserva 70px abajo para el botón de restaurar
            int anchoGrilla = this.ClientSize.Width - (margenX * 2);
            int altoGrilla = Math.Max(180, this.ClientSize.Height - y - 75);

            dgvBackupsLocales = new WinForms.DataGridView
            {
                Location = new Point(margenX, y),
                Size = new Size(anchoGrilla, altoGrilla),
                Anchor = WinForms.AnchorStyles.Top | WinForms.AnchorStyles.Left | WinForms.AnchorStyles.Right | WinForms.AnchorStyles.Bottom
            };

            dgvBackupsLocales.Columns.Add("Archivo", "Nombre del Archivo");
            dgvBackupsLocales.Columns.Add("Ruta", "Ruta Completa");
            dgvBackupsLocales.Columns.Add("Fecha", "Fecha de Creación");
            dgvBackupsLocales.Columns["Ruta"].Visible = false;

            AplicarDisenoSobrioGrid(dgvBackupsLocales);
            this.Controls.Add(dgvBackupsLocales);

            // FILA 4: BOTÓN INFERIOR ANCLADO
            btnRestaurarBackup = EstiloApp.CrearBoton(
                "RESTAURAR SISTEMA (CONTROL CRUZADO)",
                new Point(margenX, this.ClientSize.Height - 50),
                340,
                EstiloApp.RojoNeon
            );
            btnRestaurarBackup.Anchor = WinForms.AnchorStyles.Bottom | WinForms.AnchorStyles.Left;
            btnRestaurarBackup.Click += BtnRestaurarBackup_Click;
            this.Controls.Add(btnRestaurarBackup);

            CargarListaBackupsLocales();
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

        private void BtnExaminar_Click(object sender, EventArgs e)
        {
            using (WinForms.FolderBrowserDialog fbd = new WinForms.FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == WinForms.DialogResult.OK)
                {
                    txtRutaDestino.Text = fbd.SelectedPath;
                    txtRutaDestino.TextButton = fbd.SelectedPath;
                    CargarListaBackupsLocales();
                }
            }
        }

        private void CargarListaBackupsLocales()
        {
            dgvBackupsLocales.Rows.Clear();
            string ruta = string.IsNullOrWhiteSpace(txtRutaDestino.Text) ? txtRutaDestino.TextButton : txtRutaDestino.Text;

            if (Directory.Exists(ruta))
            {
                DirectoryInfo dir = new DirectoryInfo(ruta);
                foreach (FileInfo archivo in dir.GetFiles("*.bak"))
                {
                    dgvBackupsLocales.Rows.Add(archivo.Name, archivo.FullName, archivo.CreationTime.ToString("dd/MM/yyyy HH:mm:ss"));
                }
            }
            dgvBackupsLocales.ClearSelection();
        }




        private string ObtenerRuta()
        {
            return string.IsNullOrWhiteSpace(txtRutaDestino.Text) ? txtRutaDestino.TextButton : txtRutaDestino.Text;
        }

        private void CargarConfiguracion()
        {
            try
            {
                _config = _backupNegocio.ObtenerConfig();

                int idx = cmbDestino.Items.IndexOf(_config.TipoDestino);
                cmbDestino.SelectedIndex = idx >= 0 ? idx : 0;

                txtRutaDestino.Text = _config.RutaDestino;
                txtRutaDestino.TextButton = _config.RutaDestino;
                chkAutomatico.Checked = _config.Activo;

                TimeSpan h;
                if (TimeSpan.TryParse(_config.HoraProgramada, out h))
                    dtpHoraAuto.Value = DateTime.Today.Add(h);

                CargarListaBackupsLocales();
            }
            catch (Exception ex)
            {
                _config = new ConfigBackup();
                WinForms.MessageBox.Show("No se pudo cargar la configuración de backup: " + ex.Message,
                    "Aviso", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
            }
        }

        private void BtnGuardarConfig_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigBackup cfg = new ConfigBackup
                {
                    Activo = chkAutomatico.Checked,
                    TipoDestino = cmbDestino.SelectedItem.ToString(),
                    RutaDestino = ObtenerRuta(),
                    HoraProgramada = dtpHoraAuto.Value.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                    CantidadAConservar = _config != null ? _config.CantidadAConservar : 5,
                    UltimoBackup = _config != null ? _config.UltimoBackup : null
                };

                _backupNegocio.GuardarConfig(cfg);
                _config = cfg;
                BackupScheduler.RecargarConfig();

                WinForms.MessageBox.Show("Configuración de backup guardada.", "Éxito",
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WinForms.MessageBox.Show("Error al guardar la configuración: " + ex.Message, "Error",
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
            }
        }

        // BACKUP LIBRE: sin autorización, cualquier usuario, en cualquier momento
        private async void BtnCrearBackup_Click(object sender, EventArgs e)
        {
            btnCrearBackup.Enabled = false;

            string carpeta = ObtenerRuta();
            string destino = cmbDestino.SelectedItem.ToString();
            int conservar = _config != null ? _config.CantidadAConservar : 5;

            try
            {
                var res = await Task.Run(() =>
                {
                    string msg, ruta;
                    bool ok = _backupNegocio.CrearBackup(carpeta, destino, conservar, "Manual", out msg, out ruta);
                    return new { ok, msg, ruta };
                });

                CargarListaBackupsLocales();

                if (res.ok)
                {
                    string texto = $"Respaldo generado con éxito en:\n{res.ruta}";
                    if (!string.IsNullOrEmpty(res.msg)) texto += "\n\n" + res.msg;
                    WinForms.MessageBox.Show(texto, "Éxito", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
                }
                else
                {
                    WinForms.MessageBox.Show("Error al generar el backup: " + res.msg, "Error",
                        WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnCrearBackup.Enabled = true;
            }
        }

        // SOLO LA RESTAURACIÓN pide aprobación de un segundo usuario (Admin <-> Gerente)
        private bool SolicitarAutorizacionCruzada(out Usuario aprobador)
        {
            bool autorizado = false;
            Usuario aprobadorLocal = null;

            using (WinForms.Form frmAuth = new WinForms.Form())
            {
                frmAuth.Text = "Autorización requerida para restaurar";
                frmAuth.Size = new Size(430, 260);
                frmAuth.StartPosition = WinForms.FormStartPosition.CenterParent;
                frmAuth.FormBorderStyle = WinForms.FormBorderStyle.FixedDialog;
                frmAuth.MaximizeBox = false;
                frmAuth.MinimizeBox = false;
                frmAuth.BackColor = Color.FromArgb(18, 18, 18);

                string textoInfo = RequierePerfilDistinto
                    ? "CONTROL CRUZADO: debe autorizar un usuario con el\notro perfil (Administrador ↔ Gerente)."
                    : "CONTROL CRUZADO: debe autorizar otro usuario\nAdministrador o Gerente.";

                WinForms.Label lblInfo = new WinForms.Label()
                {
                    Text = textoInfo,
                    ForeColor = Color.WhiteSmoke,
                    Location = new Point(20, 20),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };

                WinForms.Label lblEmail = new WinForms.Label() { Text = "Email:", ForeColor = Color.LightGray, Location = new Point(20, 80), AutoSize = true };
                WinForms.TextBox txtEmail = new WinForms.TextBox() { Location = new Point(140, 77), Width = 240, BackColor = Color.FromArgb(28, 28, 28), ForeColor = Color.White, BorderStyle = WinForms.BorderStyle.FixedSingle };

                WinForms.Label lblPass = new WinForms.Label() { Text = "Contraseña:", ForeColor = Color.LightGray, Location = new Point(20, 120), AutoSize = true };
                WinForms.TextBox txtPass = new WinForms.TextBox() { Location = new Point(140, 117), Width = 240, BackColor = Color.FromArgb(28, 28, 28), ForeColor = Color.White, PasswordChar = '●', BorderStyle = WinForms.BorderStyle.FixedSingle };

                WinForms.Button btnConfirmar = new WinForms.Button() { Text = "AUTORIZAR", Location = new Point(140, 165), Width = 115, BackColor = EstiloApp.RojoNeon, ForeColor = Color.White, FlatStyle = WinForms.FlatStyle.Flat };
                WinForms.Button btnCancelar = new WinForms.Button() { Text = "CANCELAR", Location = new Point(265, 165), Width = 115, BackColor = EstiloApp.Gris, ForeColor = Color.White, FlatStyle = WinForms.FlatStyle.Flat };
                btnConfirmar.FlatAppearance.BorderSize = 0;
                btnCancelar.FlatAppearance.BorderSize = 0;

                btnConfirmar.Click += (s, e) =>
                {
                    string msjError;
                    Usuario u = new UsuarioNegocio().Login(txtEmail.Text, txtPass.Text, out msjError);

                    if (u == null)
                    {
                        WinForms.MessageBox.Show(msjError, "Error de credenciales",
                            WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
                        return;
                    }

                    if (u.IdPerfil != 1 && u.IdPerfil != 4)
                    {
                        WinForms.MessageBox.Show("Permiso denegado. El autorizante debe ser Administrador o Gerente.",
                            "Acceso Denegado", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
                    }
                    else if (u.IdUsuario == SesionActual.IdUsuario)
                    {
                        WinForms.MessageBox.Show("Control Cruzado: el autorizante no puede ser el mismo usuario que solicita la restauración.",
                            "Violación de Seguridad", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Stop);
                    }
                    else if (RequierePerfilDistinto && u.IdPerfil == SesionActual.IdPerfil)
                    {
                        WinForms.MessageBox.Show("La autorización debe provenir del otro perfil (Administrador ↔ Gerente).",
                            "Perfil no válido", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
                    }
                    else
                    {
                        aprobadorLocal = u;
                        autorizado = true;
                        frmAuth.Close();
                    }
                };

                btnCancelar.Click += (s, e) => frmAuth.Close();

                frmAuth.Controls.Add(lblInfo);
                frmAuth.Controls.Add(lblEmail);
                frmAuth.Controls.Add(txtEmail);
                frmAuth.Controls.Add(lblPass);
                frmAuth.Controls.Add(txtPass);
                frmAuth.Controls.Add(btnConfirmar);
                frmAuth.Controls.Add(btnCancelar);
                frmAuth.AcceptButton = btnConfirmar;

                frmAuth.ShowDialog();
            }

            aprobador = aprobadorLocal;
            return autorizado;
        }

        private void BtnRestaurarBackup_Click(object sender, EventArgs e)
        {
            if (dgvBackupsLocales.CurrentRow == null)
            {
                WinForms.MessageBox.Show("Seleccione un archivo de respaldo de la tabla para restaurar.", "Aviso",
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
                return;
            }

            string archivo = dgvBackupsLocales.CurrentRow.Cells["Ruta"].Value.ToString();
            string nombre = dgvBackupsLocales.CurrentRow.Cells["Archivo"].Value.ToString();

            if (WinForms.MessageBox.Show(
                    $"Se reemplazará TODA la información actual por la del archivo:\n{nombre}\n\n" +
                    "Antes se guardará automáticamente una copia del estado actual (PreRestore_...).\n\n¿Desea continuar?",
                    "Restaurar base de datos",
                    WinForms.MessageBoxButtons.YesNo, WinForms.MessageBoxIcon.Warning) != WinForms.DialogResult.Yes)
                return;

            Usuario aprobador;
            if (!SolicitarAutorizacionCruzada(out aprobador))
            {
                WinForms.MessageBox.Show("Restauración cancelada. No se obtuvo la autorización cruzada.", "Operación abortada",
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
                return;
            }

            WinForms.Cursor.Current = WinForms.Cursors.WaitCursor;
            string mensaje;
            bool ok = _backupNegocio.Restaurar(archivo, SesionActual.IdUsuario, aprobador.IdUsuario, out mensaje);
            WinForms.Cursor.Current = WinForms.Cursors.Default;

            if (ok)
            {
                WinForms.MessageBox.Show("Sistema restaurado con éxito. La aplicación se reiniciará.", "Éxito",
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
                WinForms.Application.Restart();
            }
            else
            {
                WinForms.MessageBox.Show(mensaje, "Error en la restauración",
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
            }
        }



    }
}