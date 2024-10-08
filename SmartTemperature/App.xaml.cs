using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Managers;
using SharedResources.Models;
using SmartTemperature.ViewModels;
using SmartTemperature.Views;
using System.Diagnostics;
using System.Windows;

namespace SmartTemperature
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? host;
        private readonly ILogger _logger;
        private readonly IoTHubManager _hub;

        public App()
        {
            host = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowVM>();

                services.AddTransient<HomeView>();
                services.AddTransient<HomeVM>();

                services.AddTransient<SettingsView>();
                services.AddTransient<SettingsVM>();

                services.AddSingleton<IDatabaseContext>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<SQLiteContext>>();
                    return new SQLiteContext(logger, () => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                });

            }).Build();

        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var mainWindow = host!.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            using var cts = new CancellationTokenSource();
            try
            {
                var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=798197a0-e07f-453c-92c5-9a2121a2d673;SharedAccessKey=KaJ+2FmYJPW2L2OVn84wX5fSC3hgVHCbfAIoTFAUVuA=";
                var dc = new DeviceClientHandler("798197a0-e07f-453c-92c5-9a2121a2d673", "SmartTemp", "Temp", connectionString);

                var settings = new DeviceSettings
                {
                    Id = dc.Settings.DeviceId,
                    Type = dc.Settings.DeviceType,
                    ConnectionString = connectionString,
                    Location = "Kitchen"
                };

                var initializeResult = await dc.Initialize();
                if (!initializeResult.Succeeded)
                {
                    Debug.WriteLine($"Device initialization failed: {initializeResult.Message}");
                }

                var database = host!.Services.GetRequiredService<IDatabaseContext>();

                await database.SaveSettingsAsync(settings, null!);

                await host!.RunAsync(cts.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during startup: {ex.Message}");
            }
        }
    }
}
