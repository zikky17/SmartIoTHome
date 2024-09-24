using SharedResources.Handlers;
using SmartTemperature.ViewModels;
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

namespace SmartTemperature
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
                var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=798197a0-e07f-453c-92c5-9a2121a2d673;SharedAccessKey=KaJ+2FmYJPW2L2OVn84wX5fSC3hgVHCbfAIoTFAUVuA=";
                var dc = new DeviceClientHandler("798197a0-e07f-453c-92c5-9a2121a2d673", "SmartTemp", "Temp", connectionString);

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