using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Diagnostics;

namespace SharedResources.Handlers
{
    public class DeviceTwinHandler
    {
        private DeviceClient _client;
        private readonly string _connectionString;


        public DeviceTwinHandler(string connectionString)
        {
            _client = DeviceClient.CreateFromConnectionString(connectionString);
            _connectionString = connectionString;
        }


        public async Task<bool> InitializeConnectionAsync()
        {
            try
            {
                _client = DeviceClient.CreateFromConnectionString(_connectionString, TransportType.Mqtt);

                Twin twin = await _client.GetTwinAsync();

                Debug.WriteLine("Connection to IoT Hub established.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to connect to IoT Hub: {ex.Message}");
                return false;
            }
        }

        public async Task<Twin> GetDeviceTwinAsync()
        {
            var twin = await _client.GetTwinAsync();

            return twin;
        }
        public async Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback)
        {
            if (_client != null)
            {
                await _client.SetDesiredPropertyUpdateCallbackAsync(callback, null);
            }
        }
    }
}
