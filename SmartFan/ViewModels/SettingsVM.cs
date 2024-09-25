using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Data;
using SharedResources.Models;

namespace SmartFan.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabaseContext _databaseContext;

        private DeviceSettings _deviceSettings;
        public DeviceSettings DeviceSettings
        {
            get => _deviceSettings;
            set
            {
                _deviceSettings = value;
                OnPropertyChanged();
            }
        }

        public SettingsVM(IDatabaseContext databaseContext, IServiceProvider serviceProvider)
        {
            _databaseContext = databaseContext;
            _serviceProvider = serviceProvider;

            LoadSettingsAsync().ConfigureAwait(false);

        }

        private async Task LoadSettingsAsync()
        {
            var result = await _databaseContext.GetSettingsAsync("cabb9896-0fba-47d2-b67d-0279a9745284");
            if (result.Succeeded)
            {
                DeviceSettings = result.Content;
            }
        }

        [RelayCommand]
        private void GoToHome()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<HomeVM>();
        }
    }
}
