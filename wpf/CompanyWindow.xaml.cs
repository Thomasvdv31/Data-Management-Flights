using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_dal.Repositories;
using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Data_Management_Flights_wpf
{
    /// <summary>
    /// Interaction logic for CompanyWindow.xaml
    /// </summary>
    public partial class CompanyWindow : UserControl
    {
        // Repository instanties
        private ICompany CompanyRepository = new CompanyRepository();

        private IFlight FlightRepository = new FlightRepository();

        public CompanyWindow()
        {
            InitializeComponent();
            // Initialiseer de knoppen als uitgeschakeld
            btnAddAirport.IsEnabled = false;
            btnEditAirport.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void AddOrEditCompany(bool isInsert)
        {
            // Maak een nieuwe Airport instantie op basis van de ingevoerde gegevens
            Company company = new Company(txtCompanyName.Text);

            if (company.IsValid())
            {
                if (isInsert)
                {
                    // Voeg de bedrijf toe aan de repository
                    CompanyRepository.InsertCompany(company);
                    datagridCompany.ItemsSource = CompanyRepository.GetCompanies(txtSearchName.Text);
                    ClearFields();
                }
                else
                {
                    // Haal het ID op van de geselecteerde luchthaven in het datagrid.
                    // Het "?."-gedeelte zorgt ervoor dat er geen uitzondering optreedt als er geen luchthaven is geselecteerd.
                    int? id = ((Company)datagridCompany.SelectedItem)?.Id;
                    if (id != null) // Controleer of het ID niet null is, wat betekent dat er een luchthaven is geselecteerd.
                    {
                        // Maak een nieuwe Airport instantie met bijgewerkte gegevens
                        Company company2 = new Company(txtCompanyName.Text)
                        {
                            // Het "?."-gedeelte zorgt ervoor dat de property "Id" wordt bijgewerkt met het ID van de geselecteerde luchthaven.
                            Id = (int)id
                        };

                        // Werk de bedrijf bij in de repository
                        CompanyRepository.EditeCompany(company2);
                        datagridCompany.ItemsSource = CompanyRepository.GetCompanies(txtSearchName.Text);
                        MessageBox.Show("Company successfully updated.", "Succes", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                        /* Zoek het bijgewerkte airport-item in de datagrid opnieuw.
                           gebruik van LINQ's FirstOrDefault-methode om de eerste overeenkomende
                           Airport te vinden op basis van de Id-eigenschap. Dit verbetert de efficiëntie*/
                        var SelectedCompany = datagridCompany.Items.Cast<Company>().FirstOrDefault(ap => ap.Id == company2.Id);
                        if (SelectedCompany != null)
                        {
                            // Als er een overeenkomend item is gevonden, stellen we het in als het geselecteerde item
                            datagridCompany.SelectedItem = SelectedCompany;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select a company", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show(company.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditCompany(true);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (datagridCompany.SelectedItem != null)
            {
                if (!CompanyRepository.CompanyExists(((Company)datagridCompany.SelectedItem).Id))
                {
                    if (MessageBox.Show("Are you sure ?", "Confirm Deletion", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        // Verwijder airport
                        if (CompanyRepository.DeleteCompany(((Company)datagridCompany.SelectedItem).Id))
                        {
                            MessageBox.Show("Airport is deleted.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                            ClearFields();
                            datagridCompany.ItemsSource = CompanyRepository.GetCompanies(txtSearchName.Text);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Companies that are registered for flights cannot be deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a Company.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditCompany_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditCompany(false);
        }

        private void btnReceiveCompanies_Click(object sender, RoutedEventArgs e)
        {
            btnEditAirport.IsEnabled = true;
            btnAddAirport.IsEnabled = true;
            btnDelete.IsEnabled = true;
            ClearFields();
            checkDeparture.IsChecked = false;
            checkUpcoming.IsChecked = false;

            datagridCompany.ItemsSource = CompanyRepository.GetCompanies(txtSearchName.Text);
        }

        private void checkDeparture_Checked(object sender, RoutedEventArgs e)
        {
            ShowDepartingFlightData();
        }

        private void checkDeparture_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowAllFlightData();
        }

        private void checkUpcoming_Checked(object sender, RoutedEventArgs e)
        {
            checkDeparture.IsChecked = false;
            ShowArrivingFlightData();
        }

        private void checkUpcoming_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowAllFlightData();
        }

        private void ClearFields()
        {
            txtCompanyName.Text = string.Empty; // Wis de tekst in het tekstveld voor de luchthavennaam

            datagridCompany.SelectedItem = null; // Maak de geselecteerde rij in het luchthaven-datagrid leeg
            FlightData.ItemsSource = null; // Wis de gegevensbron voor het vluchtgegevens-datagrid
        }

        private void datagridCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datagridCompany.SelectedItem != null)
            {
                SetData(((Company)datagridCompany.SelectedItem).Id); // Haal de gegevens op voor de geselecteerde luchthaven
            }

            ShowAllFlightData(); // Toon alle vluchtgegevens
            if (checkDeparture.IsChecked == true)
            {
                ShowDepartingFlightData(); // Toon vluchtgegevens voor vertrekkende vluchten
            }
            else if (checkUpcoming.IsChecked == true)
            {
                ShowArrivingFlightData(); // Toon vluchtgegevens voor aankomende vluchten
            }
        }

        private void SetData(int id)
        {
            Company company = CompanyRepository.GetCompanyById(id); // Haal de maatschappij gegevens op aan de hand van het ID
            if (company != null)
            {
                // Vul de tekstvelden in met de gegevens van de luchthaven
                txtCompanyName.Text = company.Naam;
            }
        }

        private void ShowAllFlightData()
        {
            lbCheck.Content = "Upcoming & Departing Flights";
            if (datagridCompany.SelectedItem != null)
            {
                lbCheck.Content = $"Upcoming & Departing Flights for {((Company)datagridCompany.SelectedItem).Naam}"; // Stel de inhoud van een label in op "Aankomende & Vertrekkende Vluchten"
                // Haal vluchten op die vertrekken vanaf de geselecteerde luchthaven
                var DepartureVluchten = CompanyRepository.GetFlightsByAirportDepartureLocation(((Company)datagridCompany.SelectedItem).Id);

                // Haal vluchten op die aankomen op de geselecteerde luchthaven
                var ArrivalVluchten = CompanyRepository.GetFlightsByAirportArrivalLocation(((Company)datagridCompany.SelectedItem).Id);

                // Maak een lijst van vertrekvluchtgegevens
                var departureInfo = DepartureVluchten.Select(vlucht => vlucht.FlightDepartureInfo());

                // Maak een lijst van aankomstvluchtgegevens
                var arrivalInfo = ArrivalVluchten.Select(vlucht => vlucht.FlightArrivalInfo());

                // Stel de gegevensbron voor het vluchtgegevens-datagrid in op de gecombineerde lijst van vertrek- en aankomstvluchtgegevens
                FlightData.ItemsSource = departureInfo.Concat(arrivalInfo);
            }
        }

        private void ShowArrivingFlightData()
        {
            checkDeparture.IsChecked = false; // Schakel het selectievakje voor vertrekkende vluchten uit
            lbCheck.Content = "Arriving Flights";
            if (datagridCompany.SelectedItem != null)
            {
                lbCheck.Content = $"Arriving Flights for {((Company)datagridCompany.SelectedItem).Naam}"; // Stel de inhoud van een label in op "Aankomende Vluchten"
                // Haal vluchten op die aankomen op de geselecteerde luchthaven
                var ArrivalVluchten = CompanyRepository.GetFlightsByAirportArrivalLocation(((Company)datagridCompany.SelectedItem).Id);
                // Stel de gegevensbron voor het vluchtgegevens-datagrid in op de lijst van aankomstvluchtgegevens
                FlightData.ItemsSource = ArrivalVluchten.Select(vlucht => vlucht.FlightArrivalInfo());
            }
        }

        private void ShowDepartingFlightData()
        {
            checkUpcoming.IsChecked = false; // Schakel het selectievakje voor aankomende vluchten uit
            lbCheck.Content = "Departure Flights";
            if (datagridCompany.SelectedItem != null)
            {
                lbCheck.Content = $"Departure Flights for {((Company)datagridCompany.SelectedItem).Naam}"; // Stel de inhoud van een label in op "Vertrekkende Vluchten"
                // Haal vluchten op die vertrekken vanaf de geselecteerde luchthaven
                var DepartureVluchten = CompanyRepository.GetFlightsByAirportDepartureLocation(((Company)datagridCompany.SelectedItem).Id);
                // Stel de gegevensbron voor het vluchtgegevens-datagrid in op de lijst van vertrekvluchtgegevens
                FlightData.ItemsSource = DepartureVluchten.Select(vlucht => vlucht.FlightDepartureInfo());
            }
        }
    }
}