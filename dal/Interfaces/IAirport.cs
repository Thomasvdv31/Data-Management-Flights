using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_dal.Interfaces
{
    public interface IAirport
    {
        public bool DeleteAirport(int airportId);

        bool EditeAirport(Airport airport);

        public Airport GetAirportById(int id);

        public IEnumerable<Airport> GetAirports(string naam);

        public IEnumerable<Airport> GetAirportsOnCountry(string country);

        public IEnumerable<Airport> GetAirportsOnNameAndCountry(string name, string country);

        public IEnumerable<Airport> GetAirportVluchten(int id);

        public IEnumerable<Airport> GetAllAirportNames();

        bool InsertAirport(Airport airport);

        public bool IsIataCodeExists(string iataCode);

        public bool VluchtExists(int luchthavenid);
    }
}