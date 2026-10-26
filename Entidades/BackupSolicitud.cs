using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AorusMarket.Entidades
{
    public class BackupSolicitud
    {
        public int Id { get; set; }
        public int SolicitanteId { get; set; }
        public int? AprobadorId { get; set; }
        public string Estado { get; set; } // "Pendiente", "Aprobado", "Completado"
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaProgramada { get; set; } // La fecha/hora que el usuario eligió
        public string RutaArchivo { get; set; }
    }
}
