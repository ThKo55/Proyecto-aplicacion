using System.Drawing;
using System.Windows.Forms;

namespace AorusMarket.Utilidades
{
    public static class EstiloApp
    {
        public static readonly Color Fondo = Color.FromArgb(12, 12, 12);
        public static readonly Color FondoPanel = Color.FromArgb(20, 20, 20);
        public static readonly Color RojoNeon = Color.FromArgb(255, 23, 68);
        public static readonly Color RojoOscuro = Color.FromArgb(90, 10, 20);
        public static readonly Color Gris = Color.FromArgb(140, 140, 140);
        public static readonly Color Blanco = Color.White;
        public static readonly Color Verde = Color.FromArgb(0, 200, 120);

        public static Font FuenteTitulo => new Font("Arial Black", 18F, FontStyle.Bold);
        public static Font FuenteLabel => new Font("Segoe UI", 9F, FontStyle.Bold);
        public static Font FuenteTexto => new Font("Segoe UI", 10F);
        public static Font FuenteBoton => new Font("Segoe UI", 10F, FontStyle.Bold);

        public static Label CrearTitulo(string texto, Point ubicacion)
        {
            return new Label
            {
                Text = texto,
                Font = FuenteTitulo,
                ForeColor = RojoNeon,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = ubicacion
            };
        }

        public static Label CrearLabel(string texto, Point ubicacion)
        {
            return new Label
            {
                Text = texto,
                Font = FuenteLabel,
                ForeColor = RojoNeon,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = ubicacion
            };
        }

        public static TextBox CrearTextBox(Point ubicacion, int ancho)
        {
            return new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = FondoPanel,
                ForeColor = Blanco,
                Font = FuenteTexto,
                Location = ubicacion,
                Size = new Size(ancho, 28)
            };
        }

        public static ComboBox CrearComboBox(Point ubicacion, int ancho)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = FondoPanel,
                ForeColor = Blanco,
                Font = FuenteTexto,
                FlatStyle = FlatStyle.Flat,
                Location = ubicacion,
                Size = new Size(ancho, 28)
            };
        }

        public static Button CrearBoton(string texto, Point ubicacion, int ancho, Color colorBorde)
        {
            Button btn = new Button
            {
                Text = texto,
                Font = FuenteBoton,
                Size = new Size(ancho, 38),
                Location = ubicacion,
                FlatStyle = FlatStyle.Flat,
                BackColor = Fondo,
                ForeColor = colorBorde,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = colorBorde;
            btn.FlatAppearance.MouseOverBackColor = colorBorde;
            btn.MouseEnter += (s, e) => btn.ForeColor = Color.Black;
            btn.MouseLeave += (s, e) => btn.ForeColor = colorBorde;
            return btn;
        }

        public static void EstilizarGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = FondoPanel;
            dgv.BorderStyle = BorderStyle.None;
            dgv.ForeColor = Blanco;
            dgv.GridColor = Color.FromArgb(40, 40, 40);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Fondo;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = RojoNeon;
            dgv.ColumnHeadersDefaultCellStyle.Font = FuenteLabel;
            dgv.ColumnHeadersHeight = 34;
            dgv.DefaultCellStyle.BackColor = FondoPanel;
            dgv.DefaultCellStyle.ForeColor = Blanco;
            dgv.DefaultCellStyle.SelectionBackColor = RojoOscuro;
            dgv.DefaultCellStyle.SelectionForeColor = Blanco;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.RowTemplate.Height = 32;
            dgv.Font = FuenteTexto;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }
    }
}