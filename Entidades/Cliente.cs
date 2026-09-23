using System;
namespace AorusMarket.Entidades
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Dni { get; set; }
        public string CuilCuit { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        // Relación con tabla Dirección
        public int IdDireccion { get; set; }
        public string Calle { get; set; }
        public string Altura { get; set; }

        public bool Activo { get; set; }
    }
}