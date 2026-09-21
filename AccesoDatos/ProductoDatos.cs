using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AorusMarket.Entidades; // Acceso a la Entidad Producto

namespace AorusMarket.AccesoDatos
{
    public class ProductoDatos
    {
        // 1. Método para listar los productos activos y cruzarlos con su categoría
        public List<Producto> Listar()
        {
            List<Producto> lista = new List<Producto>();

            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Usamos INNER JOIN para traer el nombre de la categoría correspondiente
                string query = @"
                    SELECT p.id_producto, p.nombre, p.descripcion, p.id_categoria, c.nombre AS Categoria 
                    FROM producto p
                    INNER JOIN categoria c ON p.id_categoria = c.id_categoria
                    WHERE p.activo = 1";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.CommandType = CommandType.Text;

                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto()
                            {
                                IdProducto = Convert.ToInt32(dr["id_producto"]),
                                Nombre = dr["nombre"].ToString(),
                                Descripcion = dr["descripcion"].ToString(),
                                IdCategoria = Convert.ToInt32(dr["id_categoria"]),
                                NombreCategoria = dr["Categoria"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Producto>();
                    Console.WriteLine("Error al listar productos: " + ex.Message);
                }
            }
            return lista;
        }

        // 2. Método para guardar un nuevo producto
        public bool Insertar(Producto obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "INSERT INTO producto (nombre, descripcion, id_categoria) VALUES (@nombre, @descripcion, @id_categoria)";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(obj.Descripcion) ? (object)DBNull.Value : obj.Descripcion);
                cmd.Parameters.AddWithValue("@id_categoria", obj.IdCategoria);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Console.WriteLine("Error al insertar producto: " + ex.Message);
                }
            }
            return respuesta;
        }

        // 3. Método para actualizar un producto existente
        public bool Editar(Producto obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "UPDATE producto SET nombre = @nombre, descripcion = @descripcion, id_categoria = @id_categoria WHERE id_producto = @id";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(obj.Descripcion) ? (object)DBNull.Value : obj.Descripcion);
                cmd.Parameters.AddWithValue("@id_categoria", obj.IdCategoria);
                cmd.Parameters.AddWithValue("@id", obj.IdProducto);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Console.WriteLine("Error al editar producto: " + ex.Message);
                }
            }
            return respuesta;
        }

        // 4. Método para Eliminar (Baja Lógica)
        public bool Eliminar(int id)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Baja lógica para no romper el historial de ventas
                string query = "UPDATE producto SET activo = 0 WHERE id_producto = @id";
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
                    Console.WriteLine("Error al eliminar producto: " + ex.Message);
                }
            }
            return respuesta;
        }
    }
}