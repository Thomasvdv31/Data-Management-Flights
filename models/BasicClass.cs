using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Data_Management_Flights_models
{
    public abstract class BasicClass : IDataErrorInfo
    {
        // Implementatie van de Error-eigenschap van IDataErrorInfo
        public string Error
        {
            get
            {
                string foutmeldingen = "";
                // Loop door alle eigenschappen van het huidige object
                foreach (var property in this.GetType().GetProperties())
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        // Roep de indexer aan om de validatiemelding voor elke eigenschap op te halen
                        string fout = this[property.Name];
                        if (!string.IsNullOrWhiteSpace(fout))
                        {
                            foutmeldingen += fout + Environment.NewLine;
                        }
                    }
                }
                return foutmeldingen;
            }
        }

        // Abstracte indexer die in afgeleide klassen moet worden geïmplementeerd
        public abstract string this[string columnName] { get; }

        // Controleert of het object geldig is door te controleren of de Error-eigenschap leeg is
        public bool IsValid()
        {
            return string.IsNullOrWhiteSpace(Error);
        }
    }
}