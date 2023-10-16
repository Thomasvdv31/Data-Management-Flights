using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Data_Management_Flights_models
{
    public class User : BasicClass
    {
        // Eigenschappen van de gebruiker
        public DateTime? BirthDate { get; set; }

        public string Country { get; set; }
        public string Email { get; set; }
        public string HouseNumber { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string noteFilePath { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string UserId { get; set; }
        public string ZipCode { get; set; }

        // Constructor voor het maken van een gebruiker met alle eigenschappen
        public User(string userid,
                    string password,
                    string name,
                    string lastname,
                    string housenumber,
                    string street,
                    string zipcode,
                    string phone,
                    string country,
                    string email,
                    DateTime? birthdate)
        {
            UserId = userid;
            Password = password;
            Name = name;
            LastName = lastname;
            HouseNumber = housenumber;
            Street = street;
            ZipCode = zipcode;
            Phone = phone;
            Country = country;
            Email = email;
            BirthDate = birthdate;
        }

        // Constructor voor het maken van een gebruiker met beperkte eigenschappen
        public User(string userid, string password, string name)
        {
            UserId = userid;
            Password = password;
            Name = name;
        }

        // Implementatie van de indexer van BasicClass voor validatie van eigenschappen
        public override string this[string columnName]
        {
            get
            {
                if (nameof(BirthDate) == columnName && (BirthDate.Value.Date == DateTime.MinValue.Date || BirthDate.Value.Date > DateTime.Now))
                {
                    return "- Invalid Birthdate.";
                }
                if (nameof(Country) == columnName && string.IsNullOrWhiteSpace(Country))
                {
                    return "- Country is required.";
                }
                if (nameof(Name) == columnName && string.IsNullOrWhiteSpace(Name))
                {
                    return "- Name is required.";
                }
                if (nameof(LastName) == columnName && string.IsNullOrWhiteSpace(LastName))
                {
                    return "- Last name is required.";
                }
                if (nameof(Email) == columnName && string.IsNullOrWhiteSpace(Email))
                {
                    return "- Email is required.";
                }
                if (nameof(HouseNumber) == columnName && string.IsNullOrWhiteSpace(HouseNumber))
                {
                    return "- House Number is required.";
                }
                if (nameof(Street) == columnName && string.IsNullOrWhiteSpace(Street))
                {
                    return "- Street is required.";
                }
                if (nameof(ZipCode) == columnName && string.IsNullOrWhiteSpace(ZipCode))
                {
                    return "- ZipCode is required.";
                }
                if (nameof(UserId) == columnName && string.IsNullOrWhiteSpace(UserId))
                {
                    return "- User ID is required.";
                }
                if (nameof(Password) == columnName && string.IsNullOrWhiteSpace(Password))
                {
                    return "- Password is required.";
                }
                if (nameof(Phone) == columnName && string.IsNullOrWhiteSpace(Phone))
                {
                    return "- Phone is required.";
                }
                return string.Empty;
            }
        }
    }
}