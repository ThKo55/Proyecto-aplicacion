using System;
using System.Linq;

namespace AorusMarket.Utilidades
{
    public static class Validaciones
    {
        // Edad exacta (la resta de años falla si todavía no cumplió años este año)
        public static int CalcularEdad(DateTime nacimiento)
        {
            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - nacimiento.Year;
            if (nacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        public static bool PasswordValida(string password, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                mensaje = "La contraseña debe tener al menos 8 caracteres.";
                return false;
            }
            if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            {
                mensaje = "La contraseña debe combinar letras y números.";
                return false;
            }
            return true;
        }
    }
}