using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;
using System.Data;

namespace AorusMarket.Negocio
{
    public class BackupNegocio
    {
        private BackupDatos datos = new BackupDatos();

        public bool Solicitar(BackupSolicitud solicitud)
        {
            if (solicitud.FechaProgramada < DateTime.Now)
                throw new Exception("La fecha programada no puede ser en el pasado.");

            return datos.RegistrarSolicitud(solicitud);
        }

        public bool Aprobar(int idSolicitud, int solicitanteId, int aprobadorId, string ruta)
        {
            // REGLA CLAVE CRUZADA: No puede aprobar quien lo solicitó
            if (solicitanteId == aprobadorId)
                throw new Exception("El aprobador debe ser un usuario diferente al solicitante.");

            // Se ejecuta y se guarda en la ruta indicada por el formulario
            return datos.EjecutarBackup(idSolicitud, aprobadorId, ruta);
        }

        public DataTable ListarPendientes()
        {
            // Solo le pasa la pelota a la capa de Datos para que traiga la grilla
            return datos.ListarPendientes();
        }


    }
}
