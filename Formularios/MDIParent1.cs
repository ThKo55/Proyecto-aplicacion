using System;
using System.Drawing;
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

        // EL NUEVO INDICADOR VISUAL (La rayita que se mueve)
        private System.Windows.Forms.Panel pnlIndicador;

        private System.Windows.Forms.Label lblUsuarioActivo;
        private System.Windows.Forms.Label lblLogoTexto;

        private CyberButton btnPuntoVenta, btnClientes, btnStock, btnProductos, btnCategorias, btnUsuarios, btnSucursales, btnDashboard, btnCerrarSesion;

        private Form formularioActivo = null;
        private CyberButton botonActivo = null;

        public MDIParent1()
        {
            InitializeComponent();
            ConfigurarFormulario();
            ConstruirInterfazModerno();
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
            // 1. EL MENÚ LATERAL (SIDEBAR)
            panelMenuLateral = new System.Windows.Forms.Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = EstiloApp.FondoPanel
            };

            // 1.1 Panel arriba para el Botón Hamburguesa, Logo y Usuario
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

            // 1.2 EL INDICADOR ACTIVO (Rayita roja vertical)
            pnlIndicador = new System.Windows.Forms.Panel
            {
                Size = new Size(4, 42),
                BackColor = EstiloApp.RojoNeon,
                Left = 0,
                Visible = false // Se oculta hasta que selecciones un botón
            };
            panelMenuLateral.Controls.Add(pnlIndicador);

            // 2. EL PANEL CONTENEDOR 
            panelContenedor = new System.Windows.Forms.Panel
            {
                Dock = DockStyle.Fill,
                BackColor = EstiloApp.Fondo
            };

            this.Controls.Add(panelContenedor);
            this.Controls.Add(panelMenuLateral);
            panelMenuLateral.SendToBack();
            panelContenedor.BringToFront();

            // 3. CREACIÓN DE BOTONES DEL MENÚ LATERAL
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
                // CAMBIAMOS LA FUENTE A "Segoe UI Emoji" PARA QUE LOS ICONOS NO SE ROMPAN
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

        private void ResaltarBotonActivo(CyberButton btnClickeado)
        {
            CyberButton[] todosLosBotones = { btnPuntoVenta, btnClientes, btnStock, btnProductos, btnCategorias, btnUsuarios, btnSucursales, btnDashboard };

            foreach (var b in todosLosBotones)
            {
                if (b != null)
                {
                    b.ColorBackground = Color.Transparent;
                    b.ForeColor = EstiloApp.Gris;
                }
            }

            btnClickeado.ColorBackground = EstiloApp.RojoOscuro;
            btnClickeado.ForeColor = EstiloApp.Blanco;

            botonActivo = btnClickeado;

            // HACEMOS QUE LA RAYITA ROJA PERSIGA AL BOTÓN PRESIONADO
            pnlIndicador.Top = btnClickeado.Top;
            pnlIndicador.Visible = true;
            pnlIndicador.BringToFront();

            this.ActiveControl = null;
        }

        private void BtnToggle_Click(object sender, EventArgs e)
        {
            CyberButton[] todosLosBotones = { btnPuntoVenta, btnClientes, btnStock, btnProductos, btnCategorias, btnUsuarios, btnSucursales, btnDashboard, btnCerrarSesion };

            if (panelMenuLateral.Width == 250)
            {
                panelMenuLateral.Width = 60;
                lblLogoTexto.Visible = false;
                lblUsuarioActivo.Visible = false;

                foreach (var b in todosLosBotones)
                {
                    if (b != null)
                    {
                        b.Size = new Size(50, 42);
                        if (b.Tag.ToString().Contains(" "))
                            b.TextButton = b.Tag.ToString().Split(' ')[0];
                    }
                }
            }
            else
            {
                panelMenuLateral.Width = 250;
                lblLogoTexto.Visible = true;
                lblUsuarioActivo.Visible = true;

                foreach (var b in todosLosBotones)
                {
                    if (b != null)
                    {
                        b.Size = new Size(240, 42);
                        b.TextButton = b.Tag.ToString();
                    }
                }
            }
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
            if (SesionActual.IdPerfil != 1)
            {
                btnUsuarios.Visible = false;
                btnSucursales.Visible = false;
                btnDashboard.Visible = false;
            }

            if (SesionActual.IdPerfil == 2)
            {
                btnStock.Visible = false;
                btnProductos.Visible = false;
                btnCategorias.Visible = false;

                ResaltarBotonActivo(btnPuntoVenta);
                AbrirFormularioEnPanel(new FrmPuntoVenta());
            }
            else if (SesionActual.IdPerfil == 3)
            {
                btnPuntoVenta.Visible = false;
                btnClientes.Visible = false;

                ResaltarBotonActivo(btnStock);
                AbrirFormularioEnPanel(new FrmStock());
            }
            else
            {
                ResaltarBotonActivo(btnDashboard);
                AbrirFormularioEnPanel(new FrmDashboard());
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
    }
}