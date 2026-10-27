using System;
using System.Drawing;
using System.Windows.Forms;
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmReportes : Form
    {
        private ComboBox cmbTipoReporte;
        private DateTimePicker dtpDesde, dtpHasta;
        private CyberButton btnGenerar;
        private DataGridView dgvReporte;

        private ReporteNegocio _reporteNegocio = new ReporteNegocio();

        public FrmReportes()
        {
            // InitializeComponent(); // Descomentalo si usás el diseñador visual de VS
            this.BackColor = EstiloApp.Fondo;
            this.Text = "Módulo de Reportes";
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Controls.Add(EstiloApp.CrearTitulo("MÓDULO DE REPORTES Y ESTADÍSTICAS", new Point(30, 20)));

            int x = 30, y = 80;

            // 1. Selector de Tipo de Reporte
            this.Controls.Add(EstiloApp.CrearLabel("SELECCIONE EL REPORTE A VISUALIZAR:", new Point(x, y)));
            cmbTipoReporte = new ComboBox
            {
                Location = new Point(x, y + 20),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTipoReporte.Items.AddRange(new object[] {
                "1. Recaudación por Medio de Pago",
                "2. Evolución de Ventas Diarias",
                "3. Comparativa por Sucursales",
                "4. Desempeño por Cajero",
                "5. Distribución por Categoría de Producto"
            });
            cmbTipoReporte.SelectedIndex = 0; // Selecciona el primero por defecto
            this.Controls.Add(cmbTipoReporte);

            // 2. Filtros de Fecha
            this.Controls.Add(EstiloApp.CrearLabel("DESDE:", new Point(x + 380, y)));
            dtpDesde = new DateTimePicker
            {
                Location = new Point(x + 380, y + 20),
                Size = new Size(130, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F),
                Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) // Primer día del mes
            };
            this.Controls.Add(dtpDesde);

            this.Controls.Add(EstiloApp.CrearLabel("HASTA:", new Point(x + 530, y)));
            dtpHasta = new DateTimePicker
            {
                Location = new Point(x + 530, y + 20),
                Size = new Size(130, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F),
                Value = DateTime.Now // Día actual
            };
            this.Controls.Add(dtpHasta);

            // 3. Botón Generar
            btnGenerar = EstiloApp.CrearBoton("GENERAR REPORTE", new Point(x + 690, y + 10), 200, EstiloApp.Verde);
            btnGenerar.Click += BtnGenerar_Click;
            this.Controls.Add(btnGenerar);

            y += 70;

          
            // 4. Grilla Dinámica (Acá se van a mostrar los datos)
            dgvReporte = new DataGridView
            {
                Location = new Point(x, y),

                // LA SOLUCIÓN: En vez de "1000", le decimos que ocupe el ancho de la ventana menos 60 píxeles de margen
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - y - 30),

                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // APLICAMOS EL NUEVO DISEÑO PREMIUM
            AplicarDisenoPremium(dgvReporte);

            this.Controls.Add(dgvReporte);
        }

        private void AplicarDisenoPremium(DataGridView grid)
        {
            // 1. Apagar los estilos por defecto de Windows (CRÍTICO para poder pintar las cabeceras)
            grid.EnableHeadersVisualStyles = false;

            // 2. Ajustes generales del contenedor
            grid.BackgroundColor = Color.FromArgb(45, 45, 48); // Fondo oscuro base
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // Solo líneas divisorias horizontales
            grid.GridColor = Color.FromArgb(70, 70, 70); // Color de las líneas
            grid.RowHeadersVisible = false; // Oculta la columna selectora izquierda

            // 3. Estilo de la Cabecera (Títulos)
            DataGridViewCellStyle estiloCabecera = new DataGridViewCellStyle();
            estiloCabecera.BackColor = Color.FromArgb(30, 30, 30); // Gris muy oscuro
            estiloCabecera.ForeColor = Color.White;
            estiloCabecera.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            estiloCabecera.Alignment = DataGridViewContentAlignment.MiddleCenter;
            estiloCabecera.SelectionBackColor = Color.FromArgb(30, 30, 30); // Evita que se pinte de azul al hacer clic
            grid.ColumnHeadersDefaultCellStyle = estiloCabecera;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 45; // Cabecera más alta

            // 4. Estilo de las Filas (Datos)
            DataGridViewCellStyle estiloFila = new DataGridViewCellStyle();
            estiloFila.BackColor = Color.FromArgb(50, 50, 55); // Gris intermedio
            estiloFila.ForeColor = Color.WhiteSmoke;
            estiloFila.Font = new Font("Segoe UI", 11F);
            estiloFila.SelectionBackColor = Color.FromArgb(220, 53, 69); // Rojo Aorus al seleccionar
            estiloFila.SelectionForeColor = Color.White;
            grid.DefaultCellStyle = estiloFila;

            // 5. Filas alternadas (Efecto cebra sutil para facilitar la lectura)
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(55, 55, 60);

            // 6. Configuración de comportamiento
            grid.RowTemplate.Height = 40; // Filas bien altas
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
        }

        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            DateTime desde = dtpDesde.Value;
            DateTime hasta = dtpHasta.Value;

            // Limpiamos la grilla antes de cargar el nuevo reporte
            dgvReporte.DataSource = null;
            dgvReporte.Columns.Clear();

            // Dependiendo de lo que eligió en el ComboBox, llamamos a un método distinto
            switch (cmbTipoReporte.SelectedIndex)
            {
                case 0: // Medio de Pago
                    dgvReporte.DataSource = _reporteNegocio.RecaudacionPorMedioPago(desde, hasta);
                    break;
                case 1: // Evolución
                    dgvReporte.DataSource = _reporteNegocio.EvolucionVentas(desde, hasta);
                    break;
                case 2: // Sucursales
                    dgvReporte.DataSource = _reporteNegocio.VentasPorSucursal(desde, hasta);
                    break;
                case 3: // Cajeros
                    dgvReporte.DataSource = _reporteNegocio.VentasPorCajero(desde, hasta);
                    break;
                case 4: // Categorías
                    dgvReporte.DataSource = _reporteNegocio.VentasPorCategoria(desde, hasta);
                    break;
            }

            DarFormatoColumnas();
        }

        private void DarFormatoColumnas()
        {
            if (dgvReporte.Columns.Count == 0) return;

            // Recorremos las columnas generadas automáticamente para darles formato visual
            foreach (DataGridViewColumn col in dgvReporte.Columns)
            {
                // Si la columna es de dinero, le ponemos el formato Moneda ($)
                if (col.Name.Contains("Total") || col.Name.Contains("Recaudado") || col.Name.Contains("Vendido"))
                {
                    col.DefaultCellStyle.Format = "C2";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (col.Name.Contains("Cantidad"))
                {
                    // Si la columna es de cantidad, la centramos
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    // Para textos normales (nombres, fechas, sucursales), alineamos a la izquierda pero con margen
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }

                // Le damos un poco de espaciado a los nombres de las cabeceras (ej: "TotalRecaudado" -> "Total Recaudado")
                col.HeaderText = System.Text.RegularExpressions.Regex.Replace(col.HeaderText, "([A-Z])", " $1").Trim();
            }

            dgvReporte.ClearSelection();
        }
    }
}