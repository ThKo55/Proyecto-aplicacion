using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AorusMarket.Entidades;

namespace AorusMarket.AccesoDatos
{
    public class UsuarioDatos
    {
        // 0. Método de Autenticación (Login) - No lo borres, lo usa el FrmLogin
        public Usuario Autenticar(string email, string password)
        {
            Usuario objUsuario = null;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT 
                        u.id_usuario, u.nombre, u.apellido, u.email, 
                        u.id_perfil, p.nombre AS NombrePerfil, 
                        u.id_sucursal, s.nombre AS NombreSucursal
                    FROM usuario u
                    INNER JOIN perfil p ON u.id_perfil = p.id_perfil
                    INNER JOIN sucursal s ON u.id_sucursal = s.id_sucursal
                    WHERE u.email = @email AND u.password = @password AND u.activo = 1";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);

                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            objUsuario = new Usuario()
                            {
                                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                Nombre = dr["nombre"].ToString(),
                                Apellido = dr["apellido"].ToString(),
                                Email = dr["email"].ToString(),
                                IdPerfil = Convert.ToInt32(dr["id_perfil"]),
                                NombrePerfil = dr["NombrePerfil"].ToString(),
                                IdSucursal = Convert.ToInt32(dr["id_sucursal"]),
                                NombreSucursal = dr["NombreSucursal"].ToString()
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    objUsuario = null;
                    Console.WriteLine("Error de autenticación BD: " + ex.Message);
                }
            }
            return objUsuario;
        }

        public List<ComboUsuario> ObtenerDatosCombo(string tabla, string campoId, string campoNombre)
        {
            List<ComboUsuario> lista = new List<ComboUsuario>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string filtro = tabla == "sucursal" ? "WHERE activo = 1" : "";
                string query = $"SELECT {campoId}, {campoNombre} FROM {tabla} {filtro} ORDER BY {campoNombre} ASC";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ComboUsuario()
                            {
                                Id = Convert.ToInt32(dr[campoId]),
                                Nombre = dr[campoNombre].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar combo {tabla}: " + ex.Message);
                }
            }
            return lista;
        }

        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Agregamos telefono, direccion y fecha_nacimiento al SELECT
                string query = @"
                    SELECT u.id_usuario, u.nombre, u.apellido, u.email, u.telefono, u.direccion, u.fecha_nacimiento, 
                           u.id_perfil, u.id_sucursal, p.nombre AS NombrePerfil, s.nombre AS NombreSucursal 
                    FROM usuario u
                    INNER JOIN perfil p ON u.id_perfil = p.id_perfil
                    INNER JOIN sucursal s ON u.id_sucursal = s.id_sucursal
                    WHERE u.activo = 1";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario()
                            {
                                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                Nombre = dr["nombre"].ToString(),
                                Apellido = dr["apellido"].ToString(),
                                Email = dr["email"].ToString(),
                                Telefono = dr["telefono"] != DBNull.Value ? dr["telefono"].ToString() : "",
                                Direccion = dr["direccion"] != DBNull.Value ? dr["direccion"].ToString() : "",
                                FechaNacimiento = dr["fecha_nacimiento"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_nacimiento"]) : (DateTime?)null,
                                IdPerfil = Convert.ToInt32(dr["id_perfil"]),
                                IdSucursal = Convert.ToInt32(dr["id_sucursal"]),
                                NombrePerfil = dr["NombrePerfil"].ToString(),
                                NombreSucursal = dr["NombreSucursal"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar usuarios: " + ex.Message);
                }
            }
            return lista;
        }

        public bool Insertar(Usuario obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "INSERT INTO usuario (nombre, apellido, email, password, telefono, direccion, fecha_nacimiento, id_perfil, id_sucursal) VALUES (@nom, @ape, @email, @pass, @tel, @dir, @fec, @perfil, @sucursal)";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nom", obj.Nombre);
                cmd.Parameters.AddWithValue("@ape", obj.Apellido);
                cmd.Parameters.AddWithValue("@email", obj.Email);
                cmd.Parameters.AddWithValue("@pass", obj.Password);
                cmd.Parameters.AddWithValue("@tel", string.IsNullOrEmpty(obj.Telefono) ? (object)DBNull.Value : obj.Telefono);
                cmd.Parameters.AddWithValue("@dir", string.IsNullOrEmpty(obj.Direccion) ? (object)DBNull.Value : obj.Direccion);
                cmd.Parameters.AddWithValue("@fec", obj.FechaNacimiento.HasValue ? (object)obj.FechaNacimiento.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@perfil", obj.IdPerfil);
                cmd.Parameters.AddWithValue("@sucursal", obj.IdSucursal);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al insertar usuario: " + ex.Message);
                }
            }
            return respuesta;
        }

        public bool Editar(Usuario obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "UPDATE usuario SET nombre = @nom, apellido = @ape, email = @email, telefono = @tel, direccion = @dir, fecha_nacimiento = @fec, id_perfil = @perfil, id_sucursal = @sucursal ";
                if (!string.IsNullOrEmpty(obj.Password))
                {
                    query += ", password = @pass ";
                }
                query += "WHERE id_usuario = @id";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@nom", obj.Nombre);
                cmd.Parameters.AddWithValue("@ape", obj.Apellido);
                cmd.Parameters.AddWithValue("@email", obj.Email);
                cmd.Parameters.AddWithValue("@tel", string.IsNullOrEmpty(obj.Telefono) ? (object)DBNull.Value : obj.Telefono);
                cmd.Parameters.AddWithValue("@dir", string.IsNullOrEmpty(obj.Direccion) ? (object)DBNull.Value : obj.Direccion);
                cmd.Parameters.AddWithValue("@fec", obj.FechaNacimiento.HasValue ? (object)obj.FechaNacimiento.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@perfil", obj.IdPerfil);
                cmd.Parameters.AddWithValue("@sucursal", obj.IdSucursal);
                cmd.Parameters.AddWithValue("@id", obj.IdUsuario);

                if (!string.IsNullOrEmpty(obj.Password))
                {
                    cmd.Parameters.AddWithValue("@pass", obj.Password);
                }

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al editar usuario: " + ex.Message);
                }
            }
            return respuesta;
        }

        public bool Eliminar(int id)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "UPDATE usuario SET activo = 0 WHERE id_usuario = @id";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    oConexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar usuario: " + ex.Message);
                }
            }
            return respuesta;
        }
    }
}