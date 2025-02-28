using DreamScape.Data;
using DreamScape.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DreamScape.Pages
{
    public sealed partial class MainPage : Page
    {
        private List<Item> allUserItems = new List<Item>();

        public MainPage()
        {
            this.InitializeComponent();
            LoadUserItems();
        }

        private void LoadUserItems()
        {
            int? userId = AuthService.CurrentUserId;

            if (userId == null)
            {
                NavigationService.NavigateTo(typeof(LoginPage));
                return;
            }

            using (var db = new AppDbContext())
            {
                allUserItems = db.UserItems
                                 .Where(ui => ui.UserId == userId)
                                 .Select(ui => ui.Item)
                                 .ToList();
            }

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string searchQuery = SearchBox.Text.ToLower();
            string selectedType = (TypeFilterBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string selectedRarity = (RarityFilterBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filteredItems = allUserItems.Where(item =>
                (string.IsNullOrEmpty(searchQuery) ||
                 item.Name.ToLower().Contains(searchQuery) ||
                 item.Description.ToLower().Contains(searchQuery)) &&
                (selectedType == "Alle" || string.IsNullOrEmpty(selectedType) || item.Type == selectedType) &&
                (selectedRarity == "Alle" || string.IsNullOrEmpty(selectedRarity) || item.Rarity == selectedRarity)
            ).ToList();

            UserItemsListView.ItemsSource = filteredItems;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthService.Logout();
            ShowMessage("U bent uitgelogd!");
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

        private async void UserItemsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (Item)e.ClickedItem;

            ContentDialog itemDetailsDialog = new ContentDialog()
            {
                Title = clickedItem.Name,
                Content = $"Beschrijving: {clickedItem.Description}\n" +
                          $"Type: {clickedItem.Type}\n" +
                          $"Zeldzaamheid: {clickedItem.Rarity}\n" +
                          $"Power: {clickedItem.Power}\n" +
                          $"Speed: {clickedItem.Speed}\n" +
                          $"Durability: {clickedItem.Durability}\n" +
                          $"Magische Eigenschappen: {clickedItem.MagicalProperties}",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await itemDetailsDialog.ShowAsync();
        }

        private async void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            int? userId = AuthService.CurrentUserId;

            if (userId == null)
            {
                NavigationService.NavigateTo(typeof(LoginPage));
                return;
            }

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    ShowMessage("Gebruiker niet gevonden.");
                    return;
                }

                var usernameTextBox = new TextBox { Text = user.Username, PlaceholderText = "Gebruikersnaam" };
                var emailTextBox = new TextBox { Text = user.Email, PlaceholderText = "E-mail" };
                var passwordBox = new PasswordBox { PlaceholderText = "Nieuw Wachtwoord (optioneel)" };
                var confirmPasswordBox = new PasswordBox { PlaceholderText = "Herhaal Wachtwoord" };

                var dialog = new ContentDialog
                {
                    Title = "Mijn Profiel",
                    PrimaryButtonText = "Terug",
                    SecondaryButtonText = "Bewerken",
                    Content = new StackPanel
                    {
                        Children = { usernameTextBox, emailTextBox, passwordBox, confirmPasswordBox }
                    },
                    XamlRoot = this.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    string newUsername = usernameTextBox.Text.Trim();
                    string newEmail = emailTextBox.Text.Trim();
                    string newPassword = passwordBox.Password;
                    string confirmPassword = confirmPasswordBox.Password;

                    if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newEmail))
                        {
                            ShowMessage("Alle velden moeten ingevuld zijn.");
                            return;
                        }

                    if (!ValidationService.IsValidEmail(newEmail))
                    {
                        ShowMessage("Voer een geldig e-mailadres in.");
                        return;
                    }

                    if (ValidationService.IsUsernameTaken(newUsername, userId))
                    {
                        ShowMessage($"De gebruikersnaam '{newUsername}' is al in gebruik. Kies een andere gebruikersnaam.");
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(newPassword) || !string.IsNullOrWhiteSpace(confirmPassword))
                    {
                        if (!ValidationService.ArePasswordsMatching(newPassword, confirmPassword))
                        {
                            ShowMessage("De wachtwoorden komen niet overeen.");
                            return;
                        }

                        if (!ValidationService.IsValidPassword(newPassword))
                        {
                            ShowMessage("Het wachtwoord moet minimaal 8 tekens lang zijn en een combinatie van letters en cijfers bevatten.");
                            return;
                        }

                        user.Password = newPassword;
                    }


                    user.Username = newUsername;
                    user.Email = newEmail;
                    user.Password = newPassword;

                    db.SaveChanges();

                    ShowMessage("Profiel succesvol bijgewerkt!");
                }
            }
        }


    }
}
