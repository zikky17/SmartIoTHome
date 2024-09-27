using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Data;
using SharedResources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLight.ViewModels
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
            var result = await _databaseContext.GetSettingsAsync("f0468151-3b8b-4d92-8e42-cf679a27796f");
            if (result.Succeeded)
            {
                DeviceSettings = result.Content;
            }
        }

        [RelayCommand]
        private async Task ResetSettings(DeviceSettings device)
        {
            var result = await _databaseContext.DeleteDeviceSettingsAsync(device);
            if (result.Succeeded)
            {
                await _databaseContext.GetSettingsAsync(device.Id);
                var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
                mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsVM>();
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
