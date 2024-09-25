using Microsoft.Azure.Devices.Shared;
using SharedResources.Handlers;
using System.Configuration;
using System.Diagnostics;
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

        public DeviceTwinHandler _twinHandler = new("HostName=gurra-iothub.azure-devices.net;DeviceId=cabb9896-0fba-47d2-b67d-0279a9745284;SharedAccessKey=ZY2h+rdNJIKDCWG39rJtofVgQYpNfeL0buMulj4Ml9A=");

        private async Task InitializeTwinHandlerAsync()
        {
            try
            {
                await _twinHandler.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged);
                Debug.WriteLine("Desired property callback registered successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to register desired property callback: {ex.Message}");
            }

            await UpdateAnimationAsync();
        }

        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            Debug.WriteLine("Desired properties changed.");

            if (desiredProperties.Contains("deviceState"))
            {
                bool deviceState = desiredProperties["deviceState"];
                Debug.WriteLine($"Device State changed to: {deviceState}");

                if (deviceState)
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

        public async Task UpdateAnimationAsync()
        {
            var twin = await _twinHandler.GetDeviceTwinAsync();
            if (twin != null && twin.Properties.Reported.Contains("deviceState"))
            {
                bool deviceState = twin.Properties.Reported["deviceState"];
                if (deviceState)
                {
                    StartDeviceAnimation();
                }
                else
                {
                    StopDeviceAnimation();
                }
            }
        }

        public void StartDeviceAnimation()
        {
                var storyBoard = (BeginStoryboard)TryFindResource("rotate-sb");
                storyBoard.Storyboard.Begin();
        }

        public void StopDeviceAnimation()
        {
                var storyBoard = (BeginStoryboard)TryFindResource("rotate-sb");
                storyBoard.Storyboard.Stop();
        }
    }
}
