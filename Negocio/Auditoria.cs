using System;
using System.Data;
using AorusMarket.AccesoDatos;
using AorusMarket.Utilidades;

namespace AorusMarket.Negocio
{
    public static class Auditoria
    {
        private static readonly AuditoriaDatos _datos = new AuditoriaDatos();

        // Registra la acción a nombre del usuario con sesión iniciada
        public static void Registrar(string accion, string entidad, string detalle = null)
        {
            int? id = SesionActual.IdUsuario > 0 ? SesionActual.IdUsuario : (int?)null;
            RegistrarComo(id, SesionActual.NombreCompleto, accion, entidad, detalle);
        }

        // Para eventos sin sesión (ej: login fallido)
        public static void RegistrarComo(int? idUsuario, string usuario, string accion, string entidad, string detalle = null)
        {
            // La auditoría nunca debe romper la operación principal
            try { _datos.Registrar(idUsuario, usuario, accion, entidad, detalle); }
            catch { }
        }

        public static DataTable Listar(DateTime desde, DateTime hasta, string entidad)
        {
            return _datos.Listar(desde, hasta, entidad);
        }
    }
}