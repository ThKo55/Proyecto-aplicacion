using System;
using System.Drawing;
using System.Windows.Forms;

namespace AorusMarket.Formularios
{
    public partial class FrmDetalleAuditoria : Form
    {
        public FrmDetalleAuditoria(string fecha, string usuario, string accion, string detalle)
        {
            // Agrandamos un poquito el alto de la ventana para que respire mejor
            this.Size = new Size(500, 460);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Información del Registro";

            // Título principal
            Label lblTitulo = new Label
            {
                Text = "DETALLE DE LA OPERACIÓN",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.MediumSeaGreen,
                AutoSize = true,
                Location = new Point(25, 20)
            };
            this.Controls.Add(lblTitulo);

            // Información básica (le quitamos la palabra Detalle de acá para que no empuje hacia abajo)
            Label lblInfo = new Label
            {
                Text = $"📅  FECHA:   {fecha}\n\n" +
                       $"👤  USUARIO:   {usuario}\n\n" +
                       $"⚙️  ACCIÓN:   {accion}",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(25, 70),
                AutoSize = true,
                ForeColor = Color.LightGray
            };
            this.Controls.Add(lblInfo);

            // Título del Detalle separado y con su propia posición (Y = 175)
            Label lblDetalleTitulo = new Label
            {
                Text = "📝  DETALLE:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(25, 175),
                AutoSize = true,
                ForeColor = Color.LightGray
            };
            this.Controls.Add(lblDetalleTitulo);

            // Panel contenedor posicionado más abajo (Y = 205) para no superponerse
            Panel pnlDetalle = new Panel
            {
                Location = new Point(25, 205),
                Size = new Size(430, 130),
                AutoScroll = true,
                BackColor = Color.FromArgb(18, 18, 18)
            };

            // Label que muestra el texto, ahora con un límite de ancho (400) 
            // que deja espacio para que no se coma la barra de desplazamiento
            Label lblDetalleContenido = new Label
            {
                Text = detalle,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.White,
                Location = new Point(0, 0),
                AutoSize = true,
                MaximumSize = new Size(400, 0) // Clave para que haga salto de línea correctamente
            };

            pnlDetalle.Controls.Add(lblDetalleContenido);
            this.Controls.Add(pnlDetalle);

            // Botón de Cerrar movido un poco más abajo (Y = 360)
            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(175, 360),
                Size = new Size(150, 35),
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCerrar);
        }
    }
}