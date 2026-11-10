using System;
using System.Data;
using System.Data.SqlClient;

namespace AorusMarket.AccesoDatos
{
    public class AuditoriaDatos
    {
        public void Registrar(int? idUsuario, string usuario, string accion, string entidad, string detalle)
        {
            const string sql = @"
                INSERT INTO auditoria_acciones (id_usuario, usuario, accion, entidad, detalle)
                VALUES (@idu, @usr, @accion, @entidad, @detalle)";

            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@idu", idUsuario.HasValue ? (object)idUsuario.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@usr", (object)Recortar(usuario, 150) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@accion", Recortar(accion, 60) ?? string.Empty);
                cmd.Parameters.AddWithValue("@entidad", Recortar(entidad, 60) ?? string.Empty);
                cmd.Parameters.AddWithValue("@detalle", (object)Recortar(detalle, 500) ?? DBNull.Value);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable Listar(DateTime desde, DateTime hasta, string entidad)
        {
            DataTable tabla = new DataTable();

            string sql = @"
                SELECT fecha AS Fecha,
                       ISNULL(usuario, 'Sistema') AS Usuario,
                       accion AS Accion,
                       entidad AS Entidad,
                       ISNULL(detalle, '') AS Detalle
                FROM auditoria_acciones
                WHERE fecha >= @desde AND fecha < @hasta";

            if (!string.IsNullOrEmpty(entidad)) sql += " AND entidad = @entidad";
            sql += " ORDER BY fecha DESC";

            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@desde", desde.Date);
                cmd.Parameters.AddWithValue("@hasta", hasta.Date.AddDays(1));
                if (!string.IsNullOrEmpty(entidad)) cmd.Parameters.AddWithValue("@entidad", entidad);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(tabla);
                }
            }
            return tabla;
        }

        private static string Recortar(string texto, int max)
        {
            if (string.IsNullOrEmpty(texto)) return null;
            return texto.Length <= max ? texto : texto.Substring(0, max);
        }
    }
}