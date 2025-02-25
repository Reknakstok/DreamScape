using DreamScape.Data;
using DreamScape.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace DreamScape.Pages
{
    public sealed partial class MainPage : Page
    {
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
                var userItems = db.UserItems
                                   .Where(ui => ui.UserId == userId)
                                   .Select(ui => ui.Item)
                                   .ToList();

                UserItemsListView.ItemsSource = userItems;
            }
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
                          $"Rarity: {clickedItem.Rarity}\n" +
                          $"Power: {clickedItem.Power}\n" +
                          $"Speed: {clickedItem.Speed}\n" +
                          $"Durability: {clickedItem.Durability}\n" +
                          $"Magical Properties: {clickedItem.MagicalProperties}",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await itemDetailsDialog.ShowAsync();
        }
    }
}
