using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;

namespace AorusMarket.Formularios
{
    public partial class MDIParent1 : Form
    {
        private MenuStrip menuPrincipal;
        private StatusStrip barraEstado;
        private ToolStripStatusLabel lblUsuarioActivo;

        private ToolStripMenuItem menuUsuarios;
        private ToolStripMenuItem menuSucursales;
        private ToolStripMenuItem menuCategorias;
        private ToolStripMenuItem menuProductos;
        private ToolStripMenuItem menuStock;
        private ToolStripMenuItem menuClientes;
        private ToolStripMenuItem menuPuntoVenta;
        private ToolStripMenuItem menuDashboard;

        public MDIParent1()
        {
            InitializeComponent();
            ConfigurarFormulario();
            ConstruirMenu();
            ConstruirBarraEstado();
            AplicarPermisosPorPerfil();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "AorusMarket";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = EstiloApp.Fondo;
        }

        private void ConstruirMenu()
        {
            menuPrincipal = new MenuStrip
            {
                BackColor = EstiloApp.Fondo,
                ForeColor = EstiloApp.Blanco,
                Font = EstiloApp.FuenteTexto,
                Renderer = new ToolStripProfessionalRenderer(new ColoresMenu())
            };

            var menuArchivo = new ToolStripMenuItem("Archivo");
            var itemCerrarSesion = new ToolStripMenuItem("Cerrar Sesión");
            itemCerrarSesion.Click += ItemCerrarSesion_Click;
            menuArchivo.DropDownItems.Add(itemCerrarSesion);

            var menuAdministracion = new ToolStripMenuItem("Administración");
            menuUsuarios = new ToolStripMenuItem("Usuarios");
            menuSucursales = new ToolStripMenuItem("Sucursales");
            menuCategorias = new ToolStripMenuItem("Categorías");
            menuProductos = new ToolStripMenuItem("Productos");
            menuUsuarios.Click += (s, e) => AbrirHijo(new FrmUsuarios());
            menuSucursales.Click += (s, e) => AbrirHijo(new FrmSucursales());
            menuCategorias.Click += (s, e) => AbrirHijo(new FrmCategorias());
            menuProductos.Click += (s, e) => AbrirHijo(new FrmProductos());
            menuAdministracion.DropDownItems.AddRange(new ToolStripItem[]
                { menuUsuarios, menuSucursales, menuCategorias, menuProductos });

            var menuStockPadre = new ToolStripMenuItem("Stock");
            menuStock = new ToolStripMenuItem("Gestión de Stock");
            menuStock.Click += (s, e) => AbrirHijo(new FrmStock());
            menuStockPadre.DropDownItems.Add(menuStock);

            var menuVentas = new ToolStripMenuItem("Ventas");
            menuPuntoVenta = new ToolStripMenuItem("Punto de Venta");
            menuClientes = new ToolStripMenuItem("Clientes");
            menuPuntoVenta.Click += (s, e) => AbrirHijo(new FrmPuntoVenta());
            menuClientes.Click += (s, e) => AbrirHijo(new FrmClientes());
            menuVentas.DropDownItems.AddRange(new ToolStripItem[] { menuPuntoVenta, menuClientes });

            var menuReportes = new ToolStripMenuItem("Reportes");
            menuDashboard = new ToolStripMenuItem("Dashboard");
            menuDashboard.Click += (s, e) => AbrirHijo(new FrmDashboard());
            menuReportes.DropDownItems.Add(menuDashboard);

            menuPrincipal.Items.AddRange(new ToolStripItem[]
                { menuArchivo, menuAdministracion, menuStockPadre, menuVentas, menuReportes });

            this.MainMenuStrip = menuPrincipal;
            this.Controls.Add(menuPrincipal);
        }

        private void ConstruirBarraEstado()
        {
            barraEstado = new StatusStrip { BackColor = EstiloApp.Fondo };
            lblUsuarioActivo = new ToolStripStatusLabel
            {
                ForeColor = EstiloApp.RojoNeon,
                Text = $"{SesionActual.NombreCompleto} - {SesionActual.NombrePerfil} - Sucursal: {SesionActual.NombreSucursal}"
            };
            barraEstado.Items.Add(lblUsuarioActivo);
            this.Controls.Add(barraEstado);
        }

        private void AplicarPermisosPorPerfil()
        {
            // 1 = Administrador, 2 = Cajero, 3 = Gestor de Stock (según tabla perfil)
            if (SesionActual.IdPerfil != 1)
            {
                menuUsuarios.Visible = false;
                menuSucursales.Visible = false;
                menuDashboard.Visible = false;
            }
            if (SesionActual.IdPerfil == 2) // Cajero: no ve stock
            {
                menuStock.Visible = false;
                menuCategorias.Visible = false;
                menuProductos.Visible = false;
            }
            if (SesionActual.IdPerfil == 3) // Gestor de Stock: no ve punto de venta
            {
                menuPuntoVenta.Visible = false;
            }
        }

        private void AbrirHijo(Form formHijo)
        {
            formHijo.MdiParent = this;
            formHijo.WindowState = FormWindowState.Maximized;
            formHijo.Show();
        }

        private void ItemCerrarSesion_Click(object sender, EventArgs e)
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

    // Clase auxiliar para pintar el menú en negro/rojo
    internal class ColoresMenu : ProfessionalColorTable
    {
        public override Color MenuItemSelected => EstiloApp.RojoOscuro;
        public override Color MenuItemBorder => EstiloApp.RojoNeon;
        public override Color MenuItemSelectedGradientBegin => EstiloApp.RojoOscuro;
        public override Color MenuItemSelectedGradientEnd => EstiloApp.RojoOscuro;
        public override Color ToolStripDropDownBackground => EstiloApp.FondoPanel;
        public override Color ImageMarginGradientBegin => EstiloApp.FondoPanel;
        public override Color ImageMarginGradientMiddle => EstiloApp.FondoPanel;
        public override Color ImageMarginGradientEnd => EstiloApp.FondoPanel;
        public override Color MenuBorder => EstiloApp.RojoNeon;
        public override Color MenuStripGradientBegin => EstiloApp.Fondo;
        public override Color MenuStripGradientEnd => EstiloApp.Fondo;
    }
}