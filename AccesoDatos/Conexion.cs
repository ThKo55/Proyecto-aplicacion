using System.Data.SqlClient; // Usamos la librería nativa para evitar errores

namespace AorusMarket.AccesoDatos
{
    public static class Conexion
    {
        // Corregimos el nombre de la base a SuperMercadoDB
        private static readonly string cadena =
            "Server=localhost,1433;Database=SuperMercadoDB;User Id=sa;Password=Contraseña123!;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadena);
        }
    }
}