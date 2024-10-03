using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Models;
using System.Diagnostics;

namespace SmartHub.ViewModels;

public class DeviceSettingsVM
{
    private readonly IDatabaseContext _context;
    private readonly AzureHub _iotHub;

    public DeviceSettingsVM(IDatabaseContext context, AzureHub iotHub)
    {
        _context = context;
        _iotHub = iotHub;
    }

    public string ConnectionString { get; set; } = null!;
    public bool ConnectionState { get; set; }

    public string DeviceId { get; set; } = null!;
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }

    public Timer? Timer { get; set; }
    public int TimerInterval { get; set; } = 4000;

    public async Task ActivateDevice(SmartDeviceModel device)
    {

       await OnDeviceStateChanged(device);
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
            var settings = await _context.GetSettingsAsync(device.DeviceId);
            if (methodName == "stop")
            {
                settings.Content!.DeviceState = false;
            }
            else
            {
                settings.Content!.DeviceState = true;
            }
            _context.SaveSettingsAsync(settings.Content);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
