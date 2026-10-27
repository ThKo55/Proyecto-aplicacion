using System;

namespace AorusMarket.Entidades
{
    public class ReporteMedioPago
    {
        public string MedioPago { get; set; }
        public decimal TotalRecaudado { get; set; }
    }

    public class ReporteEvolucion
    {
        public string Fecha { get; set; }
        public decimal TotalVendido { get; set; }
    }

    public class ReporteSucursal
    {
        public string Sucursal { get; set; }
        public int CantidadVentas { get; set; }
        public decimal TotalRecaudado { get; set; }
    }

    public class ReporteCajero
    {
        public string Cajero { get; set; }
        public int CantidadVentas { get; set; }
        public decimal TotalRecaudado { get; set; }
    }

    public class ReporteCategoria
    {
        public string Categoria { get; set; }
        public int CantidadProductos { get; set; }
        public decimal TotalRecaudado { get; set; }
    }
}