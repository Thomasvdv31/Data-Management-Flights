using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_models
{
    public class Airport : BasicClass
    {
        // Eigenschappen van de luchthaven
        public string Huisnummer { get; set; }

        public string IataCode { get; set; }
        public int Id { get; set; }
        public string Land { get; set; }
        public string Naam { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        public IEnumerable<Vlucht> Vluchten { get; set; }

        // Lege constructor
        public Airport()
        {
        }

        // Constructor om de luchthavengegevens in te stellen bij het maken van een nieuw object
        public Airport(string huisnummer, string iataCode, string land, string naam, string postcode, string straat)
        {
            Huisnummer = huisnummer;
            IataCode = iataCode;
            Land = land;
            Naam = naam;
            Postcode = postcode;
            Straat = straat;
        }

        // Implementatie van de indexer om validatiemeldingen voor eigenschappen terug te geven
        public override string this[string columnName]
        {
            get // Controleert de opgegeven kolomnaam en geeft de bijbehorende validatiemelding terug
            {
                if (nameof(Naam) == columnName && string.IsNullOrWhiteSpace(Naam))
                {
                    return "Name is required.";
                }
                if (nameof(Land) == columnName && string.IsNullOrWhiteSpace(Land))
                {
                    return "Country is required.";
                }
                if (nameof(Straat) == columnName && string.IsNullOrWhiteSpace(Straat))
                {
                    return "Street is required.";
                }
                if (nameof(IataCode) == columnName && string.IsNullOrWhiteSpace(IataCode))
                {
                    return "Iatacode is required.";
                }
                if (nameof(Postcode) == columnName && string.IsNullOrWhiteSpace(Postcode))
                {
                    return "Zipcode is required.";
                }
                if (nameof(Huisnummer) == columnName && string.IsNullOrWhiteSpace(Huisnummer))
                {
                    return "House number is required.";
                }
                if (nameof(Id) == columnName && Id == null)
                {
                    return "Select an airport";
                }
                return string.Empty;
            }
        }

        // Override van ToString-methode om de naam van de luchthaven terug te geven
        public override string ToString()
        {
            return $"{Naam} ({Land})";
        }
    }
}