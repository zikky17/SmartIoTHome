using SharedResources.Handlers;
using SmartFan.ViewModels;
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
            
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=e677eda9-6bf4-48b9-83da-60785bf972e6;SharedAccessKey=ANy2Thlvr2L0/I+mj5RTUjsIUKvCXtSoVAIoTAeZDmY=";
                var dc = new DeviceClientHandler("e677eda9-6bf4-48b9-83da-60785bf972e6", "SmartFan", "Fan", connectionString);
                var disconnectResult = dc.Disconnect();
            };

            Environment.Exit(0);
        }

        private void TopWindowBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}