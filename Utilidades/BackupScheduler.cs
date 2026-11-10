using System;
using System.Threading.Tasks;
using AorusMarket.Entidades;
using AorusMarket.Negocio;

namespace AorusMarket.Utilidades
{
    public static class BackupScheduler
    {
        private static System.Windows.Forms.Timer _timer;
        private static readonly BackupNegocio _negocio = new BackupNegocio();
        private static ConfigBackup _config;
        private static bool _ejecutando;
        private static DateTime _ultimoIntento = DateTime.MinValue;

        public static void Iniciar()
        {
            Detener();

            // Solo desde equipos de Administrador o Gerente (evita backups duplicados desde cajas)
            if (SesionActual.IdPerfil != 1 && SesionActual.IdPerfil != 4) return;

            RecargarConfig();
            _timer = new System.Windows.Forms.Timer { Interval = 30000 };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public static void Detener()
        {
            if (_timer == null) return;
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        public static void RecargarConfig()
        {
            try { _config = _negocio.ObtenerConfig(); }
            catch { }
        }

        private static async void Timer_Tick(object sender, EventArgs e)
        {
            if (_ejecutando || _config == null || !_config.Activo) return;

            TimeSpan hora;
            if (!TimeSpan.TryParse(_config.HoraProgramada, out hora)) return;

            DateTime ahora = DateTime.Now;
            DateTime programada = ahora.Date + hora;

            // Ventana de 2 horas desde la hora programada (por si la app se abrió un rato tarde)
            bool enVentana = ahora >= programada && ahora < programada.AddHours(2);
            bool yaHecho = _config.UltimoBackup.HasValue && _config.UltimoBackup.Value.Date == ahora.Date;
            bool reintentoPronto = (ahora - _ultimoIntento).TotalMinutes < 30;

            if (!enVentana || yaHecho || reintentoPronto) return;

            _ejecutando = true;
            _ultimoIntento = ahora;

            ConfigBackup cfg = _config;
            try
            {
                bool ok = await Task.Run(() =>
                {
                    string msg, ruta;
                    return _negocio.CrearBackup(cfg.RutaDestino, cfg.TipoDestino,
                                                cfg.CantidadAConservar, "Automático",
                                                out msg, out ruta);
                });

                if (ok) _config.UltimoBackup = DateTime.Now;
            }
            catch { }
            finally { _ejecutando = false; }
        }
    }
}