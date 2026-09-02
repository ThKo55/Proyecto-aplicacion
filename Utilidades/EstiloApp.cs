using System;
using System.Drawing;
using System.Windows.Forms;
using ReaLTaiizor.Controls;

namespace AorusMarket.Utilidades
{
    public static class EstiloApp
    {
        // ==========================================
        // 1. PALETA DE COLORES (Minimalista y Suave)
        // ==========================================
        public static readonly Color Fondo = Color.FromArgb(38, 41, 44);
        public static readonly Color FondoPanel = Color.FromArgb(50, 54, 58);
        public static readonly Color RojoNeon = Color.FromArgb(235, 87, 87);
        public static readonly Color RojoOscuro = Color.FromArgb(190, 70, 70);
        public static readonly Color Gris = Color.FromArgb(170, 175, 180);
        public static readonly Color Blanco = Color.FromArgb(245, 245, 245);
        public static readonly Color Verde = Color.FromArgb(70, 190, 130);

        // ==========================================
        // 2. FUENTES (TIPOGRAFÍA) - Corregido para evitar fugas de memoria
        // ==========================================
        public static readonly Font FuenteTitulo = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font FuenteLabel = new Font("Segoe UI", 9F, FontStyle.Bold);
        public static readonly Font FuenteTexto = new Font("Segoe UI", 10F);
        public static readonly Font FuenteBoton = new Font("Segoe UI", 10F, FontStyle.Bold);

        public static void AplicarEstiloFormulario(Form frm)
        {
            frm.BackColor = Fondo;
        }

        // ==========================================
        // 3. MÉTODOS PARA CREAR CONTROLES DINÁMICOS
        // ==========================================

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
                ForeColor = Gris,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = ubicacion
            };
        }

        // CyberTextBox corregido (Usa ForeColor en lugar de ColorText)
        // CAJAS DE TEXTO CORREGIDAS
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

                TextButton = "" // <--- ESTO BORRA EL TEXTO "cyberText" POR DEFECTO
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

        // CyberButton corregido (Usa ForeColor y eventos para el Hover)
        // BOTONES CORREGIDOS
        public static CyberButton CrearBoton(string texto, Point ubicacion, int ancho, Color colorPrimario)
        {
            CyberButton btn = new CyberButton
            {
                TextButton = texto, // <--- CAMBIAMOS 'Text' POR 'TextButton' PARA QUE MUESTRE "GUARDAR", "NUEVO", ETC.
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

            // Eventos para cambiar la letra a blanco cuando se pasa el mouse
            btn.MouseEnter += (s, e) => btn.ForeColor = Blanco;
            btn.MouseLeave += (s, e) => btn.ForeColor = colorPrimario;

            return btn;
        }

        public static void EstilizarGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = FondoPanel;
            dgv.BorderStyle = BorderStyle.None;
            dgv.ForeColor = Blanco;
            dgv.GridColor = Color.FromArgb(60, 64, 68);
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