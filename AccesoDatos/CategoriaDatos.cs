using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AorusMarket.Entidades; // Importamos la capa de Entidades

namespace AorusMarket.AccesoDatos
{
    public class CategoriaDatos
    {
        // Método para listar las categorías activas en la grilla
        public List<Categoria> Listar()
        {
            List<Categoria> lista = new List<Categoria>();

            // El bloque 'using' asegura que la conexión se cierre y libere automáticamente
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Solo traemos las que tienen activo = 1 (baja lógica)
                string query = "SELECT id_categoria, nombre, descripcion FROM categoria WHERE activo = 1";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.CommandType = CommandType.Text;

                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Categoria()
                            {
                                IdCategoria = Convert.ToInt32(dr["id_categoria"]),
                                Nombre = dr["nombre"].ToString(),
                                Descripcion = dr["descripcion"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Si hay error, devolvemos una lista vacía y lo registramos
                    lista = new List<Categoria>();
                    Console.WriteLine("Error en Listar Categorias: " + ex.Message);
                }
            }
            return lista;
        }

        // Método para Insertar una nueva categoría
        public bool Insertar(Categoria obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "INSERT INTO categoria (nombre, descripcion) VALUES (@nombre, @descripcion)";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", obj.Descripcion);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0; // Si afectó a más de 0 filas, fue exitoso
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Console.WriteLine("Error al insertar: " + ex.Message);
                }
            }
            return respuesta;
        }

        // Método para Editar una categoría existente
        public bool Editar(Categoria obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "UPDATE categoria SET nombre = @nombre, descripcion = @descripcion WHERE id_categoria = @id";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", obj.Descripcion);
                cmd.Parameters.AddWithValue("@id", obj.IdCategoria);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Console.WriteLine("Error al editar: " + ex.Message);
                }
            }
            return respuesta;
        }

        // Método para Eliminar (Baja Lógica)
        public bool Eliminar(int id)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // NO hacemos DELETE, hacemos UPDATE del estado activo a 0
                string query = "UPDATE categoria SET activo = 0 WHERE id_categoria = @id";
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
                    Console.WriteLine("Error al eliminar: " + ex.Message);
                }
            }
            return respuesta;
        }
    }
}