using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Data_Management_Flights_dal
{
    public static class UserFileHandler
    {
        private static string[] lines;
        private static string noteFilePath = "Codes.txt"; // Pad naar het bestand met codes

        static UserFileHandler()
        {
            lines = File.ReadAllLines(noteFilePath);
        }

        //verwijderd code wanneer geregistreerd
        public static void RemoveCode(string code)
        {
            // Filtert de regels waarin de code voorkomt en slaat ze opnieuw op in de array
            //deze methode verwijdert de regels waarin de opgegeven code voorkomt.
            lines = lines.Where(line => !line.Contains(code)).ToArray();
            // Schrijft de bijgewerkte regels terug naar het bestand
            File.WriteAllLines(noteFilePath, lines);
        }

        //Zoekt of code bestaat
        public static bool SearchCode(string code)
        {
            if (lines.Any(line => line.Contains(code)))
            {
                return true;
            }

            throw new Exception("User ID not found.");
        }
    }
}