using Microsoft.AspNetCore.Components;
using SharedResources.Communication;
using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Managers;
using SharedResources.Models;
using SharedResources.Services;
using System.Diagnostics;

namespace SmartHub.ViewModels;

public class HomeVM
{
    private readonly HttpClient _http;
    private readonly AzureHub _iotHub;
    private readonly DeviceStateService _deviceStateService;

    public Timer? Timer { get; set; }
    public int TimerInterval { get; set; } = 4000;
    public string? ResponseMessage { get; private set; }
    private readonly NavigationManager _navigationManager;
    private readonly IDbContextMAUI _context;

    public HomeVM(AzureHub iotHub, HttpClient http, NavigationManager navigationManager, IDbContextMAUI context, DeviceStateService deviceStateService)
    {
        _iotHub = iotHub;
        _http = http;
        _navigationManager = navigationManager;
        _context = context;
        _deviceStateService = deviceStateService;
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
        try
        {
            var methodName = device.DeviceState ? "stop" : "start";
            await _iotHub.SendDirectMethodAsync(device.DeviceId, methodName);

            var history = new DeviceStateHistory
            {
                Id = device.DeviceId,
                State = device.DeviceState,
                TimeStamp = DateTime.Now
            };

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task DeleteDeviceAsync(SmartDeviceModel device)
    {

        if (!string.IsNullOrEmpty(device.DeviceId))
        {
            var hubManager = new IoTHubManager("HostName=gurra-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=/6xdlTOp1WhRRbgMsWuAS+FCnQSBLRI9BAIoTAU4LdE=");

            try
            {
                var response = await hubManager.RemoveDeviceAsync(device.DeviceId);
                if (response.Succeeded)
                {
                    ResponseMessage = $"{device.DeviceName} is now deleted.";
                    var email = new EmailCommunication();
                    email.Send(await _context.GetRegisteredEmailAsync(), "Azure IoT Hub Notification", $"Your Device {device.DeviceName} was deleted.", "");
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

        _deviceStateService.SelectedDevice = device;

        _navigationManager.NavigateTo($"/device-settings/{device.DeviceId}");
    }
}
