﻿using CommunityToolkit.Mvvm.ComponentModel;
using SharedResources.Data;
using SharedResources.Handlers;

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

        private readonly AzureHub _hub;
        
        private void LoadHubSettings()
        {
            hubConnectionString = _hub.GetHubConnectionString();
        }

    }
}
