using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AorusMarket.Entidades; // Importamos el molde de la entidad Cliente

namespace AorusMarket.AccesoDatos
{
    public class ClienteDatos
    {
        // 1. Método para buscar todos los clientes activos y enviarlos a la grilla
        public List<Cliente> Listar()
        {
            List<Cliente> lista = new List<Cliente>();

            // 'using' abre la conexión y asegura que se cierre sola al terminar
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Solo listamos los clientes donde activo = 1
                string query = "SELECT id_cliente, nombre, apellido, dni, telefono, email, direccion FROM cliente WHERE activo = 1";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.CommandType = CommandType.Text;

                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Cliente()
                            {
                                IdCliente = Convert.ToInt32(dr["id_cliente"]),
                                Nombre = dr["nombre"].ToString(),
                                Apellido = dr["apellido"].ToString(),
                                Dni = dr["dni"].ToString(),
                                Telefono = dr["telefono"].ToString(),
                                Email = dr["email"].ToString(),
                                Direccion = dr["direccion"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Cliente>(); // Si falla, devuelve lista vacía para no romper el programa
                    Console.WriteLine("Error al listar clientes: " + ex.Message);
                }
            }
            return lista;
        }

        // 2. Método para guardar un nuevo cliente en la base de datos
        public bool Insertar(Cliente obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "INSERT INTO cliente (nombre, apellido, dni, telefono, email, direccion) VALUES (@nombre, @apellido, @dni, @telefono, @email, @direccion)";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("@apellido", obj.Apellido);
                cmd.Parameters.AddWithValue("@dni", string.IsNullOrEmpty(obj.Dni) ? (object)DBNull.Value : obj.Dni);
                cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(obj.Telefono) ? (object)DBNull.Value : obj.Telefono);
                cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(obj.Email) ? (object)DBNull.Value : obj.Email);
                cmd.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(obj.Direccion) ? (object)DBNull.Value : obj.Direccion);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0; // True si afectó 1 o más filas
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Console.WriteLine("Error al insertar cliente: " + ex.Message);
                }
            }
            return respuesta;
        }

        // 3. Método para sobreescribir los datos de un cliente que ya existe
        public bool Editar(Cliente obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "UPDATE cliente SET nombre = @nombre, apellido = @apellido, dni = @dni, telefono = @telefono, email = @email, direccion = @direccion WHERE id_cliente = @id";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("@apellido", obj.Apellido);
                cmd.Parameters.AddWithValue("@dni", string.IsNullOrEmpty(obj.Dni) ? (object)DBNull.Value : obj.Dni);
                cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(obj.Telefono) ? (object)DBNull.Value : obj.Telefono);
                cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(obj.Email) ? (object)DBNull.Value : obj.Email);
                cmd.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(obj.Direccion) ? (object)DBNull.Value : obj.Direccion);
                cmd.Parameters.AddWithValue("@id", obj.IdCliente);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Console.WriteLine("Error al editar cliente: " + ex.Message);
                }
            }
            return respuesta;
        }

        // 4. Método para Eliminar (Aplicamos Baja Lógica como mejoramos en el modelo)
        public bool Eliminar(int id)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Solo ocultamos el cliente, NO hacemos DELETE para proteger el historial de ventas
                string query = "UPDATE cliente SET activo = 0 WHERE id_cliente = @id";
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
                    Console.WriteLine("Error al eliminar cliente: " + ex.Message);
                }
            }
            return respuesta;
        }
    }
}