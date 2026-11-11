namespace AorusMarket.Entidades
{
    public class AlertaStock
    {
        public string Producto { get; set; }
        public string Sucursal { get; set; }
        public int Cantidad { get; set; }
        public int StockMinimo { get; set; }
    }
}