using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;
using AorusMarket.Utilidades;

namespace AorusMarket.Negocio
{
    public class StockNegocio
    {
        private StockDatos _stockDatos = new StockDatos();

        public List<ComboBasico> ObtenerProductos() => _stockDatos.ObtenerDatosCombo("producto", "id_producto", "nombre");
        public List<ComboBasico> ObtenerSucursales() => _stockDatos.ObtenerDatosCombo("sucursal", "id_sucursal", "nombre");

        public List<StockSucursal> Listar() => _stockDatos.Listar();

        public bool Guardar(string strProd, string strSuc, string strCant, string strPrec, string strMinimo, int idUsuario, out string mensaje)
        {
            mensaje = string.Empty;
            if (!Permisos.Requerir("modificar el catálogo y el stock", out mensaje, Perfiles.Administrador, Perfiles.GestorStock, Perfiles.Gerente)) return false;

            if (string.IsNullOrEmpty(strProd) || string.IsNullOrEmpty(strSuc))
            {
                mensaje = "Debe seleccionar un Producto y una Sucursal.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(strCant) || string.IsNullOrWhiteSpace(strPrec) || string.IsNullOrWhiteSpace(strMinimo))
            {
                mensaje = "Debe completar la Cantidad, el Precio y el Stock Mínimo.";
                return false;
            }

            // Con cultura es-AR el punto es separador de miles: "1500.50" se leería como 150050
            if (strPrec.Contains("."))
            {
                mensaje = "Use la coma como separador decimal en el precio (ejemplo: 1500,50).";
                return false;
            }

            if (!int.TryParse(strCant, out int cantidad) || !decimal.TryParse(strPrec, out decimal precio) || !int.TryParse(strMinimo, out int stockMinimo))
            {
                mensaje = "Cantidad, Precio y Stock Mínimo deben ser valores numéricos válidos.";
                return false;
            }

            if (cantidad < 0 || stockMinimo < 0)
            {
                mensaje = "Las cantidades no pueden ser menores a 0.";
                return false;
            }

            if (precio <= 0)
            {
                mensaje = "El precio debe ser mayor a $0.00.";
                return false;
            }

            if (precio > 10000000m)
            {
                mensaje = "El precio ingresado es demasiado alto. Verifique el valor.";
                return false;
            }

            if (cantidad > 1000000 || stockMinimo > 1000000)
            {
                mensaje = "La cantidad o el stock mínimo exceden el máximo permitido (1.000.000).";
                return false;
            }

            StockSucursal obj = new StockSucursal()
            {
                IdProducto = Convert.ToInt32(strProd),
                IdSucursal = Convert.ToInt32(strSuc),
                Cantidad = cantidad,
                Precio = precio,
                StockMinimo = stockMinimo
            };

            bool resultado = _stockDatos.Guardar(obj, idUsuario);
            if (!resultado)
                mensaje = "Ocurrió un error en la base de datos al intentar guardar el stock.";
            else
                Auditoria.Registrar("Ajuste de stock / precio", "Stock",
                    $"Producto #{obj.IdProducto} | Sucursal #{obj.IdSucursal} | Cant: {cantidad} | Precio: {precio:0.00} | Mín: {stockMinimo}");

            return resultado;
        }

        public bool Eliminar(int idStock, out string mensaje)
        {
            mensaje = string.Empty;
            if (!Permisos.Requerir("modificar el catálogo y el stock", out mensaje, Perfiles.Administrador, Perfiles.GestorStock, Perfiles.Gerente)) return false;
            bool resultado = _stockDatos.Eliminar(idStock, SesionActual.IdUsuario);

            if (resultado)
                Auditoria.Registrar("Baja de stock", "Stock", $"Registro de stock #{idStock} dado de baja (se conserva el historial)");
            else
                mensaje = "No se pudo dar de baja el registro de stock (puede que ya no exista).";

            return resultado;
        }

        public bool HayStockSuficiente(int idProducto, int idSucursal, int cantidadSolicitada)
        {
            int stockActual = _stockDatos.ObtenerStock(idProducto, idSucursal);
            return stockActual >= cantidadSolicitada;
        }

        public bool StockEsCritico(int idProducto, int idSucursal)
        {
            Producto p = _stockDatos.ObtenerProductoPorSucursal(idProducto, idSucursal);
            if (p != null && p.StockActual <= p.StockMinimo) return true;
            return false;
        }
    }
}