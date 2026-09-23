using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;

namespace AorusMarket.Formularios
{
    public partial class MDIParent1 : Form
    {
        private System.Windows.Forms.Panel panelMenuLateral;
        private System.Windows.Forms.Panel panelContenedor;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Panel pnlIndicador;
        private System.Windows.Forms.Label lblUsuarioActivo;
        private System.Windows.Forms.Label lblLogoTexto;

        private CyberButton btnPuntoVenta, btnClientes, btnStock, btnProductos, btnCategorias, btnUsuarios, btnSucursales, btnDashboard, btnCerrarSesion, btnHistorial, btnAuditoria;

        private Form formularioActivo = null;
        private CyberButton botonActivo = null;
        private System.Windows.Forms.Timer timerIndicador;

        public MDIParent1()
        {
            InitializeComponent();
            ConfigurarFormulario();
            ConstruirInterfazModerno();

            ActivarDoubleBuffering(panelContenedor);
            ActivarDoubleBuffering(panelMenuLateral);

            AplicarPermisosPorPerfil();
            this.FormClosed += (s, e) => Application.Exit();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "AorusMarket - Sistema de Gestión";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = EstiloApp.Fondo;
        }

        private void ConstruirInterfazModerno()
        {
            panelMenuLateral = new System.Windows.Forms.Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = EstiloApp.FondoPanel
            };

            panelLogo = new System.Windows.Forms.Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.FromArgb(30, 34, 38) };

            CyberButton btnToggle = new CyberButton
            {
                TextButton = "☰",
                Size = new Size(40, 40),
                Location = new Point(10, 25),
                Alpha = 20,
                Rounding = true,
                RoundingInt = 8,
                ColorBackground = Color.Transparent,
                ColorBackground_Pen = Color.Transparent,
                ForeColor = EstiloApp.Blanco,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnToggle.Click += BtnToggle_Click;

            lblLogoTexto = new System.Windows.Forms.Label
            {
                Text = "AORUS",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = EstiloApp.RojoNeon,
                AutoSize = true,
                Location = new Point(60, 20)
            };

            lblUsuarioActivo = new System.Windows.Forms.Label
            {
                Text = SesionActual.NombrePerfil,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = EstiloApp.Gris,
                AutoSize = true,
                Location = new Point(65, 55)
            };

            panelLogo.Controls.Add(btnToggle);
            panelLogo.Controls.Add(lblLogoTexto);
            panelLogo.Controls.Add(lblUsuarioActivo);
            panelMenuLateral.Controls.Add(panelLogo);

            pnlIndicador = new System.Windows.Forms.Panel
            {
                Size = new Size(4, 42),
                BackColor = EstiloApp.RojoNeon,
                Left = 0,
                Visible = false
            };
            panelMenuLateral.Controls.Add(pnlIndicador);

            panelContenedor = new System.Windows.Forms.Panel
            {
                Dock = DockStyle.Fill,
                BackColor = EstiloApp.Fondo
            };

            this.Controls.Add(panelContenedor);
            this.Controls.Add(panelMenuLateral);
            panelMenuLateral.SendToBack();
            panelContenedor.BringToFront();

            int yPos = 110;
            int btnHeight = 45;

            btnPuntoVenta = CrearBotonMenu("🛒 Punto de Venta", yPos);
            btnPuntoVenta.Click += (s, e) => { ResaltarBotonActivo(btnPuntoVenta); AbrirFormularioEnPanel(new FrmPuntoVenta()); };
            yPos += btnHeight;

            btnClientes = CrearBotonMenu("👥 Clientes", yPos);
            btnClientes.Click += (s, e) => { ResaltarBotonActivo(btnClientes); AbrirFormularioEnPanel(new FrmClientes()); };
            yPos += btnHeight;

            btnStock = CrearBotonMenu("📦 Stock", yPos);
            btnStock.Click += (s, e) => { ResaltarBotonActivo(btnStock); AbrirFormularioEnPanel(new FrmStock()); };
            yPos += btnHeight;

            btnProductos = CrearBotonMenu("🏷️ Productos", yPos);
            btnProductos.Click += (s, e) => { ResaltarBotonActivo(btnProductos); AbrirFormularioEnPanel(new FrmProductos()); };
            yPos += btnHeight;

            btnCategorias = CrearBotonMenu("📁 Categorías", yPos);
            btnCategorias.Click += (s, e) => { ResaltarBotonActivo(btnCategorias); AbrirFormularioEnPanel(new FrmCategorias()); };
            yPos += btnHeight;

            btnUsuarios = CrearBotonMenu("👤 Usuarios", yPos);
            btnUsuarios.Click += (s, e) => { ResaltarBotonActivo(btnUsuarios); AbrirFormularioEnPanel(new FrmUsuarios()); };
            yPos += btnHeight;


            btnSucursales = CrearBotonMenu("🏢 Sucursales", yPos);
            btnSucursales.Click += (s, e) => { ResaltarBotonActivo(btnSucursales); AbrirFormularioEnPanel(new FrmSucursales()); };
            yPos += btnHeight;
             btnHistorial = CrearBotonMenu("📜 Hist. Ventas", yPos);
            btnHistorial.Click += (s, e) => { ResaltarBotonActivo(btnHistorial); AbrirFormularioEnPanel(new FrmHistorialVentas()); };
            panelMenuLateral.Controls.Add(btnHistorial);
            yPos += btnHeight; 
                               
             btnAuditoria = CrearBotonMenu("🔎 Auditoría Stock", yPos);
            btnAuditoria.Click += (s, e) => { ResaltarBotonActivo(btnAuditoria); AbrirFormularioEnPanel(new FrmAuditoriaStock()); };
            panelMenuLateral.Controls.Add(btnAuditoria);
            yPos += btnHeight; 

            btnDashboard = CrearBotonMenu("📊 Dashboard", yPos);
            btnDashboard.Click += (s, e) => { ResaltarBotonActivo(btnDashboard); AbrirFormularioEnPanel(new FrmDashboard()); };

            btnCerrarSesion = CrearBotonMenu("🚪 Cerrar Sesión", this.ClientSize.Height - 60);
            btnCerrarSesion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCerrarSesion.Click += BtnCerrarSesion_Click;

            panelMenuLateral.Controls.Add(btnPuntoVenta);
            panelMenuLateral.Controls.Add(btnClientes);
            panelMenuLateral.Controls.Add(btnStock);
            panelMenuLateral.Controls.Add(btnProductos);
            panelMenuLateral.Controls.Add(btnCategorias);
            panelMenuLateral.Controls.Add(btnUsuarios);
            panelMenuLateral.Controls.Add(btnSucursales);
            panelMenuLateral.Controls.Add(btnDashboard);
            panelMenuLateral.Controls.Add(btnCerrarSesion);

            timerIndicador = new System.Windows.Forms.Timer { Interval = 15 };
            timerIndicador.Tick += TimerIndicador_Tick;
        }

        private CyberButton CrearBotonMenu(string texto, int yPos)
        {
            CyberButton btn = new CyberButton
            {
                TextButton = texto,
                Tag = texto,
                Location = new Point(5, yPos),
                Size = new Size(240, 42),
                Alpha = 20,
                Rounding = true,
                RoundingInt = 8,
                ColorBackground = Color.Transparent,
                ColorBackground_Pen = Color.Transparent,
                ForeColor = EstiloApp.Gris,
                Font = new Font("Segoe UI Emoji", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btn.MouseEnter += (s, e) =>
            {
                if (botonActivo != btn)
                {
                    btn.ColorBackground = Color.FromArgb(60, 64, 68);
                    btn.ForeColor = EstiloApp.Blanco;
                }
            };
            btn.MouseLeave += (s, e) =>
            {
                if (botonActivo != btn)
                {
                    btn.ColorBackground = Color.Transparent;
                    btn.ForeColor = EstiloApp.Gris;
                }
            };

            return btn;
        }

        private void TimerIndicador_Tick(object sender, EventArgs e)
        {
            if (botonActivo == null) return;

            int distancia = botonActivo.Top - pnlIndicador.Top;

            if (Math.Abs(distancia) <= 1)
            {
                pnlIndicador.Top = botonActivo.Top;
                timerIndicador.Stop();
            }
            else
            {
                pnlIndicador.Top += distancia / 3;
            }
        }

        private void BtnToggle_Click(object sender, EventArgs e)
        {
            if (panelMenuLateral.Width == 250)
            {
                panelMenuLateral.Width = 60;
                lblLogoTexto.Visible = false;
                lblUsuarioActivo.Visible = false;
                ModificarTextoBotones(true);
            }
            else
            {
                panelMenuLateral.Width = 250;
                lblLogoTexto.Visible = true;
                lblUsuarioActivo.Visible = true;
                ModificarTextoBotones(false);
            }
        }

        private void ModificarTextoBotones(bool soloIconos)
        {
            CyberButton[] todosLosBotones = { btnPuntoVenta, btnClientes, btnStock, btnProductos, btnCategorias, btnUsuarios, btnSucursales, btnDashboard, btnCerrarSesion };

            foreach (var b in todosLosBotones)
            {
                if (b != null)
                {
                    if (soloIconos)
                    {
                        b.Size = new Size(50, 42);
                        if (b.Tag.ToString().Contains(" "))
                            b.TextButton = b.Tag.ToString().Split(' ')[0];
                    }
                    else
                    {
                        b.Size = new Size(240, 42);
                        b.TextButton = b.Tag.ToString();
                    }
                }
            }
        }

        private void ResaltarBotonActivo(CyberButton btnClickeado)
        {
            CyberButton[] todosLosBotones = { btnPuntoVenta, btnClientes, btnStock, btnProductos, btnCategorias, btnUsuarios, btnSucursales, btnDashboard };

            foreach (var b in todosLosBotones)
            {
                if (b != null)
                {
                    b.ColorBackground = Color.Transparent;
                    b.ForeColor = EstiloApp.Gris;
                    b.Refresh();
                }
            }

            btnClickeado.ColorBackground = EstiloApp.RojoOscuro;
            btnClickeado.ForeColor = EstiloApp.Blanco;
            btnClickeado.Refresh();

            botonActivo = btnClickeado;

            pnlIndicador.Visible = true;
            pnlIndicador.BringToFront();
            timerIndicador.Start();

            this.ActiveControl = null;
        }

        private void AbrirFormularioEnPanel(Form formHijo)
        {
            if (formularioActivo != null)
            {
                formularioActivo.Close();
            }

            formularioActivo = formHijo;
            formHijo.TopLevel = false;
            formHijo.FormBorderStyle = FormBorderStyle.None;
            formHijo.Dock = DockStyle.Fill;
            formHijo.BackColor = EstiloApp.Fondo;

            panelContenedor.Controls.Add(formHijo);
            panelContenedor.Tag = formHijo;
            formHijo.BringToFront();
            formHijo.Show();
        }

        private void AplicarPermisosPorPerfil()
        {
            // 1. ADMINISTRADOR (IdPerfil = 1)
            if (SesionActual.IdPerfil == 1)
            {
                btnPuntoVenta.Visible = false; // El admin no usa la caja

                ResaltarBotonActivo(btnDashboard);
                pnlIndicador.Top = btnDashboard.Top;
                AbrirFormularioEnPanel(new FrmDashboard());
            }
            // 2. CAJERO / VENTAS (IdPerfil = 2)
            else if (SesionActual.IdPerfil == 2)
            {
                btnStock.Visible = false;
                btnProductos.Visible = false;
                btnCategorias.Visible = false;
                btnUsuarios.Visible = false;
                btnSucursales.Visible = false;
                btnDashboard.Visible = false;

                // --- ACÁ OCULTAMOS LOS HISTORIALES AL CAJERO ---
                btnHistorial.Visible = false;
                btnAuditoria.Visible = false;

                ResaltarBotonActivo(btnPuntoVenta);
                pnlIndicador.Top = btnPuntoVenta.Top;
                AbrirFormularioEnPanel(new FrmPuntoVenta());
            }
            // 3. GESTOR DE STOCK (IdPerfil = 3)
            else if (SesionActual.IdPerfil == 3)
            {
                btnPuntoVenta.Visible = false;
                btnClientes.Visible = false;
                btnUsuarios.Visible = false;
                btnSucursales.Visible = false;
                btnDashboard.Visible = false;

              
                // --- ACÁ OCULTAMOS LOS HISTORIALES AL CAJERO ---
                btnHistorial.Visible = false;
                btnAuditoria.Visible = false;

                ResaltarBotonActivo(btnStock);
                pnlIndicador.Top = btnStock.Top;
                AbrirFormularioEnPanel(new FrmStock());
            }
        }

        private void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show("¿Seguro que desea cerrar sesión?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp == DialogResult.Yes)
            {
                SesionActual.CerrarSesion();
                this.Hide();
                FrmLogin login = new FrmLogin();
                login.Show();
            }
        }

        private void ActivarDoubleBuffering(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }
    }
}