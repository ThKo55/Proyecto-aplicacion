using System;

namespace AorusMarket.Entidades
{
    public class ConfigBackup
    {
        public bool Activo { get; set; } = false;
        public string TipoDestino { get; set; } = "Carpeta Interna / PC";
        public string RutaDestino { get; set; } = @"C:\AorusMarket\Backups\";
        public string HoraProgramada { get; set; } = "03:00";
        public int CantidadAConservar { get; set; } = 5;
        public DateTime? UltimoBackup { get; set; }
    }
}