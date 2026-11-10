using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

            string codigo = obj.CodigoBarras == null ? null : obj.CodigoBarras.Trim();
            if (!string.IsNullOrEmpty(codigo))
            {
                if (!Regex.IsMatch(codigo, @"^\d{8,14}$"))
                {
                    mensaje = "El código de barras debe tener entre 8 y 14 números.";
                    return false;
                }
                if (_productoDatos.ExisteCodigoBarras(codigo, obj.IdProducto))
                {
                    mensaje = "Ya existe un producto (activo o dado de baja) con ese código de barras.";
                    return false;
                }
            }
            obj.CodigoBarras = codigo;


            bool nuevo = obj.IdProducto == 0;
            bool resultado = nuevo ? _productoDatos.Insertar(obj) : _productoDatos.Editar(obj);
            if (!resultado)
                mensaje = nuevo ? "No se pudo insertar el producto." : "No se pudo actualizar el producto.";
            else
                Auditoria.Registrar(nuevo ? "Alta de producto" : "Modificación de producto", "Producto", obj.Nombre);
            return resultado;
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = _productoDatos.Eliminar(id);
            if (resultado) Auditoria.Registrar("Baja de categoría", "Categoría", $"Id {id}");   // ajustá texto/entidad en cada una
            else mensaje = "...";   // lo que ya tenías
            return resultado;
        }
    }
}