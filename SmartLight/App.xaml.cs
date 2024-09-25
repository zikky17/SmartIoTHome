using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Models;
using SmartLight.ViewModels;
using SmartLight.Views;
using System.Configuration;
using System.Diagnostics;
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
                var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=f0468151-3b8b-4d92-8e42-cf679a27796f;SharedAccessKey=6ycjkPeWyIRubkKcKX9BjTmOuZn0mBr6tAIoTN5ynLI=";
                var dc = new DeviceClientHandler("f0468151-3b8b-4d92-8e42-cf679a27796f", "SmartLight", "Light", connectionString);

                var settings = new DeviceSettings
                {
                    Id = dc.Settings.DeviceId,
                    Type = dc.Settings.DeviceType,
                    ConnectionString = connectionString,
                    Location = "Living Room"
                };

                var initializeResult = await dc.Initialize();
                if (!initializeResult.Succeeded)
                {
                    Debug.WriteLine($"Device initialization failed: {initializeResult.Message}");
                }

                var database = host!.Services.GetRequiredService<IDatabaseContext>();

                await database.SaveSettingsAsync(settings);

                await host!.RunAsync(cts.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during startup: {ex.Message}");
            }
        }
    }
}
