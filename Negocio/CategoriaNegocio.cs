using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos; // Importa los Datos
using AorusMarket.Entidades;   // Importa las Entidades

namespace AorusMarket.Negocio
{
    public class CategoriaNegocio
    {
        // Instanciamos la capa de datos
        private CategoriaDatos _categoriaDatos = new CategoriaDatos();

        // Pide la lista de categorías a la BD
        public List<Categoria> Listar()
        {
            return _categoriaDatos.Listar();
        }

        // Valida y decide si debe INSERTAR o EDITAR
        public bool Guardar(Categoria obj, out string mensaje)
        {
            mensaje = string.Empty;

            // 1. Validaciones de Reglas de Negocio
            if (string.IsNullOrWhiteSpace(obj.Nombre))
            {
                mensaje = "El nombre de la categoría no puede estar vacío.";
                return false;
            }

            // 2. Si el ID es 0, es un registro nuevo (INSERTAR)
            if (obj.IdCategoria == 0)
            {
                bool resultado = _categoriaDatos.Insertar(obj);
                if (!resultado) mensaje = "No se pudo insertar la categoría.";
                return resultado;
            }
            // 3. Si el ID es mayor a 0, es una modificación (EDITAR)
            else
            {
                bool resultado = _categoriaDatos.Editar(obj);
                if (!resultado) mensaje = "No se pudo actualizar la categoría.";
                return resultado;
            }
        }

        // Solicita la eliminación lógica
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