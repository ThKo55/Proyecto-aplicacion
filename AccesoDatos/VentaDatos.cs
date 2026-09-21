using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AorusMarket.Entidades;

namespace AorusMarket.AccesoDatos
{
    public class VentaDatos
    {
        // 1. Buscador de productos para el POS
        public List<BusquedaProducto> BuscarProducto(string texto, int idSucursal)
        {
            List<BusquedaProducto> lista = new List<BusquedaProducto>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT p.id_producto, ss.id_stock_sucursal, p.nombre, ss.precio, ss.cantidad
                    FROM producto p
                    INNER JOIN stock_sucursal ss ON p.id_producto = ss.id_producto
                    WHERE ss.id_sucursal = @idSucursal 
                      AND p.activo = 1 
                      AND p.nombre LIKE @texto";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@idSucursal", idSucursal);
                cmd.Parameters.AddWithValue("@texto", "%" + texto + "%");

                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new BusquedaProducto()
                            {
                                IdProducto = Convert.ToInt32(dr["id_producto"]),
                                IdStockSucursal = Convert.ToInt32(dr["id_stock_sucursal"]),
                                Nombre = dr["nombre"].ToString(),
                                Precio = Convert.ToDecimal(dr["precio"]),
                                StockDisponible = Convert.ToInt32(dr["cantidad"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error en buscador: " + ex.Message);
                }
            }
            return lista;
        }

        // 2. Transacción de Venta
        public bool ConfirmarVenta(Venta objVenta)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                oConexion.Open();
                SqlTransaction transaccion = oConexion.BeginTransaction();
                try
                {
                    string queryVenta = @"
                        INSERT INTO venta (total, metodo_pago, id_cliente, id_usuario, id_sucursal) 
                        OUTPUT INSERTED.id_venta
                        VALUES (@total, @metodo, @cliente, @usuario, @sucursal)";

                    SqlCommand cmdVenta = new SqlCommand(queryVenta, oConexion, transaccion);
                    cmdVenta.Parameters.AddWithValue("@total", objVenta.Total);
                    cmdVenta.Parameters.AddWithValue("@metodo", objVenta.MetodoPago);
                    cmdVenta.Parameters.AddWithValue("@cliente", objVenta.IdCliente.HasValue ? (object)objVenta.IdCliente.Value : DBNull.Value);
                    cmdVenta.Parameters.AddWithValue("@usuario", objVenta.IdUsuario);
                    cmdVenta.Parameters.AddWithValue("@sucursal", objVenta.IdSucursal);

                    int idVentaGenerado = Convert.ToInt32(cmdVenta.ExecuteScalar());

                    foreach (var item in objVenta.Detalles)
                    {
                        string queryDetalle = "INSERT INTO detalleVenta (id_venta, id_producto, cantidad, precio_unitario, subtotal) VALUES (@idVenta, @idProd, @cant, @precio, @subt)";
                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, oConexion, transaccion);
                        cmdDetalle.Parameters.AddWithValue("@idVenta", idVentaGenerado);
                        cmdDetalle.Parameters.AddWithValue("@idProd", item.IdProducto);
                        cmdDetalle.Parameters.AddWithValue("@cant", item.Cantidad);
                        cmdDetalle.Parameters.AddWithValue("@precio", item.PrecioUnitario);
                        cmdDetalle.Parameters.AddWithValue("@subt", item.Subtotal);
                        cmdDetalle.ExecuteNonQuery();

                        string queryStock = @"
                            UPDATE stock_sucursal SET cantidad = cantidad - @cant 
                            OUTPUT INSERTED.id_stock_sucursal
                            WHERE id_producto = @idProd AND id_sucursal = @sucursal";
                        SqlCommand cmdStock = new SqlCommand(queryStock, oConexion, transaccion);
                        cmdStock.Parameters.AddWithValue("@cant", item.Cantidad);
                        cmdStock.Parameters.AddWithValue("@idProd", item.IdProducto);
                        cmdStock.Parameters.AddWithValue("@sucursal", objVenta.IdSucursal);

                        int idStockSucursal = Convert.ToInt32(cmdStock.ExecuteScalar());

                        string queryAuditoria = "INSERT INTO movimiento_stock (id_stock_sucursal, cantidad_variacion, tipo_movimiento, id_usuario) VALUES (@idStock, @variacion, 'Venta', @usuario)";
                        SqlCommand cmdAuditoria = new SqlCommand(queryAuditoria, oConexion, transaccion);
                        cmdAuditoria.Parameters.AddWithValue("@idStock", idStockSucursal);
                        cmdAuditoria.Parameters.AddWithValue("@variacion", -item.Cantidad);
                        cmdAuditoria.Parameters.AddWithValue("@usuario", objVenta.IdUsuario);
                        cmdAuditoria.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    respuesta = true;
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    respuesta = false;
                    Console.WriteLine("Error crítico en Venta: " + ex.Message);
                }
            }
            return respuesta;
        }

        // 3. NUEVO: Traer Historial con filtros de Fecha
        public List<VentaHistorial> ListarHistorial(DateTime fechaDesde, DateTime fechaHasta, int idSucursalFiltro)
        {
            List<VentaHistorial> lista = new List<VentaHistorial>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Usamos ISNULL para cuando el cliente es Consumidor Final
                string query = @"
                    SELECT v.id_venta, v.fecha, 
                           ISNULL(c.nombre + ' ' + c.apellido, 'Consumidor Final') AS Cliente,
                           s.nombre AS Sucursal, 
                           u.nombre + ' ' + u.apellido AS Cajero,
                           v.total, v.metodo_pago, v.estado
                    FROM venta v
                    LEFT JOIN cliente c ON v.id_cliente = c.id_cliente
                    INNER JOIN sucursal s ON v.id_sucursal = s.id_sucursal
                    INNER JOIN usuario u ON v.id_usuario = u.id_usuario
                    WHERE CAST(v.fecha AS DATE) BETWEEN @desde AND @hasta ";

                // Si es cajero o gerente de sucursal, solo ve su sucursal. (Si es 0, ve todas).
                if (idSucursalFiltro > 0)
                {
                    query += " AND v.id_sucursal = @idSuc";
                }

                query += " ORDER BY v.fecha DESC";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@desde", fechaDesde.Date);
                cmd.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                if (idSucursalFiltro > 0) cmd.Parameters.AddWithValue("@idSuc", idSucursalFiltro);

                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new VentaHistorial()
                            {
                                IdVenta = Convert.ToInt32(dr["id_venta"]),
                                Fecha = Convert.ToDateTime(dr["fecha"]).ToString("dd/MM/yyyy HH:mm"),
                                Cliente = dr["Cliente"].ToString(),
                                Sucursal = dr["Sucursal"].ToString(),
                                Cajero = dr["Cajero"].ToString(),
                                Total = Convert.ToDecimal(dr["total"]),
                                MetodoPago = dr["metodo_pago"].ToString(),
                                Estado = Convert.ToInt32(dr["estado"]) == 1 ? "Confirmada" : "Anulada"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar historial: " + ex.Message);
                }
            }
            return lista;
        }

        // 4. NUEVO: Transacción pesada de Anulación
        public bool AnularVenta(int idVenta, int idUsuarioAuditoria)
        {
            bool respuesta = false;
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                oConexion.Open();
                SqlTransaction transaccion = oConexion.BeginTransaction();
                try
                {
                    // A. Obtenemos datos de la venta
                    string qVenta = "SELECT id_sucursal, estado FROM venta WHERE id_venta = @id";
                    SqlCommand cmdVenta = new SqlCommand(qVenta, oConexion, transaccion);
                    cmdVenta.Parameters.AddWithValue("@id", idVenta);

                    int idSucursal = 0;
                    int estado = 0;
                    using (SqlDataReader dr = cmdVenta.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            idSucursal = Convert.ToInt32(dr["id_sucursal"]);
                            estado = Convert.ToInt32(dr["estado"]);
                        }
                    }

                    if (estado == 2) throw new Exception("La venta ya se encuentra anulada.");

                    // B. Cambiamos estado de la venta
                    string qUpdate = "UPDATE venta SET estado = 2 WHERE id_venta = @id";
                    SqlCommand cmdUpdate = new SqlCommand(qUpdate, oConexion, transaccion);
                    cmdUpdate.Parameters.AddWithValue("@id", idVenta);
                    cmdUpdate.ExecuteNonQuery();

                    // C. Leemos los detalles de la venta (lo que hay que devolver)
                    string qDetalle = "SELECT id_producto, cantidad FROM detalleVenta WHERE id_venta = @id";
                    SqlCommand cmdDetalle = new SqlCommand(qDetalle, oConexion, transaccion);
                    cmdDetalle.Parameters.AddWithValue("@id", idVenta);

                    List<DetalleVenta> detalles = new List<DetalleVenta>();
                    using (SqlDataReader dr = cmdDetalle.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            detalles.Add(new DetalleVenta()
                            {
                                IdProducto = Convert.ToInt32(dr["id_producto"]),
                                Cantidad = Convert.ToInt32(dr["cantidad"])
                            });
                        }
                    }

                    // D. Devolvemos el stock y generamos el registro de auditoría
                    foreach (var d in detalles)
                    {
                        string qStock = @"
                            UPDATE stock_sucursal SET cantidad = cantidad + @cant 
                            OUTPUT INSERTED.id_stock_sucursal
                            WHERE id_producto = @idProd AND id_sucursal = @idSuc";

                        SqlCommand cmdStock = new SqlCommand(qStock, oConexion, transaccion);
                        cmdStock.Parameters.AddWithValue("@cant", d.Cantidad);
                        cmdStock.Parameters.AddWithValue("@idProd", d.IdProducto);
                        cmdStock.Parameters.AddWithValue("@idSuc", idSucursal);

                        object objStock = cmdStock.ExecuteScalar();
                        if (objStock != null)
                        {
                            int idStockSucursal = Convert.ToInt32(objStock);

                            string qMov = @"
                                INSERT INTO movimiento_stock (id_stock_sucursal, cantidad_variacion, tipo_movimiento, id_usuario) 
                                VALUES (@idStock, @cant, 'Anulación', @idUsr)";
                            SqlCommand cmdMov = new SqlCommand(qMov, oConexion, transaccion);
                            cmdMov.Parameters.AddWithValue("@idStock", idStockSucursal);
                            cmdMov.Parameters.AddWithValue("@cant", d.Cantidad);
                            cmdMov.Parameters.AddWithValue("@idUsr", idUsuarioAuditoria);
                            cmdMov.ExecuteNonQuery();
                        }
                    }

                    transaccion.Commit();
                    respuesta = true;
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    Console.WriteLine("Error al anular venta: " + ex.Message);
                }
            }
            return respuesta;
        }
    }
}