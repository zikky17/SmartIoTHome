using Microsoft.Azure.Devices.Client;

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
    }
}
