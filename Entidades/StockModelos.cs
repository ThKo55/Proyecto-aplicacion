using System;

namespace AorusMarket.Entidades
{
    public class StockSucursal
    {
        public int IdStock { get; set; }
        public int IdProducto { get; set; }
        public int IdSucursal { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }

        public string Producto { get; set; }
        public string Sucursal { get; set; }
    }

    public class ComboBasico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    // NUEVO MOLDE: Para la pantalla de Auditoría
    public class MovimientoStock
    {
        public int IdMovimiento { get; set; }
        public string Fecha { get; set; }
        public string Producto { get; set; }
        public string Sucursal { get; set; }
        public int CantidadVariacion { get; set; }
        public string TipoMovimiento { get; set; }
        public string Usuario { get; set; }
    }
}