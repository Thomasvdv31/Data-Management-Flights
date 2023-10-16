using Data_Management_Flights_dal.Repositories;
using Data_Management_Flights_wpf;
using Data_Management_Flights_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        // Instantieert een UserRepository-object om gebruikersgegevens te beheren
        private UserRepository userRepository = new UserRepository();

        public LoginWindow()
        {
            InitializeComponent();
        }

        // Wist de tekstvelden voor gebruikers-ID en wachtwoord
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtUserID.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtPasswordUnmask.Text = string.Empty;
        }

        // Opent het registratiescherm
        private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateFields(); // Valideert of alle vereiste velden zijn ingevuld
                // Haalt de gebruiker op aan de hand van de opgegeven gebruikers-ID
                User user = userRepository.OphalenUserViaUserId(txtUserID.Text);
                ValidateUser(user); // Valideert of de gebruiker bestaat en het juiste wachtwoord heeft
                // Opent het startscherm (homepage) met de ingelogde gebruiker
                Homepage homepage = new Homepage(user);
                homepage.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                // Toont een foutmelding als er een fout optreedt tijdens het inloggen
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Verbergt texfield en laat password box zien
        private void CheckHidePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPasswordUnmask.Visibility = Visibility.Hidden;
            txtPassword.Visibility = Visibility.Visible;
            // Kopieert de inhoud van het ongemaskeerde wachtwoord naar het gemaskeerde wachtwoord
            txtPassword.Password = txtPasswordUnmask.Text;
        }

        // Verbergt passwordbox en laat textfield zien
        private void CheckShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtPasswordUnmask.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Hidden;
            // Kopieert de inhoud van het gemaskeerde wachtwoord naar het ongemaskeerde wachtwoord
            txtPasswordUnmask.Text = txtPassword.Password;
        }

        private void ValidateFields()
        {
            // Gooit een uitzondering als zowel de gebruikers-ID als het wachtwoord ontbreken
            if ((txtPassword.Password == string.Empty && txtPasswordUnmask.Text == string.Empty) && txtUserID.Text == string.Empty)
            {
                throw new Exception("Please fill in your User ID and password");
            }
            // Gooit een uitzondering als de gebruikers-ID ontbreekt
            else if (txtUserID.Text == string.Empty)
            {
                throw new Exception("Please fill in your User ID.");
            }
            // Gooit een uitzondering als het wachtwoord ontbreekt
            else if (txtPassword.Password == string.Empty && txtPasswordUnmask.Text == string.Empty)
            {
                throw new Exception("Please fill in your password.");
            }
        }

        private void ValidateUser(User user)
        {
            // Gooit een uitzondering als het ingevoerde wachtwoord onjuist is (ongemaskeerd wachtwoord wordt gecontroleerd)
            if (checkShowPassword.IsChecked == true)
            {
                if (user.Password != txtPasswordUnmask.Text)
                {
                    throw new Exception("Wrong password");
                }
            }
            // Gooit een uitzondering als het ingevoerde wachtwoord onjuist is (gemaskeerd wachtwoord wordt gecontroleerd)
            else if (checkShowPassword.IsChecked == false)
            {
                if (user.Password != txtPassword.Password)
                {
                    throw new Exception("Wrong password");
                }
            }
            // Toont een melding bij een succesvolle aanmelding, inclusief de naam van de gebruiker
            MessageBox.Show($"Login Successful! Welcome {user.Name}", "success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}