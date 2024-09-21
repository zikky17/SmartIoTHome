using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedResources.Handlers;
using SmartLight.ViewModels;
using SmartLight.Views;
using System.Windows;

namespace SmartLight
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? host;
        private readonly ILogger _logger;

        public App()
        {
            host = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowVM>();

                services.AddSingleton<HomeView>();
                services.AddSingleton<HomeVM>();

            }).Build();

        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var mainWindow = host!.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            using var cts = new CancellationTokenSource();
            try
            {
                InitializeDevice();
                await host!.RunAsync(cts.Token);
            }
            catch { }

            await host!.RunAsync();

          
        }

        private void InitializeDevice()
        {
            var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=f0468151-3b8b-4d92-8e42-cf679a27796f;SharedAccessKey=6ycjkPeWyIRubkKcKX9BjTmOuZn0mBr6tAIoTN5ynLI=";
            var dc = new DeviceClientHandler("f0468151-3b8b-4d92-8e42-cf679a27796f", "SmartLight", "Light", connectionString);

            var initalizeResult = dc.Initialize();

            dc.Settings.DeviceStateChanged += (deviceState) =>
            {
                _logger.LogInformation($"{deviceState}");
            };
        }

    }

}
