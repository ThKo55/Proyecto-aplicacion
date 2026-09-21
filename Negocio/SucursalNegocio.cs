using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class SucursalNegocio
    {
        // Traemos a nuestro "obrero" de la base de datos
        private SucursalDatos _sucursalDatos = new SucursalDatos();

        // Solicita el listado de sucursales
        public List<Sucursal> Listar()
        {
            return _sucursalDatos.Listar();
        }

        // Valida la información del formulario y decide si debe INSERTAR o EDITAR
        public bool Guardar(Sucursal obj, out string mensaje)
        {
            mensaje = string.Empty;

            // 1. Reglas de negocio y Validaciones
            if (string.IsNullOrWhiteSpace(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Direccion))
            {
                mensaje = "El nombre y la dirección son campos obligatorios.";
                return false;
            }

            // 2. Lógica de enrutamiento: Si el ID es 0, lo creamos. Si tiene ID, lo editamos.
            if (obj.IdSucursal == 0)
            {
                bool resultado = _sucursalDatos.Insertar(obj);
                if (!resultado) mensaje = "No se pudo insertar la sucursal. Verifique la conexión.";
                return resultado;
            }
            else
            {
                bool resultado = _sucursalDatos.Editar(obj);
                if (!resultado) mensaje = "No se pudo actualizar la sucursal.";
                return resultado;
            }
        }

        // Delega la petición de eliminación
        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = _sucursalDatos.Eliminar(id);
            if (!resultado)
            {
                mensaje = "Ocurrió un error al intentar eliminar la sucursal.";
            }
            return resultado;
        }
    }
}