using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using DreamScape.Data;
using DreamScape.Pages;

namespace DreamScape
{
    public sealed partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();

            NavigationService.Initialize(MainFrame); 
            NavigationService.NavigateTo(typeof(Pages.LoginPage));

            //using (var db = new AppDbContext())
            //{
            //    db.Database.EnsureDeleted();
            //    db.Database.EnsureCreated();
            //}
        }
    }
}
