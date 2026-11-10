using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class VentaNegocio
    {
        private VentaDatos _ventaDatos = new VentaDatos();

        // 1. Buscador de productos
        public List<BusquedaProducto> BuscarProducto(string texto, int idSucursal)
        {
            return _ventaDatos.BuscarProducto(texto, idSucursal);
        }

        // 2. Confirmar la venta
        public bool ConfirmarVenta(Venta objVenta, out string mensaje)
        {
            mensaje = string.Empty;

            if (objVenta.Detalles.Count == 0)
            {
                mensaje = "No se puede confirmar una venta sin productos.";
                return false;
            }

            bool respuesta = _ventaDatos.ConfirmarVenta(objVenta);
            if (!respuesta)
                mensaje = "Ocurrió un error crítico al intentar registrar la venta en la base de datos.";
            else
                Auditoria.Registrar("Venta confirmada", "Venta",
                    $"Venta #{objVenta.IdVenta} | Total: {objVenta.Total:0.00} | Pago: {objVenta.MetodoPago} | Sucursal #{objVenta.IdSucursal}");

            return respuesta;

        }

        // 3. Listar el Historial
        public List<VentaHistorial> ListarHistorial(DateTime desde, DateTime hasta, int idUsuarioFiltro)
        {
            return _ventaDatos.ListarHistorial(desde, hasta, idUsuarioFiltro);
        }

        // 4. Obtener Detalles
        public List<DetalleHistorial> ObtenerDetallesDeVenta(int idVenta)
        {
            return _ventaDatos.ObtenerDetallesDeVenta(idVenta);
        }

        // 5. Anular Venta
        public bool AnularVenta(int idVenta, string estadoActual, int idUsuarioAuditoria, out string mensaje)
        {
            mensaje = string.Empty;

            if (estadoActual == "Anulada")
            {
                mensaje = "La venta ya se encuentra anulada.";
                return false;
            }

            bool respuesta = _ventaDatos.AnularVenta(idVenta, idUsuarioAuditoria);

            if (!respuesta)
                mensaje = "Ocurrió un error al intentar anular la venta y devolver el stock.";
            else
                Auditoria.Registrar("Venta anulada", "Venta", $"Venta #{idVenta} anulada y stock devuelto");

            return respuesta;
        }

        // 6. EL MÉTODO QUE FALTABA: Obtener Clientes para el desplegable del POS
        public List<Cliente> ObtenerClientesCombo()
        {
            // Reutilizamos el método Listar que ya está creado y funcionando en ClienteNegocio
            return new ClienteNegocio().Listar();
        }
    }
}