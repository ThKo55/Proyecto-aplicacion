using System;
using System.Collections.Generic;
using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;

namespace AorusMarket.Negocio
{
    public class DashboardNegocio
    {
        private DashboardDatos _dashboardDatos = new DashboardDatos();

        // Obtiene las sucursales para el ComboBox
        public List<SucursalCombo> ObtenerSucursalesCombo()
        {
            return _dashboardDatos.ObtenerSucursalesCombo();
        }

        // Procesa los filtros de fechas y delega a Datos
        public MetricasDashboard ObtenerMetricas(int idSucursal, DateTime desde, DateTime hasta, out string mensajeError)
        {
            mensajeError = string.Empty;

            // Validación de lógica de negocio: La fecha DESDE no puede ser mayor a la fecha HASTA
            if (desde.Date > hasta.Date)
            {
                mensajeError = "La fecha 'Desde' no puede ser mayor que la fecha 'Hasta'.";
                return new MetricasDashboard() { ProductoMasVendido = "-" };
            }

            // Aseguramos que la fecha "Desde" empiece a las 00:00:00 y "Hasta" termine a las 23:59:59
            DateTime fechaDesdeFormat = desde.Date;
            DateTime fechaHastaFormat = hasta.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            return _dashboardDatos.ObtenerMetricas(idSucursal, fechaDesdeFormat, fechaHastaFormat);
        }

        // Lo mismo para el Grid de ventas
        public List<VentaReciente> ObtenerVentasRecientes(int idSucursal, DateTime desde, DateTime hasta)
        {
            // Ajustamos las fechas para cubrir el rango completo
            DateTime fechaDesdeFormat = desde.Date;
            DateTime fechaHastaFormat = hasta.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            return _dashboardDatos.ObtenerVentasRecientes(idSucursal, fechaDesdeFormat, fechaHastaFormat);
        }
    }
}