﻿using Microsoft.Azure.Devices;
using SharedResources.Handlers;
using SharedResources.Managers;
using SharedResources.Models;
using System.Diagnostics;

namespace SmartHub.ViewModels;

public class HomeVM
{
    private readonly HttpClient _http;
    private readonly AzureHub _iotHub;
    public Timer? Timer { get; set; }
    public int TimerInterval { get; set; } = 4000;
    public string? ResponseMessage { get; private set; }

    public HomeVM(AzureHub iotHub, HttpClient http)
    {
        _iotHub = iotHub;
        _http = http;
    }

    public async Task<IEnumerable<SmartDeviceModel>> GetDevicesAsync()
    {
        return await _iotHub.GetDevicesAsync();
    }

    public async Task OnDeviceStateChanged(SmartDeviceModel device)
    {
        Timer?.Change(Timeout.Infinite, Timeout.Infinite);
        await SendDirectMethodAsync(device);
        Timer?.Change(TimerInterval, TimerInterval);
    }

    public async Task SendDirectMethodAsync(SmartDeviceModel device)
    {
        var methodName = device.DeviceState ? "stop" : "start";
        await _iotHub.SendDirectMethodAsync(device.DeviceId, methodName);
    }

    public async Task DeleteDeviceAsync(SmartDeviceModel device)
    {
        if(!string.IsNullOrEmpty(device.DeviceId))
        {
            var hubManager = new IoTHubManager("HostName=gurra-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=/6xdlTOp1WhRRbgMsWuAS+FCnQSBLRI9BAIoTAU4LdE=");

            try
            {
              var response = await hubManager.RemoveDeviceAsync(device.DeviceId);
                if (response.Succeeded)
                {
                    ResponseMessage = $"{device.DeviceName} is now deleted.";
                    await HideMessageAfterDelay();
                }
                else
                {
                    ResponseMessage = "Failed to delete device.";
                    await HideMessageAfterDelay();
                }
            }
            catch (Exception ex)
            {
                ResponseMessage += ex.ToString();
                await HideMessageAfterDelay();
            }

        }
    }

    private async Task HideMessageAfterDelay()
    {
        await Task.Delay(3000); 
        ResponseMessage = null;  
    }
}
