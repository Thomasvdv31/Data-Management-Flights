using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_dal.Repositories;
using Data_Management_Flights_models;
using MaterialDesignThemes.Wpf;
using System.Data.SqlTypes;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Data_Management_Flights_wpf
{
    /// <summary>
    /// Interaction logic for FlightWindow.xaml
    /// </summary>
    public partial class FlightWindow : UserControl
    {
        // Ophalen van de nodige repositories
        private IAirplane airplaneRepository = new AirplaneRepository();

        private IAirport airportRepository = new AirportRepository();
        private ICompany companyRepository = new CompanyRepository();
        private IFlight flightRepository = new FlightRepository();
        private IPilot pilotRepository = new PilotRepository();

        public FlightWindow()
        {
            InitializeComponent();
            GetData();
        }

        // Vlucht toevoegen
        private void btnAddFlight_Click(object sender, RoutedEventArgs e)
        {
            Save(true);
        }

        // Velden wissen
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        //Vlucht verwijderen
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbFlights.SelectedItem != null)
            {
                if (MessageBox.Show("Are you sure?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Delete Flight based on the current selected Vlucht Id
                    if (flightRepository.DeleteFlight((int)((Vlucht)lbFlights.SelectedItem).Id))
                    {
                        lbFlights.ItemsSource = flightRepository.GetAllFlights();
                        MessageBox.Show("Flight is deleted.", "Successful", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        ClearFields();
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a flight.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Vlucht bewerken
        private void btnEditFlight_Click(object sender, RoutedEventArgs e)
        {
            Save(false);
        }

        // Alle Vluchten ophalen
        private void btnReceiveFlight_Click(object sender, RoutedEventArgs e)
        {
            lbFlights.ItemsSource = flightRepository.GetAllFlights();
        }

        // Reset alle velden en selected vlucht
        private void ClearFields()
        {
            cbCompany.SelectedItem = null;
            cbAirplanes.SelectedItem = null;
            cbArrivalLocation.SelectedItem = null;
            cbDepatureLocation.SelectedItem = null;
            cbPilotName.SelectedItem = null;
            txtGate.Text = null;
            txtMoney.Text = null;
            dpArrivalDate.SelectedDate = null;
            tpArrivalTime.SelectedTime = null;
            dpDepatureDate.SelectedDate = null;
            tpDepatureTime.SelectedTime = null;
            dpActualArrivalDate.SelectedDate = null;
            tpActualArrivalTime.SelectedTime = null;
            lbFlights.SelectedItem = null;
            FlightData.Content = null;
            dpActualArrivalDate.FontSize = 10;
            tpActualArrivalTime.FontSize = 10;
        }

        //combineert een geselecteerde datum van een DatePicker met een geselecteerde tijd van een TimePicker en retourneert een DateTime-object.
        private DateTime? CombineDateAndTime(DatePicker datePicker, TimePicker timePicker)
        {
            // Haal de geselecteerde datum op uit de DatePicker
            DateTime? selectedDate = datePicker.SelectedDate;
            // Haal de geselecteerde tijd van het timePicker-besturingselement op. Als er geen tijd is geselecteerd,
            // wordt de waarde null toegewezen. De TimeOfDay-eigenschap van TimeSpan wordt gebruikt om de tijdcomponent
            // van de geselecteerde tijd op te halen. Als de geselecteerde tijd null is, wordt TimeSpan.Zero gebruikt als standaardwaarde.
            TimeSpan selectedTime = timePicker.SelectedTime?.TimeOfDay ?? TimeSpan.Zero;

            if (selectedDate.HasValue)
            {
                // Combineer de geselecteerde datum en tijd tot een DateTime-object
                return new DateTime(
                    selectedDate.Value.Year,
                    selectedDate.Value.Month,
                    selectedDate.Value.Day,
                    selectedTime.Hours,
                    selectedTime.Minutes,
                    selectedTime.Seconds);
            }

            return null;
        }

        // Event handler om passagiersinformatie weer te geven en formuliergegevens in te vullen wanneer een vlucht is geselecteerd in de datagridFlights.
        private void datagridFlights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = lbFlights.SelectedItem;

            if (selectedItem is Vlucht)
            {
                var vlucht = (Vlucht)selectedItem;
                FlightData.Content = vlucht.PassengerInfo();

                SetData((int)((Vlucht)lbFlights.SelectedItem).Id);
            }
        }

        //Haalt de benodigde gegevens op om de dropdownlijsten in het formulier te vullen.
        // De "Id" van elke optie wordt gebruikt als de waarde van de geselecteerde optie.
        private void GetData()
        {
            cbCompany.ItemsSource = companyRepository.GetAllCompanyNames();
            cbCompany.SelectedValuePath = "Id";
            cbAirplanes.ItemsSource = airplaneRepository.GetAllAirplaneModels();
            cbAirplanes.SelectedValuePath = "Id";
            cbArrivalLocation.ItemsSource = airportRepository.GetAllAirportNames();
            cbArrivalLocation.SelectedValuePath = "Id";
            cbDepatureLocation.ItemsSource = airportRepository.GetAllAirportNames();
            cbDepatureLocation.SelectedValuePath = "Id";
            cbPilotName.ItemsSource = pilotRepository.GetAllPilotNames();
            cbPilotName.SelectedValuePath = "Id";
        }

        private void Save(bool isInsert)
        {
            // Controleert of een item is geselecteerd in de lbFlights ListBox.
            // En of dat de 'isInsert' variabele waar of onwaar is.
            if (lbFlights.SelectedItem != null || isInsert)
            {
                try
                {
                    ValidateValues();
                    // Parse de waarde van txtGate.Text naar een integer (int) en wijs deze toe aan de variabele parsedGate.
                    // Als txtGate.Text leeg is of alleen spaties bevat, wordt parsedGate toegewezen aan null (int?null).
                    // Dit wordt bereikt met behulp van ternary operator.
                    int? parsedGate = string.IsNullOrWhiteSpace(txtGate.Text) ? (int?)null : int.Parse(txtGate.Text);

                    //Zelfde principe voor de waarde txtPrice.Text
                    decimal? parsedPrice = string.IsNullOrWhiteSpace(txtMoney.Text) ? (decimal?)null : decimal.Parse(txtMoney.Text);

                    // Combineer Datum en Tijd
                    DateTime? combinedArrivalDateTime = CombineDateAndTime(dpArrivalDate, tpArrivalTime);
                    DateTime? combinedDepatureDateTime = CombineDateAndTime(dpDepatureDate, tpDepatureTime);
                    DateTime? combinedActualArrivalDateTime = CombineDateAndTime(dpActualArrivalDate, tpActualArrivalTime);

                    Vlucht flight = new Vlucht(((Vlucht)lbFlights.SelectedItem)?.Id,
                                               ((Airport)cbArrivalLocation.SelectedItem)?.Id,
                                               parsedGate,
                                               combinedArrivalDateTime,
                                               ((Company)cbCompany.SelectedItem)?.Id,
                                               ((Pilot)cbPilotName.SelectedItem)?.Id,
                                               parsedPrice,
                                               combinedDepatureDateTime,
                                               ((Airport)cbDepatureLocation.SelectedItem)?.Id,
                                               ((Airplane)cbAirplanes.SelectedItem)?.Id,
                                               combinedActualArrivalDateTime);

                    if (flight.IsValid())
                    {
                        if (isInsert)
                        {
                            flightRepository.InsertFlight(flight);
                            MessageBox.Show("Flight Succesfully Added.", "Successful", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        }
                        else
                        {
                            flightRepository.UpdateFlight(flight);
                            MessageBox.Show("Flight Succesfully Updated.", "Successful", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        }
                        lbFlights.ItemsSource = flightRepository.GetAllFlights();
                        GetData();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show(flight.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a flight", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Gegevens van een vlucht in te stellen op de overeenkomstige velden in de flight window.
        private void SetData(int flightId)
        {
            Vlucht vlucht = flightRepository.GetFlightOnId(flightId);

            if (vlucht != null)
            {
                cbCompany.SelectedValue = vlucht.MaatschappijId;
                cbAirplanes.SelectedValue = vlucht.VliegtuigId;
                cbArrivalLocation.SelectedValue = vlucht.AankomstLocatie;
                cbDepatureLocation.SelectedValue = vlucht.VertrekLocatie;
                cbPilotName.SelectedValue = vlucht.PilootId;
                txtGate.Text = vlucht.Gate.ToString();
                // Wijs de waarde van vlucht.Prijs toe aan de txtMoney TextBox-veld na conversie naar een numerieke tekst.
                txtMoney.Text = Conversions.ConvertMoneyToNumericText((decimal)vlucht.Prijs);
                dpArrivalDate.SelectedDate = vlucht.GeplandeAankomst;
                tpArrivalTime.SelectedTime = vlucht.GeplandeAankomst;
                dpDepatureDate.SelectedDate = vlucht.Vertrek;
                tpDepatureTime.SelectedTime = vlucht.Vertrek;
                dpActualArrivalDate.SelectedDate = vlucht.WerkelijkeAankomst;
                tpActualArrivalTime.SelectedTime = vlucht.WerkelijkeAankomst;
                if (vlucht.WerkelijkeAankomst != null)
                {
                    dpActualArrivalDate.FontSize = 15;
                    tpActualArrivalTime.FontSize = 15;
                }
                else
                {
                    dpActualArrivalDate.FontSize = 10;
                    tpActualArrivalTime.FontSize = 10;
                }
            }
        }

        // Valideert de ingevoerde waarden in de tekstvakken voor gate en prijs.
        // Als de ingevoerde waarde geen geldige numerieke waarde is, wordt er een uitzondering gegenereerd.
        private void ValidateValues()
        {
            if (!string.IsNullOrWhiteSpace(txtGate.Text) && !int.TryParse(txtGate.Text, out _))
            {
                throw new Exception("Invalid Gate. (Numeric value only)\n");
            }
            if (!string.IsNullOrWhiteSpace(txtMoney.Text) && !decimal.TryParse(txtMoney.Text, out _))
            {
                throw new Exception("Invalid Price. (Numeric value only)\n");
            }
        }
    }
}