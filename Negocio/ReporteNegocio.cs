using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class ReporteNegocio
    {
        private ReporteDatos _reporteDatos = new ReporteDatos();

        public List<ReporteMedioPago> RecaudacionPorMedioPago(DateTime desde, DateTime hasta)
        {
            return _reporteDatos.RecaudacionPorMedioPago(desde, hasta);
        }

        public List<ReporteEvolucion> EvolucionVentas(DateTime desde, DateTime hasta)
        {
            return _reporteDatos.EvolucionVentas(desde, hasta);
        }

        public List<ReporteSucursal> VentasPorSucursal(DateTime desde, DateTime hasta)
        {
            return _reporteDatos.VentasPorSucursal(desde, hasta);
        }

        public List<ReporteCajero> VentasPorCajero(DateTime desde, DateTime hasta)
        {
            return _reporteDatos.VentasPorCajero(desde, hasta);
        }

        public List<ReporteCategoria> VentasPorCategoria(DateTime desde, DateTime hasta)
        {
            return _reporteDatos.VentasPorCategoria(desde, hasta);
        }
    }
}