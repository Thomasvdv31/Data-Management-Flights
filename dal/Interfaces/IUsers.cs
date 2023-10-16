using System;
using System.Collections.Generic;
using System.Text;
using Data_Management_Flights_models;

namespace Data_Management_Flights_dal.Interfaces
{
    public interface IUsers
    {
        bool InsertUser(User user);

        public User OphalenUserViaUserId(string userid);
    }
}