using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AorusMarket.Entidades;

namespace AorusMarket.AccesoDatos
{
    public class StockDatos
    {
        public List<ComboBasico> ObtenerDatosCombo(string tabla, string campoId, string campoNombre)
        {
            List<ComboBasico> lista = new List<ComboBasico>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = $"SELECT {campoId}, {campoNombre} FROM {tabla} WHERE activo = 1 ORDER BY {campoNombre} ASC";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                            lista.Add(new ComboBasico() { Id = Convert.ToInt32(dr[campoId]), Nombre = dr[campoNombre].ToString() });
                    }
                }
                catch { }
            }
            return lista;
        }

        public List<StockSucursal> Listar()
        {
            List<StockSucursal> lista = new List<StockSucursal>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT ss.id_stock_sucursal, p.nombre AS producto, s.nombre AS sucursal, 
                           ss.cantidad, ss.precio, ss.stock_minimo, ss.id_producto, ss.id_sucursal
                    FROM stock_sucursal ss
                    INNER JOIN producto p ON ss.id_producto = p.id_producto
                    INNER JOIN sucursal s ON ss.id_sucursal = s.id_sucursal
                    WHERE p.activo = 1 AND s.activo = 1";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new StockSucursal()
                            {
                                IdStock = Convert.ToInt32(dr["id_stock_sucursal"]),
                                Producto = dr["producto"].ToString(),
                                Sucursal = dr["sucursal"].ToString(),
                                Cantidad = Convert.ToInt32(dr["cantidad"]),
                                Precio = Convert.ToDecimal(dr["precio"]),
                                StockMinimo = Convert.ToInt32(dr["stock_minimo"]),
                                IdProducto = Convert.ToInt32(dr["id_producto"]),
                                IdSucursal = Convert.ToInt32(dr["id_sucursal"])
                            });
                        }
                    }
                }
                catch { }
            }
            return lista;
        }

        public bool Guardar(StockSucursal obj, int idUsuarioLogueado)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                oConexion.Open();
                SqlTransaction transaccion = oConexion.BeginTransaction();
                try
                {
                    string checkQuery = "SELECT id_stock_sucursal, cantidad FROM stock_sucursal WHERE id_producto = @idProd AND id_sucursal = @idSuc";
                    SqlCommand cmdCheck = new SqlCommand(checkQuery, oConexion, transaccion);
                    cmdCheck.Parameters.AddWithValue("@idProd", obj.IdProducto);
                    cmdCheck.Parameters.AddWithValue("@idSuc", obj.IdSucursal);

                    int idStockExistente = 0;
                    int cantidadAnterior = 0;

                    using (SqlDataReader dr = cmdCheck.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            idStockExistente = Convert.ToInt32(dr["id_stock_sucursal"]);
                            cantidadAnterior = Convert.ToInt32(dr["cantidad"]);
                        }
                    }

                    int idStockAfectado = 0;
                    int variacion = 0;

                    if (idStockExistente > 0)
                    {
                        string updateQuery = "UPDATE stock_sucursal SET cantidad = @cant, precio = @prec, stock_minimo = @min, activo = 1 WHERE id_stock_sucursal = @idStock";
                        SqlCommand cmdUpdate = new SqlCommand(updateQuery, oConexion, transaccion);
                        cmdUpdate.Parameters.AddWithValue("@cant", obj.Cantidad);
                        cmdUpdate.Parameters.AddWithValue("@prec", obj.Precio);
                        cmdUpdate.Parameters.AddWithValue("@min", obj.StockMinimo);
                        cmdUpdate.Parameters.AddWithValue("@idStock", idStockExistente);
                        cmdUpdate.ExecuteNonQuery();

                        idStockAfectado = idStockExistente;
                        variacion = obj.Cantidad - cantidadAnterior;
                    }
                    else
                    {
                        string insertQuery = @"
                            INSERT INTO stock_sucursal (id_producto, id_sucursal, cantidad, precio, stock_minimo) 
                            OUTPUT INSERTED.id_stock_sucursal
                            VALUES (@idProd, @idSuc, @cant, @prec, @min)";
                        SqlCommand cmdInsert = new SqlCommand(insertQuery, oConexion, transaccion);
                        cmdInsert.Parameters.AddWithValue("@idProd", obj.IdProducto);
                        cmdInsert.Parameters.AddWithValue("@idSuc", obj.IdSucursal);
                        cmdInsert.Parameters.AddWithValue("@cant", obj.Cantidad);
                        cmdInsert.Parameters.AddWithValue("@prec", obj.Precio);
                        cmdInsert.Parameters.AddWithValue("@min", obj.StockMinimo);

                        idStockAfectado = Convert.ToInt32(cmdInsert.ExecuteScalar());
                        variacion = obj.Cantidad;
                    }

                    if (variacion != 0)
                    {
                        string movQuery = @"
                            INSERT INTO movimiento_stock (id_stock_sucursal, cantidad_variacion, tipo_movimiento, id_usuario)
                            VALUES (@idStock, @var, 'Ajuste Manual', @idUsuario)";
                        SqlCommand cmdMov = new SqlCommand(movQuery, oConexion, transaccion);
                        cmdMov.Parameters.AddWithValue("@idStock", idStockAfectado);
                        cmdMov.Parameters.AddWithValue("@var", variacion);
                        cmdMov.Parameters.AddWithValue("@idUsuario", idUsuarioLogueado);
                        cmdMov.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    respuesta = true;
                }
                catch { transaccion.Rollback(); }
            }
            return respuesta;
        }

        // Baja lógica: el registro deja de verse, pero se conserva todo el historial de movimientos
        public bool Eliminar(int idStock, int idUsuario)
        {
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                oConexion.Open();
                SqlTransaction tx = oConexion.BeginTransaction();
                try
                {
                    int cantidadActual;
                    using (SqlCommand cmdSel = new SqlCommand(
                        "SELECT cantidad FROM stock_sucursal WHERE id_stock_sucursal = @id AND activo = 1", oConexion, tx))
                    {
                        cmdSel.Parameters.AddWithValue("@id", idStock);
                        object res = cmdSel.ExecuteScalar();
                        if (res == null) { tx.Rollback(); return false; }
                        cantidadActual = Convert.ToInt32(res);
                    }

                    using (SqlCommand cmdUpd = new SqlCommand(
                        "UPDATE stock_sucursal SET activo = 0, cantidad = 0 WHERE id_stock_sucursal = @id", oConexion, tx))
                    {
                        cmdUpd.Parameters.AddWithValue("@id", idStock);
                        cmdUpd.ExecuteNonQuery();
                    }

                    // Si había unidades, queda registrado que salieron por la baja
                    if (cantidadActual != 0)
                    {
                        using (SqlCommand cmdMov = new SqlCommand(
                            @"INSERT INTO movimiento_stock (id_stock_sucursal, cantidad_variacion, tipo_movimiento, id_usuario)
                      VALUES (@id, @var, 'Baja de Stock', @usr)", oConexion, tx))
                        {
                            cmdMov.Parameters.AddWithValue("@id", idStock);
                            cmdMov.Parameters.AddWithValue("@var", -cantidadActual);
                            cmdMov.Parameters.AddWithValue("@usr", idUsuario);
                            cmdMov.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                    return true;
                }
                catch { tx.Rollback(); return false; }
            }
        }

        public List<MovimientoStock> ListarMovimientos(DateTime desde, DateTime hasta)
        {
            List<MovimientoStock> lista = new List<MovimientoStock>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT m.id_movimiento, m.fecha, p.nombre AS Producto, s.nombre AS Sucursal,
                           m.cantidad_variacion, m.tipo_movimiento, u.nombre + ' ' + u.apellido AS Usuario
                    FROM movimiento_stock m
                    INNER JOIN stock_sucursal ss ON m.id_stock_sucursal = ss.id_stock_sucursal
                    INNER JOIN producto p ON ss.id_producto = p.id_producto
                    INNER JOIN sucursal s ON ss.id_sucursal = s.id_sucursal
                    INNER JOIN usuario u ON m.id_usuario = u.id_usuario
                    WHERE p.activo = 1 AND s.activo = 1 AND ss.activo = 1"";
                    ORDER BY m.fecha DESC";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@desde", desde.Date);
                cmd.Parameters.AddWithValue("@hasta", hasta.Date);
                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MovimientoStock()
                            {
                                IdMovimiento = Convert.ToInt32(dr["id_movimiento"]),
                                Fecha = Convert.ToDateTime(dr["fecha"]).ToString("dd/MM/yyyy HH:mm"),
                                Producto = dr["Producto"].ToString(),
                                Sucursal = dr["Sucursal"].ToString(),
                                CantidadVariacion = Convert.ToInt32(dr["cantidad_variacion"]),
                                TipoMovimiento = dr["tipo_movimiento"].ToString(),
                                Usuario = dr["Usuario"].ToString()
                            });
                        }
                    }
                }
                catch { }
            }
            return lista;
        }

        public int ObtenerStock(int idProducto, int idSucursal)
        {
            int cantidad = 0;
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string query = "SELECT ISNULL(cantidad, 0) FROM stock_sucursal WHERE id_producto = @idProducto AND id_sucursal = @idSucursal AND activo = 1";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@idProducto", idProducto);
                cmd.Parameters.AddWithValue("@idSucursal", idSucursal);
                try { conexion.Open(); object res = cmd.ExecuteScalar(); if (res != null) cantidad = Convert.ToInt32(res); } catch { }
            }
            return cantidad;
        }

        public Producto ObtenerProductoPorSucursal(int idProducto, int idSucursal)
        {
            Producto p = null;
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT id_producto, ISNULL(cantidad, 0) AS StockActual, ISNULL(stock_minimo, 10) AS StockMinimo
                    FROM stock_sucursal
                    WHERE id_producto = @idProducto AND id_sucursal = @idSucursal AND activo = 1";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@idProducto", idProducto);
                cmd.Parameters.AddWithValue("@idSucursal", idSucursal);

                try
                {
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            p = new Producto()
                            {
                                IdProducto = Convert.ToInt32(dr["id_producto"]),
                                StockActual = Convert.ToInt32(dr["StockActual"]),
                                StockMinimo = Convert.ToInt32(dr["StockMinimo"])
                            };
                        }
                    }
                }
                catch { }
            }
            return p;
        }
    }
}