using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;
using AorusMarket.Utilidades;

namespace AorusMarket.Negocio
{
    public class UsuarioNegocio
    {
        private UsuarioDatos _usuarioDatos = new UsuarioDatos();

        public Usuario Login(string email, string password, out string mensaje)
        {
            mensaje = string.Empty;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                mensaje = "Debe ingresar el correo y la contraseña.";
                return null;
            }

            Usuario usuarioEncontrado = _usuarioDatos.Autenticar(email, password);
            if (usuarioEncontrado == null) mensaje = "Correo o contraseña incorrectos, o el usuario está inactivo.";
            return usuarioEncontrado;
        }

        public List<ComboUsuario> ObtenerPerfiles() => _usuarioDatos.ObtenerPerfiles();
        public List<ComboUsuario> ObtenerSucursales() => _usuarioDatos.ObtenerSucursales();
        public List<Usuario> Listar(string filtroDni = "") => _usuarioDatos.Listar(filtroDni);

        public bool Guardar(Usuario obj, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Dni) || !Regex.IsMatch(obj.Dni, @"^\d{7,8}$"))
            { mensaje = "El DNI es obligatorio y debe contener entre 7 y 8 números."; return false; }

            if (string.IsNullOrWhiteSpace(obj.Nombre) || obj.Nombre.Trim().Length < 2)
            { mensaje = "El Nombre es obligatorio y debe tener al menos 2 letras."; return false; }

            if (string.IsNullOrWhiteSpace(obj.Apellido) || obj.Apellido.Trim().Length < 2)
            { mensaje = "El Apellido es obligatorio y debe tener al menos 2 letras."; return false; }

            if (string.IsNullOrWhiteSpace(obj.Email) || !Regex.IsMatch(obj.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { mensaje = "Debe ingresar un Email válido (ejemplo@dominio.com)."; return false; }

            if (string.IsNullOrWhiteSpace(obj.Telefono) || !Regex.IsMatch(obj.Telefono, @"^\d{6,15}$"))
            { mensaje = "El Teléfono es obligatorio y debe contener solo números."; return false; }

            if (string.IsNullOrWhiteSpace(obj.Calle))
            { mensaje = "La Calle de la dirección es obligatoria."; return false; }

            if (string.IsNullOrWhiteSpace(obj.Altura) || !Regex.IsMatch(obj.Altura, @"^\d+$"))
            { mensaje = "La Altura de la dirección es obligatoria y debe contener solo números."; return false; }

            if (obj.IdPerfil <= 0) { mensaje = "Debe seleccionar un Perfil válido."; return false; }
            if (obj.IdSucursal <= 0) { mensaje = "Debe seleccionar una Sucursal válida."; return false; }

            // Validación de Edad razonable
            if (obj.FechaNacimiento.HasValue)
            {
                int edad = DateTime.Now.Year - obj.FechaNacimiento.Value.Year;
                if (edad < 18 || edad > 100)
                {
                    mensaje = "El usuario debe ser mayor de edad y tener una fecha de nacimiento válida.";
                    return false;
                }
            }

            var listaExistente = _usuarioDatos.Listar();
            if (listaExistente.Any(u => u.Dni == obj.Dni && u.IdUsuario != obj.IdUsuario))
            { mensaje = "El DNI ingresado ya pertenece a otro usuario en el sistema."; return false; }

            if (listaExistente.Any(u => u.Email.ToLower() == obj.Email.ToLower() && u.IdUsuario != obj.IdUsuario))
            { mensaje = "El Email ingresado ya se encuentra registrado en el sistema."; return false; }

            if (obj.IdUsuario == 0)
            {
                if (string.IsNullOrWhiteSpace(obj.Password) || obj.Password.Length < 4)
                { mensaje = "La Contraseña es obligatoria para nuevos usuarios (mínimo 4 caracteres)."; return false; }

                return _usuarioDatos.Insertar(obj);
            }
            else
            {
                return _usuarioDatos.Editar(obj);
            }
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;

            // Validación: Evitar auto-eliminación
            if (id == SesionActual.IdUsuario)
            {
                mensaje = "No puede eliminar su propio usuario mientras tiene la sesión iniciada.";
                return false;
            }

            return _usuarioDatos.Eliminar(id);
        }
    }
}