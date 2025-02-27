using DreamScape.Data;
using DreamScape.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DreamScape.Pages
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowMessage("Alle velden moeten ingevuld zijn.");
                return;
            }

            if (!ValidationService.IsValidEmail(email))
            {
                ShowMessage("Voer een geldig e-mailadres in.");
                return;
            }

            if (ValidationService.IsUsernameTaken(username))
            {
                ShowMessage($"De gebruikersnaam '{username}' is al in gebruik. Kies een andere gebruikersnaam.");
                return;
            }

            if (!ValidationService.ArePasswordsMatching(password, confirmPassword))
            {
                ShowMessage("De wachtwoorden komen niet overeen.");
                return;
            }

            if (!ValidationService.IsValidPassword(password))
            {
                ShowMessage("Het wachtwoord moet minimaal 8 tekens lang zijn en een combinatie van letters en cijfers bevatten.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    Role = "Speler",
                    Password = password
                };

                db.Users.Add(newUser);
                db.SaveChanges();
            }

            ShowMessage("Registratie succesvol! Je kunt nu inloggen.");
            NavigationService.NavigateTo(typeof(LoginPage));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigateTo(typeof(LoginPage));
        }

        private void ShowMessage(string message)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Informatie",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }
}
