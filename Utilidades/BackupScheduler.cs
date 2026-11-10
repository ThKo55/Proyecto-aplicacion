using AorusMarket.Entidades;
using AorusMarket.Negocio;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AorusMarket.Utilidades
{
    public static class BackupScheduler
    {
        private static Timer _timer;
        private static readonly BackupNegocio _negocio = new BackupNegocio();
        private static ConfigBackup _config;
        private static bool _ejecutando;
        private static string _ultimoMinutoEjecutado = "";

        public static void Iniciar()
        {
            Detener();
            RecargarConfig();

            _timer = new Timer { Interval = 10000 }; // Revisa cada 10 segundos
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public static void Detener()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        public static void RecargarConfig()
        {
            try
            {
                _config = _negocio.ObtenerConfig();
            }
            catch { }
        }

        private static async void Timer_Tick(object sender, EventArgs e)
        {
            if (_ejecutando || _config == null || !_config.Activo) return;

            if (!TimeSpan.TryParse(_config.HoraProgramada, out TimeSpan horaConfigurada)) return;

            string horaActualMinuto = DateTime.Now.ToString("HH:mm");
            string horaProgramadaMinuto = horaConfigurada.ToString(@"hh\:mm");

            // Si es la hora programada y todavía NO se ejecutó en este mismo minuto exacto
            if (horaActualMinuto == horaProgramadaMinuto && _ultimoMinutoEjecutado != horaActualMinuto)
            {
                _ultimoMinutoEjecutado = horaActualMinuto; // Marcamos este minuto como procesado
                _ejecutando = true;

                try
                {
                    string rutaDirectorio = string.IsNullOrWhiteSpace(_config.RutaDestino)
                                            ? @"C:\Backups_AorusMarket"
                                            : _config.RutaDestino;

                    if (!Directory.Exists(rutaDirectorio))
                    {
                        Directory.CreateDirectory(rutaDirectorio);
                    }

                    // Ejecución asíncrona para no congelar la interfaz
                    await Task.Run(() =>
                    {
                        string msg, ruta;
                        _negocio.CrearBackup(rutaDirectorio,
                                             _config.TipoDestino,
                                             _config.CantidadAConservar,
                                             "Automático",
                                             out msg, out ruta);
                    });

                    _config.UltimoBackup = DateTime.Now;
                    Console.WriteLine("[ÉXITO] ¡Backup automático generado correctamente en: " + rutaDirectorio + "]");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Falló el backup automático: {ex.Message}");
                }
                finally
                {
                    _ejecutando = false;
                }
            }

            // Si cambia el minuto, permitimos que pueda volver a ejecutarse al día siguiente a la misma hora
            if (horaActualMinuto != horaProgramadaMinuto)
            {
                _ultimoMinutoEjecutado = "";
            }
        }
    }
}