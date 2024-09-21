using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharedResources.Data;
using SharedResources.Factories;
using SharedResources.Models;

namespace SmartHub.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IDatabaseContext _context;

        public SettingsVM(IDatabaseContext context)
        {
            _context = context;
            GetDeviceSettingsAsync().ConfigureAwait(false);
        }

        [ObservableProperty]
        private bool isConfigured = false;

        [ObservableProperty]
        private DeviceSettings? settings;

        [RelayCommand]
        public async Task ConfigureSettings()
        {
            await _context.SaveSettingsAsync(DeviceSettingsFactory.Create());
            await GetDeviceSettingsAsync();
        }

        public async Task GetDeviceSettingsAsync()
        {
            var response = await _context.GetSettingsAsync();
            Settings = response.Content;
            IsConfigured = Settings != null;
        }

        [RelayCommand]
        public async Task ResetSettings()
        {
            await _context.ResetSettingsAsync();
            await GetDeviceSettingsAsync();
        }
    }
}
