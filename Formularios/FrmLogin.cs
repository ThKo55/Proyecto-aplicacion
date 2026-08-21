using System;
using System.Drawing;
using System.Windows.Forms;

namespace AorusMarket.Formularios
{
    public partial class FrmLogin : Form
    {
        // Paleta neón negro/rojo
        private readonly Color colorFondo = Color.FromArgb(12, 12, 12);
        private readonly Color colorRojoNeon = Color.FromArgb(255, 23, 68);
        private readonly Color colorRojoOscuro = Color.FromArgb(90, 10, 20);
        private readonly Color colorGris = Color.FromArgb(140, 140, 140);
        private readonly Color colorCampo = Color.FromArgb(20, 20, 20);

        private TextBox txtEmail;
        private TextBox txtPassword;
        private Label lblMensaje;
        private Button btnIngresar;

        public FrmLogin()
        {
            InitializeComponent();
            ConfigurarFormulario();
            ConstruirInterfaz();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Iniciar Sesión - AorusMarket";
            this.ClientSize = new Size(420, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = colorFondo;
            this.Font = new Font("Segoe UI", 10F);
        }

        private void ConstruirInterfaz()
        {
            int anchoForm = this.ClientSize.Width;

            // ---------- LOGO / TÍTULO ----------
            Label lblAorus = new Label
            {
                Text = "AORUS",
                Font = new Font("Arial Black", 26F, FontStyle.Bold),
                ForeColor = colorRojoNeon,
                BackColor = Color.Transparent,
                AutoSize = true
            };

            Label lblMarket = new Label
            {
                Text = "MARKET",
                Font = new Font("Arial Black", 26F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true
            };

            using (Graphics g = this.CreateGraphics())
            {
                SizeF tam1 = g.MeasureString(lblAorus.Text, lblAorus.Font);
                SizeF tam2 = g.MeasureString(lblMarket.Text, lblMarket.Font);
                int anchoTotal = (int)(tam1.Width + tam2.Width);
                int left = (anchoForm - anchoTotal) / 2;

                lblAorus.Location = new Point(left, 40);
                lblMarket.Location = new Point(left + (int)tam1.Width, 40);
            }

            Label lblSubtitulo = new Label
            {
                Text = "S I S T E M A   D E   G E S T I Ó N",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = colorGris,
                BackColor = Color.Transparent,
                AutoSize = true
            };
            lblSubtitulo.Location = new Point(
                (anchoForm - TextRenderer.MeasureText(lblSubtitulo.Text, lblSubtitulo.Font).Width) / 2, 95);

            Panel lineaDivisoria = new Panel
            {
                BackColor = colorRojoNeon,
                Size = new Size(280, 2),
                Location = new Point((anchoForm - 280) / 2, 130)
            };

            // ---------- CAMPO EMAIL ----------
            Label lblEmail = new Label
            {
                Text = "EMAIL",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = colorRojoNeon,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(60, 175)
            };

            Panel panelEmailFondo = new Panel
            {
                BackColor = colorCampo,
                Location = new Point(55, 195),
                Size = new Size(310, 32)
            };

            txtEmail = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = colorCampo,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F),
                Location = new Point(65, 202),
                Size = new Size(290, 26)
            };

            Panel lineaEmail = new Panel
            {
                BackColor = colorRojoNeon,
                Size = new Size(310, 2),
                Location = new Point(55, 227)
            };

            // ---------- CAMPO CONTRASEÑA ----------
            Label lblPassword = new Label
            {
                Text = "CONTRASEÑA",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = colorRojoNeon,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(60, 255)
            };

            Panel panelPasswordFondo = new Panel
            {
                BackColor = colorCampo,
                Location = new Point(55, 275),
                Size = new Size(310, 32)
            };

            txtPassword = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = colorCampo,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F),
                Location = new Point(65, 282),
                Size = new Size(290, 26),
                PasswordChar = '●'
            };

            Panel lineaPassword = new Panel
            {
                BackColor = colorRojoNeon,
                Size = new Size(310, 2),
                Location = new Point(55, 307)
            };

            // ---------- MENSAJE DE ERROR ----------
            lblMensaje = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = colorRojoNeon,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(310, 20),
                Location = new Point(55, 320)
            };

            // ---------- BOTÓN INGRESAR ----------
            btnIngresar = new Button
            {
                Text = "INGRESAR",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Size = new Size(310, 42),
                Location = new Point(55, 360),
                FlatStyle = FlatStyle.Flat,
                BackColor = colorFondo,
                ForeColor = colorRojoNeon,
                Cursor = Cursors.Hand
            };
            btnIngresar.FlatAppearance.BorderSize = 2;
            btnIngresar.FlatAppearance.BorderColor = colorRojoNeon;
            btnIngresar.FlatAppearance.MouseOverBackColor = colorRojoNeon;
            btnIngresar.FlatAppearance.MouseDownBackColor = colorRojoOscuro;

            btnIngresar.MouseEnter += (s, e) => btnIngresar.ForeColor = Color.Black;
            btnIngresar.MouseLeave += (s, e) => btnIngresar.ForeColor = colorRojoNeon;

            btnIngresar.Click += BtnIngresar_Click;
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    BtnIngresar_Click(s, e);
            };

            // ---------- PIE ----------
            Label lblPie = new Label
            {
                Text = "© 2026 AorusMarket",
                Font = new Font("Segoe UI", 8F),
                ForeColor = colorGris,
                BackColor = Color.Transparent,
                AutoSize = true
            };
            lblPie.Location = new Point(
                (anchoForm - TextRenderer.MeasureText(lblPie.Text, lblPie.Font).Width) / 2, 500);

            // ---------- AGREGAR AL FORM ----------
            this.Controls.Add(lblAorus);
            this.Controls.Add(lblMarket);
            this.Controls.Add(lblSubtitulo);
            this.Controls.Add(lineaDivisoria);
            this.Controls.Add(lblEmail);
            this.Controls.Add(panelEmailFondo);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lineaEmail);
            this.Controls.Add(lblPassword);
            this.Controls.Add(panelPasswordFondo);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lineaPassword);
            this.Controls.Add(lblMensaje);
            this.Controls.Add(btnIngresar);
            this.Controls.Add(lblPie);

            txtEmail.BringToFront();
            txtPassword.BringToFront();
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMensaje.Text = "Debe completar todos los campos";
                return;
            }

            // TODO: reemplazar por validación real contra la base de datos
            Utilidades.SesionActual.IdUsuario = 1;
            Utilidades.SesionActual.NombreCompleto = "Usuario de Prueba";
            Utilidades.SesionActual.IdPerfil = 1;
            Utilidades.SesionActual.NombrePerfil = "Administrador";
            Utilidades.SesionActual.IdSucursal = 1;
            Utilidades.SesionActual.NombreSucursal = "Casa Central";

            this.Hide();
            MDIParent1 mdi = new MDIParent1();
            mdi.FormClosed += (s, args) => this.Close();
            mdi.Show();
        }
    }
}