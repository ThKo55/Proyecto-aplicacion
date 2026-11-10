using AorusMarket.Entidades;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.IO;



namespace AorusMarket.AccesoDatos
{
    public class BackupDatos
    {
        public ConfigBackup ObtenerConfig()
        {
            ConfigBackup cfg = new ConfigBackup();

            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT activo, tipo_destino, ruta_destino, hora_programada,
                         cantidad_a_conservar, ultimo_backup
                  FROM configuracion_backup WHERE id = 1", cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        cfg.Activo = Convert.ToBoolean(dr["activo"]);
                        cfg.TipoDestino = dr["tipo_destino"].ToString();

                        string ruta = dr["ruta_destino"].ToString();
                        if (!string.IsNullOrWhiteSpace(ruta)) cfg.RutaDestino = ruta;

                        cfg.HoraProgramada = dr["hora_programada"].ToString();
                        cfg.CantidadAConservar = Convert.ToInt32(dr["cantidad_a_conservar"]);
                        cfg.UltimoBackup = dr["ultimo_backup"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(dr["ultimo_backup"]);
                    }
                }
            }
            return cfg;
        }

        public void GuardarConfig(ConfigBackup c)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM configuracion_backup WHERE id = 1)
                    UPDATE configuracion_backup
                       SET activo = @activo, tipo_destino = @tipo, ruta_destino = @ruta,
                           hora_programada = @hora, cantidad_a_conservar = @cant
                     WHERE id = 1
                ELSE
                    INSERT INTO configuracion_backup
                        (id, activo, tipo_destino, ruta_destino, hora_programada, cantidad_a_conservar)
                    VALUES (1, @activo, @tipo, @ruta, @hora, @cant)";

            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@activo", c.Activo);
                cmd.Parameters.AddWithValue("@tipo", c.TipoDestino);
                cmd.Parameters.AddWithValue("@ruta", c.RutaDestino ?? string.Empty);
                cmd.Parameters.AddWithValue("@hora", c.HoraProgramada);
                cmd.Parameters.AddWithValue("@cant", c.CantidadAConservar);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ActualizarUltimoBackup(DateTime fecha)
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE configuracion_backup SET ultimo_backup = @f WHERE id = 1", cn))
            {
                cmd.Parameters.AddWithValue("@f", fecha);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Registra en backup_registro cada backup / restauración
        public void RegistrarOperacion(string tipo, string origen, string archivo,
                                       int? idSolicitante, int? idAprobador,
                                       string estado, string detalle)
        {
            const string sql = @"
                INSERT INTO backup_registro
                    (tipo_operacion, origen, archivo, id_usuario_solicitante,
                     id_usuario_aprobador, estado, detalle)
                VALUES (@tipo, @origen, @archivo, @sol, @apr, @estado, @detalle)";

            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@origen", origen);
                cmd.Parameters.AddWithValue("@archivo", (object)Recortar(archivo, 500) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@sol", idSolicitante.HasValue ? (object)idSolicitante.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@apr", idAprobador.HasValue ? (object)idAprobador.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@estado", estado);
                cmd.Parameters.AddWithValue("@detalle", (object)Recortar(detalle, 500) ?? DBNull.Value);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable ListarRegistro()
        {
            DataTable tabla = new DataTable();
            const string sql = @"
                SELECT TOP 200
                       r.fecha AS Fecha, r.tipo_operacion AS Tipo, r.origen AS Origen,
                       r.estado AS Estado,
                       ISNULL(us.nombre + ' ' + us.apellido, 'Sistema') AS Solicitante,
                       ISNULL(ua.nombre + ' ' + ua.apellido, '-') AS Aprobador,
                       r.archivo AS Archivo, r.detalle AS Detalle
                FROM backup_registro r
                LEFT JOIN usuario us ON r.id_usuario_solicitante = us.id_usuario
                LEFT JOIN usuario ua ON r.id_usuario_aprobador = ua.id_usuario
                ORDER BY r.fecha DESC";

            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (SqlDataAdapter da = new SqlDataAdapter(sql, cn))
            {
                da.Fill(tabla);
            }
            return tabla;
        }

        private static string Recortar(string texto, int max)
        {
            if (string.IsNullOrEmpty(texto)) return null;
            return texto.Length <= max ? texto : texto.Substring(0, max);
        }


        public async Task EjecutarBackupAutomaticoAsync(string rutaDestino)
        {
            string nombreArchivo = $"SuperMercadoDB_Auto_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            string rutaCompleta = Path.Combine(rutaDestino, nombreArchivo);

            await Task.Run(() =>
            {
                // NOTA: Si en tu archivo Conexion.cs usas un método para obtener la cadena, 
                // reemplaza este string por tu variable de conexión (ej. Conexion.cadena)
                string cadenaConexion = "Server=TU_SERVIDOR;Database=master;Integrated Security=True;";

                using (SqlConnection con = new SqlConnection(cadenaConexion))
                {
                    con.Open();
                    string query = $@"BACKUP DATABASE SuperMercadoDB 
                                      TO DISK = '{rutaCompleta}' 
                                      WITH FORMAT, MEDIANAME = 'AutoBackup', NAME = 'Backup Automatico'";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = 300; // Evita que dé error por tiempo de espera
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }


    }
}