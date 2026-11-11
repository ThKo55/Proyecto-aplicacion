using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using AorusMarket.Entidades;

namespace AorusMarket.Utilidades
{
    /// <summary>
    /// Genera el ticket de venta en PDF con formato de rollo de 80 mm.
    /// Es independiente de la factura: no pide dónde guardar, lo deja en
    /// Documentos\AorusMarket\Tickets y lo abre para imprimirlo.
    /// </summary>
    public static class GeneradorTicket
    {
        private const float AnchoPagina = 226.77f; // 80 mm en puntos
        private const float Margen = 10f;

        public static string GenerarTicket(int idVenta, string fecha, string cliente, string cajero, string sucursal,
                                           decimal total, string metodoPago, List<DetalleHistorial> detalles,
                                           decimal? montoEntregado = null, bool abrirAutomatico = true)
        {
            try
            {
                string carpeta = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AorusMarket", "Tickets");
                Directory.CreateDirectory(carpeta);
                string ruta = Path.Combine(carpeta, $"Ticket_Nro{idVenta}.pdf");

                Font fTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                Font fNormal = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                Font fNegrita = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
                Font fTotal = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

                void ArmarContenido(Document doc)
                {
                    doc.Add(Centrado("AORUS MARKET", fTitulo));
                    if (!string.IsNullOrWhiteSpace(sucursal)) doc.Add(Centrado("Sucursal: " + sucursal, fNormal));
                    doc.Add(Centrado("Ticket N° " + idVenta, fNegrita));
                    doc.Add(Separador());

                    doc.Add(new Paragraph("Fecha: " + fecha, fNormal));
                    doc.Add(new Paragraph("Cajero: " + cajero, fNormal));
                    doc.Add(new Paragraph("Cliente: " + cliente, fNormal));
                    doc.Add(Separador());

                    PdfPTable tabla = new PdfPTable(2) { WidthPercentage = 100 };
                    tabla.SetWidths(new float[] { 60f, 40f });
                    foreach (DetalleHistorial item in detalles)
                    {
                        tabla.AddCell(Celda(item.NombreProducto, fNormal, 2, Element.ALIGN_LEFT, 0f));
                        tabla.AddCell(Celda($"{item.Cantidad} x {item.PrecioUnitario:C2}", fNormal, 1, Element.ALIGN_LEFT, 5f));
                        tabla.AddCell(Celda(item.SubTotal.ToString("C2"), fNegrita, 1, Element.ALIGN_RIGHT, 5f));
                    }
                    doc.Add(tabla);
                    doc.Add(Separador());

                    PdfPTable totales = new PdfPTable(2) { WidthPercentage = 100 };
                    totales.SetWidths(new float[] { 45f, 55f });
                    totales.AddCell(Celda("TOTAL", fTotal, 1, Element.ALIGN_LEFT, 3f));
                    totales.AddCell(Celda(total.ToString("C2"), fTotal, 1, Element.ALIGN_RIGHT, 3f));
                    totales.AddCell(Celda("Medio de pago", fNormal, 1, Element.ALIGN_LEFT, 1f));
                    totales.AddCell(Celda(metodoPago, fNormal, 1, Element.ALIGN_RIGHT, 1f));

                    if (montoEntregado.HasValue && metodoPago == "Efectivo")
                    {
                        totales.AddCell(Celda("Efectivo recibido", fNormal, 1, Element.ALIGN_LEFT, 1f));
                        totales.AddCell(Celda(montoEntregado.Value.ToString("C2"), fNormal, 1, Element.ALIGN_RIGHT, 1f));
                        totales.AddCell(Celda("Vuelto", fNegrita, 1, Element.ALIGN_LEFT, 1f));
                        totales.AddCell(Celda((montoEntregado.Value - total).ToString("C2"), fNegrita, 1, Element.ALIGN_RIGHT, 1f));
                    }
                    doc.Add(totales);
                    doc.Add(Separador());

                    doc.Add(Centrado("¡GRACIAS POR SU COMPRA!", fNegrita));
                    doc.Add(Centrado("Documento no válido como factura", fNormal));
                }

                // Primera pasada: página muy alta, solo para medir cuánto ocupa el contenido
                float altoUsado;
                using (MemoryStream ms = new MemoryStream())
                {
                    altoUsado = Escribir(ms, 5000f, ArmarContenido);
                }

                // Segunda pasada: página con la altura justa, sin papel de más
                using (FileStream fs = new FileStream(ruta, FileMode.Create))
                {
                    Escribir(fs, altoUsado + Margen, ArmarContenido);
                }

                if (abrirAutomatico) Process.Start(ruta);
                return ruta;
            }
            catch (IOException)
            {
                MessageBox.Show("No se pudo guardar el ticket. Si ya está abierto en un visor de PDF, cerralo e intentá de nuevo.",
                    "Ticket", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("GeneradorTicket.GenerarTicket", ex);
                MessageBox.Show("No se pudo generar el ticket: " + ex.Message,
                    "Ticket", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Escribe el PDF y devuelve la altura que ocupó el contenido
        private static float Escribir(Stream destino, float alto, Action<Document> armar)
        {
            Document doc = new Document(new Rectangle(AnchoPagina, alto), Margen, Margen, Margen, Margen);
            PdfWriter writer = PdfWriter.GetInstance(doc, destino);
            doc.Open();
            armar(doc);
            float usado = alto - writer.GetVerticalPosition(false) + Margen;
            doc.Close();
            return usado;
        }

        private static Paragraph Centrado(string texto, Font fuente)
        {
            return new Paragraph(texto, fuente) { Alignment = Element.ALIGN_CENTER };
        }

        private static Paragraph Separador()
        {
            return new Paragraph(new Chunk(new DottedLineSeparator())) { SpacingBefore = 2f, SpacingAfter = 4f };
        }

        private static PdfPCell Celda(string texto, Font fuente, int colspan, int alineacion, float espacioAbajo)
        {
            return new PdfPCell(new Phrase(texto, fuente))
            {
                Colspan = colspan,
                HorizontalAlignment = alineacion,
                Border = Rectangle.NO_BORDER,
                PaddingLeft = 0f,
                PaddingRight = 0f,
                PaddingTop = 0f,
                PaddingBottom = espacioAbajo
            };
        }
    }
}