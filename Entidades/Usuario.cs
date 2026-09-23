using System;
namespace AorusMarket.Entidades
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int IdPerfil { get; set; }
        public int IdSucursal { get; set; }

        // Relación con tabla Dirección
        public int IdDireccion { get; set; }
        public string Calle { get; set; }
        public string Altura { get; set; }

        public bool Activo { get; set; }
        public string NombrePerfil { get; set; }
        public string NombreSucursal { get; set; }
    }
    public class ComboUsuario { public int Id { get; set; } public string Nombre { get; set; } }
}