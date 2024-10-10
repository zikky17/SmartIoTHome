using CommunityToolkit.Mvvm.ComponentModel;
using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Models;
using System.Diagnostics;

namespace SmartHub.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IDbContextMAUI _context;

        public SettingsVM(IDbContextMAUI context)
        {
            _context = context;
            LoadHubSettings();
        }

        [ObservableProperty]
        public string hubConnectionString = null!;

        [ObservableProperty]
        public HubSettings settings = new();

        [ObservableProperty]
        public string currentEmail;
        public string? Message { get; private set; }

        private readonly AzureHub _hub;
        
        private void LoadHubSettings()
        {
            var connectionString = _context.GetHubConnectionString();
            HubConnectionString = connectionString.Result.ToString();
        }

        public async Task SaveEmailAddress()
        {
          await _context.RegisterEmailAddress(Settings);
          Message = $"{Settings.Email} Saved.";
          await LoadEmailAddressAsync();
        }

        public async Task LoadEmailAddressAsync()
        {
            try
            {
                var email = await _context.GetRegisteredEmailAsync();
                if (email != null)
                {
                    CurrentEmail = email;
                }
                else
                {
                    CurrentEmail = "TestEmail@live.se";
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
          
        }

    }
}
