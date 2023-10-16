using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_models
{
    public class Brand : BasicClass
    {
        //Properties
        public int Id { get; set; }

        public string Naam { get; set; }

        //Lege Constructor
        public Brand()
        {
        }

        // Parameters worden toegewezen aan de overeenkomstige Properties
        public Brand(int id, string naam)
        {
            Id = id;
            Naam = naam;
        }

        //Valideer properties
        public override string this[string columnName]
        {
            get
            {
                if (nameof(Naam) == columnName && string.IsNullOrWhiteSpace(Naam))
                {
                    return "Name is required.";
                }
                return string.Empty;
            }
        }

        //ToString-methode van het object en retourneert een stringrepresentatie (naam) van een bedrijf.
        public override string ToString()
        {
            return Naam;
        }
    }
}