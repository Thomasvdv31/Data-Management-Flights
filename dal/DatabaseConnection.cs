using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Data_Management_Flights_dal
{
    public static class DatabaseConnection
    {
        // Methode om de connection string op te halen op basis van de naam
        public static string Connectionstring(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}