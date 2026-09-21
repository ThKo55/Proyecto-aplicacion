using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class ClienteNegocio
    {
        // Traemos a nuestro "obrero" de la base de datos
        private ClienteDatos _clienteDatos = new ClienteDatos();

        // Solicita el listado de clientes a la capa de datos
        public List<Cliente> Listar()
        {
            return _clienteDatos.Listar();
        }

        // Valida la información del formulario y decide si debe INSERTAR o EDITAR
        public bool Guardar(Cliente obj, out string mensaje)
        {
            mensaje = string.Empty;

            // 1. Reglas de negocio y Validaciones
            if (string.IsNullOrWhiteSpace(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Apellido))
            {
                mensaje = "El nombre y el apellido son campos obligatorios.";
                return false;
            }

            // 2. Lógica de enrutamiento: Si el ID es 0, lo creamos. Si tiene ID, lo editamos.
            if (obj.IdCliente == 0)
            {
                bool resultado = _clienteDatos.Insertar(obj);
                if (!resultado) mensaje = "No se pudo insertar el cliente. Verifique si el DNI o Email ya existen.";
                return resultado;
            }
            else
            {
                bool resultado = _clienteDatos.Editar(obj);
                if (!resultado) mensaje = "No se pudo actualizar el cliente.";
                return resultado;
            }
        }

        // Delega la petición de eliminación
        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = _clienteDatos.Eliminar(id);
            if (!resultado)
            {
                mensaje = "Ocurrió un error al intentar eliminar el cliente.";
            }
            return resultado;
        }
    }
}