﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Data;
using SharedResources.Models;
using SmartFan.Models;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace SmartFan.ViewModels
{
    public partial class SettingsVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContextWPF _databaseContext;

        public SmartFanModel SmartFanModel { get; set; }
        public DeviceSettings DeviceSettings;
        public List<DeviceStateHistory> DeviceHistory;


        public SettingsVM(IDbContextWPF databaseContext, IServiceProvider serviceProvider)
        {
            _databaseContext = databaseContext;
            _serviceProvider = serviceProvider;

            SmartFanModel = new SmartFanModel();
            LoadSettingsAsync().ConfigureAwait(false);

        }

        private async Task LoadSettingsAsync()
        {
            var result = await _databaseContext.GetSettingsAsync("cabb9896-0fba-47d2-b67d-0279a9745284");
            if (result.Succeeded)
            {
                DeviceSettings = result.Content!;

                SmartFanModel.Id = DeviceSettings!.Id;
                SmartFanModel.Location = DeviceSettings.Location!;
                SmartFanModel.Type = DeviceSettings.Type!;
                SmartFanModel.ConnectionString = DeviceSettings.ConnectionString!;
                SmartFanModel.DeviceState = DeviceSettings.DeviceState.ToString();
                SmartFanModel.DeviceState = SmartFanModel.DeviceState == "False" ? "Off" : "On";
                SmartFanModel.HasSettings = true;
            }

            await GetTwinProperties();
        }

        public async Task GetTwinProperties()
        {
            var connectionString = "HostName=gurra-iothub.azure-devices.net;DeviceId=cabb9896-0fba-47d2-b67d-0279a9745284;SharedAccessKey=SULmlJ9u55cCjNBiC2wT6IsdzTyJPhS5t6J4RYjg4wM=";

            var client = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);

            try
            {
                var twin = await client.GetTwinAsync();

                var reportedProperties = twin.Properties.Reported;

                if (reportedProperties.Contains("deviceState"))
                {
                    SmartFanModel.DeviceState = (string)reportedProperties["deviceState"];
                    if (SmartFanModel.DeviceState == "False")
                    {
                        SmartFanModel.DeviceState = "Off";
                    }
                    else
                    {
                        SmartFanModel.DeviceState = "On";
                    }
                }
                else
                {
                    Debug.WriteLine("DeviceState not found in reported properties.");

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving twin: {ex.Message}");

            }
        }

        [RelayCommand]
        private async Task ResetSettings(DeviceSettings device)
        {
            SmartFanModel.HasSettings = false;
            var result = await _databaseContext.DeleteDeviceSettingsAsync(device);
            if (result.Succeeded)
            {
                var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
                mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsVM>();
            }
        }

        [RelayCommand]
        private async Task CreateNewSettings()
        {
            SmartFanModel.HasSettings = true;

            DeviceSettings.Location = SmartFanModel.Location;
            DeviceSettings.Type = SmartFanModel.Type;
            DeviceSettings.ConnectionString = SmartFanModel.ConnectionString;

            await _databaseContext.SaveSettingsAsync(DeviceSettings, null!);

            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsVM>();
        }

        [RelayCommand]
        private async Task OpenHistory(string deviceId)
        {
            DeviceHistory = await _databaseContext.GetDeviceHistory(deviceId);

            var historyStringBuilder = new StringBuilder();

            foreach (var item in DeviceHistory)
            {
                historyStringBuilder.AppendLine(item.TimeStamp.ToString());
                historyStringBuilder.AppendLine("DeviceState: " + item.State.ToString());
                historyStringBuilder.AppendLine("");
            }

            MessageBox.Show(historyStringBuilder.ToString(), "Device History");
        }


        [RelayCommand]
        private void GoToHome()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindowVM>();
            mainWindow.CurrentViewModel = _serviceProvider.GetRequiredService<HomeVM>();
        }
    }
}
