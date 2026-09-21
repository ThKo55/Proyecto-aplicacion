using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AorusMarket.Entidades; // Importamos los modelos

namespace AorusMarket.AccesoDatos
{
    public class DashboardDatos
    {
        // 1. Método para llenar el ComboBox de Sucursales
        public List<SucursalCombo> ObtenerSucursalesCombo()
        {
            List<SucursalCombo> lista = new List<SucursalCombo>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Solo traemos las sucursales activas
                string query = "SELECT id_sucursal, nombre FROM sucursal WHERE activo = 1";
                SqlCommand cmd = new SqlCommand(query, oConexion);
                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new SucursalCombo()
                            {
                                IdSucursal = Convert.ToInt32(dr["id_sucursal"]),
                                Nombre = dr["nombre"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al cargar combo sucursales: " + ex.Message);
                }
            }
            return lista;
        }

        // 2. Método para obtener las métricas de las Tarjetas Superiores
        public MetricasDashboard ObtenerMetricas(int idSucursal, DateTime desde, DateTime hasta)
        {
            MetricasDashboard metricas = new MetricasDashboard() { TotalVendido = 0, CantidadVentas = 0, ProductoMasVendido = "-" };

            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Consulta compleja: Suma totales, cuenta ventas y usa una subconsulta para sacar el producto más vendido
                // (@idSucursal = 0) funciona como un "comodín" para traer todas las sucursales
                string query = @"
                    SELECT 
                        ISNULL(SUM(total), 0) AS TotalVendido,
                        COUNT(id_venta) AS CantidadVentas,
                        ISNULL((
                            SELECT TOP 1 p.nombre 
                            FROM detalleVenta dv 
                            INNER JOIN producto p ON dv.id_producto = p.id_producto
                            INNER JOIN venta v2 ON v2.id_venta = dv.id_venta
                            WHERE v2.estado = 1 
                              AND v2.fecha >= @desde AND v2.fecha <= @hasta
                              AND (@idSucursal = 0 OR v2.id_sucursal = @idSucursal)
                            GROUP BY p.nombre 
                            ORDER BY SUM(dv.cantidad) DESC
                        ), '-') AS ProductoTop
                    FROM venta 
                    WHERE estado = 1 
                      AND fecha >= @desde AND fecha <= @hasta
                      AND (@idSucursal = 0 OR id_sucursal = @idSucursal)";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@idSucursal", idSucursal);
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);

                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            metricas.TotalVendido = Convert.ToDecimal(dr["TotalVendido"]);
                            metricas.CantidadVentas = Convert.ToInt32(dr["CantidadVentas"]);
                            metricas.ProductoMasVendido = dr["ProductoTop"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error en Métricas: " + ex.Message);
                }
            }
            return metricas;
        }

        // 3. Método para listar las ventas recientes en la grilla
        public List<VentaReciente> ObtenerVentasRecientes(int idSucursal, DateTime desde, DateTime hasta)
        {
            List<VentaReciente> lista = new List<VentaReciente>();
            using (SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                // Unimos Venta, Sucursal y Cliente. 
                // Usamos ISNULL en el cliente para mostrar 'Consumidor Final' si la venta no tuvo cliente.
                string query = @"
                    SELECT 
                        v.id_venta, v.fecha, s.nombre AS Sucursal,
                        ISNULL(c.nombre + ' ' + c.apellido, 'Consumidor Final') AS Cliente,
                        v.total, v.metodo_pago,
                        CASE v.estado WHEN 1 THEN 'Confirmada' WHEN 2 THEN 'Anulada' ELSE 'Desconocido' END AS Estado
                    FROM venta v
                    INNER JOIN sucursal s ON v.id_sucursal = s.id_sucursal
                    LEFT JOIN cliente c ON v.id_cliente = c.id_cliente
                    WHERE v.fecha >= @desde AND v.fecha <= @hasta
                      AND (@idSucursal = 0 OR v.id_sucursal = @idSucursal)
                    ORDER BY v.fecha DESC";

                SqlCommand cmd = new SqlCommand(query, oConexion);
                cmd.Parameters.AddWithValue("@idSucursal", idSucursal);
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);

                try
                {
                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new VentaReciente()
                            {
                                IdVenta = Convert.ToInt32(dr["id_venta"]),
                                Fecha = Convert.ToDateTime(dr["fecha"]),
                                Sucursal = dr["Sucursal"].ToString(),
                                Cliente = dr["Cliente"].ToString(),
                                Total = Convert.ToDecimal(dr["total"]),
                                MetodoPago = dr["metodo_pago"].ToString(),
                                Estado = dr["Estado"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar ventas recientes: " + ex.Message);
                }
            }
            return lista;
        }
    }
}