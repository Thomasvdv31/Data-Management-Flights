using Data_Management_Flights_dal.Interfaces;
using Data_Management_Flights_dal.Repositories;
using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for PilotWindow.xaml
    /// </summary>
    public partial class PilotWindow : UserControl
    {
        //repositories Ophalen
        private IPilot PilotRepository = new PilotRepository();

        public PilotWindow()
        {
            InitializeComponent();
            btnAddPilot.IsEnabled = false;
            btnEditPilot.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void AddOrEditPilot(bool isInsert)
        {
            // Maak een nieuwe piloot instantie op basis van de ingevoerde gegevens

            Pilot pilot = new Pilot(txtPilotLastname.Text, txtPilotName.Text, txtBirthdate.SelectedDate ?? DateTime.MinValue);

            if (pilot.IsValid())
            {
                if (isInsert)
                {
                    // Voeg de piloot toe aan de repository
                    PilotRepository.InsertPilot(pilot);
                    datagridPilots.ItemsSource = PilotRepository.GetPilots(txtSearchName.Text, txtSearchLastName.Text);
                    ClearFields();
                    MessageBox.Show("Pilot successfully Added.", "Succes", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    // Haal het ID op van de geselecteerde piloot in het datagrid.
                    // Het "?."-gedeelte zorgt ervoor dat er geen uitzondering optreedt als er geen piloot is geselecteerd.
                    int? id = ((Pilot)datagridPilots.SelectedItem)?.Id;
                    if (id != null) // Controleer of het ID niet null is, wat betekent dat er een piloot is geselecteerd.
                    {
                        // Maak een nieuwe piloot instantie met bijgewerkte gegevens
                        Pilot pilot2 = new Pilot(txtPilotLastname.Text, txtPilotName.Text, txtBirthdate.SelectedDate ?? DateTime.MinValue)
                        {
                            // Het "?."-gedeelte zorgt ervoor dat de property "Id" wordt bijgewerkt met het ID van de geselecteerde piloot.
                            Id = (int)id
                        };

                        // Werk piloot bij in de repository
                        PilotRepository.EditePilot(pilot2);
                        datagridPilots.ItemsSource = PilotRepository.GetPilots(txtSearchName.Text, txtSearchLastName.Text);
                        MessageBox.Show("Pilot successfully updated.", "Succes", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                        /* Zoek het bijgewerkte piloot-item in de datagrid opnieuw.
                           gebruik van LINQ's FirstOrDefault-methode om de eerste overeenkomende
                           piloot te vinden op basis van de Id-eigenschap. Dit verbetert de efficiëntie*/
                        var SelectedPilot = datagridPilots.Items.Cast<Pilot>().FirstOrDefault(ap => ap.Id == pilot2.Id);
                        if (SelectedPilot != null)
                        {
                            // Als er een overeenkomend item is gevonden, stellen we het in als het geselecteerde item
                            datagridPilots.SelectedItem = SelectedPilot;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select a pilot", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show(pilot.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //luchthaven toevoegen
        private void btnAddPilot_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditPilot(true);
        }

        //velden leegmaken
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        // Verwijder Luchthaven
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (datagridPilots.SelectedItem != null)
            {
                if (!PilotRepository.PilotExists(((Pilot)datagridPilots.SelectedItem).Id))
                {
                    if (MessageBox.Show("Are you sure ?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // Verwijder piloot
                        if (PilotRepository.DeletePilot(((Pilot)datagridPilots.SelectedItem).Id))
                        {
                            MessageBox.Show("Pilot is deleted.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                            ClearFields();
                            btnEditPilot.IsEnabled = false;
                            btnDelete.IsEnabled = false;
                            datagridPilots.ItemsSource = PilotRepository.GetPilots(txtSearchName.Text, txtSearchLastName.Text);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Pilots that are registered for flights cannot be deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a Pilot.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Bewerk Luchthaven
        private void btnEditPilot_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditPilot(false);
        }

        // Methode voor het ontvangen van luchthavens wanneer de bijbehorende knop wordt geklikt.
        private void btnReceivePilots_Click(object sender, RoutedEventArgs e)
        {
            btnEditPilot.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnAddPilot.IsEnabled = true;

            ClearFields();

            datagridPilots.ItemsSource = PilotRepository.GetPilots(txtSearchName.Text, txtSearchLastName.Text);
        }

        private void ClearFields()
        {
            txtPilotName.Text = string.Empty; // Wis de tekst in het tekstveld voor de piloot naam
            txtPilotLastname.Text = string.Empty;
            txtBirthdate.Text = string.Empty;
            datagridPilots.SelectedItem = null; // Maak de geselecteerde rij in het piloot-datagrid leeg
            FlightData.ItemsSource = null; // Wis de gegevensbron voor het piloot-datagrid
        }

        private void datagridPilots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datagridPilots.SelectedItem != null)
            {
                btnDelete.IsEnabled = true;
                btnEditPilot.IsEnabled = true;
                ShowPilotData();
                SetData(((Pilot)datagridPilots.SelectedItem).Id); // Haal de gegevens op voor de geselecteerde piloot
            }
        }

        private void SetData(int id)
        {
            Pilot pilot = PilotRepository.GetPilotById(id); // Haal de piloot gegevens op aan de hand van het ID
            if (pilot != null)
            {
                // Vul de tekstvelden in met de gegevens van de piloot
                txtPilotName.Text = pilot.Voornaam;
                txtPilotLastname.Text = pilot.Naam;
                txtBirthdate.Text = pilot.Geboortedatum?.ToString("dd/MM/yyyy");
            }
        }

        //Methode om de vluchtgegevens van een geselecteerde piloot weer te geven.
        private void ShowPilotData()
        {
            var pilotData = PilotRepository.GetPilotFlights(((Pilot)datagridPilots.SelectedItem).Id);
            FlightData.ItemsSource = pilotData.Select(pilot => pilot.flightdata());
        }
    }
}