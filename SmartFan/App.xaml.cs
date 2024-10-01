using Microsoft.Azure.Devices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Managers;
using SharedResources.Models;
using SmartFan.ViewModels;
using SmartFan.Views;
using System.Configuration;
using System.Diagnostics;
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
                var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=cabb9896-0fba-47d2-b67d-0279a9745284;SharedAccessKey=SULmlJ9u55cCjNBiC2wT6IsdzTyJPhS5t6J4RYjg4wM=";
                var dc = new DeviceClientHandler("cabb9896-0fba-47d2-b67d-0279a9745284", "SmartFan", "Fan", connectionString);

                var hubConnectionString = "HostName=gurra-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=/6xdlTOp1WhRRbgMsWuAS+FCnQSBLRI9BAIoTAU4LdE=";

                var initializeResult = await dc.Initialize();
                if (!initializeResult.Succeeded)
                {
                    Debug.WriteLine($"Device initialization failed: {initializeResult.Message}");
                }

                var iotHubManager = new IoTHubManager(hubConnectionString);
                var device = new Device(dc.Settings.DeviceId);

                bool desiredState = dc.Settings.DeviceState;
                bool updateSuccess = await iotHubManager.UpdateDesiredPropertyAsync(device, "deviceState", desiredState.ToString().ToLower());

                if (updateSuccess)
                {
                    Debug.WriteLine("Desired properties updated successfully.");
                }
                else
                {
                    Debug.WriteLine("Failed to update desired properties.");
                }

                var settings = new DeviceSettings
                {
                    Id = dc.Settings.DeviceId,
                    Type = dc.Settings.DeviceType,
                    ConnectionString = connectionString,
                    Location = "Living Room"
                };


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
