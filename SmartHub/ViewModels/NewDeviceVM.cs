using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Net.Http.Json;
using SharedResources.Managers;
using SharedResources.Models;

namespace SmartHub.ViewModels
{
    public class NewDeviceVM
    {
        private readonly HttpClient _http;
        public DeviceRegistrationRequest? RegistrationRequest { get; set; } = new DeviceRegistrationRequest();
        public string? ResponseMessage { get; private set; }

        public NewDeviceVM(HttpClient http)
        {
            _http = http;
        }

        public async Task RegisterDevice()
        {
            if (string.IsNullOrEmpty(RegistrationRequest?.DeviceType) ||
                string.IsNullOrEmpty(RegistrationRequest.DeviceName))
            {
                ResponseMessage = "Device Name and Device Type are required.";
                return;
            }

            try
            {
                var hubManager = new IoTHubManager("HostName=gurra-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=/6xdlTOp1WhRRbgMsWuAS+FCnQSBLRI9BAIoTAU4LdE=");

                var registeredDevice = await hubManager.RegisterDeviceAsync(RegistrationRequest.DeviceId, RegistrationRequest.DeviceName);

                var result = await _http.PostAsJsonAsync("https://azurefunctions20240924121104.azurewebsites.net/api/DeviceRegistration?code=jpY81b3DaKeobBk8s7zmXwYOmEBxCBqEt_Cl2Z0L9fkHAzFu23X0Dg%3D%3D", RegistrationRequest);

                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<DeviceRegistrationResponse>(content);

                    DeviceClient client = DeviceClient.CreateFromConnectionString(response!.ConnectionString);

                    var twinCollection = new TwinCollection
                    {
                        ["deviceName"] = response.DeviceName,
                        ["deviceType"] = response.DeviceType
                    };

                    await client.UpdateReportedPropertiesAsync(twinCollection);

                    ResponseMessage = $"Device registered with the name: {response.DeviceName}";
                }
                else
                {
                    ResponseMessage = "Failed to register device.";
                }
            }
            catch (Exception ex)
            {
                ResponseMessage = $"Error registering device: {ex.Message}";
            }
        }
    }
}
