using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
namespace SmartLight.ViewModels
{
    public partial class HomeVM(IServiceProvider serviceProvider) : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;


        [RelayCommand]
        private void GoToSettings()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsVM>();
        }
    }
}
