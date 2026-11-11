using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Entidades;
using AorusMarket.Negocio;
using AorusMarket.Utilidades;

namespace AorusMarket.Formularios
{
    public static class DialogoAutorizacion
    {
        private const int MaxIntentos = 3;

        // Pide las credenciales de OTRO usuario con alguno de los perfiles permitidos.
        // Devuelve el usuario que autorizó, o null si se canceló o falló.
        public static Usuario Solicitar(IWin32Window owner, string titulo, string motivo,
                                        bool exigirPerfilDistinto, params int[] perfilesPermitidos)
        {
            Usuario autorizante = null;
            int intentos = 0;

            using (Form dlg = new Form())
            {
                dlg.Text = titulo;
                dlg.ClientSize = new Size(440, 290);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.BackColor = EstiloApp.Fondo;

                Label lblMotivo = new Label
                {
                    Text = motivo,
                    ForeColor = Color.WhiteSmoke,
                    Location = new Point(20, 15),
                    Size = new Size(400, 80),
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
                };

                Label lblEmail = new Label { Text = "Email:", ForeColor = Color.LightGray, Location = new Point(20, 110), AutoSize = true };
                TextBox txtEmail = new TextBox { Location = new Point(140, 107), Width = 280, BackColor = EstiloApp.FondoPanel, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

                Label lblPass = new Label { Text = "Contraseña:", ForeColor = Color.LightGray, Location = new Point(20, 150), AutoSize = true };
                TextBox txtPass = new TextBox { Location = new Point(140, 147), Width = 280, BackColor = EstiloApp.FondoPanel, ForeColor = Color.White, PasswordChar = '●', BorderStyle = BorderStyle.FixedSingle };

                Button btnOk = new Button { Text = "AUTORIZAR", Location = new Point(160, 210), Size = new Size(125, 34), BackColor = EstiloApp.RojoNeon, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                Button btnCancel = new Button { Text = "CANCELAR", Location = new Point(295, 210), Size = new Size(125, 34), BackColor = EstiloApp.Gris, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnOk.FlatAppearance.BorderSize = 0;
                btnCancel.FlatAppearance.BorderSize = 0;

                btnOk.Click += (s, e) =>
                {
                    string msg;
                    Usuario u = new UsuarioNegocio().Login(txtEmail.Text.Trim(), txtPass.Text, out msg);
                    string error = null;

                    if (u == null) error = msg;
                    else if (Array.IndexOf(perfilesPermitidos, u.IdPerfil) < 0)
                        error = "El usuario no tiene un perfil habilitado para autorizar.";
                    else if (u.IdUsuario == SesionActual.IdUsuario)
                        error = "El autorizante no puede ser el mismo usuario que realiza la operación.";
                    else if (exigirPerfilDistinto && u.IdPerfil == SesionActual.IdPerfil)
                        error = "La autorización debe provenir de un perfil distinto al suyo.";

                    if (error == null)
                    {
                        autorizante = u;
                        dlg.Close();
                        return;
                    }

                    intentos++;
                    Auditoria.RegistrarComo(SesionActual.IdUsuario, SesionActual.NombreCompleto,
                        "Autorización fallida", "Seguridad", titulo + " | credenciales de: " + txtEmail.Text.Trim());

                    if (intentos >= MaxIntentos)
                    {
                        MessageBox.Show("Demasiados intentos fallidos. La operación fue cancelada.",
                            "Autorización denegada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        dlg.Close();
                        return;
                    }

                    MessageBox.Show(error + "\nIntentos restantes: " + (MaxIntentos - intentos),
                        "Autorización denegada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPass.Clear();
                    txtPass.Focus();
                };
                btnCancel.Click += (s, e) => dlg.Close();

                dlg.Controls.Add(lblMotivo);
                dlg.Controls.Add(lblEmail);
                dlg.Controls.Add(txtEmail);
                dlg.Controls.Add(lblPass);
                dlg.Controls.Add(txtPass);
                dlg.Controls.Add(btnOk);
                dlg.Controls.Add(btnCancel);
                dlg.AcceptButton = btnOk;
                dlg.CancelButton = btnCancel;

                dlg.ShowDialog(owner);
            }
            return autorizante;
        }
    }
}