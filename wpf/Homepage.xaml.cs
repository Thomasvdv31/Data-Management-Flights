using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using wpf;

namespace Data_Management_Flights_wpf
{
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : Window
    {
        private User currentUser;

        public Homepage(User user)
        {
            InitializeComponent();

            currentUser = user; // Stelt de huidige gebruiker in
            lbCurrenUser.Content = "Current User: " + currentUser.Name; // Toont de naam van de huidige gebruiker in het label
        }

        public void SetActiveUserControl(UserControl userControl)
        {
            // Verbergt het vorige actieve gebruikersbesturingselement (bijv. Airport)
            Airport.Visibility = Visibility.Collapsed;
            Company.Visibility = Visibility.Collapsed;
            Pilot.Visibility = Visibility.Collapsed;
            Flight.Visibility = Visibility.Collapsed;

            // Toont het geselecteerde gebruikersbesturingselement
            userControl.Visibility = Visibility.Visible;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selectedItem = listBox.SelectedItem as ListBoxItem;
            if (selectedItem != null)
            {
                string selectedItemContent = selectedItem.Content.ToString();

                // Schakelt op basis van het geselecteerde item
                switch (selectedItemContent)
                {
                    case "Airports": // Activeert het Airport-gebruikersbesturingselement
                        //SetActiveUserControl(Airport);
                        AirportWindow airportWindow = new AirportWindow();
                        gridContainer.Children.Add(airportWindow);
                        break;

                    case "Airline Companies":
                        //SetActiveUserControl(Company);
                        CompanyWindow companyWindow = new CompanyWindow();
                        gridContainer.Children.Add(companyWindow);
                        break;

                    case "Pilots":
                        //SetActiveUserControl(Pilots);
                        PilotWindow pilotWindow = new PilotWindow();
                        gridContainer.Children.Add(pilotWindow);
                        break;

                    case "Flights":
                        FlightWindow flightWindow = new FlightWindow();
                        gridContainer.Children.Add(flightWindow);
                        break;

                    default:
                        break;
                }
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Maakt een nieuw inlogvenster aan
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
            // Toont een melding dat de gebruiker succesvol is uitgelogd
            MessageBox.Show("Logged out successfully!");
        }
    }
}