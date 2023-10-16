using Data_Management_Flights_dal.Repositories;
using Data_Management_Flights_models;
using Data_Management_Flights_dal;
using Microsoft.Win32;
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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private LoginWindow loginWindow = new LoginWindow();
        private UserRepository userRepository = new UserRepository();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // Leegmaken van alle tekstvelden
            txtEmail.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtConfirmPassword.Password = string.Empty;
            txtPasswordUnmask.Text = string.Empty;
            txtConfirmPasswordUnmask.Text = string.Empty;
            txtName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtPhone.Text = string.Empty;
            dpBirthDate.Text = string.Empty;
            txtCountry.Text = string.Empty;
            txtZipcode.Text = string.Empty;
            txtGiveIdEmployee.Text = string.Empty;
            txtHouseNumber.Text = string.Empty;
            txtStreet.Text = string.Empty;
        }

        private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Openen van het LoginWindow
            loginWindow.Show();
            this.Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void CheckHidePassword_Checked(object sender, RoutedEventArgs e)
        {
            // Wachtwoord tonen
            txtPasswordUnmask.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Hidden;
            txtPasswordUnmask.Text = txtPassword.Password;

            // Bevestigingswachtwoord tonen
            txtConfirmPasswordUnmask.Visibility = Visibility.Visible;
            txtConfirmPassword.Visibility = Visibility.Hidden;
            txtConfirmPasswordUnmask.Text = txtConfirmPassword.Password;
        }

        private void CheckHidePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            // Wachtwoord verbergen
            txtPasswordUnmask.Visibility = Visibility.Hidden;
            txtPassword.Visibility = Visibility.Visible;
            txtPassword.Password = txtPasswordUnmask.Text;

            // Bevestigingswachtwoord verbergen
            txtConfirmPasswordUnmask.Visibility = Visibility.Hidden;
            txtConfirmPassword.Visibility = Visibility.Visible;
            txtConfirmPassword.Password = txtConfirmPasswordUnmask.Text;
        }

        // Valideren van de gebruiker en opslaan in de database
        private void Save()
        {
            try
            {
                ValidatePassword();
                UserFileHandler.SearchCode(txtGiveIdEmployee.Text);
                User user = new User(txtGiveIdEmployee.Text,
                                 txtPassword.Password,
                                 txtName.Text,
                                 txtLastName.Text,
                                 txtHouseNumber.Text,
                                 txtStreet.Text,
                                 txtZipcode.Text,
                                 txtPhone.Text,
                                 txtCountry.Text,
                                 txtEmail.Text,
                                 dpBirthDate.SelectedDate ?? DateTime.MinValue
                                 );

                if (user.IsValid())
                {
                    userRepository.InsertUser(user);
                    UserFileHandler.RemoveCode(txtGiveIdEmployee.Text);
                    MessageBox.Show("Registration Succesfull!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                    loginWindow.Show();
                }
                else
                {
                    MessageBox.Show(user.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Valideren of het wachtwoord overeenkomt met het bevestigingswachtwoord
        private void ValidatePassword()
        {
            if (txtPassword.Password != txtConfirmPassword.Password || txtPasswordUnmask.Text != txtConfirmPasswordUnmask.Text)
            {
                throw new Exception("Password does not match.");
            }
        }
    }
}