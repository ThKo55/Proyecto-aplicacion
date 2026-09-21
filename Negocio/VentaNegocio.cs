using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class VentaNegocio
    {
        private VentaDatos _ventaDatos = new VentaDatos();
        private ClienteDatos _clienteDatos = new ClienteDatos();

        public List<Cliente> ObtenerClientesCombo() => _clienteDatos.Listar();
        public List<BusquedaProducto> BuscarProducto(string texto, int idSucursal) => _ventaDatos.BuscarProducto(texto, idSucursal);

        public bool ConfirmarVenta(Venta objVenta, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (objVenta.Detalles.Count == 0)
            {
                mensajeError = "No se puede confirmar una venta vacía.";
                return false;
            }

            if (string.IsNullOrEmpty(objVenta.MetodoPago))
            {
                mensajeError = "Debe seleccionar un método de pago.";
                return false;
            }

            bool resultado = _ventaDatos.ConfirmarVenta(objVenta);
            if (!resultado) mensajeError = "Ocurrió un error en la base de datos al procesar la venta.";

            return resultado;
        }

        // NUEVO
        public List<VentaHistorial> ListarHistorial(DateTime desde, DateTime hasta, int idSucursal)
        {
            return _ventaDatos.ListarHistorial(desde, hasta, idSucursal);
        }

        // NUEVO
        public bool AnularVenta(int idVenta, string estadoActual, int idUsuario, out string mensaje)
        {
            mensaje = string.Empty;

            if (estadoActual == "Anulada")
            {
                mensaje = "Esta venta ya se encuentra anulada.";
                return false;
            }

            bool resultado = _ventaDatos.AnularVenta(idVenta, idUsuario);
            if (!resultado) mensaje = "No se pudo anular la venta debido a un error interno de base de datos.";

            return resultado;
        }
    }
}