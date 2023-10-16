using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Linq;
using Dapper;
using System.Security.Policy;
using System.Reflection;
using System.Xml.Linq;

namespace Data_Management_Flights_dal.Repositories
{
    public class FlightRepository : BaseRepository, IFlight
    {
        // Verwijdert de gegevens van een vlucht uit de database.
        public bool DeleteFlight(int flightId)
        {
            string sql = @"DELETE FROM Vlucht.Vlucht
                           WHERE id = @flightId";

            var parameters = new
            {
                flightId
            };

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                var affectedRows = db.Execute(sql, parameters);
                if (affectedRows >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        // Haalt alle vluchten met hun passagiers op uit de database.
        public IEnumerable<Vlucht> GetAllFlights()
        {
            var sql = @"SELECT V.*, '' AS SplitCol, P.*, '' AS SplitCol, LHD.*, '' AS SplitCol, LHA.*, '' AS SplitCol, M.*, '' AS SplitCol, VT.*, '' AS SplitCol, PA.*
                        FROM Vlucht.Vlucht V
                        INNER JOIN Vlucht.Piloot P ON V.pilootId = P.Id
                        INNER JOIN Vlucht.Luchthaven LHD ON LHD.id = V.vertrekLocatie
                        INNER JOIN Vlucht.Luchthaven LHA ON LHA.id = V.aankomstLocatie
                        INNER JOIN Vlucht.Maatschappij M ON M.id = V.maatschappijId
                        INNER JOIN Vlucht.Vliegtuig VT ON VT.id = V.vliegtuigId
                        LEFT JOIN Vlucht.PassagierVlucht PV ON PV.vluchtId = V.id
                        LEFT JOIN Vlucht.Passagier PA ON PA.id = PV.passagierId
                        ORDER BY V.id";

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                var flights = db.Query<Vlucht, Pilot, Airport, Airport, Company, Airplane, Passenger, Vlucht>(
                    sql,
                    (flight, pilot, airportD, airportA, company, airplane, passenger) =>
                    {
                        flight.Pilot = pilot;
                        flight.Depature = airportD;
                        flight.Arrival = airportA;
                        flight.Company = company;
                        flight.Airplane = airplane;
                        flight.Passengers = new List<Passenger>() { passenger };
                        return flight;
                    },
                    splitOn: "SplitCol"
                );

                return SortFlights(flights);
            }
        }

        // Query om een vlucht op te halen op basis van id
        public Vlucht GetFlightOnId(int flightid)
        {
            string sql = "SELECT * FROM Vlucht.Vlucht WHERE id = @flightid";
            var parameters = new { flightid };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                return db.QuerySingleOrDefault<Vlucht>(sql, parameters);
            }
        }

        // De SQL-query selecteert vluchten die aankomen op de opgegeven luchthaven (airportId).
        public IEnumerable<Vlucht> GetFlightsByAirportArrivalLocation(int airportId)
        {
            string sql = @"SELECT * FROM Vlucht.Vlucht V
                           INNER JOIN Vlucht.Luchthaven LH ON V.vertrekLocatie = LH.id
                           WHERE V.aankomstLocatie = @airportId";
            var parameters = new { airportId };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                return db.Query<Vlucht, Airport, Vlucht>(
                    sql,
                    (vlucht, airport) =>
                    {
                        // Het resultaat van de query wordt gemapt naar de Vlucht- en Airport-objecten.
                        // Hier wordt de Airport aan de Vlucht toegewezen als een lijst met één element.
                        vlucht.Airport = new List<Airport>() { airport };
                        return vlucht;
                    },
                    new { airportId }
                );
            }
        }

        // De SQL-query selecteert vluchten die vertrekken vanaf de opgegeven luchthaven (airportId).
        public IEnumerable<Vlucht> GetFlightsByAirportDepartureLocation(int airportId)
        {
            string sql = @"SELECT * FROM Vlucht.Vlucht V
                           INNER JOIN Vlucht.Luchthaven LH ON V.aankomstLocatie = LH.id
                           WHERE V.vertrekLocatie = @airportId";
            var parameters = new { airportId };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                return db.Query<Vlucht, Airport, Vlucht>(
                    sql,
                    (vlucht, airport) =>
                    {
                        // Het resultaat van de query wordt gemapt naar de Vlucht- en Airport-objecten.
                        // Hier wordt de Airport aan de Vlucht toegewezen als een lijst met één element.
                        vlucht.Airport = new List<Airport>() { airport };
                        return vlucht;
                    },
                    new { airportId }
                );
            }
        }

        // Voegt een nieuw vlucht toe aan de database.
        public bool InsertFlight(Vlucht flight)
        {
            string sql = @"INSERT INTO Vlucht.Vlucht (maatschappijId, vliegtuigId, vertrek, geplandeAankomst, werkelijkeAankomst, vertrekLocatie, aankomstLocatie, gate, prijs, pilootId)
                           VALUES (@maatschappijid, @vliegtuigid, @vertrek, @geplandeaankomst, @werkelijkeaankomst, @vertreklocatie, @aankomstlocatie, @gate, @prijs, @pilootid)";

            var parameters = new
            {
                @maatschappijid = flight.MaatschappijId,
                @vliegtuigid = flight.VliegtuigId,
                @vertrek = flight.Vertrek,
                @geplandeaankomst = flight.GeplandeAankomst,
                @werkelijkeaankomst = flight.WerkelijkeAankomst,
                @vertreklocatie = flight.VertrekLocatie,
                @aankomstlocatie = flight.AankomstLocatie,
                @gate = flight.Gate,
                @prijs = flight.Prijs,
                @pilootid = flight.PilootId
            };

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                var affectedRows = db.Execute(sql, parameters);
                if (affectedRows == 1)
                {
                    return true;
                }
            }

            return false;
        }

        // Bewerkt de gegevens van een vlucht in de database.
        public bool UpdateFlight(Vlucht flight)
        {
            string sql = @"UPDATE Vlucht.Vlucht SET maatschappijId = @maatschappijid,
                                                    vliegtuigId =  @vliegtuigid,
                                                    vertrek = @vertrek,
                                                    geplandeAankomst = @geplandeaankomst,
                                                    werkelijkeAankomst = @werkelijkeaankomst,
                                                    vertrekLocatie = @vertreklocatie,
                                                    aankomstLocatie =  @aankomstlocatie,
                                                    gate = @gate,
                                                    prijs = @prijs,
                                                    pilootId = @pilootid
                                                    WHERE id = @vluchtid";

            var parameters = new
            {
                @vluchtid = flight.Id,
                @maatschappijid = flight.MaatschappijId,
                @vliegtuigid = flight.VliegtuigId,
                @vertrek = flight.Vertrek,
                @geplandeaankomst = flight.GeplandeAankomst,
                @werkelijkeaankomst = flight.WerkelijkeAankomst,
                @vertreklocatie = flight.VertrekLocatie,
                @aankomstlocatie = flight.AankomstLocatie,
                @gate = flight.Gate,
                @prijs = flight.Prijs,
                @pilootid = flight.PilootId
            };

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                var affectedRows = db.Execute(sql, parameters);
                if (affectedRows == 1)
                {
                    return true;
                }
            }

            return false;
        }

        //Sorteert vluchten op basis van hun Id en groepeert ze.
        private static IEnumerable<Vlucht> SortFlights(IEnumerable<Vlucht> vluchten)
        {
            var sorted = vluchten.GroupBy(vlucht => vlucht.Id);

            List<Vlucht> flightWithPassengers = new List<Vlucht>();

            foreach (var sort in sorted)  // Itereer over elke groep vluchten.
            {
                var vlucht = sort.First();
                List<Passenger> allPassengers = new List<Passenger>();

                foreach (var v in sort) // Itereer over elke vlucht in de groep.
                {
                    // Voeg de eerste passagier van elke vlucht toe aan de lijst met passagiers.
                    allPassengers.Add(v.Passengers.First());
                }

                vlucht.Passengers = allPassengers;
                flightWithPassengers.Add(vlucht);
            }

            return flightWithPassengers;
        }
    }
}