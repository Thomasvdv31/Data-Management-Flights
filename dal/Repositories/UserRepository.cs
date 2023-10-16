using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Data_Management_Flights_models;
using System.Data.SqlClient;
using Data_Management_Flights_dal.Interfaces;
using System.Data;
using System.Security.Cryptography;

namespace Data_Management_Flights_dal.Repositories
{
    public class UserRepository : BaseRepository, IUsers
    {
        // Voegt een nieuwe gebruiker toe aan de database.
        public bool InsertUser(User user)
        {
            string sql = @"INSERT INTO Users.Users (UserId, Password, Email, Name, LastName, Phone, HouseNumber, Street, ZipCode, Country, BirthDate)
                           VALUES (@userid, @password, @email, @name, @lastname, @phone, @housenumber, @street, @zipcode, @country, @birthdate)";

            var parameters = new
            {
                @userid = user.UserId,
                @password = user.Password,
                @email = user.Email,
                @name = user.Name,
                @lastname = user.LastName,
                @phone = user.Phone,
                @housenumber = user.HouseNumber,
                @street = user.Street,
                @zipcode = user.ZipCode,
                @country = user.Country,
                @birthdate = user.BirthDate
            };

            using (IDbConnection db = new SqlConnection(UserConnectionString))
            {
                var affectedRows = db.Execute(sql, parameters);
                if (affectedRows == 1)
                {
                    return true;
                }
            }

            return false;
        }

        // Query om een gebruiker op te halen op basis van id
        public User OphalenUserViaUserId(string userid)
        {
            string sql = @"SELECT U.UserId, U.Password, U.Name FROM Users.Users U WHERE U.UserId = @userid";
            var parameters = new { userid };

            using (IDbConnection db = new SqlConnection(UserConnectionString))
            {
                var result = db.QuerySingleOrDefault(sql, parameters);

                // Controleer of er een resultaat is gevonden.
                if (result == null)
                {
                    throw new Exception("User not found.");
                }

                // Maak een nieuwe User-object aan met behulp van de verkregen gegevens en retourneer deze.
                User user = new User(result.UserId,
                                     result.Password,
                                     result.Name);

                return user;
            }
        }
    }
}