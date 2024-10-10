using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Models;
using System.Diagnostics;

namespace SmartFan.ViewModels
{
    public partial class HomeVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContextWPF _context;

        public HomeVM(IServiceProvider serviceProvider, IDbContextWPF context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        [RelayCommand]
        private void GoToSettings()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsVM>();
        }

    }
}
