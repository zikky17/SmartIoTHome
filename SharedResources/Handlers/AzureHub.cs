using Microsoft.Azure.Devices;
using SharedResources.Models;
using System.Diagnostics;

namespace SharedResources.Handlers;

public class AzureHub
{
    private readonly string _connectionString = "HostName=gurra-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=/6xdlTOp1WhRRbgMsWuAS+FCnQSBLRI9BAIoTAU4LdE=";
    private readonly RegistryManager? _registry;
    private readonly ServiceClient? _serviceClient;
    private readonly RegistryManager? _deviceManagement;

    public AzureHub()
    {
        _registry = RegistryManager.CreateFromConnectionString(_connectionString);
        _deviceManagement = RegistryManager.CreateFromConnectionString(_connectionString);
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


    public async Task<SmartDeviceModel> RegisterDeviceAsync(string deviceId, string deviceName)
    {
        if (string.IsNullOrEmpty(deviceId))
            return null!;

        SmartDeviceModel deviceInstance = new()
        {
            Device = await _deviceManagement!.GetDeviceAsync(deviceId) ?? await _deviceManagement!.AddDeviceAsync(new Device(deviceId))
        };

        await UpdateDesiredPropertyAsync(deviceInstance.Device, nameof(deviceName), deviceName);

        deviceInstance.ConnectionString = GetDeviceConnectionString(deviceInstance.Device);
        deviceInstance.Twin = await _deviceManagement.GetTwinAsync(deviceInstance.Device.Id);

        return deviceInstance;
    }

    public string GetDeviceConnectionString(Device device)
    {
        var deviceConnectionString = $"{_connectionString!.Split(";")[0]};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
        return deviceConnectionString ?? null!;
    }

    public async Task<bool> UpdateDesiredPropertyAsync(Device device, string key, string value)
    {
        try
        {
            var twin = await _deviceManagement!.GetTwinAsync(device.Id);
            twin.Properties.Desired[key] = value;

            await _deviceManagement.UpdateTwinAsync(device.Id, twin, twin.ETag);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task SendDirectMethodAsync(string deviceId, string methodName)
    {
        var methodInvocation = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds(10) };
        var response = await _serviceClient!.InvokeDeviceMethodAsync(deviceId, methodInvocation);
    }
}
