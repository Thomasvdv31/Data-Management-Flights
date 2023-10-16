using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;

namespace Data_Management_Flights_dal.Repositories
{
    public class AirportRepository : BaseRepository, IAirport
    {
        // De SQL-query verwijdert een luchthaven met de opgegeven airportId.
        public bool DeleteAirport(int airportId)
        {
            string sql = @"DELETE FROM Vlucht.Luchthaven
                           WHERE id = @airportId";

            var parameters = new
            {
                airportId
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

        // De SQL-query werkt de gegevens van een luchthaven bij op basis van de opgegeven Airport-object.
        public bool EditeAirport(Airport airport)
        {
            string sql = @"UPDATE Vlucht.Luchthaven SET
                                            naam = @naam,
                                            land = @land,
                                            postcode = @postcode,
                                            huisnummer = @huisnummer,
                                            straat = @straat
                                            WHERE id = @airid";

            var parameters = new
            {
                @naam = airport.Naam,
                @land = airport.Land,
                @postcode = airport.Postcode,
                @huisnummer = airport.Huisnummer,
                @straat = airport.Straat,
                @airid = airport.Id
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
            // Als er geen rijen zijn beïnvloed of als er meer dan één rij is beïnvloed,
            // wordt false geretourneerd om aan te geven dat de update is mislukt.
            return false;
        }

        // Query om een luchthaven op te halen op basis van id
        public Airport GetAirportById(int id)
        {
            string sql = "SELECT * FROM Vlucht.Luchthaven L WHERE L.id = @id";
            var parameters = new { id };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en retourneert een enkele luchthaven of null
                return db.QuerySingleOrDefault<Airport>(sql, parameters);
            }
        }

        // Query om luchthavens op te halen op basis van naam (met gedeeltelijke overeenkomst)
        public IEnumerable<Airport> GetAirports(string name = "")
        {
            string sql = @"SELECT * FROM Vlucht.Luchthaven L WHERE L.naam LIKE '%'+ @name +'%'";
            var parameters = new { name };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en retourneert een lijst van luchthavens
                return db.Query<Airport>(sql, parameters);
            }
        }

        // Query om luchthavens op te halen op basis van het land
        public IEnumerable<Airport> GetAirportsOnCountry(string country)
        {
            string sql = @"SELECT * FROM Vlucht.Luchthaven L WHERE L.land LIKE '%'+ @country + '%'";
            var parameters = new { country };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en retourneert een lijst van luchthavens
                return db.Query<Airport>(sql, parameters);
            }
        }

        // Query om luchthavens op te halen op basis van naam (met gedeeltelijke overeenkomst) en land
        public IEnumerable<Airport> GetAirportsOnNameAndCountry(string name, string country)
        {
            string sql = @"SELECT * FROM Vlucht.Luchthaven L WHERE L.naam LIKE '%'+ @name +'%' AND L.land LIKE '%'+ @country + '%'";
            var parameters = new { name, country };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en retourneert een lijst van luchthavens
                return db.Query<Airport>(sql, parameters);
            }
        }

        // Query om luchthavens op te halen en bijbehorende vluchten toe te voegen op basis van een specifiek id
        public IEnumerable<Airport> GetAirportVluchten(int id)
        {
            string sql = @"SELECT * FROM Vlucht.Luchthaven L
                           INNER JOIN Vlucht.Vlucht V ON V.vertrekLocatie = L.id
                           WHERE L.id = @id";

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en mapt het resultaat naar de Airport en Vlucht klassen
                return db.Query<Airport, Vlucht, Airport>(
                    sql,
                    (airport, vlucht) =>
                    {
                        airport.Vluchten = new List<Vlucht>() { vlucht };
                        return airport;
                    },
                    new { id }
                );
            }
        }

        //SQL-query uit om alle luchthavens op te halen,
        public IEnumerable<Airport> GetAllAirportNames()
        {
            string sql = @"SELECT * FROM Vlucht.Luchthaven L ORDER BY naam";

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                return db.Query<Airport>(sql);
            }
        }

        // Query om een nieuwe luchthaven toe te voegen aan de database
        public bool InsertAirport(Airport airport)
        {
            string sql = @"INSERT INTO Vlucht.Luchthaven (naam, land, postcode, huisnummer, straat, iataCode)
                           VALUES (@naam, @land, @postcode, @huisnummer, @straat, @iataCode)";

            var parameters = new
            {
                @naam = airport.Naam,
                @land = airport.Land,
                @postcode = airport.Postcode,
                @huisnummer = airport.Huisnummer,
                @straat = airport.Straat,
                @iataCode = airport.IataCode
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

        // Query om te controleren of een IATA-code al bestaat in de database
        public bool IsIataCodeExists(string iataCode)
        {
            string sql = "SELECT COUNT(*) FROM Vlucht.Luchthaven L WHERE L.IataCode = @iataCode";
            var parameters = new { iataCode };

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en haalt het aantal rijen op dat overeenkomt met de IATA-code
                int count = db.ExecuteScalar<int>(sql, parameters);
                // Retourneert true als er ten minste één rij overeenkomt met de IATA-code, anders false
                return count > 0;
            }
        }

        // Query om te controleren of luchthaven een associatie heeft met vluchten in de database
        public bool VluchtExists(int luchthavenid)
        {
            string sql = "SELECT COUNT(*) FROM Vlucht.Vlucht V WHERE V.vertrekLocatie = @luchthavenid OR V.aankomstLocatie = @luchthavenid";
            var parameters = new { luchthavenid };

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en haalt het aantal rijen op
                int count = db.ExecuteScalar<int>(sql, parameters);  // Haal de eerste waarde op uit de eerste rij van het resultaat
                // Retourneert true als er ten minste één vlucht is met de gegeven luchthaven als vertrek- of aankomstlocatie, anders false
                return count > 0;
            }
        }
    }
}