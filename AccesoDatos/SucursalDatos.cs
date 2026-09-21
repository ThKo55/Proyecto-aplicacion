using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AorusMarket.Entidades; // Importamos el molde de Entidades

namespace AorusMarket.AccesoDatos
{
    public class SucursalDatos
    {
        // 1. Método para buscar todas las sucursales activas y enviarlas a la grilla
        public List<Sucursal> Listar()
        {
            List<Sucursal> lista = new List<Sucursal>();

            // 'using' abre la conexión y asegura que se cierre sola al terminar
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Solo listamos las sucursales donde activo = 1
                string query = "SELECT id_sucursal, nombre, direccion, telefono FROM sucursal WHERE activo = 1";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.CommandType = CommandType.Text;

                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Sucursal()
                            {
                                IdSucursal = Convert.ToInt32(dr["id_sucursal"]),
                                Nombre = dr["nombre"].ToString(),
                                Direccion = dr["direccion"].ToString(),
                                Telefono = dr["telefono"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Sucursal>(); // Si falla, devuelve lista vacía
                    Console.WriteLine("Error al listar sucursales: " + ex.Message);
                }
            }
            return lista;
        }

        // 2. Método para guardar una nueva sucursal en la BD
        public bool Insertar(Sucursal obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "INSERT INTO sucursal (nombre, direccion, telefono) VALUES (@nombre, @direccion, @telefono)";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(obj.Direccion) ? (object)DBNull.Value : obj.Direccion);
                cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(obj.Telefono) ? (object)DBNull.Value : obj.Telefono);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0; // True si afectó 1 o más filas
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Console.WriteLine("Error al insertar sucursal: " + ex.Message);
                }
            }
            return respuesta;
        }

        // 3. Método para modificar los datos de una sucursal que ya existe
        public bool Editar(Sucursal obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "UPDATE sucursal SET nombre = @nombre, direccion = @direccion, telefono = @telefono WHERE id_sucursal = @id";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(obj.Direccion) ? (object)DBNull.Value : obj.Direccion);
                cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(obj.Telefono) ? (object)DBNull.Value : obj.Telefono);
                cmd.Parameters.AddWithValue("@id", obj.IdSucursal);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Console.WriteLine("Error al editar sucursal: " + ex.Message);
                }
            }
            return respuesta;
        }

        // 4. Método para Eliminar (Aplicamos Baja Lógica para no romper historiales de venta/stock)
        public bool Eliminar(int id)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // NO hacemos DELETE. Ocultamos la sucursal actualizando el estado 'activo' a 0
                string query = "UPDATE sucursal SET activo = 0 WHERE id_sucursal = @id";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Console.WriteLine("Error al eliminar sucursal: " + ex.Message);
                }
            }
            return respuesta;
        }
    }
}