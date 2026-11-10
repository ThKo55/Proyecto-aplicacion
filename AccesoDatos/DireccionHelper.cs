using System;
using System.Data.SqlClient;

namespace AorusMarket.AccesoDatos
{
    internal static class DireccionHelper
    {
        // Devuelve el id de dirección que debe quedar asignado a la persona.
        // - Si la dirección actual es solo suya: se actualiza en el lugar.
        // - Si la comparte con otra persona (o no tiene): se crea una nueva y propia.
        public static int GuardarDireccion(SqlConnection cn, SqlTransaction tx,
                                           int idDireccionActual, string calle, string altura)
        {
            calle = calle ?? string.Empty;
            altura = altura ?? string.Empty;

            if (idDireccionActual > 0)
            {
                int usos;
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT (SELECT COUNT(*) FROM usuario WHERE id_direccion = @id)
                           + (SELECT COUNT(*) FROM cliente WHERE id_direccion = @id)", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", idDireccionActual);
                    usos = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (usos <= 1)
                {
                    using (SqlCommand cmdUpd = new SqlCommand(
                        "UPDATE direccion SET calle = @calle, altura = @altura WHERE id_direccion = @id", cn, tx))
                    {
                        cmdUpd.Parameters.AddWithValue("@calle", calle);
                        cmdUpd.Parameters.AddWithValue("@altura", altura);
                        cmdUpd.Parameters.AddWithValue("@id", idDireccionActual);
                        cmdUpd.ExecuteNonQuery();
                    }
                    return idDireccionActual;
                }
            }

            using (SqlCommand cmdIns = new SqlCommand(
                "INSERT INTO direccion (calle, altura) OUTPUT INSERTED.id_direccion VALUES (@calle, @altura)", cn, tx))
            {
                cmdIns.Parameters.AddWithValue("@calle", calle);
                cmdIns.Parameters.AddWithValue("@altura", altura);
                return Convert.ToInt32(cmdIns.ExecuteScalar());
            }
        }
    }
}