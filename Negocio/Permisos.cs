using System.Linq;
using AorusMarket.Negocio;

namespace AorusMarket.Utilidades
{
    public static class Perfiles
    {
        public const int Administrador = 1;
        public const int Cajero = 2;
        public const int GestorStock = 3;
        public const int Gerente = 4;
    }

    public static class Permisos
    {
        public static bool Tiene(params int[] perfilesPermitidos)
        {
            return perfilesPermitidos.Contains(SesionActual.IdPerfil);
        }

        // Si el perfil no alcanza: devuelve false, arma el mensaje y deja el intento en la auditoría
        public static bool Requerir(string accion, out string mensaje, params int[] perfilesPermitidos)
        {
            mensaje = string.Empty;
            if (Tiene(perfilesPermitidos)) return true;

            mensaje = "Su perfil no tiene permiso para " + accion + ".";
            Auditoria.Registrar("Acceso denegado", "Seguridad", "Intento de: " + accion);
            return false;
        }
    }
}