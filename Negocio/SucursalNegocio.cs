using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;
using AorusMarket.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            if (!Permisos.Requerir("gestionar sucursales", out mensaje, Perfiles.Administrador)) return false;

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

            bool nuevo = obj.IdSucursal == 0;
            bool resultado = nuevo ? _sucursalDatos.Insertar(obj) : _sucursalDatos.Editar(obj);
            if (!resultado)
                mensaje = nuevo ? "No se pudo insertar la sucursal. Verifique la conexión." : "No se pudo actualizar la sucursal.";
            else
                Auditoria.Registrar(nuevo ? "Alta de sucursal" : "Modificación de sucursal", "Sucursal", obj.Nombre);
            return resultado;
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            if (!Permisos.Requerir("gestionar sucursales", out mensaje, Perfiles.Administrador)) return false;
            bool resultado = _sucursalDatos.Eliminar(id);
            if (resultado) Auditoria.Registrar("Baja de categoría", "Categoría", $"Id {id}");   // ajustá texto/entidad en cada una
            else mensaje = "...";   // lo que ya tenías
            return resultado;
        }
    }
}