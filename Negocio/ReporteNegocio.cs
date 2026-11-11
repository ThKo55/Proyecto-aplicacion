using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;
using AorusMarket.Utilidades;
using System;
using System.Collections.Generic;

namespace AorusMarket.Negocio
{
    public class ReporteNegocio
    {
        private ReporteDatos _reporteDatos = new ReporteDatos();

        private bool FechasValidas(DateTime desde, DateTime hasta)
        {
            if (!Permisos.Tiene(Perfiles.Administrador, Perfiles.Gerente)) return false;
            return desde.Date <= hasta.Date;
        }

        public List<ReporteMedioPago> RecaudacionPorMedioPago(DateTime desde, DateTime hasta)
        {
            if (!FechasValidas(desde, hasta)) return new List<ReporteMedioPago>();
            return _reporteDatos.RecaudacionPorMedioPago(desde, hasta);
        }

        public List<ReporteEvolucion> EvolucionVentas(DateTime desde, DateTime hasta)
        {
            if (!FechasValidas(desde, hasta)) return new List<ReporteEvolucion>();
            return _reporteDatos.EvolucionVentas(desde, hasta);
        }

        public List<ReporteSucursal> VentasPorSucursal(DateTime desde, DateTime hasta)
        {
            if (!FechasValidas(desde, hasta)) return new List<ReporteSucursal>();
            return _reporteDatos.VentasPorSucursal(desde, hasta);
        }

        public List<ReporteCajero> VentasPorCajero(DateTime desde, DateTime hasta)
        {
            if (!FechasValidas(desde, hasta)) return new List<ReporteCajero>();
            return _reporteDatos.VentasPorCajero(desde, hasta);
        }

        public List<ReporteCategoria> VentasPorCategoria(DateTime desde, DateTime hasta)
        {
            if (!FechasValidas(desde, hasta)) return new List<ReporteCategoria>();
            return _reporteDatos.VentasPorCategoria(desde, hasta);
        }
    }
}