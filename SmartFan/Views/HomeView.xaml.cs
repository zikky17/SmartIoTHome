using Microsoft.Azure.Devices.Shared;
using SharedResources.Handlers;
using System.Configuration;
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
            UpdateAnimationAsync().ConfigureAwait(false); 
        }

        private bool IsActive = false;
        public DeviceTwinHandler _twinHandler = new(ConfigurationManager.AppSettings["FanConnectionString"]!);

        private async Task InitializeTwinHandlerAsync()
        {
            await _twinHandler.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged);

            await UpdateAnimationAsync();
        }

        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            if (desiredProperties.Contains("deviceState"))
            {
                bool deviceState = desiredProperties["deviceState"];
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

        public async Task UpdateAnimationAsync()
        {
            var twin = await _twinHandler.GetDeviceTwinAsync();
            if (twin != null && twin.Properties.Reported.Contains("deviceState"))
            {
                bool deviceState = twin.Properties.Reported["deviceState"];
                if (deviceState)
                {
                    StopDeviceAnimation();
                }
                else
                {
                    StartDeviceAnimation();
                }
            }
        }

        public void StartDeviceAnimation()
        {
            if (!IsActive)
            {
                var storyBoard = (BeginStoryboard)TryFindResource("rotate-sb");
                IsActive = true;
                storyBoard.Storyboard.Begin();
            }
        }

        public void StopDeviceAnimation()
        {
            if (IsActive)
            {
                var storyBoard = (BeginStoryboard)TryFindResource("rotate-sb");
                IsActive = false;
                storyBoard.Storyboard.Stop();
            }
        }




    }
}
