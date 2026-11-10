using System;

namespace AorusMarket.Entidades
{
    public class BackupSolicitud
    {
        public int Id { get; set; }
        public int SolicitanteId { get; set; }
        public string SolicitanteNombre { get; set; } // Agregado para la grilla
        public int? AprobadorId { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaProgramada { get; set; }
        public string RutaArchivo { get; set; }
    }
}