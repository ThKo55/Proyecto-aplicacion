using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AorusMarket.Entidades;

namespace AorusMarket.AccesoDatos
{
    public class ClienteDatos
    {
        public List<Cliente> Listar(string filtroDni = "")
        {
            List<Cliente> lista = new List<Cliente>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT c.id_cliente, c.dni, c.cuil_cuit, c.nombre, c.apellido, c.email, c.telefono, c.fecha_nacimiento, 
                           c.id_direccion, d.calle, d.altura
                    FROM cliente c
                    INNER JOIN direccion d ON c.id_direccion = d.id_direccion
                    WHERE c.activo = 1";

                if (!string.IsNullOrEmpty(filtroDni)) query += " AND c.dni LIKE @filtro";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                if (!string.IsNullOrEmpty(filtroDni)) cmd.Parameters.AddWithValue("@filtro", "%" + filtroDni + "%");

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
                                Dni = dr["dni"].ToString(),
                                CuilCuit = dr["cuil_cuit"].ToString(),
                                Nombre = dr["nombre"].ToString(),
                                Apellido = dr["apellido"].ToString(),
                                Email = dr["email"].ToString(),
                                Telefono = dr["telefono"].ToString(),
                                FechaNacimiento = dr["fecha_nacimiento"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_nacimiento"]) : (DateTime?)null,
                                IdDireccion = Convert.ToInt32(dr["id_direccion"]),
                                Calle = dr["calle"].ToString(),
                                Altura = dr["altura"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            return lista;
        }

        public bool Insertar(Cliente obj)
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

                    string qCli = "INSERT INTO cliente (dni, cuil_cuit, nombre, apellido, email, telefono, fecha_nacimiento, id_direccion) VALUES (@dni, @cuil, @nom, @ape, @email, @tel, @fec, @iddir)";
                    SqlCommand cmdCli = new SqlCommand(qCli, oConexion, tx);
                    cmdCli.Parameters.AddWithValue("@dni", obj.Dni ?? "");
                    cmdCli.Parameters.AddWithValue("@cuil", obj.CuilCuit ?? "");
                    cmdCli.Parameters.AddWithValue("@nom", obj.Nombre);
                    cmdCli.Parameters.AddWithValue("@ape", obj.Apellido);
                    cmdCli.Parameters.AddWithValue("@email", obj.Email ?? "");
                    cmdCli.Parameters.AddWithValue("@tel", obj.Telefono ?? "");
                    cmdCli.Parameters.AddWithValue("@fec", obj.FechaNacimiento.HasValue ? (object)obj.FechaNacimiento.Value : DBNull.Value);
                    cmdCli.Parameters.AddWithValue("@iddir", idDir);

                    cmdCli.ExecuteNonQuery();
                    tx.Commit();
                    respuesta = true;
                }
                catch { tx.Rollback(); }
            }
            return respuesta;
        }

        public bool Editar(Cliente obj)
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

                    string qCli = "UPDATE cliente SET dni=@dni, cuil_cuit=@cuil, nombre=@nom, apellido=@ape, email=@email, telefono=@tel, fecha_nacimiento=@fec WHERE id_cliente=@id";
                    SqlCommand cmdCli = new SqlCommand(qCli, oConexion, tx);
                    cmdCli.Parameters.AddWithValue("@dni", obj.Dni ?? "");
                    cmdCli.Parameters.AddWithValue("@cuil", obj.CuilCuit ?? "");
                    cmdCli.Parameters.AddWithValue("@nom", obj.Nombre);
                    cmdCli.Parameters.AddWithValue("@ape", obj.Apellido);
                    cmdCli.Parameters.AddWithValue("@email", obj.Email ?? "");
                    cmdCli.Parameters.AddWithValue("@tel", obj.Telefono ?? "");
                    cmdCli.Parameters.AddWithValue("@fec", obj.FechaNacimiento.HasValue ? (object)obj.FechaNacimiento.Value : DBNull.Value);
                    cmdCli.Parameters.AddWithValue("@id", obj.IdCliente);

                    cmdCli.ExecuteNonQuery();
                    tx.Commit();
                    respuesta = true;
                }
                catch { tx.Rollback(); }
            }
            return respuesta;
        }

        public bool Eliminar(int id)
        {
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("UPDATE cliente SET activo = 0 WHERE id_cliente = @id", oConexion);
                cmd.Parameters.AddWithValue("@id", id);
                oConexion.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}