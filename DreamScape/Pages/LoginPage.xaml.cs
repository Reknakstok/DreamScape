using DreamScape.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (AuthService.Login(username, password))
            {
                NavigationService.NavigateTo(typeof(MainPage));
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
