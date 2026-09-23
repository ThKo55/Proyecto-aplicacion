using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; // IMPORTANTE PARA VALIDAR EMAIL
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class ClienteNegocio
    {
        private ClienteDatos _clienteDatos = new ClienteDatos();
        public List<Cliente> Listar(string filtroDni = "") => _clienteDatos.Listar(filtroDni);

        public bool Guardar(Cliente obj, out string mensaje)
        {
            mensaje = string.Empty;

            // 1. VALIDACIONES DE CAMPOS OBLIGATORIOS
            if (string.IsNullOrWhiteSpace(obj.Dni)) { mensaje = "El DNI es obligatorio."; return false; }
            if (string.IsNullOrWhiteSpace(obj.Nombre)) { mensaje = "El Nombre es obligatorio."; return false; }
            if (string.IsNullOrWhiteSpace(obj.Apellido)) { mensaje = "El Apellido es obligatorio."; return false; }

            // 2. VALIDACIÓN DE DIRECCIÓN
            if (string.IsNullOrWhiteSpace(obj.Calle) || string.IsNullOrWhiteSpace(obj.Altura))
            {
                mensaje = "La Calle y la Altura son obligatorias.";
                return false;
            }

            // 3. VALIDACIÓN DE EMAIL Y FORMATO
            if (string.IsNullOrWhiteSpace(obj.Email)) { mensaje = "El Email es obligatorio."; return false; }
            if (!Regex.IsMatch(obj.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                mensaje = "El formato del Email no es válido (ejemplo@dominio.com).";
                return false;
            }

            // 4. VALIDACIÓN DE DNI REPETIDO
            var listaExistente = _clienteDatos.Listar();
            bool dniRepetido = listaExistente.Any(c => c.Dni == obj.Dni && c.IdCliente != obj.IdCliente);
            if (dniRepetido) { mensaje = "El DNI ingresado ya se encuentra registrado en otro cliente."; return false; }

            if (obj.IdCliente == 0) return _clienteDatos.Insertar(obj);
            else return _clienteDatos.Editar(obj);
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            return _clienteDatos.Eliminar(id);
        }
    }
}