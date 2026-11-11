using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AorusMarket.Entidades;

namespace AorusMarket.AccesoDatos
{
    public class ProductoDatos
    {
        public List<Producto> Listar()
        {
            List<Producto> lista = new List<Producto>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT p.id_producto, p.codigo_barras, p.nombre, p.marca, p.descripcion, p.unidad_medida, p.id_categoria, c.nombre AS Categoria 
                    FROM producto p
                    INNER JOIN categoria c ON p.id_categoria = c.id_categoria
                    WHERE p.activo = 1";

                SqlCommand cmd = new SqlCommand(query, oConexion);
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
                                CodigoBarras = dr["codigo_barras"] != DBNull.Value ? dr["codigo_barras"].ToString() : "",
                                Nombre = dr["nombre"].ToString(),
                                Marca = dr["marca"] != DBNull.Value ? dr["marca"].ToString() : "",
                                Descripcion = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : "",
                                UnidadMedida = dr["unidad_medida"] != DBNull.Value ? dr["unidad_medida"].ToString() : "",
                                IdCategoria = Convert.ToInt32(dr["id_categoria"]),
                                NombreCategoria = dr["Categoria"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Error al listar productos: ", ex);
                }
            }
            return lista;
        }

        public bool Insertar(Producto obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "INSERT INTO producto (codigo_barras, nombre, marca, descripcion, unidad_medida, id_categoria) VALUES (@cb, @nom, @mar, @desc, @um, @cat)";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@cb", string.IsNullOrEmpty(obj.CodigoBarras) ? (object)DBNull.Value : obj.CodigoBarras);
                cmd.Parameters.AddWithValue("@nom", obj.Nombre);
                cmd.Parameters.AddWithValue("@mar", string.IsNullOrEmpty(obj.Marca) ? (object)DBNull.Value : obj.Marca);
                cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(obj.Descripcion) ? (object)DBNull.Value : obj.Descripcion);
                cmd.Parameters.AddWithValue("@um", string.IsNullOrEmpty(obj.UnidadMedida) ? (object)DBNull.Value : obj.UnidadMedida);
                cmd.Parameters.AddWithValue("@cat", obj.IdCategoria);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex) { Logger.Error("Error al insertar producto: ", ex); }
            }
            return respuesta;
        }

        public bool Editar(Producto obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "UPDATE producto SET codigo_barras=@cb, nombre=@nom, marca=@mar, descripcion=@desc, unidad_medida=@um, id_categoria=@cat WHERE id_producto=@id";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@cb", string.IsNullOrEmpty(obj.CodigoBarras) ? (object)DBNull.Value : obj.CodigoBarras);
                cmd.Parameters.AddWithValue("@nom", obj.Nombre);
                cmd.Parameters.AddWithValue("@mar", string.IsNullOrEmpty(obj.Marca) ? (object)DBNull.Value : obj.Marca);
                cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(obj.Descripcion) ? (object)DBNull.Value : obj.Descripcion);
                cmd.Parameters.AddWithValue("@um", string.IsNullOrEmpty(obj.UnidadMedida) ? (object)DBNull.Value : obj.UnidadMedida);
                cmd.Parameters.AddWithValue("@cat", obj.IdCategoria);
                cmd.Parameters.AddWithValue("@id", obj.IdProducto);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex) { Logger.Error("Error al editar producto: ", ex); }
            }
            return respuesta;
        }

        public bool Eliminar(int id)
        {
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("UPDATE producto SET activo = 0 WHERE id_producto = @id", oConexion);
                cmd.Parameters.AddWithValue("@id", id);
                try
                {
                    oConexion.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex) { Logger.Error("ProductoDatos.ExisteNombre", ex); return false; }
            }
        }



        public bool ExisteCodigoBarras(string codigo, int idProductoExcluido)
        {
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM producto WHERE codigo_barras = @cb AND id_producto <> @id", oConexion);
                cmd.Parameters.AddWithValue("@cb", codigo);
                cmd.Parameters.AddWithValue("@id", idProductoExcluido);
                try
                {
                    oConexion.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
                catch (Exception ex) { Logger.Error("ProductoDatos.ExisteNombre", ex); return false; }
            }
        }
    }
}