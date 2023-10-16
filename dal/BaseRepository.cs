using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_dal
{
    public abstract class BaseRepository
    {
        protected string UserConnectionString { get; } // Connection string voor gebruikersdatabase
        protected string VluchtenConnectionString { get; } // Connection string voor vluchtendatabase

        public BaseRepository()
        {
            // Initialiseren van de connection strings met behulp van de DatabaseConnection klasse
            UserConnectionString = DatabaseConnection.Connectionstring("UsersConnectionString");
            VluchtenConnectionString = DatabaseConnection.Connectionstring("VluchtenConnectionString");
        }
    }
}