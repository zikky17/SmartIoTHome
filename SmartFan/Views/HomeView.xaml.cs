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
            _twinHandler = new DeviceTwinHandler("HostName=gurra-iothub.azure-devices.net;DeviceId=cabb9896-0fba-47d2-b67d-0279a9745284;SharedAccessKey=ZY2h+rdNJIKDCWG39rJtofVgQYpNfeL0buMulj4Ml9A=");
            InitializeTwinHandlerAsync().ConfigureAwait(false);
        }

        private readonly DeviceTwinHandler _twinHandler;

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
