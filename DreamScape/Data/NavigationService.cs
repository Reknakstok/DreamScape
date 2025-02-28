using Microsoft.UI.Xaml.Controls;

namespace DreamScape.Data
{
    public static class NavigationService
    {
        private static Frame _mainFrame;

        public static void Initialize(Frame frame)
        {
            _mainFrame = frame;
        }

        public static void NavigateTo(System.Type pageType)
        {
            _mainFrame?.Navigate(pageType);
        }
    }
}
