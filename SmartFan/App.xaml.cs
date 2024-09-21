using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedResources.Handlers;
using SmartFan.ViewModels;
using SmartFan.Views;
using System.Windows;

namespace SmartFan
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
            var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=cabb9896-0fba-47d2-b67d-0279a9745284;SharedAccessKey=ZY2h+rdNJIKDCWG39rJtofVgQYpNfeL0buMulj4Ml9A=";
            var dc = new DeviceClientHandler("cabb9896-0fba-47d2-b67d-0279a9745284", "SmartFan", "Fan", connectionString);

            var initalizeResult = dc.Initialize();

            dc.Settings.DeviceStateChanged += (deviceState) =>
            {
                _logger.LogInformation($"{deviceState}");
            };
        }
    }
}
