using Microsoft.Azure.Devices;
using SharedResources.Models;
using System.Diagnostics;

namespace SharedResources.Managers;

public class IoTHubManager
{
    private readonly string _connectionString;
    private readonly ServiceClient? _serviceClient;
    private readonly RegistryManager? _deviceManagement;

    public IoTHubManager(string connectionString)
    {
        _connectionString = connectionString;
        _deviceManagement = RegistryManager.CreateFromConnectionString(_connectionString);
        _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
    }

    public async Task<DeviceInstance> RegisterDeviceAsync(string deviceId, string deviceName)
    {
        if (string.IsNullOrEmpty(deviceId))
            return null!;

        DeviceInstance deviceInstance = new()
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



}
