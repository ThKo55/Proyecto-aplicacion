using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace AorusMarket.Utilidades
{
    /// <summary>
    /// Exporta cualquier DataGridView a un archivo .xlsx.
    /// - Solo exporta lo que el usuario ve: columnas visibles y filas visibles (respeta filtros/buscadores).
    /// - Omite las columnas de botones (por ejemplo "Ver Detalle").
    /// - Los montos y cantidades se guardan como números, así se pueden sumar en Excel.
    /// </summary>
    public static class ExportadorExcel
    {
        private const string FormatoMoneda = "$ #,##0.00";
        private const string FormatoFecha = "dd/MM/yyyy HH:mm";
        private const int FilaEncabezado = 4;
        private const double AnchoMaximoColumna = 60;

        public static void Exportar(DataGridView grid, string titulo, string nombreArchivoBase, string subtitulo = null)
        {
            if (grid == null) return;

            var columnas = grid.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible && !(c is DataGridViewButtonColumn))
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            var filas = grid.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Visible && !r.IsNewRow)
                .ToList();

            if (columnas.Count == 0 || filas.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Exportar a Excel",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string tituloSeguro = string.IsNullOrWhiteSpace(titulo) ? "Datos" : titulo.Trim();

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Libro de Excel (*.xlsx)|*.xlsx";
                sfd.FileName = $"{nombreArchivoBase}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var libro = new XLWorkbook())
                    {
                        var hoja = libro.Worksheets.Add(NombreHojaSeguro(tituloSeguro));

                        // --- Título e información del reporte ---
                        var celdaTitulo = hoja.Cell(1, 1);
                        celdaTitulo.SetValue("AORUS MARKET - " + tituloSeguro.ToUpper());
                        celdaTitulo.Style.Font.Bold = true;
                        celdaTitulo.Style.Font.FontSize = 14;
                        celdaTitulo.Style.Font.FontColor = XLColor.FromHtml("#DC3545");

                        string info = $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm} por {SesionActual.NombreCompleto}";
                        if (!string.IsNullOrWhiteSpace(subtitulo)) info = subtitulo + "  |  " + info;

                        var celdaInfo = hoja.Cell(2, 1);
                        celdaInfo.SetValue(info);
                        celdaInfo.Style.Font.Italic = true;
                        celdaInfo.Style.Font.FontColor = XLColor.Gray;

                        // --- Encabezados ---
                        for (int c = 0; c < columnas.Count; c++)
                            hoja.Cell(FilaEncabezado, c + 1).SetValue(columnas[c].HeaderText);

                        var rangoEncabezado = hoja.Range(FilaEncabezado, 1, FilaEncabezado, columnas.Count);
                        rangoEncabezado.Style.Font.Bold = true;
                        rangoEncabezado.Style.Font.FontColor = XLColor.White;
                        rangoEncabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#DC3545");

                        // --- Datos ---
                        for (int f = 0; f < filas.Count; f++)
                        {
                            for (int c = 0; c < columnas.Count; c++)
                            {
                                object valor = filas[f].Cells[columnas[c].Index].Value;
                                EscribirCelda(hoja.Cell(FilaEncabezado + 1 + f, c + 1), valor);
                            }
                        }

                        // --- Formato final ---
                        int ultimaFila = FilaEncabezado + filas.Count;
                        var rangoTabla = hoja.Range(FilaEncabezado, 1, ultimaFila, columnas.Count);
                        rangoTabla.SetAutoFilter();
                        hoja.SheetView.FreezeRows(FilaEncabezado);

                        // Se ajusta cada columna solo con las filas de la tabla (sin contar el título de arriba)
                        for (int c = 1; c <= columnas.Count; c++)
                        {
                            var columna = hoja.Column(c);
                            columna.AdjustToContents(FilaEncabezado, ultimaFila);
                            if (columna.Width > AnchoMaximoColumna) columna.Width = AnchoMaximoColumna;
                        }

                        libro.SaveAs(sfd.FileName);
                    }

                    if (MessageBox.Show("Archivo exportado correctamente.\n\n¿Desea abrirlo ahora?",
                            "Exportar a Excel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Process.Start(sfd.FileName);
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("No se pudo guardar el archivo. Si está abierto en Excel, cerralo e intentá de nuevo.",
                        "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    Logger.Error("ExportadorExcel.Exportar", ex);
                    MessageBox.Show("No se pudo exportar a Excel: " + ex.Message,
                        "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Números y fechas se guardan con su tipo real; todo lo demás como texto
        // (así un código de barras como "00123456" no pierde los ceros del principio).
        private static void EscribirCelda(IXLCell celda, object valor)
        {
            if (valor == null || valor == DBNull.Value) return;

            switch (valor)
            {
                case decimal d:
                    celda.Value = d;
                    celda.Style.NumberFormat.Format = FormatoMoneda;
                    break;
                case double db:
                    celda.Value = db;
                    celda.Style.NumberFormat.Format = FormatoMoneda;
                    break;
                case int i:
                    celda.Value = i;
                    break;
                case long l:
                    celda.Value = l;
                    break;
                case DateTime dt:
                    celda.Value = dt;
                    celda.Style.NumberFormat.Format = FormatoFecha;
                    break;
                default:
                    celda.SetValue(valor.ToString());
                    break;
            }
        }

        // Excel limita el nombre de la hoja a 31 caracteres y no admite [ ] : * ? / \
        private static string NombreHojaSeguro(string titulo)
        {
            string nombre = new string(titulo.Where(ch => "[]:*?/\\".IndexOf(ch) < 0).ToArray()).Trim();
            if (nombre.Length == 0) nombre = "Datos";
            return nombre.Length > 31 ? nombre.Substring(0, 31) : nombre;
        }
    }
}