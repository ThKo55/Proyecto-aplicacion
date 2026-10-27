using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AorusMarket.Entidades;

namespace AorusMarket.AccesoDatos
{
    public class ReporteDatos
    {
        // 1. Recaudación por Medio de Pago
        public List<ReporteMedioPago> RecaudacionPorMedioPago(DateTime desde, DateTime hasta)
        {
            List<ReporteMedioPago> lista = new List<ReporteMedioPago>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"SELECT metodo_pago, SUM(total) as Total 
                                 FROM venta 
                                 WHERE estado = 1 AND CAST(fecha AS DATE) BETWEEN @desde AND @hasta 
                                 GROUP BY metodo_pago";
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
                            lista.Add(new ReporteMedioPago()
                            {
                                MedioPago = dr["metodo_pago"].ToString(),
                                TotalRecaudado = Convert.ToDecimal(dr["Total"])
                            });
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            return lista;
        }

        // 2. Evolución de Ventas (Día a Día)
        public List<ReporteEvolucion> EvolucionVentas(DateTime desde, DateTime hasta)
        {
            List<ReporteEvolucion> lista = new List<ReporteEvolucion>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"SELECT CAST(fecha AS DATE) as FechaVenta, SUM(total) as Total 
                                 FROM venta 
                                 WHERE estado = 1 AND CAST(fecha AS DATE) BETWEEN @desde AND @hasta 
                                 GROUP BY CAST(fecha AS DATE) 
                                 ORDER BY CAST(fecha AS DATE)";
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
                            lista.Add(new ReporteEvolucion()
                            {
                                Fecha = Convert.ToDateTime(dr["FechaVenta"]).ToString("dd/MM/yyyy"),
                                TotalVendido = Convert.ToDecimal(dr["Total"])
                            });
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            return lista;
        }

        // 3. Comparativa por Sucursales
        public List<ReporteSucursal> VentasPorSucursal(DateTime desde, DateTime hasta)
        {
            List<ReporteSucursal> lista = new List<ReporteSucursal>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"SELECT s.nombre, COUNT(v.id_venta) as Cantidad, SUM(v.total) as Total 
                                 FROM venta v 
                                 INNER JOIN sucursal s ON v.id_sucursal = s.id_sucursal 
                                 WHERE v.estado = 1 AND CAST(v.fecha AS DATE) BETWEEN @desde AND @hasta 
                                 GROUP BY s.nombre";
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
                            lista.Add(new ReporteSucursal()
                            {
                                Sucursal = dr["nombre"].ToString(),
                                CantidadVentas = Convert.ToInt32(dr["Cantidad"]),
                                TotalRecaudado = Convert.ToDecimal(dr["Total"])
                            });
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            return lista;
        }

        // 4. Desempeño por Cajero
        public List<ReporteCajero> VentasPorCajero(DateTime desde, DateTime hasta)
        {
            List<ReporteCajero> lista = new List<ReporteCajero>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"SELECT u.nombre + ' ' + u.apellido as Cajero, COUNT(v.id_venta) as Cantidad, SUM(v.total) as Total 
                                 FROM venta v 
                                 INNER JOIN usuario u ON v.id_usuario = u.id_usuario 
                                 WHERE v.estado = 1 AND CAST(v.fecha AS DATE) BETWEEN @desde AND @hasta 
                                 GROUP BY u.nombre, u.apellido";
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
                            lista.Add(new ReporteCajero()
                            {
                                Cajero = dr["Cajero"].ToString(),
                                CantidadVentas = Convert.ToInt32(dr["Cantidad"]),
                                TotalRecaudado = Convert.ToDecimal(dr["Total"])
                            });
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            return lista;
        }

        // 5. Distribución por Categoría de Producto
        public List<ReporteCategoria> VentasPorCategoria(DateTime desde, DateTime hasta)
        {
            List<ReporteCategoria> lista = new List<ReporteCategoria>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                string query = @"SELECT c.nombre as Categoria, SUM(dv.cantidad) as Cantidad, SUM(dv.subtotal) as Total 
                                 FROM detalleVenta dv 
                                 INNER JOIN venta v ON dv.id_venta = v.id_venta
                                 INNER JOIN producto p ON dv.id_producto = p.id_producto 
                                 INNER JOIN categoria c ON p.id_categoria = c.id_categoria 
                                 WHERE v.estado = 1 AND CAST(v.fecha AS DATE) BETWEEN @desde AND @hasta 
                                 GROUP BY c.nombre";
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
                            lista.Add(new ReporteCategoria()
                            {
                                Categoria = dr["Categoria"].ToString(),
                                CantidadProductos = Convert.ToInt32(dr["Cantidad"]),
                                TotalRecaudado = Convert.ToDecimal(dr["Total"])
                            });
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            return lista;
        }
    }
}