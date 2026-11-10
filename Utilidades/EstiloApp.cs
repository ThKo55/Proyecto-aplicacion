using System;
using System.Drawing;
using System.Windows.Forms;
using ReaLTaiizor.Controls;

namespace AorusMarket.Utilidades
{
    public static class EstiloApp
    {
        public static readonly Color Fondo = Color.FromArgb(18, 18, 18);
        public static readonly Color FondoPanel = Color.FromArgb(28, 28, 28);
        public static readonly Color RojoNeon = Color.FromArgb(220, 53, 69);
        public static readonly Color RojoOscuro = Color.FromArgb(176, 42, 55);
        public static readonly Color Gris = Color.FromArgb(170, 175, 180);
        public static readonly Color Blanco = Color.FromArgb(245, 245, 245);
        public static readonly Color Verde = Color.FromArgb(40, 167, 69);

        public static readonly Font FuenteTitulo = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font FuenteLabel = new Font("Segoe UI", 9F, FontStyle.Bold);
        public static readonly Font FuenteTexto = new Font("Segoe UI", 10F);
        public static readonly Font FuenteBoton = new Font("Segoe UI", 10F, FontStyle.Bold);

        public static void AplicarEstiloFormulario(Form frm)
        {
            frm.BackColor = Fondo;
        }

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
                ForeColor = Color.LightGray,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = ubicacion
            };
        }

        public static CyberTextBox CrearTextBox(Point ubicacion, int ancho, bool esPassword = false)
        {
            return new CyberTextBox
            {
                Location = ubicacion,
                Size = new Size(ancho, 34),
                Alpha = 20,
                Rounding = true,
                RoundingInt = 8,
                ColorBackground = FondoPanel,
                ColorBackground_Pen = Gris,
                ForeColor = Blanco,
                Font = FuenteTexto,
                Password = esPassword,
                TextButton = ""
            };
        }

        public static string LeerTexto(CyberTextBox txt)
        {
            string val = string.IsNullOrWhiteSpace(txt.TextButton) ? txt.Text : txt.TextButton;
            return val?.Trim() ?? string.Empty;
        }

        public static void EscribirTexto(CyberTextBox txt, string valor)
        {
            string v = valor ?? string.Empty;
            txt.Text = v;
            txt.TextButton = v;
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

        public static CyberButton CrearBoton(string texto, Point ubicacion, int ancho, Color colorPrimario)
        {
            CyberButton btn = new CyberButton
            {
                TextButton = texto,
                Location = ubicacion,
                Size = new Size(ancho, 38),
                Alpha = 20,
                Rounding = true,
                RoundingInt = 8,
                ColorBackground = FondoPanel,
                ColorBackground_Pen = colorPrimario,
                ForeColor = colorPrimario,
                Font = FuenteBoton,
                Cursor = Cursors.Hand
            };

            btn.MouseEnter += (s, e) => btn.ForeColor = Blanco;
            btn.MouseLeave += (s, e) => btn.ForeColor = colorPrimario;

            return btn;
        }

        public static void EstilizarGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Fondo;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(40, 40, 40);
            grid.RowHeadersVisible = false;

            DataGridViewCellStyle estiloCabecera = new DataGridViewCellStyle();
            estiloCabecera.BackColor = FondoPanel;
            estiloCabecera.ForeColor = Color.LightGray;
            estiloCabecera.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            estiloCabecera.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloCabecera.SelectionBackColor = FondoPanel;
            grid.ColumnHeadersDefaultCellStyle = estiloCabecera;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;

            DataGridViewCellStyle estiloFila = new DataGridViewCellStyle();
            estiloFila.BackColor = Fondo;
            estiloFila.ForeColor = Color.WhiteSmoke;
            estiloFila.Font = new Font("Segoe UI", 10F);
            estiloFila.SelectionBackColor = RojoNeon;
            estiloFila.SelectionForeColor = Color.White;
            grid.DefaultCellStyle = estiloFila;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 22, 22);
            grid.RowTemplate.Height = 35;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}