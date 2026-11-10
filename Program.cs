using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AorusMarket.Formularios;
using System.Globalization;
using System.Threading;

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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmLogin());
        }
    }
}