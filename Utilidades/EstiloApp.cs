using System;
using System.Drawing;
using System.Windows.Forms;

namespace AorusMarket.Utilidades
{
    public static class EstiloApp
    {
        // ==========================================
        // 1. PALETA DE COLORES (Minimalista y Suave)
        // ==========================================
        public static readonly Color Fondo = Color.FromArgb(38, 41, 44);       // Gris pizarra suave (no negro puro)
        public static readonly Color FondoPanel = Color.FromArgb(50, 54, 58);  // Gris un poco más claro para contraste
        public static readonly Color RojoNeon = Color.FromArgb(235, 87, 87);   // Rojo pastel/coral (no lastima la vista)
        public static readonly Color RojoOscuro = Color.FromArgb(190, 70, 70); // Rojo mate para cuando pasas el mouse
        public static readonly Color Gris = Color.FromArgb(170, 175, 180);     // Gris claro para textos secundarios
        public static readonly Color Blanco = Color.FromArgb(245, 245, 245);   // Blanco humo (más suave que el blanco puro)
        public static readonly Color Verde = Color.FromArgb(70, 190, 130);     // Verde pastel

        // ==========================================
        // 2. FUENTES (TIPOGRAFÍA)
        // ==========================================
        public static Font FuenteTitulo => new Font("Arial Black", 18F, FontStyle.Bold);
        public static Font FuenteLabel => new Font("Segoe UI", 9F, FontStyle.Bold);
        public static Font FuenteTexto => new Font("Segoe UI", 10F);
        public static Font FuenteBoton => new Font("Segoe UI", 10F, FontStyle.Bold);

        // ==========================================
        // 3. MÉTODOS PARA APLICAR ESTILOS A CONTROLES EXISTENTES (Recomendado)
        // ==========================================

        public static void AplicarEstiloFormulario(Form frm)
        {
            frm.BackColor = Fondo;
        }

        public static void AplicarEstiloTextBox(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = FondoPanel;
            txt.ForeColor = Blanco;
            txt.Font = FuenteTexto;
        }

        public static void AplicarEstiloComboBox(ComboBox cmb)
        {
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.BackColor = FondoPanel;
            cmb.ForeColor = Blanco;
            cmb.Font = FuenteTexto;
            cmb.FlatStyle = FlatStyle.Flat;
        }

        public static void AplicarEstiloLabel(Label lbl, bool esTitulo = false)
        {
            lbl.BackColor = Color.Transparent;
            if (esTitulo)
            {
                lbl.Font = FuenteTitulo;
                lbl.ForeColor = RojoNeon;
            }
            else
            {
                lbl.Font = FuenteLabel;
                lbl.ForeColor = Blanco; // Se usa blanco o gris claro para que contraste con el fondo oscuro
            }
        }

        public static void AplicarEstiloBoton(Button btn, Color colorBorde)
        {
            btn.Font = FuenteBoton;
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Fondo;
            btn.ForeColor = colorBorde;
            btn.Cursor = Cursors.Hand;

            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = colorBorde;
            btn.FlatAppearance.MouseOverBackColor = colorBorde;

            // Evitamos suscribir eventos duplicados quitándolos primero
            btn.MouseEnter -= Btn_MouseEnter;
            btn.MouseLeave -= Btn_MouseLeave;

            // Agregamos el evento de hover
            btn.MouseEnter += Btn_MouseEnter;
            btn.MouseLeave += Btn_MouseLeave;
        }

        // Eventos compartidos para el efecto hover (pasar el mouse por encima) de los botones
        private static void Btn_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.ForeColor = Color.Black;
        }

        private static void Btn_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.ForeColor = btn.FlatAppearance.BorderColor;
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

        // ==========================================
        // 4. MÉTODOS PARA CREAR CONTROLES DINÁMICOS DESDE CÓDIGO
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

            btn.MouseEnter += Btn_MouseEnter;
            btn.MouseLeave += Btn_MouseLeave;

            return btn;
        }
    }
}