using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Xml.Linq;
using Dapper;

namespace Data_Management_Flights_dal.Repositories
{
    public class CompanyRepository : BaseRepository, ICompany
    {
        // Query om te controleren of bedrijf een associatie heeft met vluchten in de database
        public bool CompanyExists(int Companyid)
        {
            string sql = "SELECT COUNT(*) FROM Vlucht.Vlucht V WHERE V.maatschappijId = @companyid";
            var parameters = new { Companyid };

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en haalt het aantal rijen op
                int count = db.ExecuteScalar<int>(sql, parameters);  // Haal de eerste waarde op uit de eerste rij van het resultaat
                // Retourneert true als er ten minste één vlucht is met de gegeven bedrijf als maatschappij id, anders false
                return count > 0;
            }
        }

        // Verwijdert de gegevens van een bedrijf uit de database.
        public bool DeleteCompany(int companyId)
        {
            string sql = @"DELETE FROM Vlucht.Maatschappij
                           WHERE id = @companyId";

            var parameters = new
            {
                companyId
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

            return false;
        }

        // Bewerkt de gegevens van een bedrijf in de database.
        public bool EditeCompany(Company company)
        {
            string sql = @"UPDATE Vlucht.Maatschappij SET
                                            naam = @naam
                                            WHERE id = @companyid";

            var parameters = new
            {
                @naam = company.Naam,
                @companyid = company.Id
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

        // Haalt alle bedrijfsnamen op uit de database.
        public IEnumerable<Company> GetAllCompanyNames()
        {
            string sql = @"SELECT * FROM Vlucht.Maatschappij M ORDER BY naam";

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                return db.Query<Company>(sql);
            }
        }

        // Query om bedrijven op te halen op basis van naam (met gedeeltelijke overeenkomst)
        public IEnumerable<Company> GetCompanies(string name = "")
        {
            string sql = @"SELECT * FROM Vlucht.Maatschappij M WHERE M.naam LIKE '%'+ @name +'%'";
            var parameters = new { name };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en retourneert een lijst van bedrijven
                return db.Query<Company>(sql, parameters);
            }
        }

        // Query om een bedrijf op te halen op basis van id
        public Company GetCompanyById(int id)
        {
            string sql = "SELECT * FROM Vlucht.Maatschappij M WHERE M.id = @id";
            var parameters = new { id };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                // Voert de query uit en retourneert een enkel bedrijf of null
                return db.QuerySingleOrDefault<Company>(sql, parameters);
            }
        }

        // haalt vluchten op op basis van de aankomstlocatie van de luchthaven voor een specifiek bedrijf.
        public IEnumerable<Company> GetFlightsByAirportArrivalLocation(int companyId)
        {
            string sql = @"SELECT M.*, V.*, LH.* FROM Vlucht.Maatschappij M
                           INNER JOIN Vlucht.Vlucht V ON V.maatschappijId = M.id
                           INNER JOIN Vlucht.Luchthaven LH ON V.aankomstLocatie = LH.id
                           WHERE V.maatschappijId = @companyId;";
            var parameters = new { companyId };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                var result = db.Query<Company, Vlucht, Airport, Company>(
                    sql,
                    (company, vlucht, airport) =>
                    {
                        // Maak nieuwe lijsten voor de vluchten en luchthavens in het Company-object.
                        company.Vluchten = new List<Vlucht>() { vlucht };
                        company.Airports = new List<Airport>() { airport };

                        return company;
                    },
                    splitOn: "id, id, id",
                    param: parameters
                );

                return result;
            }
        }

        // Haalt vluchten op op basis van de vertreklocatie van de luchthaven voor een specifiek bedrijf.
        public IEnumerable<Company> GetFlightsByAirportDepartureLocation(int companyId)
        {
            string sql = @"SELECT M.*, V.*, LH.* FROM Vlucht.Maatschappij M
                           INNER JOIN Vlucht.Vlucht V ON V.maatschappijId = M.id
                           INNER JOIN Vlucht.Luchthaven LH ON V.vertrekLocatie = LH.id
                           WHERE V.maatschappijId = @companyId;";
            var parameters = new { companyId };
            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                var result = db.Query<Company, Vlucht, Airport, Company>(
                    sql,
                    (company, vlucht, airport) =>
                    {
                        // Maak nieuwe lijsten aan voor de vluchten en luchthavens in het Company-object.
                        company.Vluchten = new List<Vlucht>() { vlucht };
                        company.Airports = new List<Airport>() { airport };

                        return company;
                    },
                    splitOn: "id, id, id",
                    param: parameters
                );

                return result;
            }
        }

        // Voegt een nieuw bedrijf toe aan de database.
        public bool InsertCompany(Company company)
        {
            string sql = @"INSERT INTO Vlucht.Maatschappij (naam)
                           VALUES (@naam)";

            var parameters = new
            {
                @naam = company.Naam
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
    }
}