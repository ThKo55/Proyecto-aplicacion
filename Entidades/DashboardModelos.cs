using System;

namespace AorusMarket.Entidades
{
    // 1. Entidad para poblar el ComboBox de Sucursales
    public class SucursalCombo
    {
        public int IdSucursal { get; set; }
        public string Nombre { get; set; }
    }

    // 2. Entidad para transportar los datos de las Tarjetas (Métricas)
    public class MetricasDashboard
    {
        public decimal TotalVendido { get; set; }
        public int CantidadVentas { get; set; }
        public string ProductoMasVendido { get; set; }
    }

    // 3. Entidad para transportar el cruce de datos de la Grilla de Ventas Recientes
    public class VentaReciente
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public string Sucursal { get; set; }
        public string Cliente { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }
    }
}