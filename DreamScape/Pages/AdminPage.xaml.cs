using DreamScape.Data;
using DreamScape.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace DreamScape.Pages
{
    public sealed partial class AdminPage : Page
    {
        public AdminPage()
        {
            this.InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var db = new AppDbContext())
            {
                UsersListView.ItemsSource = db.Users.ToList();
            }
        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var usernameTextBox = new TextBox { PlaceholderText = "Gebruikersnaam" };
            var emailTextBox = new TextBox { PlaceholderText = "E-mail" };
            var passwordBox = new PasswordBox { PlaceholderText = "Wachtwoord" };
            var confirmPasswordBox = new PasswordBox { PlaceholderText = "Wachtwoord herhalen" };
            var roleComboBox = new ComboBox { ItemsSource = new[] { "Speler", "Beheerder" }, SelectedIndex = 0 };

            var dialog = new ContentDialog
            {
                Title = "Nieuwe Gebruiker Toevoegen",
                PrimaryButtonText = "Opslaan",
                SecondaryButtonText = "Annuleren",
                Content = new StackPanel
                {
                    Children = { usernameTextBox, emailTextBox, passwordBox, confirmPasswordBox, roleComboBox }
                },
                XamlRoot = this.XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                string username = usernameTextBox.Text.Trim();
                string email = emailTextBox.Text.Trim();
                string password = passwordBox.Password;
                string confirmPassword = confirmPasswordBox.Password;
                string role = roleComboBox.SelectedItem?.ToString();

                if (!ValidateUserInput(username, email, out string errorMessage))
                {
                    ShowMessage(errorMessage);
                    return;
                }
                if (!ValidatePasswordInput(password, confirmPassword, out errorMessage))
                {
                    ShowMessage(errorMessage);
                    return;
                }

                using (var db = new AppDbContext())
                {
                    db.Users.Add(new User { Username = username, Email = email, Password = password, Role = role });
                    db.SaveChanges();
                }

                LoadUsers();
            }
        }

        private async void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is not User selectedUser) return;

            var usernameTextBox = new TextBox { Text = selectedUser.Username };
            var emailTextBox = new TextBox { Text = selectedUser.Email };
            var roleComboBox = new ComboBox { ItemsSource = new[] { "Speler", "Beheerder" }, SelectedItem = selectedUser.Role };

            var dialog = new ContentDialog
            {
                Title = "Gebruiker Bewerken",
                PrimaryButtonText = "Opslaan",
                SecondaryButtonText = "Annuleren",
                Content = new StackPanel
                {
                    Children = { usernameTextBox, emailTextBox, roleComboBox }
                },
                XamlRoot = this.XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                string username = usernameTextBox.Text.Trim();
                string email = emailTextBox.Text.Trim();
                string role = roleComboBox.SelectedItem?.ToString();

                if (!ValidateUserInput(username, email, out string errorMessage, selectedUser.Id))
                {
                    ShowMessage(errorMessage);
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Id == selectedUser.Id);
                    if (user != null)
                    {
                        user.Username = username;
                        user.Email = email;
                        user.Role = role;
                        db.SaveChanges();
                    }
                }

                LoadUsers();
            }
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is not User selectedUser) return;

            var dialog = new ContentDialog
            {
                Title = "Verwijder Gebruiker",
                Content = $"Weet je zeker dat je {selectedUser.Username} wilt verwijderen?",
                PrimaryButtonText = "Verwijderen",
                SecondaryButtonText = "Annuleren",
                XamlRoot = this.XamlRoot
            };

            dialog.PrimaryButtonClick += (_, _) =>
            {
                using (var db = new AppDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Id == selectedUser.Id);
                    if (user != null)
                    {
                        db.Users.Remove(user);
                        db.SaveChanges();
                    }
                }
                LoadUsers();
            };

            await dialog.ShowAsync();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthService.Logout();
            NavigationService.NavigateTo(typeof(LoginPage));
        }

        private async void ShowMessage(string message)
        {
            var messageDialog = new ContentDialog
            {
                Title = "Informatie",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await messageDialog.ShowAsync();
        }

        private bool ValidateUserInput(string username, string email, out string errorMessage, int? userId = null)
        {
            if (!ValidationService.IsValidEmail(email))
            {
                errorMessage = "Voer een geldig e-mailadres in.";
                return false;
            }

            if (ValidationService.IsUsernameTaken(username, userId))
            {
                errorMessage = $"De gebruikersnaam '{username}' is al in gebruik. Kies een andere gebruikersnaam.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private bool ValidatePasswordInput(string password, string confirmPassword, out string errorMessage)
        {
            if (!ValidationService.IsValidPassword(password))
            {
                errorMessage = "Het wachtwoord moet minimaal 8 tekens lang zijn en een combinatie van letters en cijfers bevatten.";
                return false;
            }
            if (!ValidationService.ArePasswordsMatching(password, confirmPassword))
            {
                errorMessage = "De wachtwoorden komen niet overeen.";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
    }
}