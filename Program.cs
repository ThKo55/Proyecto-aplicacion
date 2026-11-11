using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using AorusMarket.Formularios;

namespace AorusMarket
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            CultureInfo culture = new CultureInfo("es-AR");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Errores no controlados en la interfaz y en otros hilos
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => ManejarErrorInesperado(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => ManejarErrorInesperado(e.ExceptionObject as Exception);

            Logger.LimpiarAntiguos(30);
            Logger.Info("Aplicación iniciada");
            Application.Run(new FrmLogin());
        }

        private static void ManejarErrorInesperado(Exception ex)
        {
            Logger.Error("Error no controlado", ex);
            MessageBox.Show(
                "Ocurrió un error inesperado. El detalle quedó registrado en:\n" + Logger.Carpeta +
                "\n\nSi el problema continúa, comuníquese con soporte.",
                "AorusMarket", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}