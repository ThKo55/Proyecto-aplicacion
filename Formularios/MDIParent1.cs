using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            // Color gris moderno y limpio
            Color colorGrisModerno = Color.FromArgb(200, 205, 210);

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

                // 1. APAGAMOS LOS EFECTOS PROBLEMÁTICOS DE REALTAIIZOR
                Background = true,
                BackgroundPen = true,
                Lighting = false,
                RGB = false,

                // 2. DEJAMOS SOLO EL COLOR BASE TRANSPARENTE
                ColorBackground = Color.Transparent,
                ColorBackground_Pen = Color.Transparent,
                ForeColor = colorGrisModerno
            };

            btn.MouseEnter += (s, e) =>
            {
                if (botonActivo != btn)
                {
                    btn.ColorBackground = Color.FromArgb(60, 64, 68);
                    btn.ColorBackground_Pen = Color.FromArgb(60, 64, 68);
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
            // AHORA ESTÁN INCLUIDOS HISTORIAL Y AUDITORÍA
            CyberButton[] todosLosBotones = {
        btnPuntoVenta, btnClientes, btnStock, btnProductos,
        btnCategorias, btnUsuarios, btnSucursales, btnHistorial,
        btnAuditoria, btnDashboard, btnCerrarSesion
    };

            foreach (var b in todosLosBotones)
            {
                if (b != null)
                {
                    if (soloIconos)
                    {
                        b.Size = new Size(50, 42); // Tamaño encogido
                        if (b.Tag.ToString().Contains(" "))
                            b.TextButton = b.Tag.ToString().Split(' ')[0]; // Deja solo el emoji
                    }
                    else
                    {
                        b.Size = new Size(240, 42); // Tamaño normal expandido
                        b.TextButton = b.Tag.ToString(); // Restaura el texto completo
                    }
                }
            }
        }

        private void ResaltarBotonActivo(CyberButton btnClickeado)
        {
            CyberButton[] todosLosBotones = {
        btnPuntoVenta, btnClientes, btnStock, btnProductos,
        btnCategorias, btnUsuarios, btnSucursales, btnDashboard,
        btnHistorial, btnAuditoria
    };

            Color colorGrisModerno = Color.FromArgb(200, 205, 210);

            // 1. Apagamos todos los botones sin forzar el Refresh individual
            foreach (var b in todosLosBotones)
            {
                if (b != null)
                {
                    b.ColorBackground = Color.Transparent;
                    b.ColorBackground_Pen = Color.Transparent;
                    b.ForeColor = colorGrisModerno;
                }
            }

            // 2. Encendemos únicamente el botón clickeado
            if (btnClickeado != null)
            {
                btnClickeado.ColorBackground = EstiloApp.RojoOscuro;
                btnClickeado.ColorBackground_Pen = EstiloApp.RojoOscuro;
                btnClickeado.ForeColor = Color.White;

                botonActivo = btnClickeado;
                pnlIndicador.Top = btnClickeado.Top;
            }

            // 3. Forzamos un único refresco general al panel (elimina los parpadeos)
            panelMenuLateral.Refresh();

            pnlIndicador.Visible = true;
            pnlIndicador.BringToFront();

            if (timerIndicador != null)
            {
                timerIndicador.Start();
            }

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
            // 1. Ocultar TODOS los botones para dejar el menú en blanco
            CyberButton[] todosLosBotones = {
        btnPuntoVenta, btnClientes, btnStock, btnProductos,
        btnCategorias, btnUsuarios, btnSucursales, btnHistorial,
        btnAuditoria, btnDashboard
    };

            foreach (var btn in todosLosBotones)
            {
                btn.Visible = false;
            }

            // 2. Crear una lista dinámica para armar el menú a medida según el perfil
            List<CyberButton> menuDelUsuario = new List<CyberButton>();

            if (SesionActual.IdPerfil == 1) // 👑 ADMINISTRADOR
            {
                // Agregas los botones en el orden exacto que quieres que los vea el Admin
                menuDelUsuario.Add(btnDashboard);
                menuDelUsuario.Add(btnUsuarios);
                menuDelUsuario.Add(btnSucursales);
                menuDelUsuario.Add(btnAuditoria);
                menuDelUsuario.Add(btnClientes);
                menuDelUsuario.Add(btnHistorial);
                menuDelUsuario.Add(btnStock);
                menuDelUsuario.Add(btnProductos);
                menuDelUsuario.Add(btnCategorias);

                ResaltarBotonActivo(btnDashboard);
                AbrirFormularioEnPanel(new FrmDashboard());
            }
            else if (SesionActual.IdPerfil == 2) // 🛒 CAJERO
            {
                // El cajero tiene su propia "forma" de menú, solo con sus 3 opciones
                menuDelUsuario.Add(btnPuntoVenta);
                menuDelUsuario.Add(btnClientes);
                menuDelUsuario.Add(btnHistorial);

                ResaltarBotonActivo(btnPuntoVenta);
                AbrirFormularioEnPanel(new FrmPuntoVenta());
            }
            else if (SesionActual.IdPerfil == 3) // 📦 GESTOR DE STOCK
            {
                menuDelUsuario.Add(btnStock);
                menuDelUsuario.Add(btnProductos);
                menuDelUsuario.Add(btnCategorias);

                ResaltarBotonActivo(btnStock);
                AbrirFormularioEnPanel(new FrmStock());
            }

            // 3. Dibujar el menú apilando únicamente los botones de la lista generada
            int yPos = 110;
            int btnHeight = 45;

            foreach (var btn in menuDelUsuario)
            {
                btn.Visible = true; // Lo encendemos
                btn.Location = new Point(5, yPos); // Lo posicionamos sin dejar espacios
                yPos += btnHeight; // Empujamos el siguiente hacia abajo
            }

            // Acomodamos la barrita roja indicadora al botón activo
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