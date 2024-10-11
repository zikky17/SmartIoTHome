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
            LoadHubSettings().GetAwaiter();
        }

        [ObservableProperty]
        public string? hubConnectionString;

        [ObservableProperty]
        public HubSettings settings = new();

        [ObservableProperty]
        public string currentEmail;
        public string? Message { get; private set; }

        private readonly AzureHub _hub;
        
        private async Task LoadHubSettings()
        {
            var connectionString = await _context.GetHubConnectionString();
            HubConnectionString = connectionString.ToString();
        }

        public async Task SaveConnectionString()
        {
            var settings = new HubSettings
            {
                Email = CurrentEmail,
                HubConnectionString = Settings.HubConnectionString
            };

            await _context.SaveConnectionString(settings);
            Message = "Connection String saved.";
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
