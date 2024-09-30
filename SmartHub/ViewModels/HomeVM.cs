using Microsoft.AspNetCore.Components;
using Microsoft.Azure.Devices;
using SharedResources.Communication;
using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Managers;
using SharedResources.Models;
using SQLite;
using System.Diagnostics;

namespace SmartHub.ViewModels;

public class HomeVM
{
    private readonly HttpClient _http;
    private readonly AzureHub _iotHub;
    public Timer? Timer { get; set; }
    public int TimerInterval { get; set; } = 4000;
    public string? ResponseMessage { get; private set; }
    private readonly NavigationManager _navigationManager;
    private readonly IDatabaseContext _context;

    public HomeVM(AzureHub iotHub, HttpClient http, NavigationManager navigationManager, IDatabaseContext context)
    {
        _iotHub = iotHub;
        _http = http;
        _navigationManager = navigationManager;
        _context = context;
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
                    var email = new EmailCommunication();
                    email.Send(await _context.GetRegisteredEmailAsync(), $"Device Deleted: {device.DeviceName}", "<h1></h1>", "Your device is deleted.");
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

    public void NavigateToSettings(SmartDeviceModel device)
    {
        if (device == null || string.IsNullOrEmpty(device.DeviceId))
        {
            throw new ArgumentException("Device cannot be null and must have a valid DeviceId");
        }

        _navigationManager.NavigateTo($"/device-settings/{device.DeviceId}");
    }
}
