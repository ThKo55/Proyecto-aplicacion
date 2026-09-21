using System.Configuration; // Agregado para leer el App.config
using System.Data.SqlClient;

namespace AorusMarket.AccesoDatos
{
    public static class Conexion
    {
        public static SqlConnection ObtenerConexion()
        {
            // Ahora la cadena ya no está quemada en el código, la lee del archivo externo
            string cadena = ConfigurationManager.ConnectionStrings["CadenaSQL"].ConnectionString;
            return new SqlConnection(cadena);
        }
    }
}