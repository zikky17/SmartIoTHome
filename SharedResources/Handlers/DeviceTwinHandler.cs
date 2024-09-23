using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace SharedResources.Handlers
{
    public class DeviceTwinHandler
    {
        private DeviceClient _client;

        public DeviceTwinHandler(string connectionString)
        {
            _client = DeviceClient.CreateFromConnectionString(connectionString);
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
