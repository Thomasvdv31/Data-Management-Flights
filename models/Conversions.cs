using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_models
{
    public static class Conversions
    {
        // Converteert een decimale geldwaarde naar een numerieke tekst met twee decimalen.
        public static string ConvertMoneyToNumericText(decimal waarde)
        {
            return waarde.ToString("N2");
        }

        // Converteert een decimale numerieke waarde naar een geldtekst in het standaard geldformaat.
        public static string ConvertNumericToMoneyText(decimal waarde)
        {
            return waarde.ToString("c");
        }
    }
}