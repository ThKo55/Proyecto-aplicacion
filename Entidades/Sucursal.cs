using System;

namespace AorusMarket.Entidades
{
    // Esta clase representa exactamente la tabla 'sucursal' en tu base de datos
    public class Sucursal
    {
        public int IdSucursal { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; } // Representa la baja lógica (1 o 0)
    }
}