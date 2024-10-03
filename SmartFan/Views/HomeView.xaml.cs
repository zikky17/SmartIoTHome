using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Azure.Devices.Shared;
using SharedResources.Handlers;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SmartFan.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();       
        }
    }
}
