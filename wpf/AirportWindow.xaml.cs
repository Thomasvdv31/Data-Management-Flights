using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_dal.Repositories;
using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Data_Management_Flights_wpf
{
    /// <summary>
    /// Interaction logic for AirportWindow.xaml
    /// </summary>
    public partial class AirportWindow : UserControl
    {
        // Repository instanties
        private IAirport AirportRepository = new AirportRepository();

        private IFlight FlightRepository = new FlightRepository();

        public AirportWindow()
        {
            InitializeComponent();
            // Initialiseer de knoppen als uitgeschakeld
            btnAddAirport.IsEnabled = false;
            btnEditAirport.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void AddOrEditAirport(bool isInsert)
        {
            // Maak een nieuwe Airport instantie op basis van de ingevoerde gegevens
            Airport airport = new Airport(txtAirporHouseNumber.Text,
                                            txtAirportIatacode.Text,
                                            txtAirportCountry.Text,
                                            txtAirportName.Text,
                                            txtAirportZipcode.Text,
                                            txtAirportStreet.Text);

            if (airport.IsValid())
            {
                if (isInsert)
                {
                    string error = validate();
                    if (string.IsNullOrWhiteSpace(error))
                    {
                        // Voeg de airport toe aan de repository
                        AirportRepository.InsertAirport(airport);
                        datagridAirports.ItemsSource = AirportRepository.GetAirports(txtSearchName.Text);
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Haal het ID op van de geselecteerde luchthaven in het datagrid.
                    // Het "?."-gedeelte zorgt ervoor dat er geen uitzondering optreedt als er geen luchthaven is geselecteerd.
                    int? id = ((Airport)datagridAirports.SelectedItem)?.Id;
                    if (id != null) // Controleer of het ID niet null is, wat betekent dat er een luchthaven is geselecteerd.
                    {
                        // Maak een nieuwe Airport instantie met bijgewerkte gegevens
                        Airport airport2 = new Airport(txtAirporHouseNumber.Text,
                                                       txtAirportIatacode.Text,
                                                       txtAirportCountry.Text,
                                                       txtAirportName.Text,
                                                       txtAirportZipcode.Text,
                                                       txtAirportStreet.Text)
                        {
                            // Het "?."-gedeelte zorgt ervoor dat de property "Id" wordt bijgewerkt met het ID van de geselecteerde luchthaven.
                            Id = (int)id
                        };

                        // Werk de airport bij in de repository
                        AirportRepository.EditeAirport(airport2);
                        datagridAirports.ItemsSource = AirportRepository.GetAirports(txtSearchName.Text);
                        MessageBox.Show("Airport successfully updated.", "Succes", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                        /* Zoek het bijgewerkte airport-item in de datagrid opnieuw.
                           gebruik van LINQ's FirstOrDefault-methode om de eerste overeenkomende
                           Airport te vinden op basis van de Id-eigenschap. Dit verbetert de efficiëntie*/
                        var SelectedAirport = datagridAirports.Items.Cast<Airport>().FirstOrDefault(ap => ap.Id == airport2.Id);
                        if (SelectedAirport != null)
                        {
                            // Als er een overeenkomend item is gevonden, stellen we het in als het geselecteerde item
                            datagridAirports.SelectedItem = SelectedAirport;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select an airport", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show(airport.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddAirport_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditAirport(true);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (datagridAirports.SelectedItem != null)
            {
                if (!AirportRepository.VluchtExists(((Airport)datagridAirports.SelectedItem).Id))
                {
                    if (MessageBox.Show("Are you sure ?", "Confirm Deletion", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        // Verwijder airport
                        if (AirportRepository.DeleteAirport(((Airport)datagridAirports.SelectedItem).Id))
                        {
                            MessageBox.Show("Airport is deleted.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                            ClearFields();
                            datagridAirports.ItemsSource = AirportRepository.GetAirports(txtSearchName.Text);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Airports that are registered for flights cannot be deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Select an Airport.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditAirport_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditAirport(false);
        }

        private void btnReceiveAirports_Click(object sender, RoutedEventArgs e)
        {
            btnEditAirport.IsEnabled = true;
            btnAddAirport.IsEnabled = true;
            btnDelete.IsEnabled = true;
            ClearFields();
            checkDeparture.IsChecked = false;
            checkUpcoming.IsChecked = false;

            if (string.IsNullOrWhiteSpace(txtSearchCountry.Text))
            {
                // Ontvang alle airports op basis van de ingevoerde naam
                datagridAirports.ItemsSource = AirportRepository.GetAirports(txtSearchName.Text);
            }
            else if (string.IsNullOrWhiteSpace(txtSearchName.Text))
            {
                // Ontvang alle airports op basis van het ingevoerde land
                datagridAirports.ItemsSource = AirportRepository.GetAirportsOnCountry(txtSearchCountry.Text);
            }
            else
            {
                // Ontvang alle airports op basis van zowel de ingevoerde naam als land
                datagridAirports.ItemsSource = AirportRepository.GetAirportsOnNameAndCountry(txtSearchName.Text, txtSearchCountry.Text);
            }
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
            txtAirportName.Text = string.Empty; // Wis de tekst in het tekstveld voor de luchthavennaam
            txtAirportCountry.Text = string.Empty; // Wis de tekst in het tekstveld voor het land van de luchthaven
            txtAirportStreet.Text = string.Empty; // Wis de tekst in het tekstveld voor de straatnaam van de luchthaven
            txtAirporHouseNumber.Text = string.Empty; // Wis de tekst in het tekstveld voor het huisnummer van de luchthaven
            txtAirportIatacode.Text = string.Empty; // Wis de tekst in het tekstveld voor de luchthaven-IATA-code
            txtAirportZipcode.Text = string.Empty; // Wis de tekst in het tekstveld voor de postcode van de luchthaven
            datagridAirports.SelectedItem = null; // Maak de geselecteerde rij in het luchthaven-datagrid leeg
            FlightData.ItemsSource = null; // Wis de gegevensbron voor het vluchtgegevens-datagrid
            txtAirportIatacode.IsEnabled = true; // Schakel het tekstveld voor de luchthaven-IATA-code in
        }

        private void datagridAirports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtAirportIatacode.IsEnabled = false; // Schakel het tekstveld voor de luchthaven-IATA-code uit
            if (datagridAirports.SelectedItem != null)
            {
                SetData(((Airport)datagridAirports.SelectedItem).Id); // Haal de gegevens op voor de geselecteerde luchthaven
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

        private void SetData(int employeeId)
        {
            Airport airport = AirportRepository.GetAirportById(employeeId); // Haal de luchthavengegevens op aan de hand van het ID

            if (airport != null)
            {
                // Vul de tekstvelden in met de gegevens van de luchthaven
                txtAirportName.Text = airport.Naam;
                txtAirportCountry.Text = airport.Land;
                txtAirportZipcode.Text = airport.Postcode;
                txtAirporHouseNumber.Text = airport.Huisnummer;
                txtAirportStreet.Text = airport.Straat;
                txtAirportIatacode.Text = airport.IataCode;
            }
        }

        private void ShowAllFlightData()
        {
            lbCheck.Content = "Upcoming & Departing Flights"; // Stel de inhoud van een label in op "Aankomende & Vertrekkende Vluchten"
            if (datagridAirports.SelectedItem != null)
            {
                // Haal vluchten op die vertrekken vanaf de geselecteerde luchthaven
                var DepartureVluchten = FlightRepository.GetFlightsByAirportDepartureLocation(((Airport)datagridAirports.SelectedItem).Id);

                // Haal vluchten op die aankomen op de geselecteerde luchthaven
                var ArrivalVluchten = FlightRepository.GetFlightsByAirportArrivalLocation(((Airport)datagridAirports.SelectedItem).Id);

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
            lbCheck.Content = "Upcoming Flights"; // Stel de inhoud van een label in op "Aankomende Vluchten"
            if (datagridAirports.SelectedItem != null)
            {
                // Haal vluchten op die aankomen op de geselecteerde luchthaven
                var vluchten = FlightRepository.GetFlightsByAirportArrivalLocation(((Airport)datagridAirports.SelectedItem).Id);
                // Stel de gegevensbron voor het vluchtgegevens-datagrid in op de lijst van aankomstvluchtgegevens
                FlightData.ItemsSource = vluchten.Select(vlucht => vlucht.FlightArrivalInfo());
            }
        }

        private void ShowDepartingFlightData()
        {
            checkUpcoming.IsChecked = false; // Schakel het selectievakje voor aankomende vluchten uit
            lbCheck.Content = "Departure Flights"; // Stel de inhoud van een label in op "Vertrekkende Vluchten"
            if (datagridAirports.SelectedItem != null)
            {
                // Haal vluchten op die vertrekken vanaf de geselecteerde luchthaven
                var vluchten = FlightRepository.GetFlightsByAirportDepartureLocation(((Airport)datagridAirports.SelectedItem).Id);
                // Stel de gegevensbron voor het vluchtgegevens-datagrid in op de lijst van vertrekvluchtgegevens
                FlightData.ItemsSource = vluchten.Select(vlucht => vlucht.FlightDepartureInfo());
            }
        }

        private string validate()
        {
            string error = "";
            // Voeg een foutmelding toe als de IATA-code al bestaat
            if (AirportRepository.IsIataCodeExists(txtAirportIatacode.Text))
            {
                error += "IataCode already exist.";
            }
            return error;
        }
    }
}