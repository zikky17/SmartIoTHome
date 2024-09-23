using SharedResources.Handlers;
using SmartLight.ViewModels;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            AppDomain.CurrentDomain.ProcessExit += async (s, e) =>
            {
                var connectionString = ConfigurationManager.AppSettings["LightConnectionString"]!;
                var dc = new DeviceClientHandler("f0468151-3b8b-4d92-8e42-cf679a27796f", "SmartLight", "Light", connectionString);

                await dc.DisconnectAsync(connectionString);
            };

            Application.Current.Shutdown();
        }

        private void TopWindowBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}