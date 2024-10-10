using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Models;
using System.Diagnostics;

namespace SmartHub.ViewModels;

public class DeviceSettingsVM
{
    private readonly AzureHub _iotHub;
    private readonly IDbContextMAUI _context;

    public DeviceSettingsVM(AzureHub iotHub, IDbContextMAUI context)
    {
        _iotHub = iotHub;
        _context = context;
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

            var history = new DeviceStateHistory
            {
                Id = device.DeviceId,
                State = device.DeviceState,
                TimeStamp = DateTime.Now
            };

            await _context.SaveDeviceHistory(history);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
