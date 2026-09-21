using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class StockNegocio
    {
        private StockDatos _stockDatos = new StockDatos();

        public List<ComboBasico> ObtenerProductos() => _stockDatos.ObtenerDatosCombo("producto", "id_producto", "nombre");
        public List<ComboBasico> ObtenerSucursales() => _stockDatos.ObtenerDatosCombo("sucursal", "id_sucursal", "nombre");

        public List<StockSucursal> Listar() => _stockDatos.Listar();

        public bool Guardar(string strProd, string strSuc, string strCant, string strPrec, int idUsuario, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrEmpty(strProd) || string.IsNullOrEmpty(strSuc))
            {
                mensaje = "Debe seleccionar un Producto y una Sucursal.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(strCant) || string.IsNullOrWhiteSpace(strPrec))
            {
                mensaje = "Debe completar la Cantidad y el Precio.";
                return false;
            }

            if (!int.TryParse(strCant, out int cantidad) || !decimal.TryParse(strPrec, out decimal precio))
            {
                mensaje = "Cantidad y Precio deben ser valores numéricos válidos.";
                return false;
            }

            StockSucursal obj = new StockSucursal()
            {
                IdProducto = Convert.ToInt32(strProd),
                IdSucursal = Convert.ToInt32(strSuc),
                Cantidad = cantidad,
                Precio = precio
            };

            bool resultado = _stockDatos.Guardar(obj, idUsuario);
            if (!resultado) mensaje = "Ocurrió un error en la base de datos al intentar guardar el stock.";

            return resultado;
        }

        public bool Eliminar(int idStock, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = _stockDatos.Eliminar(idStock);
            if (!resultado) mensaje = "Error: Es posible que este registro de stock esté vinculado a detalles de ventas.";
            return resultado;
        }

        // NUEVO MÉTODO
        public List<MovimientoStock> ListarAuditoria(DateTime desde, DateTime hasta)
        {
            return _stockDatos.ListarMovimientos(desde, hasta);
        }
    }
}