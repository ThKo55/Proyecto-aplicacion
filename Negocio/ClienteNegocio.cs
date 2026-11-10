using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;
using AorusMarket.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AorusMarket.Negocio
{
    public class ClienteNegocio
    {
        private ClienteDatos _clienteDatos = new ClienteDatos();
        public List<Cliente> Listar(string filtroDni = "") => _clienteDatos.Listar(filtroDni);

        public bool Guardar(Cliente obj, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Dni) || !Regex.IsMatch(obj.Dni, @"^\d{7,8}$"))
            { mensaje = "El DNI es obligatorio y debe contener entre 7 y 8 números."; return false; }

            if (string.IsNullOrWhiteSpace(obj.CuilCuit) || !Regex.IsMatch(obj.CuilCuit, @"^\d{11}$"))
            { mensaje = "El CUIL/CUIT es obligatorio y debe tener exactamente 11 números (sin guiones)."; return false; }

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

            if (obj.FechaNacimiento.HasValue)
            {
                int edad = Validaciones.CalcularEdad(obj.FechaNacimiento.Value);
                if (obj.FechaNacimiento.Value.Date > DateTime.Today || edad > 100)
                {
                    mensaje = "La fecha de nacimiento ingresada no es válida.";
                    return false;
                }
            }

            var listaExistente = _clienteDatos.Listar();
            if (listaExistente.Any(c => c.Dni == obj.Dni && c.IdCliente != obj.IdCliente))
            { mensaje = "El DNI ingresado ya pertenece a otro cliente en el sistema."; return false; }

            if (listaExistente.Any(c => c.CuilCuit == obj.CuilCuit && c.IdCliente != obj.IdCliente))
            { mensaje = "El CUIL/CUIT ingresado ya se encuentra registrado en otro cliente."; return false; }

            if (listaExistente.Any(c => c.Email.ToLower() == obj.Email.ToLower() && c.IdCliente != obj.IdCliente))
            { mensaje = "El Email ingresado ya se encuentra registrado en el sistema."; return false; }

            bool nuevo = obj.IdCliente == 0;
            bool resultado = nuevo ? _clienteDatos.Insertar(obj) : _clienteDatos.Editar(obj);
            if (resultado)
                Auditoria.Registrar(nuevo ? "Alta de cliente" : "Modificación de cliente", "Cliente",
                    $"{obj.Nombre} {obj.Apellido} | DNI {obj.Dni}");
            return resultado;
        }

        // ClienteNegocio.Eliminar (reemplazalo entero):
        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = _clienteDatos.Eliminar(id);
            if (resultado) Auditoria.Registrar("Baja de cliente", "Cliente", $"Id {id}");
            return resultado;
        }
    }
}