using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Linq;

namespace Data_Management_Flights_models
{
    // Properties
    public class Vlucht : BasicClass
    {
        public int? AankomstLocatie { get; set; }

        public Airplane Airplane { get; set; }

        public IEnumerable<Airport> Airport { get; set; }

        public Airport Arrival { get; set; }

        public Company Company { get; set; }

        public Airport Depature { get; set; }

        public int? Gate { get; set; }

        public DateTime? GeplandeAankomst { get; set; }

        public int? Id { get; set; }

        public int? MaatschappijId { get; set; }

        public IEnumerable<Passenger> Passengers { get; set; }

        public int? PilootId { get; set; }

        public Pilot Pilot { get; set; }

        public decimal? Prijs { get; set; }

        public DateTime? Vertrek { get; set; }

        public int? VertrekLocatie { get; set; }

        public int? VliegtuigId { get; set; }

        public DateTime? WerkelijkeAankomst { get; set; }

        // Lege Constructor
        public Vlucht()
        { }

        // Parameters worden toegewezen aan de overeenkomstige Properties
        // De parameters zijn optioneel, aangegeven met het "?", wat betekent dat ze nullable zijn.
        public Vlucht(int? id, int? aankomstLocatie, int? gate, DateTime? geplandeAankomst, int? maatschappijId, int? pilootId, decimal? prijs, DateTime? vertrek, int? vertrekLocatie, int? vliegtuigId, DateTime? werkelijkeAankomst)
        {
            Id = id;
            AankomstLocatie = aankomstLocatie;
            Gate = gate;
            GeplandeAankomst = geplandeAankomst;
            MaatschappijId = maatschappijId;
            PilootId = pilootId;
            Prijs = prijs;
            Vertrek = vertrek;
            VertrekLocatie = vertrekLocatie;
            VliegtuigId = vliegtuigId;
            WerkelijkeAankomst = werkelijkeAankomst;
        }

        //Valideer
        public override string this[string columnName]
        {
            get
            {
                if (nameof(AankomstLocatie) == columnName && AankomstLocatie == null)
                {
                    return "Please choose an Arrival Location.";
                }
                if (nameof(Gate) == columnName && Gate == null)
                {
                    return "Please enter a gate number.";
                }
                if (nameof(GeplandeAankomst) == columnName && GeplandeAankomst == null)
                {
                    return "Arrival date and time is required.";
                }
                if (nameof(MaatschappijId) == columnName && MaatschappijId == null)
                {
                    return "Please choose a company.";
                }
                if (nameof(PilootId) == columnName && PilootId == null)
                {
                    return "Please choose a pilot.";
                }
                if (nameof(Prijs) == columnName && Prijs == null)
                {
                    return "Price is required.";
                }
                if (nameof(Vertrek) == columnName && Vertrek == null)
                {
                    return "Depature date and time is required";
                }
                if (nameof(VertrekLocatie) == columnName && VertrekLocatie == null)
                {
                    return "Depature Location is required.";
                }
                if (nameof(VliegtuigId) == columnName && VliegtuigId == null)
                {
                    return "Airplane model is required";
                }

                return string.Empty;
            }
        }

        // Methode om informatie over de vlucht bij aankomst te retourneren
        public string FlightArrivalInfo()
        {
            string result = "";
            foreach (Airport airport in Airport)
            {
                result += $"From {airport.Naam} Departure time: {Vertrek} - Expected landing time on: {GeplandeAankomst}";
            }
            return result;
        }

        // Methode om informatie over de vlucht bij vertrek te retourneren
        public string FlightDepartureInfo()
        {
            string result = "";
            foreach (Airport airport in Airport)
            {
                result += $"Towards {airport.Naam} Departure time: {Vertrek} - Expected landing time on: {GeplandeAankomst}";
            }
            return result;
        }

        //retourneert informatie over passagiers van een vlucht.
        public string PassengerInfo()
        {
            string result = "";

            // Controleer of er passagiers zijn en of minstens één passagier een naam of voornaam heeft.
            if (this.Passengers != null && this.Passengers.Any(p => !string.IsNullOrEmpty(p.Naam) || !string.IsNullOrEmpty(p.Voornaam)))
            {
                foreach (Passenger passenger in this.Passengers)
                {
                    result += $"- {passenger.Naam} {passenger.Voornaam}\n";
                }
            }
            else
            {
                // Als er geen passagiers zijn of geen passagiersnamen of -voornamen hebben, wordt een melding weergegeven.
                result = "This flight doesn't have passengers yet.";
            }

            return result;
        }

        //ToString-methode van het object en retourneert een stringrepresentatie van de Vlucht.
        public override string ToString()
        {
            string text = $"Flight {Id}:\n" +
                          $"From {Depature.Naam} ({Depature.Land}) Towards {Arrival.Naam} ({Arrival.Land})\n" +
                          $"Depature Time: {Vertrek} Arrival Time: {GeplandeAankomst}\n";

            // Indien werkelijkeaankomst beschikbaar, wordt ook de werkelijke aankomsttijd opgenomen in de string.
            if (WerkelijkeAankomst != null)
            {
                text += $"Actual Arrival Time: {WerkelijkeAankomst}\n";
            }

            // De prijs wordt geconverteerd naar een geldtekst met behulp van de Conversions.ConvertNumericToMoneyText-methode.
            text += $"Pilot: {Pilot.Naam} {Pilot.Voornaam}\n" +
                    $"Company: {Company.Naam} - Airplane model: {Airplane.Model}\n" +
                    $"Flight price: {Conversions.ConvertNumericToMoneyText((decimal)Prijs)} - Gate: {Gate}";

            return text;
        }
    }
}