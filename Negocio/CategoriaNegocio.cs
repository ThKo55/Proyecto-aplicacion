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

            if (obj.IdCategoria == 0)
            {
                bool resultado = _categoriaDatos.Insertar(obj);
                if (!resultado) mensaje = "No se pudo insertar la categoría.";
                return resultado;
            }
            else
            {
                bool resultado = _categoriaDatos.Editar(obj);
                if (!resultado) mensaje = "No se pudo actualizar la categoría.";
                return resultado;
            }
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = _categoriaDatos.Eliminar(id);
            if (!resultado)
            {
                mensaje = "Ocurrió un error al intentar eliminar la categoría.";
            }
            return resultado;
        }
    }
}