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
        }

        [ObservableProperty]
        public string hubConnectionString;

        [ObservableProperty]
        public HubSettings settings = new();

        [ObservableProperty]
        public string currentEmail;
        public string? Message { get; private set; }

        private readonly AzureHub _hub;
        
        private void LoadHubSettings()
        {
            HubConnectionString = _hub.GetHubConnectionString();
        }

        public async Task SaveEmailAddress()
        {
          await _context.RegisterEmailAddress(Settings);
          Message = $"{Settings.Email} saved as current email address.";
        }

        public async Task LoadEmailAddressAsync()
        {
           CurrentEmail = await _context.GetRegisteredEmailAsync();
        }

    }
}
