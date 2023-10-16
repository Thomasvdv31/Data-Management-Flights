using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_dal.Interfaces
{
    public interface IFlight
    {
        public bool DeleteFlight(int flightId);

        public IEnumerable<Vlucht> GetAllFlights();

        public Vlucht GetFlightOnId(int flightid);

        public IEnumerable<Vlucht> GetFlightsByAirportArrivalLocation(int airportId);

        public IEnumerable<Vlucht> GetFlightsByAirportDepartureLocation(int airportId);

        public bool InsertFlight(Vlucht flight);

        public bool UpdateFlight(Vlucht flight);
    }
}