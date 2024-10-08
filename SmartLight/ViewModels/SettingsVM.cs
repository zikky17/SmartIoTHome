using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Data;
using SharedResources.Models;
using SmartLight.Models;
using System.Text;
using System.Windows;

namespace SmartLight.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabaseContext _databaseContext;

        public SmartLightModel SmartLightModel { get; set; }

        public DeviceSettings? DeviceSettings;
        public List<DeviceStateHistory>? DeviceHistory;


        public SettingsVM(IDatabaseContext databaseContext, IServiceProvider serviceProvider)
        {
            _databaseContext = databaseContext;
            _serviceProvider = serviceProvider;
            SmartLightModel = new SmartLightModel();
            LoadSettingsAsync().ConfigureAwait(false);

        }

        private async Task LoadSettingsAsync()
        {
            var result = await _databaseContext.GetSettingsAsync("f0468151-3b8b-4d92-8e42-cf679a27796f");
            if (result.Succeeded)
            {
                DeviceSettings = result.Content!;

                SmartLightModel.Id = DeviceSettings.Id;
                SmartLightModel.Location = DeviceSettings.Location!;
                SmartLightModel.Type = DeviceSettings.Type!;
                SmartLightModel.ConnectionString = DeviceSettings.ConnectionString!;
                SmartLightModel.DeviceState = DeviceSettings.DeviceState.ToString();
                SmartLightModel.DeviceState = SmartLightModel.DeviceState == "False" ? "Off" : "On";
                SmartLightModel.HasSettings = true;
            }
        }

        [RelayCommand]
        private async Task ResetSettings(DeviceSettings device)
        {
            SmartLightModel.HasSettings = false;
            var result = await _databaseContext.DeleteDeviceSettingsAsync(device);
            if (result.Succeeded)
            {
                var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
                mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsVM>();
            }
        }

        [RelayCommand]
        private async Task CreateNewSettings()
        {
            SmartLightModel.HasSettings = true;

            DeviceSettings!.Location = SmartLightModel.Location;
            DeviceSettings.Type = SmartLightModel.Type;
            DeviceSettings.ConnectionString = SmartLightModel.ConnectionString;

            await _databaseContext.SaveSettingsAsync(DeviceSettings, null!);

            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsVM>();
        }

        [RelayCommand]
        private async Task OpenHistory(string deviceId)
        {
            DeviceHistory = await _databaseContext.GetDeviceHistory(deviceId);

            var historyStringBuilder = new StringBuilder();

            foreach (var item in DeviceHistory)
            {
                historyStringBuilder.AppendLine(item.TimeStamp.ToString());
                historyStringBuilder.AppendLine("DeviceState: " + item.State.ToString());
                historyStringBuilder.AppendLine("");
            }

            MessageBox.Show(historyStringBuilder.ToString(), "Device History");
        }

        [RelayCommand]
        private void GoToHome()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<HomeVM>();
        }
    }
}
