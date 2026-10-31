using System;
using System.Drawing;
using System.IO;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Negocio;
using WinForms = System.Windows.Forms;

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
        private WinForms.Timer timerAuto;
        private DateTime ultimaEjecucion = DateTime.MinValue;

        private BackupNegocio _backupNegocio = new BackupNegocio();

        public FrmBackup()
        {
            InitializeComponent();

            this.BackColor = Color.FromArgb(18, 18, 18);
            this.Text = "Gestión de Backups y Seguridad";
            this.MinimumSize = new Size(950, 600);
            ConstruirInterfaz();
            ConfigurarTimer();
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

        private void ConfigurarTimer()
        {
            timerAuto = new WinForms.Timer();
            timerAuto.Interval = 30000;
            timerAuto.Tick += TimerAuto_Tick;
            timerAuto.Start();
        }

        private void TimerAuto_Tick(object sender, EventArgs e)
        {
            if (!chkAutomatico.Checked) return;

            DateTime ahora = DateTime.Now;
            TimeSpan horaProg = dtpHoraAuto.Value.TimeOfDay;

            if (ahora.Hour == horaProg.Hours && ahora.Minute == horaProg.Minutes && ultimaEjecucion.Date != ahora.Date)
            {
                ultimaEjecucion = ahora.Date;
                EjecutarProcesoBackup(false);
            }
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

        private void BtnCrearBackup_Click(object sender, EventArgs e)
        {
            EjecutarProcesoBackup(true);
        }

        private void EjecutarProcesoBackup(bool mostrarMensajes)
        {
            string ruta = string.IsNullOrWhiteSpace(txtRutaDestino.Text) ? txtRutaDestino.TextButton : txtRutaDestino.Text;

            if (!Directory.Exists(ruta))
            {
                try { Directory.CreateDirectory(ruta); }
                catch
                {
                    if (mostrarMensajes)
                        WinForms.MessageBox.Show("La ruta no es válida o no tiene permisos.", "Error", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
                    return;
                }
            }

            string nombreArchivo = $"AorusMarket_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            string rutaCompleta = Path.Combine(ruta, nombreArchivo);

            string mensaje;
            bool exito = _backupNegocio.GenerarRespaldo(rutaCompleta, cmbDestino.SelectedItem.ToString(), out mensaje);

            if (exito)
            {
                CargarListaBackupsLocales();
                if (mostrarMensajes)
                    WinForms.MessageBox.Show($"Respaldo generado con éxito en:\n{rutaCompleta}", "Éxito", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
            }
            else
            {
                if (mostrarMensajes)
                    WinForms.MessageBox.Show("Error al generar el backup: " + mensaje, "Error", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
            }
        }

        private void BtnRestaurarBackup_Click(object sender, EventArgs e)
        {
            if (dgvBackupsLocales.CurrentRow == null)
            {
                WinForms.MessageBox.Show("Seleccione un archivo de respaldo de la tabla para restaurar.", "Aviso", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
                return;
            }

            string archivoSeleccionado = dgvBackupsLocales.CurrentRow.Cells["Ruta"].Value.ToString();

            WinForms.DialogResult alertaCritica = WinForms.MessageBox.Show(
                "¡ATENCIÓN ACCIÓN CRÍTICA!\n\n" +
                "Restaurar la base de datos sobrescribirá toda la información actual por el estado del archivo seleccionado.\n\n" +
                "¿Confirma que posee autorización gerencial para ejecutar esta restauración?",
                "Control Cruzado de Seguridad",
                WinForms.MessageBoxButtons.YesNo,
                WinForms.MessageBoxIcon.Stop);

            if (alertaCritica == WinForms.DialogResult.No) return;

            WinForms.DialogResult confirmacionFinal = WinForms.MessageBox.Show(
                "Esta acción es irreversible y reiniciará el sistema.\n\n¿Está absolutamente seguro?",
                "Última Advertencia",
                WinForms.MessageBoxButtons.YesNo,
                WinForms.MessageBoxIcon.Warning);

            if (confirmacionFinal == WinForms.DialogResult.Yes)
            {
                string mensaje;
                bool restaurado = _backupNegocio.RestaurarBaseDatos(archivoSeleccionado, out mensaje);

                if (restaurado)
                {
                    WinForms.MessageBox.Show("Sistema restaurado con éxito. La aplicación se reiniciará.", "Éxito", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
                    WinForms.Application.Restart();
                }
                else
                {
                    WinForms.MessageBox.Show("Error en la restauración: " + mensaje, "Error Crítico", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
                }
            }
        }
    }
}