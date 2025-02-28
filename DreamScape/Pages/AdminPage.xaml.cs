using DreamScape.Data;
using DreamScape.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
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
            LoadItems();
        }

        private void LoadUsers()
        {
            using (var db = new AppDbContext())
            {
                UsersListView.ItemsSource = db.Users.ToList();
            }
        }
        private void LoadItems()
        {
            using (var db = new AppDbContext())
            {
                ItemsListView.ItemsSource = db.Items.ToList();
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

        private async void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var nameTextBox = new TextBox { PlaceholderText = "Naam" };
            var descriptionTextBox = new TextBox { PlaceholderText = "Beschrijving" };
            var typeComboBox = new ComboBox { ItemsSource = new[] { "Accesoire", "Armor", "Wapen" }, SelectedIndex = 0 };
            var rarityComboBox = new ComboBox { ItemsSource = new[] { "Zeldzaam", "Episch", "Legendarisch" }, SelectedIndex = 0 };

            var powerSlider = new Slider { Minimum = 0, Maximum = 100, Value = 0, Header = "Kracht" };
            var speedSlider = new Slider { Minimum = 0, Maximum = 100, Value = 0, Header = "Snelheid" };
            var durabilitySlider = new Slider { Minimum = 0, Maximum = 100, Value = 0, Header = "Duurzaamheid" };

            var magicalPropertiesTextBox = new TextBox { PlaceholderText = "Magische Eigenschappen" };

            var dialog = new ContentDialog
            {
                Title = "Nieuw Item Toevoegen",
                PrimaryButtonText = "Opslaan",
                SecondaryButtonText = "Annuleren",
                Content = new StackPanel
                {
                    Children = { nameTextBox, descriptionTextBox, typeComboBox, rarityComboBox, powerSlider, speedSlider, durabilitySlider, magicalPropertiesTextBox }
                },
                XamlRoot = this.XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                string name = nameTextBox.Text.Trim();
                string description = descriptionTextBox.Text.Trim();
                string type = typeComboBox.SelectedItem?.ToString();
                string rarity = rarityComboBox.SelectedItem?.ToString();
                string magicalProperties = magicalPropertiesTextBox.Text.Trim();

                int power = (int)powerSlider.Value;
                int speed = (int)speedSlider.Value;
                int durability = (int)durabilitySlider.Value;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(rarity))
                {
                    ShowMessage("Alle velden moeten ingevuld worden.");
                    return;
                }

                using (var db = new AppDbContext())
                {
                    db.Items.Add(new Item
                    {
                        Name = name,
                        Description = description,
                        Type = type,
                        Rarity = rarity,
                        Power = power,
                        Speed = speed,
                        Durability = durability,
                        MagicalProperties = magicalProperties
                    });
                    db.SaveChanges();
                }

                LoadItems();
            }
        }


        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsListView.SelectedItem is Item selectedItem)
            {
                var nameTextBox = new TextBox { Text = selectedItem.Name, PlaceholderText = "Naam" };
                var descriptionTextBox = new TextBox { Text = selectedItem.Description, PlaceholderText = "Beschrijving" };
                var magicalPropertiesTextBox = new TextBox { Text = selectedItem.MagicalProperties, PlaceholderText = "Magische Eigenschappen" };

                var typeComboBox = new ComboBox { ItemsSource = new[] { "Accesoire", "Armor", "Wapen" }, SelectedItem = selectedItem.Type };
                var rarityComboBox = new ComboBox { ItemsSource = new[] { "Zeldzaam", "Episch", "Legendarisch" }, SelectedItem = selectedItem.Rarity };

                var powerSlider = new Slider { Minimum = 0, Maximum = 100, Value = selectedItem.Power, Header = "Kracht" };
                var speedSlider = new Slider { Minimum = 0, Maximum = 100, Value = selectedItem.Speed, Header = "Snelheid" };
                var durabilitySlider = new Slider { Minimum = 0, Maximum = 100, Value = selectedItem.Durability, Header = "Duurzaamheid" };

                var dialog = new ContentDialog
                {
                    Title = "Item Bewerken",
                    PrimaryButtonText = "Opslaan",
                    SecondaryButtonText = "Annuleren",
                    Content = new StackPanel
                    {
                        Children =
                {
                    nameTextBox,
                    descriptionTextBox,
                    typeComboBox,
                    rarityComboBox,
                    powerSlider,
                    speedSlider,
                    durabilitySlider,
                    magicalPropertiesTextBox
                }
                    },
                    XamlRoot = this.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    int power = (int)powerSlider.Value;
                    int speed = (int)speedSlider.Value;
                    int durability = (int)durabilitySlider.Value;

                    selectedItem.Name = nameTextBox.Text;
                    selectedItem.Description = descriptionTextBox.Text;
                    selectedItem.Type = typeComboBox.SelectedItem?.ToString();
                    selectedItem.Rarity = rarityComboBox.SelectedItem?.ToString();
                    selectedItem.Power = power;
                    selectedItem.Speed = speed;
                    selectedItem.Durability = durability;
                    selectedItem.MagicalProperties = magicalPropertiesTextBox.Text;

                    using (var db = new AppDbContext())
                    {
                        db.Items.Update(selectedItem);
                        db.SaveChanges();
                    }

                    LoadItems();
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsListView.SelectedItem is not Item selectedItem) return;

            var dialog = new ContentDialog
            {
                Title = "Verwijder Item",
                Content = $"Weet je zeker dat je {selectedItem.Name} wilt verwijderen?",
                PrimaryButtonText = "Verwijderen",
                SecondaryButtonText = "Annuleren",
                XamlRoot = this.XamlRoot
            };

            dialog.PrimaryButtonClick += (_, _) =>
            {
                using (var db = new AppDbContext())
                {
                    var item = db.Items.FirstOrDefault(u => u.Id == selectedItem.Id);
                    if (item != null)
                    {
                        db.Items.Remove(item);
                        db.SaveChanges();
                    }
                }
                LoadItems();
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
    }
}