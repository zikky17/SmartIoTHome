using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetty.Handlers.Tls;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Data;
using SharedResources.Models;
using SmartFan.Models;
using SmartFan.Views;

namespace SmartFan.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabaseContext _databaseContext;

        public SmartFanModel SmartFanModel { get; set; }
        public DeviceSettings DeviceSettings;


        public SettingsVM(IDatabaseContext databaseContext, IServiceProvider serviceProvider)
        {
            _databaseContext = databaseContext;
            _serviceProvider = serviceProvider;

            SmartFanModel = new SmartFanModel();
            LoadSettingsAsync().ConfigureAwait(false);

        }

        private async Task LoadSettingsAsync()
        {
            var result = await _databaseContext.GetSettingsAsync("cabb9896-0fba-47d2-b67d-0279a9745284");
            if (result.Succeeded)
            {
                DeviceSettings = result.Content;

                SmartFanModel.Id = DeviceSettings.Id;
                SmartFanModel.Location = DeviceSettings.Location;
                SmartFanModel.Type = DeviceSettings.Type;
                SmartFanModel.ConnectionString = DeviceSettings.ConnectionString;
                SmartFanModel.DeviceState = DeviceSettings.DeviceState.ToString();
                SmartFanModel.DeviceState = SmartFanModel.DeviceState == "False" ? "Off" : "On";
                SmartFanModel.HasSettings = true;
            }
        }

        [RelayCommand]
        private async Task ResetSettings(DeviceSettings device)
        {
            SmartFanModel.HasSettings = false;
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
            SmartFanModel.HasSettings = true;

            DeviceSettings.Location = SmartFanModel.Location;
            DeviceSettings.Type = SmartFanModel.Type;
            DeviceSettings.ConnectionString = SmartFanModel.ConnectionString;

            await _databaseContext.SaveSettingsAsync(DeviceSettings);

            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsVM>();
        }


        [RelayCommand]
        private void GoToHome()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<HomeVM>();
        }
    }
}
