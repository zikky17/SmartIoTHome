using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using SharedResources.Models;
using System.Diagnostics;

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
        int retryCount = 0;
        int maxRetries = 5;
        int delaySeconds = 10;

        while (retryCount < maxRetries)
        {
            try
            {
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
                break; 
            }
            catch (ThrottlingException)
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw;
                }
                await Task.Delay(delaySeconds * 1000);
                delaySeconds *= 2; 
            }
        }

        return devices;
    }

    public string GetHubConnectionString()
    {
        return _connectionString;
    }


    public async Task SendDirectMethodAsync(string deviceId, string methodName)
    {
        var devices = await GetDevicesAsync();
        var device = devices.FirstOrDefault(d => d.DeviceId == deviceId);

        if (device == null)
        {
            Debug.WriteLine($"Device '{deviceId}' not found.");
            return;
        }

        if (!device.ConnectionState)
        {
            Debug.WriteLine($"Device '{deviceId}' is not connected.");
            return;
        }

        try
        {
            var methodInvocation = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds(10) };
            var response = await _serviceClient!.InvokeDeviceMethodAsync(deviceId, methodInvocation);
            Debug.WriteLine($"Direct method '{methodName}' invoked on device '{deviceId}' with status: {response.Status}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to invoke direct method '{methodName}' on device '{deviceId}': {ex.Message}");
        }
    }


    public async Task<bool> UpdateDesiredPropertyAsync(string deviceId, string key, string value)
    {
        try
        {
            var twin = await _registry!.GetTwinAsync(deviceId);
            twin.Properties.Desired[key] = value;

            await _registry.UpdateTwinAsync(deviceId, twin, twin.ETag);
            Debug.WriteLine($"Desired property '{key}' updated to '{value}' for device '{deviceId}'");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to update desired property '{key}' for device '{deviceId}': {ex.Message}");
            return false;
        }
    }
}
