using CommunityToolkit.Mvvm.ComponentModel;
using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Models;

namespace SmartHub.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IDatabaseContext _context;

        public SettingsVM(IDatabaseContext context, AzureHub hub)
        {
            _context = context;
            _hub = hub;
            LoadHubSettings();
            LoadEmailAddress();
        }

        [ObservableProperty]
        public string hubConnectionString;

        [ObservableProperty]
        public HubSettings settings = new();

        [ObservableProperty]
        public string currentEmail;

        private readonly AzureHub _hub;
        
        private void LoadHubSettings()
        {
            hubConnectionString = _hub.GetHubConnectionString();
        }

        public void SaveEmailAddress()
        {
            _context.RegisterEmailAddress(Settings);
        }

        private async void LoadEmailAddress()
        {
           CurrentEmail = await _context.GetRegisteredEmailAsync();
        }

    }
}
