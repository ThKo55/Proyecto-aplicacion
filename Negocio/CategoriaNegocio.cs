using System;
using System.Collections.Generic;
using System.Linq;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class CategoriaNegocio
    {
        private CategoriaDatos _categoriaDatos = new CategoriaDatos();

        public List<Categoria> Listar()
        {
            return _categoriaDatos.Listar();
        }

        public bool Guardar(Categoria obj, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Nombre))
            {
                mensaje = "El nombre de la categoría no puede estar vacío.";
                return false;
            }

            // Validación de duplicados
            var listaExistente = _categoriaDatos.Listar();
            if (listaExistente.Any(c => c.Nombre.Trim().ToLower() == obj.Nombre.Trim().ToLower() && c.IdCategoria != obj.IdCategoria))
            {
                mensaje = "Ya existe una categoría registrada con ese nombre.";
                return false;
            }

            bool nuevo = obj.IdCategoria == 0;
            bool resultado = nuevo ? _categoriaDatos.Insertar(obj) : _categoriaDatos.Editar(obj);
            if (!resultado)
                mensaje = nuevo ? "No se pudo insertar la categoría." : "No se pudo actualizar la categoría.";
            else
                Auditoria.Registrar(nuevo ? "Alta de categoría" : "Modificación de categoría", "Categoría", obj.Nombre);
            return resultado;
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = _categoriaDatos.Eliminar(id);
            if (resultado) Auditoria.Registrar("Baja de categoría", "Categoría", $"Id {id}");   // ajustá texto/entidad en cada una
            else mensaje = "...";   // lo que ya tenías
            return resultado;
        }
    }
}