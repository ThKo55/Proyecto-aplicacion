using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using AorusMarket.Entidades;

namespace AorusMarket.Utilidades
{
    public static class GeneradorFactura
    {
        public static void GenerarPDF(int idVenta, string fecha, string cliente, string cajero, decimal total, string metodoPago, List<DetalleHistorial> detalles, bool abrirAutomatico = false)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
            sfd.FileName = $"Factura_Aorus_Nro{idVenta}.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Configuramos el documento tamaño A4
                    Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    // Título principal
                    Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, BaseColor.DARK_GRAY);
                    Paragraph titulo = new Paragraph("AORUS MARKET - FACTURA", fontTitulo);
                    titulo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(titulo);
                    doc.Add(new Chunk("\n\n"));

                    // Información de cabecera
                    Font fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                    Font fontBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);

                    doc.Add(new Paragraph($"Ticket N°: {idVenta}", fontBold));
                    doc.Add(new Paragraph($"Fecha: {fecha}", fontNormal));
                    doc.Add(new Paragraph($"Cliente: {cliente}", fontNormal));
                    doc.Add(new Paragraph($"Cajero: {cajero}", fontNormal));
                    doc.Add(new Paragraph($"Método de Pago: {metodoPago}", fontNormal));
                    doc.Add(new Chunk("\n\n"));

                    // Tabla de productos
                    PdfPTable tabla = new PdfPTable(4);
                    tabla.WidthPercentage = 100;
                    tabla.SetWidths(new float[] { 10f, 50f, 20f, 20f }); // Porcentajes de columnas

                    // Encabezados de tabla
                    tabla.AddCell(new PdfPCell(new Phrase("Cant.", fontBold)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabla.AddCell(new PdfPCell(new Phrase("Descripción", fontBold)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabla.AddCell(new PdfPCell(new Phrase("P. Unit", fontBold)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabla.AddCell(new PdfPCell(new Phrase("Subtotal", fontBold)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                    // Llenar datos
                    foreach (var item in detalles)
                    {
                        tabla.AddCell(item.Cantidad.ToString());
                        tabla.AddCell(item.NombreProducto);
                        tabla.AddCell("$" + item.PrecioUnitario.ToString("0.00"));
                        tabla.AddCell("$" + item.SubTotal.ToString("0.00"));
                    }
                    doc.Add(tabla);
                    doc.Add(new Chunk("\n"));

                    // Total final
                    Font fontTotal = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.RED);
                    Paragraph pTotal = new Paragraph($"TOTAL: ${total.ToString("0.00")}", fontTotal);
                    pTotal.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(pTotal);

                    doc.Close();

                    // Mostrar mensaje o abrir el PDF directamente
                    if (abrirAutomatico)
                        Process.Start(sfd.FileName);
                    else
                        MessageBox.Show("Factura PDF descargada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}