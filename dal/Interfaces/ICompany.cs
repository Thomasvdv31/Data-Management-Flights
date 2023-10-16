using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Management_Flights_dal.Interfaces
{
    public interface ICompany
    {
        public bool CompanyExists(int Companyid);

        public bool DeleteCompany(int companyId);

        bool EditeCompany(Company company);

        public IEnumerable<Company> GetAllCompanyNames();

        public IEnumerable<Company> GetCompanies(string naam);

        public Company GetCompanyById(int id);

        public IEnumerable<Company> GetFlightsByAirportArrivalLocation(int companyId);

        public IEnumerable<Company> GetFlightsByAirportDepartureLocation(int companyId);

        bool InsertCompany(Company company);
    }
}