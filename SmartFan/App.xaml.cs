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
            var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=e677eda9-6bf4-48b9-83da-60785bf972e6;SharedAccessKey=ANy2Thlvr2L0/I+mj5RTUjsIUKvCXtSoVAIoTAeZDmY=";
            var dc = new DeviceClientHandler("e677eda9-6bf4-48b9-83da-60785bf972e6", "SmartFan", "Fan", connectionString);

            var initalizeResult = dc.Initialize();

            dc.Settings.DeviceStateChanged += (deviceState) =>
            {
                _logger.LogInformation($"{deviceState}");
            };
        }
    }
}
