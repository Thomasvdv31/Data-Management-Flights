using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Dapper;
using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_models;

namespace Data_Management_Flights_dal.Repositories
{
    public class AirplaneRepository : BaseRepository, IAirplane
    {
        //SQL-query uit om alle vliegtuigmodellen op te halen,
        public IEnumerable<Airplane> GetAllAirplaneModels()
        {
            string sql = @"SELECT * FROM Vlucht.Vliegtuig V ORDER BY model";

            using (IDbConnection db = new SqlConnection(VluchtenConnectionString))
            {
                return db.Query<Airplane>(sql);
            }
        }
    }
}