using Microsoft.Azure.Devices;
using SharedResources.Handlers;
using SharedResources.Models;
using System.Diagnostics;

namespace SmartHub.ViewModels;

public class HomeVM
{
    private readonly AzureHub _iotHub;
    public Timer? Timer { get; set; }
    public int TimerInterval { get; set; } = 4000;

    public HomeVM(AzureHub iotHub)
    {
        _iotHub = iotHub;
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
}
