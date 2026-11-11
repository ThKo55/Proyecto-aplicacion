using System;
using System.IO;
using System.Text;

namespace AorusMarket
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        // Un archivo por día, por usuario de Windows
        public static string Carpeta { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AorusMarket", "Logs");

        public static void Info(string mensaje) { Escribir("INFO", mensaje, null); }
        public static void Error(string contexto, Exception ex) { Escribir("ERROR", contexto, ex); }

        private static void Escribir(string nivel, string mensaje, Exception ex)
        {
            try
            {
                lock (_lock)
                {
                    Directory.CreateDirectory(Carpeta);
                    string archivo = Path.Combine(Carpeta, "aorus_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

                    StringBuilder sb = new StringBuilder();
                    sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                      .Append(" [").Append(nivel).Append("] ").Append(mensaje);
                    if (ex != null) sb.AppendLine().Append(ex.ToString()); // incluye el stack trace
                    sb.AppendLine();

                    File.AppendAllText(archivo, sb.ToString(), Encoding.UTF8);
                }
            }
            catch { /* el logger nunca debe romper la aplicación */ }
        }

        public static void LimpiarAntiguos(int diasAConservar)
        {
            try
            {
                if (!Directory.Exists(Carpeta)) return;
                foreach (string f in Directory.GetFiles(Carpeta, "aorus_*.log"))
                    if (File.GetLastWriteTime(f) < DateTime.Now.AddDays(-diasAConservar)) File.Delete(f);
            }
            catch (Exception ex) { Logger.Error("StockDatos.Listar", ex); }
        }
    }
}