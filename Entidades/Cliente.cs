using System;

namespace AorusMarket.Entidades
{
    // Esta clase es el "molde" exacto de la tabla 'cliente' en tu base de datos SQL Server.
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public bool Activo { get; set; } // Representa la baja lógica (1 = activo, 0 = eliminado)
    }
}