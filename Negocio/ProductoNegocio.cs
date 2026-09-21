using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class ProductoNegocio
    {
        private ProductoDatos _productoDatos = new ProductoDatos();

        // Solicita el listado a la capa de datos
        public List<Producto> Listar()
        {
            return _productoDatos.Listar();
        }

        // Aplica reglas de negocio y decide si debe INSERTAR o EDITAR
        public bool Guardar(Producto obj, out string mensaje)
        {
            mensaje = string.Empty;

            // 1. Validaciones
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

            // 2. Si el ID es 0, lo insertamos
            if (obj.IdProducto == 0)
            {
                bool resultado = _productoDatos.Insertar(obj);
                if (!resultado) mensaje = "No se pudo insertar el producto.";
                return resultado;
            }
            // 3. Si el ID es mayor a 0, lo editamos
            else
            {
                bool resultado = _productoDatos.Editar(obj);
                if (!resultado) mensaje = "No se pudo actualizar el producto.";
                return resultado;
            }
        }

        // Solicita la eliminación lógica
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