using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Models;
using System.Diagnostics;

namespace SmartHub.ViewModels;

public class DeviceSettingsVM
{
    private readonly AzureHub _iotHub;

    public DeviceSettingsVM(AzureHub iotHub)
    {
        _iotHub = iotHub;
    }

   
    public string? ResponseMessage { get; private set; }

    public Timer? Timer { get; set; }
    public int TimerInterval { get; set; } = 4000;

    public async Task ActivateDevice(SmartDeviceModel device)
    {
       await OnDeviceStateChanged(device);
        ResponseMessage = "Device state has changed.";
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
            if (methodName == "stop")
            {
                device.DeviceState = false;
            }
            else
            {
                device.DeviceState = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
