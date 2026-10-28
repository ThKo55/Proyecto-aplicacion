using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // Para el gráfico en memoria
using iTextSharp.text; // Para el PDF
using iTextSharp.text.pdf; // Para el PDF
using AorusMarket.Utilidades;
using ReaLTaiizor.Controls;
using AorusMarket.Negocio;

namespace AorusMarket.Formularios
{
    public partial class FrmReportes : Form
    {
        private ComboBox cmbTipoReporte;
        private DateTimePicker dtpDesde, dtpHasta;
        private CyberButton btnGenerar, btnGenerarGrafico; // Se agregó el nuevo botón
        private DataGridView dgvReporte;

        private ReporteNegocio _reporteNegocio = new ReporteNegocio();

        public FrmReportes()
        {
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
                Font = new System.Drawing.Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTipoReporte.Items.AddRange(new object[] {
                "1. Recaudación por Medio de Pago",
                "2. Evolución de Ventas Diarias",
                "3. Comparativa por Sucursales",
                "4. Desempeño por Cajero",
                "5. Distribución por Categoría de Producto"
            });
            cmbTipoReporte.SelectedIndex = 0;
            this.Controls.Add(cmbTipoReporte);

            // 2. Filtros de Fecha
            this.Controls.Add(EstiloApp.CrearLabel("DESDE:", new Point(x + 380, y)));
            dtpDesde = new DateTimePicker
            {
                Location = new Point(x + 380, y + 20),
                Size = new Size(130, 28),
                Format = DateTimePickerFormat.Short,
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
            };
            this.Controls.Add(dtpDesde);

            this.Controls.Add(EstiloApp.CrearLabel("HASTA:", new Point(x + 530, y)));
            dtpHasta = new DateTimePicker
            {
                Location = new Point(x + 530, y + 20),
                Size = new Size(130, 28),
                Format = DateTimePickerFormat.Short,
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Value = DateTime.Now
            };
            this.Controls.Add(dtpHasta);

            // 3. Botón Generar Reporte (Tabla)
            btnGenerar = EstiloApp.CrearBoton("GENERAR TABLA", new Point(x + 690, y + 10), 160, EstiloApp.Verde);
            btnGenerar.Click += BtnGenerar_Click;
            this.Controls.Add(btnGenerar);

            // --- NUEVO: Botón Generar Gráfico en PDF ---
            btnGenerarGrafico = EstiloApp.CrearBoton("DESCARGAR GRÁFICO", new Point(x + 860, y + 10), 180, EstiloApp.Gris);
            btnGenerarGrafico.Click += BtnGenerarGrafico_Click;
            this.Controls.Add(btnGenerarGrafico);

            y += 70;

            // 4. Grilla Dinámica (Ocupa todo el ancho)
            dgvReporte = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - y - 30), // Corregido
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            AplicarDisenoPremium(dgvReporte);
            this.Controls.Add(dgvReporte);
        }

        private void AplicarDisenoPremium(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.FromArgb(45, 45, 48);
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(70, 70, 70);
            grid.RowHeadersVisible = false;

            DataGridViewCellStyle estiloCabecera = new DataGridViewCellStyle();
            estiloCabecera.BackColor = Color.FromArgb(30, 30, 30);
            estiloCabecera.ForeColor = Color.White;
            estiloCabecera.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            estiloCabecera.Alignment = DataGridViewContentAlignment.MiddleCenter;
            estiloCabecera.SelectionBackColor = Color.FromArgb(30, 30, 30);
            grid.ColumnHeadersDefaultCellStyle = estiloCabecera;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 45;

            DataGridViewCellStyle estiloFila = new DataGridViewCellStyle();
            estiloFila.BackColor = Color.FromArgb(50, 50, 55);
            estiloFila.ForeColor = Color.WhiteSmoke;
            estiloFila.Font = new System.Drawing.Font("Segoe UI", 11F);
            estiloFila.SelectionBackColor = Color.FromArgb(220, 53, 69);
            estiloFila.SelectionForeColor = Color.White;
            grid.DefaultCellStyle = estiloFila;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(55, 55, 60);
            grid.RowTemplate.Height = 40;
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

            dgvReporte.DataSource = null;
            dgvReporte.Columns.Clear();

            switch (cmbTipoReporte.SelectedIndex)
            {
                case 0: dgvReporte.DataSource = _reporteNegocio.RecaudacionPorMedioPago(desde, hasta); break;
                case 1: dgvReporte.DataSource = _reporteNegocio.EvolucionVentas(desde, hasta); break;
                case 2: dgvReporte.DataSource = _reporteNegocio.VentasPorSucursal(desde, hasta); break;
                case 3: dgvReporte.DataSource = _reporteNegocio.VentasPorCajero(desde, hasta); break;
                case 4: dgvReporte.DataSource = _reporteNegocio.VentasPorCategoria(desde, hasta); break;
            }

            DarFormatoColumnas();
        }

        private void DarFormatoColumnas()
        {
            if (dgvReporte.Columns.Count == 0) return;

            foreach (DataGridViewColumn col in dgvReporte.Columns)
            {
                if (col.Name.Contains("Total") || col.Name.Contains("Recaudado") || col.Name.Contains("Vendido"))
                {
                    col.DefaultCellStyle.Format = "C2";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (col.Name.Contains("Cantidad"))
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                col.HeaderText = System.Text.RegularExpressions.Regex.Replace(col.HeaderText, "([A-Z])", " $1").Trim();
            }
            dgvReporte.ClearSelection();
        }

        // --- MAGIA: GENERACIÓN DEL GRÁFICO EN MEMORIA Y EXPORTACIÓN A PDF ---
        private void BtnGenerarGrafico_Click(object sender, EventArgs e)
        {
            if (dgvReporte.Rows.Count == 0)
            {
                MessageBox.Show("Primero debe generar una tabla de datos para poder crear el gráfico.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
            sfd.FileName = $"Grafico_Aorus_{DateTime.Now:ddMMyyyy}.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 1. Crear el control del gráfico de forma invisible en la memoria RAM
                    Chart graficoTemp = new Chart();
                    graficoTemp.Size = new Size(900, 500); // Resolución en la que se dibujará
                    graficoTemp.BackColor = Color.White;

                    ChartArea area = new ChartArea();
                    area.AxisX.MajorGrid.LineColor = Color.LightGray;
                    area.AxisY.MajorGrid.LineColor = Color.LightGray;
                    area.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
                    area.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
                    graficoTemp.ChartAreas.Add(area);

                    Legend leyenda = new Legend();
                    leyenda.Font = new System.Drawing.Font("Segoe UI", 11F);
                    graficoTemp.Legends.Add(leyenda);

                    Series serie = new Series("Datos");
                    serie.IsValueShownAsLabel = true;
                    serie.Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold);
                    serie.Palette = ChartColorPalette.Pastel;

                    // 2. Determinar el tipo de gráfico basado en el ComboBox
                    int indexReporte = cmbTipoReporte.SelectedIndex;
                    if (indexReporte == 0) serie.ChartType = SeriesChartType.Pie;
                    else if (indexReporte == 1) { serie.ChartType = SeriesChartType.Line; serie.BorderWidth = 4; serie.Color = Color.Crimson; }
                    else if (indexReporte == 2) serie.ChartType = SeriesChartType.Column;
                    else if (indexReporte == 3) serie.ChartType = SeriesChartType.Bar;
                    else if (indexReporte == 4) serie.ChartType = SeriesChartType.Doughnut;

                    // 3. Leer los datos directamente de la grilla y pasarlos al gráfico
                    foreach (DataGridViewRow row in dgvReporte.Rows)
                    {
                        string nombreEjeX = row.Cells[0].Value.ToString();
                        decimal valorEjeY = Convert.ToDecimal(row.Cells[row.Cells.Count - 1].Value); // Asumimos que la última columna siempre es el Total
                        serie.Points.AddXY(nombreEjeX, valorEjeY);
                    }

                    graficoTemp.Series.Add(serie);
                    graficoTemp.Update(); // Forzamos a que se dibuje internamente

                    // 4. Pasar la imagen al PDF con iTextSharp
                    using (MemoryStream ms = new MemoryStream())
                    {
                        graficoTemp.SaveImage(ms, ChartImageFormat.Png);
                        iTextSharp.text.Image imagenGrafico = iTextSharp.text.Image.GetInstance(ms.GetBuffer());
                        imagenGrafico.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        imagenGrafico.ScaleToFit(520f, 400f);

                        Document doc = new Document(PageSize.A4, 30, 30, 30, 30);
                        PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                        doc.Open();

                        // Títulos
                        string textoReporte = cmbTipoReporte.Text.Substring(3).ToUpper();
                        iTextSharp.text.Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                        Paragraph titulo = new Paragraph($"AORUS MARKET - GRÁFICO DE {textoReporte}", fontTitulo);
                        titulo.Alignment = Element.ALIGN_CENTER;
                        doc.Add(titulo);

                        Paragraph subtitulo = new Paragraph($"Período: {dtpDesde.Value:dd/MM/yyyy} al {dtpHasta.Value:dd/MM/yyyy}", FontFactory.GetFont(FontFactory.HELVETICA, 12));
                        subtitulo.Alignment = Element.ALIGN_CENTER;
                        doc.Add(subtitulo);
                        doc.Add(new Chunk("\n\n"));

                        // Insertamos la imagen capturada
                        doc.Add(imagenGrafico);

                        doc.Close();
                    }

                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar el gráfico: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}