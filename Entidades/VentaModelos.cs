using System;
using System.Collections.Generic;

namespace AorusMarket.Entidades
{
    public class BusquedaProducto
    {
        public int IdProducto { get; set; }
        public int IdStockSucursal { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int StockDisponible { get; set; }
    }

    public class DetalleVenta
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    public class Venta
    {
        public int IdVenta { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public int? IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public int IdSucursal { get; set; }

        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }

    // NUEVO: Modelo para mostrar la lista de ventas en el historial
    public class VentaHistorial
    {
        public int IdVenta { get; set; }
        public string Fecha { get; set; }
        public string Cliente { get; set; }
        public string Sucursal { get; set; }
        public string Cajero { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; } // Confirmada o Anulada
    }
}