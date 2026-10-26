using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AorusMarket.Entidades;
using System.Data;
using System.Data.SqlClient;

namespace AorusMarket.AccesoDatos
{
    public class BackupDatos
    {
        // 1. Procedimiento para registrar la solicitud
        public bool RegistrarSolicitud(BackupSolicitud solicitud)
        {
            // Usamos tu método ObtenerConexion() que lee del App.config
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("SP_RegistrarSolicitudBackup", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SolicitanteId", solicitud.SolicitanteId);
                cmd.Parameters.AddWithValue("@FechaProgramada", solicitud.FechaProgramada);

                conexion.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();
                return filasAfectadas > 0;
            }
        }

        // 2. Procedimiento para aprobar y ejecutar
        public bool EjecutarBackup(int idSolicitud, int aprobadorId, string ruta)
        {
            // Usamos tu método ObtenerConexion()
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("SP_AprobarYEjecutarBackup", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);
                cmd.Parameters.AddWithValue("@AprobadorId", aprobadorId);
                cmd.Parameters.AddWithValue("@RutaArchivo", ruta);

                conexion.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();
                return filasAfectadas > 0;
            }
        }

        // 3. Método para listar pendientes
        public DataTable ListarPendientes()
        {
            DataTable tabla = new DataTable();
            // Usamos tu método ObtenerConexion()
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("SP_ListarBackupsPendientes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tabla);
            }
            return tabla;
        }
    }
}