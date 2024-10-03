using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Data;
using SharedResources.Models;
using SmartLight.Models;

namespace SmartLight.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabaseContext _databaseContext;

        public SmartLightModel SmartLightModel { get; set; }

        public DeviceSettings DeviceSettings;


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
                DeviceSettings = result.Content;

                SmartLightModel.Id = DeviceSettings.Id;
                SmartLightModel.Location = DeviceSettings.Location;
                SmartLightModel.Type = DeviceSettings.Type;
                SmartLightModel.ConnectionString = DeviceSettings.ConnectionString;
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

            DeviceSettings.Location = SmartLightModel.Location;
            DeviceSettings.Type = SmartLightModel.Type;
            DeviceSettings.ConnectionString = SmartLightModel.ConnectionString;

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
