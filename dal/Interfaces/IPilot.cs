using System;
using System.Collections.Generic;
using Data_Management_Flights_models;
using System.Text;

namespace Data_Management_Flights_dal.Interfaces
{
    public interface IPilot
    {
        public bool DeletePilot(int pilotId);

        bool EditePilot(Pilot pilot);

        public IEnumerable<Pilot> GetAllPilotNames();

        public Pilot GetPilotById(int id);

        public IEnumerable<Pilot> GetPilotFlights(int pilotId);

        public IEnumerable<Pilot> GetPilots(string name, string lastname);

        bool InsertPilot(Pilot pilot);

        public bool PilotExists(int pilotId);
    }
}