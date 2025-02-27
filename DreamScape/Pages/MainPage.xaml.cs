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
            //test123
        }
    }
}
