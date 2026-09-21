using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class UsuarioNegocio
    {
        private UsuarioDatos _usuarioDatos = new UsuarioDatos();

        // --- ESTE ES EL MÉTODO QUE TE FALTABA PARA EL LOGIN ---
        public Usuario Login(string email, string password, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                mensajeError = "Debe completar todos los campos.";
                return null;
            }

            Usuario objUsuario = _usuarioDatos.Autenticar(email, password);

            if (objUsuario == null)
            {
                mensajeError = "Correo o contraseña incorrectos, o usuario inactivo.";
                return null;
            }

            return objUsuario;
        }
        // ------------------------------------------------------

        // Solicitamos los combos
        public List<ComboUsuario> ObtenerPerfiles() => _usuarioDatos.ObtenerDatosCombo("perfil", "id_perfil", "nombre");
        public List<ComboUsuario> ObtenerSucursales() => _usuarioDatos.ObtenerDatosCombo("sucursal", "id_sucursal", "nombre");

        // Solicitamos el listado
        public List<Usuario> Listar() => _usuarioDatos.Listar();

        // Validaciones antes de guardar
        public bool Guardar(Usuario obj, out string mensaje)
        {
            mensaje = string.Empty;

            // 1. Reglas de negocio obligatorias
            if (string.IsNullOrWhiteSpace(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Apellido) || string.IsNullOrWhiteSpace(obj.Email))
            {
                mensaje = "El nombre, apellido y correo electrónico son obligatorios.";
                return false;
            }

            if (obj.IdPerfil <= 0 || obj.IdSucursal <= 0)
            {
                mensaje = "Debe seleccionar un Perfil y una Sucursal válidos.";
                return false;
            }

            // Si es un usuario NUEVO (ID = 0), la contraseña es 100% obligatoria
            if (obj.IdUsuario == 0 && string.IsNullOrWhiteSpace(obj.Password))
            {
                mensaje = "Para crear un nuevo usuario, debe asignarle una contraseña.";
                return false;
            }

            // 2. Enrutar a INSERTAR o EDITAR según el ID
            if (obj.IdUsuario == 0)
            {
                bool resultado = _usuarioDatos.Insertar(obj);
                if (!resultado) mensaje = "No se pudo crear el usuario. Verifique que el correo no esté duplicado.";
                return resultado;
            }
            else
            {
                bool resultado = _usuarioDatos.Editar(obj);
                if (!resultado) mensaje = "No se pudo actualizar el usuario.";
                return resultado;
            }
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = _usuarioDatos.Eliminar(id);
            if (!resultado)
            {
                mensaje = "Ocurrió un error al intentar eliminar el usuario.";
            }
            return resultado;
        }
    }
}