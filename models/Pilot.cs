using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_Management_Flights_models
{
    public class Pilot : BasicClass
    {
        //Properties
        public IEnumerable<Airport> ArrivalAirports { get; set; }

        public IEnumerable<Airport> DepatureAirports { get; set; }
        public DateTime? Geboortedatum { get; set; }
        public int Id { get; set; }
        public string Naam { get; set; }
        public IEnumerable<Vlucht> Vluchten { get; set; }
        public string Voornaam { get; set; }

        // Parameters worden toegewezen aan de overeenkomstige Properties
        public Pilot(string naam, string voornaam, DateTime? geboortedatum)
        {
            Naam = naam;
            Voornaam = voornaam;
            Geboortedatum = geboortedatum;
        }

        // Lege Constructor
        public Pilot()
        {
        }

        //Valideer Properties
        public override string this[string columnName]
        {
            get
            {
                if (nameof(Naam) == columnName && string.IsNullOrWhiteSpace(Naam))
                {
                    return "Name is required.";
                }
                if (nameof(Voornaam) == columnName && string.IsNullOrWhiteSpace(Voornaam))
                {
                    return "Last name is required.";
                }
                if (nameof(Geboortedatum) == columnName && (Geboortedatum.Value.Date == DateTime.MinValue.Date || Geboortedatum.Value.Date > DateTime.Now))
                {
                    return "Invalid Birthdate.";
                }

                return string.Empty;
            }
        }

        //genereert vluchtgegevens voor een piloot op basis van de eigenschappen Naam, Voornaam, DepatureAirports en ArrivalAirports.
        //De gegenereerde vluchtgegevens worden geretourneerd als een string.
        public string flightdata()
        {
            string result = "";
            foreach (var DepatureAirport in DepatureAirports)
            {
                foreach (var ArrivalAirport in ArrivalAirports)
                {
                    result += $"{Naam} {Voornaam} Has a flight from {DepatureAirport.Naam} Towards {ArrivalAirport.Naam}";
                }
            }
            return result;
        }

        //ToString-methode van het object en retourneert een stringrepresentatie van de piloot.
        public override string ToString()
        {
            return $"{Voornaam} {Naam}";
        }
    }
}