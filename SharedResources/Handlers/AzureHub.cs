using Microsoft.Azure.Devices;
using SharedResources.Models;

namespace SharedResources.Handlers;

public class AzureHub
{
    private readonly string _connectionString = "HostName=gurra-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=/6xdlTOp1WhRRbgMsWuAS+FCnQSBLRI9BAIoTAU4LdE=";
    private readonly RegistryManager? _registry;
    private readonly ServiceClient? _serviceClient;

    public AzureHub()
    {
        _registry = RegistryManager.CreateFromConnectionString(_connectionString);
        _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
    }

    public async Task<IEnumerable<SmartDeviceModel>> GetDevicesAsync()
    {
        var query = _registry!.CreateQuery("SELECT * FROM DEVICES");
        var devices = new List<SmartDeviceModel>();

        foreach (var twin in await query.GetNextAsTwinAsync())
        {
            var device = new SmartDeviceModel
            {
                DeviceId = twin.DeviceId
            };

            try { device.DeviceName = twin?.Properties?.Reported["deviceName"]?.ToString(); }
            catch { device.DeviceName = "Unknown"; }

            try { device.DeviceType = twin?.Properties?.Reported["deviceType"]?.ToString(); }
            catch { device.DeviceType = "Unknown"; }

            try
            {
                bool.TryParse(twin?.Properties?.Reported["connectionState"]?.ToString(), out bool connectionState);
                device.ConnectionState = connectionState;
            }
            catch { device.ConnectionState = false; }


            if (device.ConnectionState)
            {
                try
                {
                    bool.TryParse(twin?.Properties?.Reported["deviceState"]?.ToString(), out bool deviceState);
                    device.DeviceState = deviceState;
                }
                catch { device.DeviceState = false; }

            }
            else
            {
                device.DeviceState = false;
            }

            devices.Add(device);
        }

        return devices;
    }

    public async Task SendDirectMethodAsync(string deviceId, string methodName)
    {
        var methodInvocation = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds(10) };
        var response = await _serviceClient!.InvokeDeviceMethodAsync(deviceId, methodInvocation);
    }
}
