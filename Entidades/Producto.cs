using System;

namespace AorusMarket.Entidades
{
    // Representa un registro exacto de la tabla 'producto' en la Base de Datos
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdCategoria { get; set; }
        public bool Activo { get; set; } // Baja lógica (1 o 0)

        // Propiedad adicional (no está en la tabla producto, pero la traemos con INNER JOIN 
        // desde la tabla categoria para mostrarla en la tabla visual)
        public string NombreCategoria { get; set; }
    }
}