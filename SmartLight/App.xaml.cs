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
                await host!.RunAsync(cts.Token);
            }
            catch { }

            await host!.RunAsync();

            InitializeDevice();
        }

        private void InitializeDevice()
        {
            var dc = new DeviceClientHandler("e677eda9-6bf4-48b9-83da-60785bf972e6", "SmartLight", "Light");

            var initalizeResult = dc.Initialize();

            dc.Settings.DeviceStateChanged += (deviceState) =>
            {
                _logger.LogInformation($"{deviceState}");
            };


            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                var disconnectResult = dc.Disconnect();
               _logger.LogInformation($"Disconnect {disconnectResult}");
            };
        }
    }

}
