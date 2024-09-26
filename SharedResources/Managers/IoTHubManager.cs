using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using SharedResources.Factories;
using SharedResources.Models;
using System.Diagnostics;

namespace SharedResources.Managers;

public class IoTHubManager
{
    private readonly string _connectionString;
    private readonly ServiceClient? _serviceClient;
    private readonly RegistryManager? _registryManager;

    public IoTHubManager(string connectionString)
    {
        _connectionString = connectionString;
        _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
        _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
    }

    public async Task<DeviceInstance> RegisterDeviceAsync(string deviceId, string deviceName)
    {
        if (string.IsNullOrEmpty(deviceId))
            return null!;

        DeviceInstance deviceInstance = new()
        {
            Device = await _registryManager!.GetDeviceAsync(deviceId) ?? await _registryManager!.AddDeviceAsync(new Device(deviceId))
        };

        await UpdateDesiredPropertyAsync(deviceInstance.Device, nameof(deviceName), deviceName);

        deviceInstance.ConnectionString = GetDeviceConnectionString(deviceInstance.Device);
        deviceInstance.Twin = await _registryManager.GetTwinAsync(deviceInstance.Device.Id);

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
            var twin = await _registryManager!.GetTwinAsync(device.Id);
            twin.Properties.Desired[key] = value;

            await _registryManager.UpdateTwinAsync(device.Id, twin, twin.ETag);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<ResultResponse> RemoveDeviceAsync(string deviceId)
    {
        try
        {
            await _registryManager!.RemoveDeviceAsync(deviceId);
            return ResultResponseFactory.Success("Device is deleted.");
        }
        catch (Exception ex)
        {
            return ResultResponseFactory.Failed(ex.Message);
        }
    }


}
