using System;

namespace AorusMarket.Entidades
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string CodigoBarras { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Descripcion { get; set; }
        public string UnidadMedida { get; set; }
        public int IdCategoria { get; set; }
        public bool Activo { get; set; }

        public string NombreCategoria { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
    }
}