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
            InitializeTwinHandlerAsync().ConfigureAwait(false);
        }

        private async Task InitializeTwinHandlerAsync()
        {
            try
            {
                // Skapa en instans av DeviceTwinHandler med rätt anslutningssträng
                var deviceTwinHandler = new DeviceTwinHandler("HostName=gurra-iothub.azure-devices.net;DeviceId=cabb9896-0fba-47d2-b67d-0279a9745284;SharedAccessKey=SULmlJ9u55cCjNBiC2wT6IsdzTyJPhS5t6J4RYjg4wM=");

                // Kontrollera att anslutningen lyckas
                bool isConnected = await deviceTwinHandler.InitializeConnectionAsync();
                if (isConnected)
                {
                    // Registrera callback för desired properties när anslutningen är klar
                    await deviceTwinHandler.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged);
                    Debug.WriteLine("Desired property callback registered successfully.");
                }
                else
                {
                    Debug.WriteLine("Failed to connect to IoT Hub.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to register desired property callback: {ex.Message}");
            }
        }
        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            Debug.WriteLine("Desired properties changed.");

            if (desiredProperties.Contains("deviceState"))
            {
                string deviceState = desiredProperties["deviceState"];
                Debug.WriteLine($"Device State changed to: {deviceState}");

                if (deviceState == "true")
                {
                    StartDeviceAnimation();
                }
                else
                {
                    StopDeviceAnimation();
                }
            }
            else
            {
                Debug.WriteLine("deviceState not found in desired properties.");
            }
        }

        public void StartDeviceAnimation()
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var storyBoard = (BeginStoryboard)TryFindResource("rotate-sb");
                    storyBoard.Storyboard.Begin();
                }));
            }
            catch (InvalidOperationException)
            {
                Debug.WriteLine("rotate-sb resource not found.");
            }
        }

        public void StopDeviceAnimation()
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var storyBoard = (BeginStoryboard)TryFindResource("rotate-sb");
                    storyBoard.Storyboard.Stop();
                }));
            }
            catch (InvalidOperationException)
            {
                Debug.WriteLine("rotate-sb resource not found.");
            }
        }
    }
}
