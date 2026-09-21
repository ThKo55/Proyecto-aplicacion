using System;

namespace AorusMarket.Entidades
{
    // Esta clase representa exactamente un registro de la tabla 'categoria' en la BD
    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; } // Representa la baja lógica (1 o 0)
    }
}