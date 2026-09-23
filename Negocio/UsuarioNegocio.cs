using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; // IMPORTANTE PARA VALIDAR EMAIL
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

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
            if (usuarioEncontrado == null) mensaje = "Correo o contraseña incorrectos, o usuario inactivo.";
            return usuarioEncontrado;
        }

        public List<ComboUsuario> ObtenerPerfiles() => _usuarioDatos.ObtenerPerfiles();
        public List<ComboUsuario> ObtenerSucursales() => _usuarioDatos.ObtenerSucursales();
        public List<Usuario> Listar(string filtroDni = "") => _usuarioDatos.Listar(filtroDni);

        public bool Guardar(Usuario obj, out string mensaje)
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
            var listaExistente = _usuarioDatos.Listar();
            bool dniRepetido = listaExistente.Any(u => u.Dni == obj.Dni && u.IdUsuario != obj.IdUsuario);
            if (dniRepetido) { mensaje = "El DNI ingresado ya se encuentra registrado en otro usuario."; return false; }

            if (obj.IdUsuario == 0)
            {
                if (string.IsNullOrWhiteSpace(obj.Password)) { mensaje = "La Contraseña es obligatoria para nuevos usuarios."; return false; }
                return _usuarioDatos.Insertar(obj);
            }
            else return _usuarioDatos.Editar(obj);
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            return _usuarioDatos.Eliminar(id);
        }
    }
}