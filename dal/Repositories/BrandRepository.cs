using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Data;

namespace Data_Management_Flights_dal.Repositories
{
    public class BrandRepository : BaseRepository, IBrand
    {
        //SQL-query uit om alle maatschappijen op te halen,
        public IEnumerable<Brand> GetAllBrandNames()
        {
            string sql = @"SELECT * FROM Vlucht.Merk M ORDER BY naam";

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                return db.Query<Brand>(sql);
            }
        }
    }
}