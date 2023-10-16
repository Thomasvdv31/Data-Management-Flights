using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Dapper;
using System.Linq;
using System.Xml.Linq;

namespace Data_Management_Flights_dal.Repositories
{
    public class PilotRepository : BaseRepository, IPilot
    {
        // Verwijdert de gegevens van een piloot uit de database.
        public bool DeletePilot(int pilotId)
        {
            string sql = @"DELETE FROM Vlucht.Piloot
                           WHERE id = @pilotId";

            var parameters = new
            {
                pilotId
            };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Het aantal beïnvloede rijen wordt gecontroleerd om te bepalen of de verwijdering succesvol was.
                // Als ten minste één rij is beïnvloed, wordt true geretourneerd om aan te geven dat de verwijdering is gelukt.
                var affectedRows = db.Execute(sql, parameters);
                if (affectedRows >= 1)
                {
                    return true;
                }
            }
            // Als er geen rijen zijn beïnvloed, wordt false geretourneerd om aan te geven dat de verwijdering is mislukt.
            return false;
        }

        // Bewerkt de gegevens van een piloot in de database
        public bool EditePilot(Pilot pilot)
        {
            string sql = @"UPDATE Vlucht.Piloot SET
                                            naam = @lastname,
                                            voornaam = @name,
                                            geboortedatum = @geboortedatum
                                            WHERE id = @pilotid";

            var parameters = new
            {
                @lastname = pilot.Naam,
                @name = pilot.Voornaam,
                @geboortedatum = pilot.Geboortedatum,
                @pilotid = pilot.Id
            };

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Het aantal beïnvloede rijen wordt gecontroleerd om te bepalen of de update succesvol was.
                // Als precies één rij is beïnvloed, wordt true geretourneerd om aan te geven dat de update is gelukt.
                var affectedRows = db.Execute(sql, parameters);
                if (affectedRows == 1)
                {
                    return true;
                }
            }

            return false;
        }

        // Haalt alle piloten op uit de database.
        public IEnumerable<Pilot> GetAllPilotNames()
        {
            string sql = @"SELECT * FROM Vlucht.Piloot P ORDER BY voornaam, naam";

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                return db.Query<Pilot>(sql);
            }
        }

        // Query om een piloot op te halen op basis van id
        public Pilot GetPilotById(int id)
        {
            string sql = "SELECT * FROM Vlucht.Piloot P WHERE P.id = @id";
            var parameters = new { id };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en retourneert een enkel piloot of null
                return db.QuerySingleOrDefault<Pilot>(sql, parameters);
            }
        }

        // haalt vluchten (Vertek locatie en aankomst locatie) op van de piloot.
        public IEnumerable<Pilot> GetPilotFlights(int pilotId)
        {
            string sql = "SELECT * FROM Vlucht.Piloot P" +
                " INNER JOIN Vlucht.Vlucht V ON V.pilootId = p.id" +
                " INNER JOIN Vlucht.Luchthaven LHaankomst ON V.aankomstLocatie = LHaankomst.id" +
                " INNER JOIN Vlucht.Luchthaven LHvertrek ON V.vertrekLocatie = LHvertrek.id" +
                " WHERE P.id = @pilotId";
            var parameters = new { pilotId };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                var pilots = db.Query<Pilot, Airport, Airport, Pilot>(
                    sql,
                    (pilot, airportnaam, airportnaam2) =>
                    {
                        // Het resultaat van de query wordt gemapt naar de airport- en piloot-objecten.
                        // Hier wordt de piloot aan de airport toegewezen als een lijst met één element.
                        pilot.DepatureAirports = new List<Airport>() { airportnaam };
                        pilot.ArrivalAirports = new List<Airport>() { airportnaam2 };

                        return pilot;
                    },
                    parameters
                );
                return pilots;
            }
        }

        // Haalt alle piloten op uit de database. (kan zoeken op voornaam en achternaam)
        public IEnumerable<Pilot> GetPilots(string name, string lastname)
        {
            string sql = @"SELECT * FROM Vlucht.Piloot P WHERE P.voornaam LIKE '%'+ @name +'%' AND P.naam LIKE '%'+ @lastname +'%'";
            var parameters = new { name, lastname };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en retourneert een lijst van Piloten
                return db.Query<Pilot>(sql, parameters);
            }
        }

        // Voegt een nieuw piloot toe aan de database.
        public bool InsertPilot(Pilot pilot)
        {
            string sql = @"INSERT INTO Vlucht.Piloot (naam, voornaam, geboortedatum)
                           VALUES (@naam, @voornaam, @geboortedatum)";

            var parameters = new
            {
                @naam = pilot.Naam,
                @voornaam = pilot.Voornaam,
                @geboortedatum = pilot.Geboortedatum
            };

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en controleert het aantal beïnvloede rijen om het succes van de invoeging te bevestigen
                var affectedRows = db.Execute(sql, parameters);
                if (affectedRows == 1)
                {
                    return true;
                }
            }

            return false;
        }

        // Query om te controleren of een piloot een associatie heeft met vluchten in de database
        public bool PilotExists(int pilotId)
        {
            string sql = "SELECT COUNT(*) FROM Vlucht.Vlucht V WHERE V.pilootId = @pilotId";
            var parameters = new { pilotId };

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en haalt het aantal rijen op
                int count = db.ExecuteScalar<int>(sql, parameters);  // Haal de eerste waarde op uit de eerste rij van het resultaat
                                                                     // Retourneert true als er ten minste één vlucht is met de gegeven bedrijf als maatschappij id, anders false
                return count > 0;
            }
        }
    }
}