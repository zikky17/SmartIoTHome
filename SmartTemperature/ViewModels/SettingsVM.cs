using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetty.Handlers.Tls;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Data;
using SharedResources.Models;
using SmartTemperature.Models;
using System.Text;
using System.Windows;

namespace SmartTemperature.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContextWPF _databaseContext;

        public SmartTempModel SmartTempModel { get; set; }

        public DeviceSettings DeviceSettings;
        public List<DeviceStateHistory> DeviceHistory;


        public SettingsVM(IDbContextWPF databaseContext, IServiceProvider serviceProvider)
        {
            _databaseContext = databaseContext;
            _serviceProvider = serviceProvider;

            SmartTempModel = new SmartTempModel();
            LoadSettingsAsync().ConfigureAwait(false);

        }

        private async Task LoadSettingsAsync()
        {
            var result = await _databaseContext.GetSettingsAsync("798197a0-e07f-453c-92c5-9a2121a2d673");
            if (result.Succeeded)
            {
                DeviceSettings = result.Content!;

                SmartTempModel.Id = DeviceSettings.Id;
                SmartTempModel.Location = DeviceSettings.Location!;
                SmartTempModel.Type = DeviceSettings.Type!;
                SmartTempModel.ConnectionString = DeviceSettings.ConnectionString!;
                SmartTempModel.DeviceState = DeviceSettings.DeviceState.ToString();
                SmartTempModel.DeviceState = SmartTempModel.DeviceState == "False" ? "Off" : "On";

                SmartTempModel.HasSettings = true;
            }
        }

        [RelayCommand]
        private async Task ResetSettings(DeviceSettings device)
        {
            SmartTempModel.HasSettings = false;
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
            SmartTempModel.HasSettings = true;

            DeviceSettings.Location = SmartTempModel.Location;
            DeviceSettings.Type = SmartTempModel.Type;
            DeviceSettings.ConnectionString = SmartTempModel.ConnectionString;

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
