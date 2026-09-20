using AorusMarket.Utilidades;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient; // Agregamos esto para la prueba

namespace AorusMarket.Formularios
{
    public partial class FrmLogin : Form
    {
        // Ahora usamos directamente la paleta minimalista de EstiloApp
        private readonly Color colorFondo = EstiloApp.Fondo;
        private readonly Color colorRojoNeon = EstiloApp.RojoNeon;
        private readonly Color colorRojoOscuro = EstiloApp.RojoOscuro;
        private readonly Color colorGris = EstiloApp.Gris;
        private readonly Color colorCampo = EstiloApp.FondoPanel;

        private TextBox txtEmail;
        private TextBox txtPassword;
        private Label lblMensaje;
        private Button btnIngresar;

        public FrmLogin()
        {
            InitializeComponent();
            ConfigurarFormulario();
            ConstruirInterfaz();

          

            this.FormClosed += (s, e) => Application.Exit();
        }

        // ======================================================
        // LA PRUEBA DE FUEGO (Se ejecuta apenas abrís el sistema)
        // ======================================================
       

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

            string correo = txtEmail.Text.Trim();
            string clave = txtPassword.Text;

            try
            {
                // Abrimos la conexión a tu base de datos
                using (System.Data.SqlClient.SqlConnection conexion = AorusMarket.AccesoDatos.Conexion.ObtenerConexion())
                {
                    // Buscamos si existe un usuario con ese email y esa contraseña
                    string query = "SELECT id_usuario, nombre, apellido, id_perfil, id_sucursal FROM usuario WHERE email = @email AND password = @clave";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@email", correo);
                    cmd.Parameters.AddWithValue("@clave", clave);

                    conexion.Open();

                    // Leemos el resultado
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // Si encontró al usuario en la tabla...
                        {
                            // Llenamos la sesión con los datos reales que vinieron de SQL Server
                            Utilidades.SesionActual.IdUsuario = Convert.ToInt32(reader["id_usuario"]);
                            Utilidades.SesionActual.NombreCompleto = reader["nombre"].ToString() + " " + reader["apellido"].ToString();
                            Utilidades.SesionActual.IdPerfil = Convert.ToInt32(reader["id_perfil"]);
                            Utilidades.SesionActual.IdSucursal = Convert.ToInt32(reader["id_sucursal"]);

                            // Nombres genéricos por ahora (para no hacer la consulta más larga con JOINs)
                            Utilidades.SesionActual.NombrePerfil = "Perfil ID: " + Utilidades.SesionActual.IdPerfil;
                            Utilidades.SesionActual.NombreSucursal = "Sucursal ID: " + Utilidades.SesionActual.IdSucursal;

                            // Abrimos el menú principal
                            this.Hide();
                            MDIParent1 mdi = new MDIParent1();
                            mdi.FormClosed += (s, args) => this.Close();
                            mdi.Show();
                        }
                        else
                        {
                            // Si no lo encontró, rebotó
                            lblMensaje.Text = "Correo o contraseña incorrectos";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error de base de datos: " + ex.Message;
            }
        }
    }
}