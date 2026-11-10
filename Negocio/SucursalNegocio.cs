using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class SucursalNegocio
    {
        private SucursalDatos _sucursalDatos = new SucursalDatos();

        public List<Sucursal> Listar()
        {
            return _sucursalDatos.Listar();
        }

        public bool Guardar(Sucursal obj, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Direccion))
            {
                mensaje = "El nombre y la dirección son campos obligatorios.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(obj.Telefono) && !Regex.IsMatch(obj.Telefono, @"^\d{6,15}$"))
            {
                mensaje = "El teléfono debe contener únicamente números y tener una longitud válida.";
                return false;
            }

            // Validación de duplicados
            var listaExistente = _sucursalDatos.Listar();
            if (listaExistente.Any(s => s.Nombre.Trim().ToLower() == obj.Nombre.Trim().ToLower() && s.IdSucursal != obj.IdSucursal))
            {
                mensaje = "Ya existe una sucursal registrada con ese nombre.";
                return false;
            }

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