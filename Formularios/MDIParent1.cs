using AorusMarket.Negocio;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

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

        private CyberButton btnPuntoVenta, btnClientes, btnStock, btnProductos, btnCategorias, btnUsuarios, btnSucursales, btnDashboard, btnCerrarSesion, btnHistorial, btnAuditoria, btnBackup, btnReportes;

        private Form formularioActivo = null;
        private CyberButton botonActivo = null;
        private System.Windows.Forms.Timer timerIndicador;

        // SOLUCIÓN CARRITO: Diccionario para guardar los formularios abiertos sin destruirlos
        private Dictionary<Type, Form> formulariosCacheados = new Dictionary<Type, Form>();

        public MDIParent1()
        {
            InitializeComponent();
            ConfigurarFormulario();
            ConstruirInterfazModerno();

            ActivarDoubleBuffering(panelContenedor);
            ActivarDoubleBuffering(panelMenuLateral);

            AplicarPermisosPorPerfil();
            this.FormClosed += (s, e) => Application.Exit();
            this.FormClosed += (s, e) => BackupScheduler.Detener();
            BackupScheduler.Iniciar();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "AorusMarket - Sistema de Gestión";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(18, 18, 18);
        }

        private void ConstruirInterfazModerno()
        {
            panelMenuLateral = new System.Windows.Forms.Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(28, 28, 28)
            };

            panelLogo = new System.Windows.Forms.Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.FromArgb(28, 28, 28) };

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
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnToggle.Click += BtnToggle_Click;

            lblLogoTexto = new System.Windows.Forms.Label
            {
                Text = "AORUS",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                AutoSize = true,
                Location = new Point(60, 20)
            };

            lblUsuarioActivo = new System.Windows.Forms.Label
            {
                Text = SesionActual.NombrePerfil,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.Gray,
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
                BackColor = Color.FromArgb(220, 53, 69),
                Left = 0,
                Visible = false
            };
            panelMenuLateral.Controls.Add(pnlIndicador);

            panelContenedor = new System.Windows.Forms.Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 18, 18)
            };

            this.Controls.Add(panelContenedor);
            this.Controls.Add(panelMenuLateral);
            panelMenuLateral.SendToBack();
            panelContenedor.BringToFront();

            int yPos = 110;
            int btnHeight = 45;

            // SOLUCIÓN CARRITO: Ahora usamos AbrirFormularioEnPanel genérico <T>
            btnPuntoVenta = CrearBotonMenu("🛒 Punto de Venta", yPos);
            btnPuntoVenta.Click += (s, e) => { ResaltarBotonActivo(btnPuntoVenta); AbrirFormularioEnPanel<FrmPuntoVenta>(); };
            yPos += btnHeight;

            btnClientes = CrearBotonMenu("👥 Clientes", yPos);
            btnClientes.Click += (s, e) => { ResaltarBotonActivo(btnClientes); AbrirFormularioEnPanel<FrmClientes>(); };
            yPos += btnHeight;

            btnStock = CrearBotonMenu("📦 Stock", yPos);
            btnStock.Click += (s, e) => { ResaltarBotonActivo(btnStock); AbrirFormularioEnPanel<FrmStock>(); };
            yPos += btnHeight;

            btnProductos = CrearBotonMenu("🏷️ Productos", yPos);
            btnProductos.Click += (s, e) => { ResaltarBotonActivo(btnProductos); AbrirFormularioEnPanel<FrmProductos>(); };
            yPos += btnHeight;

            btnCategorias = CrearBotonMenu("📁 Categorías", yPos);
            btnCategorias.Click += (s, e) => { ResaltarBotonActivo(btnCategorias); AbrirFormularioEnPanel<FrmCategorias>(); };
            yPos += btnHeight;

            btnUsuarios = CrearBotonMenu("👤 Usuarios", yPos);
            btnUsuarios.Click += (s, e) => { ResaltarBotonActivo(btnUsuarios); AbrirFormularioEnPanel<FrmUsuarios>(); };
            yPos += btnHeight;

            btnSucursales = CrearBotonMenu("🏢 Sucursales", yPos);
            btnSucursales.Click += (s, e) => { ResaltarBotonActivo(btnSucursales); AbrirFormularioEnPanel<FrmSucursales>(); };
            yPos += btnHeight;

            btnHistorial = CrearBotonMenu("📜 Hist. Ventas", yPos);
            btnHistorial.Click += (s, e) => { ResaltarBotonActivo(btnHistorial); AbrirFormularioEnPanel<FrmHistorialVentas>(); };
            panelMenuLateral.Controls.Add(btnHistorial);
            yPos += btnHeight;

            btnAuditoria = CrearBotonMenu("🔎 Auditoría General", yPos);
            btnAuditoria.Click += (s, e) => { ResaltarBotonActivo(btnAuditoria); AbrirFormularioEnPanel<FrmAuditoriaGeneral>(); };
            panelMenuLateral.Controls.Add(btnAuditoria);
            yPos += btnHeight;

            btnReportes = CrearBotonMenu("📊 Reportes", yPos);
            btnReportes.Click += (s, e) => { ResaltarBotonActivo(btnReportes); AbrirFormularioEnPanel<FrmReportes>(); };
            panelMenuLateral.Controls.Add(btnReportes);
            yPos += btnHeight;

            btnBackup = CrearBotonMenu("💾 Backups", yPos);
            btnBackup.Click += (s, e) => { ResaltarBotonActivo(btnBackup); AbrirFormularioEnPanel<FrmBackup>(); };
            panelMenuLateral.Controls.Add(btnBackup);
            yPos += btnHeight;

            btnDashboard = CrearBotonMenu("📊 Dashboard", yPos);
            btnDashboard.Click += (s, e) => { ResaltarBotonActivo(btnDashboard); AbrirFormularioEnPanel<FrmDashboard>(); };

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
            Color colorGrisModerno = Color.LightGray;

            CyberButton btn = new CyberButton
            {
                TextButton = texto,
                Tag = texto,
                Location = new Point(5, yPos),
                Size = new Size(240, 42),
                Alpha = 20,
                Rounding = true,
                RoundingInt = 8,
                Font = new Font("Segoe UI Emoji", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Background = true,
                BackgroundPen = true,
                Lighting = false,
                RGB = false,
                ColorBackground = Color.Transparent,
                ColorBackground_Pen = Color.Transparent,
                ForeColor = colorGrisModerno
            };

            btn.MouseEnter += (s, e) =>
            {
                if (botonActivo != btn)
                {
                    btn.ColorBackground = Color.FromArgb(40, 40, 40);
                    btn.ColorBackground_Pen = Color.FromArgb(40, 40, 40);
                    btn.ForeColor = Color.White;
                }
            };

            btn.MouseLeave += (s, e) =>
            {
                if (botonActivo != btn)
                {
                    btn.ColorBackground = Color.Transparent;
                    btn.ColorBackground_Pen = Color.Transparent;
                    btn.ForeColor = colorGrisModerno;
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
            CyberButton[] todosLosBotones = {
                btnPuntoVenta, btnClientes, btnStock, btnProductos,
                btnCategorias, btnUsuarios, btnSucursales, btnHistorial,
                btnAuditoria, btnBackup, btnDashboard, btnCerrarSesion, btnReportes
            };

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
            CyberButton[] todosLosBotones = {
                btnPuntoVenta, btnClientes, btnStock, btnProductos,
                btnCategorias, btnUsuarios, btnSucursales, btnDashboard,
                btnHistorial, btnAuditoria, btnBackup, btnReportes
            };

            Color colorGrisModerno = Color.LightGray;

            foreach (var b in todosLosBotones)
            {
                if (b != null)
                {
                    b.ColorBackground = Color.Transparent;
                    b.ColorBackground_Pen = Color.Transparent;
                    b.ForeColor = colorGrisModerno;
                }
            }

            if (btnClickeado != null)
            {
                btnClickeado.ColorBackground = Color.FromArgb(40, 40, 40);
                btnClickeado.ColorBackground_Pen = Color.FromArgb(40, 40, 40);
                btnClickeado.ForeColor = Color.FromArgb(220, 53, 69);

                botonActivo = btnClickeado;
                pnlIndicador.Top = btnClickeado.Top;
            }

            panelMenuLateral.Refresh();

            pnlIndicador.Visible = true;
            pnlIndicador.BringToFront();

            if (timerIndicador != null)
            {
                timerIndicador.Start();
            }

            this.ActiveControl = null;
        }

        // SOLUCIÓN CARRITO: Método modificado para usar caché
        private void AbrirFormularioEnPanel<T>() where T : Form, new()
        {
            if (formularioActivo != null)
            {
                formularioActivo.Hide(); // Ocultamos en lugar de destruir
            }

            Type tipo = typeof(T);

            // Si el formulario nunca se abrió, lo creamos y lo guardamos
            if (!formulariosCacheados.ContainsKey(tipo))
            {
                T nuevoFormulario = new T();
                nuevoFormulario.TopLevel = false;
                nuevoFormulario.FormBorderStyle = FormBorderStyle.None;
                nuevoFormulario.Dock = DockStyle.Fill;
                nuevoFormulario.BackColor = Color.FromArgb(18, 18, 18);
                
                panelContenedor.Controls.Add(nuevoFormulario);
                formulariosCacheados.Add(tipo, nuevoFormulario);
            }

            // Llamamos al formulario desde la caché
            formularioActivo = formulariosCacheados[tipo];
            formularioActivo.BringToFront();
            formularioActivo.Show();
        }

        private void AplicarPermisosPorPerfil()
        {
            CyberButton[] todosLosBotones = {
                btnPuntoVenta, btnClientes, btnStock, btnProductos,
                btnCategorias, btnUsuarios, btnSucursales, btnHistorial,
                btnAuditoria, btnBackup, btnDashboard, btnReportes
            };

            foreach (var btn in todosLosBotones)
            {
                if (btn != null) btn.Visible = false;
            }

            List<CyberButton> menuDelUsuario = new List<CyberButton>();

            if (SesionActual.IdPerfil == 1)
            {
                menuDelUsuario.Add(btnDashboard);
                menuDelUsuario.Add(btnUsuarios);
                menuDelUsuario.Add(btnSucursales);
                menuDelUsuario.Add(btnAuditoria);
                menuDelUsuario.Add(btnBackup);
                menuDelUsuario.Add(btnClientes);
                menuDelUsuario.Add(btnHistorial);
                menuDelUsuario.Add(btnStock);
                menuDelUsuario.Add(btnProductos);
                menuDelUsuario.Add(btnCategorias);
                menuDelUsuario.Add(btnReportes);

                ResaltarBotonActivo(btnDashboard);
                AbrirFormularioEnPanel<FrmDashboard>();
            }
            else if (SesionActual.IdPerfil == 2)
            {
                menuDelUsuario.Add(btnPuntoVenta);
                menuDelUsuario.Add(btnClientes);
                menuDelUsuario.Add(btnHistorial); 

                ResaltarBotonActivo(btnPuntoVenta);
                AbrirFormularioEnPanel<FrmPuntoVenta>();
            }
            else if (SesionActual.IdPerfil == 3)
            {
                menuDelUsuario.Add(btnStock);
                menuDelUsuario.Add(btnProductos);
                menuDelUsuario.Add(btnCategorias);

                ResaltarBotonActivo(btnStock);
                AbrirFormularioEnPanel<FrmStock>();
            }
            else if (SesionActual.IdPerfil == 4)
            {
                menuDelUsuario.Add(btnDashboard);
                menuDelUsuario.Add(btnReportes);
                menuDelUsuario.Add(btnHistorial);
                menuDelUsuario.Add(btnClientes);
                menuDelUsuario.Add(btnStock);
                menuDelUsuario.Add(btnProductos);
                menuDelUsuario.Add(btnCategorias);
                menuDelUsuario.Add(btnBackup);

                ResaltarBotonActivo(btnDashboard);
                AbrirFormularioEnPanel<FrmDashboard>();
            }


            int yPos = 110;
            int btnHeight = 45;

            foreach (var btn in menuDelUsuario)
            {
                btn.Visible = true;
                btn.Location = new Point(5, yPos);
                yPos += btnHeight;
            }

            if (botonActivo != null)
            {
                pnlIndicador.Top = botonActivo.Top;
            }
        }

        private void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show("¿Seguro que desea cerrar sesión?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp == DialogResult.Yes)
            {
                // Limpiamos la caché al cerrar sesión para que el próximo usuario entre con todo en blanco
                foreach (var form in formulariosCacheados.Values)
                {
                    form.Close();
                }
                formulariosCacheados.Clear();

                Auditoria.Registrar("Cierre de sesión", "Sesión");

                SesionActual.CerrarSesion();
                this.Hide();
                FrmLogin login = new FrmLogin();
                login.Show();
            }
        }

        private void ActivarDoubleBuffering(Control control)
        {
            PropertyInfo prop = typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (prop != null)
            {
                prop.SetValue(control, true, null);
            }
        }






    }
}