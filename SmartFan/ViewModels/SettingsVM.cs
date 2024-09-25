using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace SmartFan.ViewModels
{
    public partial class SettingsVM(IServiceProvider serviceProvider) : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [RelayCommand]
        private void GoToHome()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<HomeVM>();
        }
    }
}
