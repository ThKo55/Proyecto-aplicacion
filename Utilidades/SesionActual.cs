namespace AorusMarket.Utilidades
{
    public static class SesionActual
    {
        public static int IdUsuario { get; set; }
        public static string NombreCompleto { get; set; }
        public static int IdPerfil { get; set; }        // 1=Admin, 2=Cajero, 3=Gestor Stock
        public static string NombrePerfil { get; set; }
        public static int IdSucursal { get; set; }
        public static string NombreSucursal { get; set; }

        public static void CerrarSesion()
        {
            IdUsuario = 0;
            NombreCompleto = null;
            IdPerfil = 0;
            NombrePerfil = null;
            IdSucursal = 0;
            NombreSucursal = null;
        }
    }
}