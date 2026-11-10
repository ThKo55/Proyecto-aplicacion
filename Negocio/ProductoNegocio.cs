using System;
using System.Collections.Generic;
using System.Linq;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class ProductoNegocio
    {
        private ProductoDatos _productoDatos = new ProductoDatos();

        public List<Producto> Listar()
        {
            return _productoDatos.Listar();
        }

        public bool Guardar(Producto obj, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Nombre))
            {
                mensaje = "El nombre del producto es obligatorio.";
                return false;
            }
            if (obj.IdCategoria <= 0)
            {
                mensaje = "Debe seleccionar una categoría válida.";
                return false;
            }

            // Validación de duplicados
            var listaExistente = _productoDatos.Listar();
            if (listaExistente.Any(p => p.Nombre.Trim().ToLower() == obj.Nombre.Trim().ToLower() && p.IdProducto != obj.IdProducto))
            {
                mensaje = "Ya existe un producto registrado con ese nombre.";
                return false;
            }

            if (obj.IdProducto == 0)
            {
                bool resultado = _productoDatos.Insertar(obj);
                if (!resultado) mensaje = "No se pudo insertar el producto.";
                return resultado;
            }
            else
            {
                bool resultado = _productoDatos.Editar(obj);
                if (!resultado) mensaje = "No se pudo actualizar el producto.";
                return resultado;
            }
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = _productoDatos.Eliminar(id);
            if (!resultado)
            {
                mensaje = "Ocurrió un error al intentar eliminar el producto.";
            }
            return resultado;
        }
    }
}