using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetty.Handlers.Tls;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Data;
using SharedResources.Models;

namespace SmartTemperature.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabaseContext _databaseContext;

        [ObservableProperty]
        public bool hasSettings;

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string location;

        [ObservableProperty]
        private string type;

        [ObservableProperty]
        private string connectionString;

        public DeviceSettings DeviceSettings;


        public SettingsVM(IDatabaseContext databaseContext, IServiceProvider serviceProvider)
        {
            _databaseContext = databaseContext;
            _serviceProvider = serviceProvider;

            LoadSettingsAsync().ConfigureAwait(false);

        }

        private async Task LoadSettingsAsync()
        {
            var result = await _databaseContext.GetSettingsAsync("798197a0-e07f-453c-92c5-9a2121a2d673");
            if (result.Succeeded)
            {
                DeviceSettings = result.Content;

                Id = DeviceSettings.Id;
                Location = DeviceSettings.Location;
                Type = DeviceSettings.Type;
                ConnectionString = DeviceSettings.ConnectionString;
                HasSettings = true;
            }
        }

        [RelayCommand]
        private async Task ResetSettings(DeviceSettings device)
        {
            HasSettings = false;
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
            HasSettings = true;

            DeviceSettings.Location = Location;
            DeviceSettings.Type = Type;
            DeviceSettings.ConnectionString = ConnectionString;

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
