using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_models
{
    public class Company : BasicClass
    {
        //Properties
        public IEnumerable<Airport> Airports { get; set; }

        public int Id { get; set; }
        public string Naam { get; set; }
        public IEnumerable<Vlucht> Vluchten { get; set; }

        // Lege constructor
        public Company()
        {
        }

        // Parameters worden toegewezen aan de overeenkomstige Properties
        public Company(string name)
        {
            Naam = name;
        }

        //Valideer
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

        // Methode om informatie over de vlucht bij aankomst te retourneren
        public string FlightArrivalInfo()
        {
            string result = "";
            foreach (Vlucht vlucht in Vluchten)
            {
                foreach (Airport airport in Airports)
                {
                    result += $"From {airport.Naam} - Expected arrival time on: {vlucht.GeplandeAankomst}";
                }
            }
            return result;
        }

        // Methode om informatie over de vlucht bij vertrek te retourneren
        public string FlightDepartureInfo()
        {
            string result = "";
            foreach (Vlucht vlucht in Vluchten)
            {
                foreach (Airport airport in Airports)
                {
                    result += $"Towards {airport.Naam} -  Expected Departure time on: {vlucht.Vertrek}";
                }
            }
            return result;
        }

        //ToString-methode van het object en retourneert een stringrepresentatie (naam) van een bedrijf.
        public override string ToString()
        {
            return Naam;
        }
    }
}