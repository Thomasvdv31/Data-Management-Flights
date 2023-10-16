using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_models
{
    public class Passenger
    {
        //Properties
        public DateTime Geboortedatum { get; set; }

        public string Huisnummer { get; set; }
        public int Id { get; set; }
        public string Land { get; set; }
        public string Naam { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        public string Voornaam { get; set; }
    }
}