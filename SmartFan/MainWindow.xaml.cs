using SharedResources.Handlers;
using SmartFan.ViewModels;
using System.Configuration;
using System.Windows;
using System.Windows.Input;

namespace SmartFan
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
                var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=cabb9896-0fba-47d2-b67d-0279a9745284;SharedAccessKey=ZY2h+rdNJIKDCWG39rJtofVgQYpNfeL0buMulj4Ml9A=";
                var dc = new DeviceClientHandler("cabb9896-0fba-47d2-b67d-0279a9745284", "SmartFan", "Fan", connectionString);

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