using DreamScape.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Globalization;

namespace DreamScape.Pages
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigateTo(typeof(RegisterPage));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var user = AuthService.Login(username, password);

            if (user != null)
            {
                if (user.Role == "Beheerder")
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "Naar welk dashboard wilt u gaan?",
                        Content = "Kies een dashboard om naar te gaan:",
                        PrimaryButtonText = "Beheerder Dashboard",
                        SecondaryButtonText = "Gebruiker Dashboard",
                        XamlRoot = this.XamlRoot
                    };
                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        NavigationService.NavigateTo(typeof(AdminPage));
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        NavigationService.NavigateTo(typeof(MainPage));
                    }
                }
                else
                {
                    NavigationService.NavigateTo(typeof(MainPage));
                }
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Fout",
                    Content = "Ongeldige gebruikersnaam of wachtwoord.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                _ = dialog.ShowAsync();
            }
        }
    }
}
