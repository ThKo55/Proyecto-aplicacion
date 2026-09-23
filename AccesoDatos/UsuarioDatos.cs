using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms; // AGREGADO PARA ALERTAS DE ERROR
using AorusMarket.Entidades;

namespace AorusMarket.AccesoDatos
{
    public class UsuarioDatos
    {
        public Usuario Autenticar(string email, string password)
        {
            Usuario obj = null;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = "SELECT id_usuario, nombre, apellido, email, id_perfil, id_sucursal FROM usuario WHERE email = @email AND password = @pass AND activo = 1";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@pass", password);
                try { oConexion.Open(); using (SqlDataReader dr = cmd.ExecuteReader()) { if (dr.Read()) { obj = new Usuario() { IdUsuario = Convert.ToInt32(dr["id_usuario"]), Nombre = dr["nombre"].ToString(), Apellido = dr["apellido"].ToString(), Email = dr["email"].ToString(), IdPerfil = Convert.ToInt32(dr["id_perfil"]), IdSucursal = Convert.ToInt32(dr["id_sucursal"]) }; } } }
                catch (Exception ex) { MessageBox.Show("Error en Autenticar: " + ex.Message); }
            }
            return obj;
        }

        public List<ComboUsuario> ObtenerPerfiles()
        {
            List<ComboUsuario> lista = new List<ComboUsuario>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("SELECT id_perfil, nombre FROM perfil", oConexion);
                try { oConexion.Open(); using (SqlDataReader dr = cmd.ExecuteReader()) { while (dr.Read()) lista.Add(new ComboUsuario() { Id = Convert.ToInt32(dr["id_perfil"]), Nombre = dr["nombre"].ToString() }); } }
                catch { }
            }
            return lista;
        }

        public List<ComboUsuario> ObtenerSucursales()
        {
            List<ComboUsuario> lista = new List<ComboUsuario>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("SELECT id_sucursal, nombre FROM sucursal WHERE activo = 1", oConexion);
                try { oConexion.Open(); using (SqlDataReader dr = cmd.ExecuteReader()) { while (dr.Read()) lista.Add(new ComboUsuario() { Id = Convert.ToInt32(dr["id_sucursal"]), Nombre = dr["nombre"].ToString() }); } }
                catch { }
            }
            return lista;
        }

        public List<Usuario> Listar(string filtroDni = "")
        {
            List<Usuario> lista = new List<Usuario>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT u.id_usuario, u.dni, u.nombre, u.apellido, u.email, u.telefono, u.fecha_nacimiento, 
                           u.id_perfil, u.id_sucursal, p.nombre AS NombrePerfil, s.nombre AS NombreSucursal,
                           u.id_direccion, d.calle, d.altura
                    FROM usuario u
                    LEFT JOIN perfil p ON u.id_perfil = p.id_perfil
                    LEFT JOIN sucursal s ON u.id_sucursal = s.id_sucursal
                    LEFT JOIN direccion d ON u.id_direccion = d.id_direccion
                    WHERE u.activo = 1";

                if (!string.IsNullOrEmpty(filtroDni)) query += " AND u.dni LIKE @filtro";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                if (!string.IsNullOrEmpty(filtroDni)) cmd.Parameters.AddWithValue("@filtro", "%" + filtroDni + "%");

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
                                Dni = dr["dni"] != DBNull.Value ? dr["dni"].ToString() : "",
                                Nombre = dr["nombre"].ToString(),
                                Apellido = dr["apellido"].ToString(),
                                Email = dr["email"].ToString(),
                                Telefono = dr["telefono"] != DBNull.Value ? dr["telefono"].ToString() : "",
                                FechaNacimiento = dr["fecha_nacimiento"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_nacimiento"]) : (DateTime?)null,
                                IdPerfil = dr["id_perfil"] != DBNull.Value ? Convert.ToInt32(dr["id_perfil"]) : 0,
                                IdSucursal = dr["id_sucursal"] != DBNull.Value ? Convert.ToInt32(dr["id_sucursal"]) : 0,
                                NombrePerfil = dr["NombrePerfil"] != DBNull.Value ? dr["NombrePerfil"].ToString() : "",
                                NombreSucursal = dr["NombreSucursal"] != DBNull.Value ? dr["NombreSucursal"].ToString() : "",
                                IdDireccion = dr["id_direccion"] != DBNull.Value ? Convert.ToInt32(dr["id_direccion"]) : 0,
                                Calle = dr["calle"] != DBNull.Value ? dr["calle"].ToString() : "",
                                Altura = dr["altura"] != DBNull.Value ? dr["altura"].ToString() : ""
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error SQL en Listar Usuarios: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return lista;
        }

        public bool Insertar(Usuario obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                oConexion.Open();
                SqlTransaction tx = oConexion.BeginTransaction();
                try
                {
                    string qDir = "INSERT INTO direccion (calle, altura) OUTPUT INSERTED.id_direccion VALUES (@calle, @altura)";
                    SqlCommand cmdDir = new SqlCommand(qDir, oConexion, tx);
                    cmdDir.Parameters.AddWithValue("@calle", obj.Calle ?? "");
                    cmdDir.Parameters.AddWithValue("@altura", obj.Altura ?? "");
                    int idDir = Convert.ToInt32(cmdDir.ExecuteScalar());

                    string qUsr = "INSERT INTO usuario (dni, nombre, apellido, email, password, telefono, fecha_nacimiento, id_perfil, id_sucursal, id_direccion) VALUES (@dni, @nom, @ape, @email, @pass, @tel, @fec, @perfil, @sucursal, @iddir)";
                    SqlCommand cmdUsr = new SqlCommand(qUsr, oConexion, tx);
                    cmdUsr.Parameters.AddWithValue("@dni", obj.Dni ?? "");
                    cmdUsr.Parameters.AddWithValue("@nom", obj.Nombre);
                    cmdUsr.Parameters.AddWithValue("@ape", obj.Apellido);
                    cmdUsr.Parameters.AddWithValue("@email", obj.Email);
                    cmdUsr.Parameters.AddWithValue("@pass", obj.Password);
                    cmdUsr.Parameters.AddWithValue("@tel", obj.Telefono ?? "");
                    cmdUsr.Parameters.AddWithValue("@fec", obj.FechaNacimiento.HasValue ? (object)obj.FechaNacimiento.Value : DBNull.Value);
                    cmdUsr.Parameters.AddWithValue("@perfil", obj.IdPerfil);
                    cmdUsr.Parameters.AddWithValue("@sucursal", obj.IdSucursal);
                    cmdUsr.Parameters.AddWithValue("@iddir", idDir);

                    cmdUsr.ExecuteNonQuery();
                    tx.Commit();
                    respuesta = true;
                }
                catch (Exception ex) { tx.Rollback(); MessageBox.Show("Error al Insertar: " + ex.Message); }
            }
            return respuesta;
        }

        public bool Editar(Usuario obj)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                oConexion.Open();
                SqlTransaction tx = oConexion.BeginTransaction();
                try
                {
                    string qDir = "UPDATE direccion SET calle = @calle, altura = @altura WHERE id_direccion = @idDir";
                    SqlCommand cmdDir = new SqlCommand(qDir, oConexion, tx);
                    cmdDir.Parameters.AddWithValue("@calle", obj.Calle ?? "");
                    cmdDir.Parameters.AddWithValue("@altura", obj.Altura ?? "");
                    cmdDir.Parameters.AddWithValue("@idDir", obj.IdDireccion);
                    cmdDir.ExecuteNonQuery();

                    string qUsr = "UPDATE usuario SET dni = @dni, nombre = @nom, apellido = @ape, email = @email, telefono = @tel, fecha_nacimiento = @fec, id_perfil = @perfil, id_sucursal = @sucursal ";
                    if (!string.IsNullOrEmpty(obj.Password)) qUsr += ", password = @pass ";
                    qUsr += "WHERE id_usuario = @id";

                    SqlCommand cmdUsr = new SqlCommand(qUsr, oConexion, tx);
                    cmdUsr.Parameters.AddWithValue("@dni", obj.Dni ?? "");
                    cmdUsr.Parameters.AddWithValue("@nom", obj.Nombre);
                    cmdUsr.Parameters.AddWithValue("@ape", obj.Apellido);
                    cmdUsr.Parameters.AddWithValue("@email", obj.Email);
                    cmdUsr.Parameters.AddWithValue("@tel", obj.Telefono ?? "");
                    cmdUsr.Parameters.AddWithValue("@fec", obj.FechaNacimiento.HasValue ? (object)obj.FechaNacimiento.Value : DBNull.Value);
                    cmdUsr.Parameters.AddWithValue("@perfil", obj.IdPerfil);
                    cmdUsr.Parameters.AddWithValue("@sucursal", obj.IdSucursal);
                    cmdUsr.Parameters.AddWithValue("@id", obj.IdUsuario);
                    if (!string.IsNullOrEmpty(obj.Password)) cmdUsr.Parameters.AddWithValue("@pass", obj.Password);

                    cmdUsr.ExecuteNonQuery();
                    tx.Commit();
                    respuesta = true;
                }
                catch (Exception ex) { tx.Rollback(); MessageBox.Show("Error al Editar: " + ex.Message); }
            }
            return respuesta;
        }

        public bool Eliminar(int id)
        {
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("UPDATE usuario SET activo = 0 WHERE id_usuario = @id", oConexion);
                cmd.Parameters.AddWithValue("@id", id);
                oConexion.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}