using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResourcesWPF
{
   public class GetDeviceTwin
    {
        public static async Task<string> GetTwinProperties(string connectionString)
        {
            var client = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);

            try
            {
                var twin = await client.GetTwinAsync();

                var reportedProperties = twin.Properties.Reported;

                if (reportedProperties.Contains("deviceState"))
                {
                    return (string)reportedProperties["deviceState"];
                }
                else
                {
                    Debug.WriteLine("DeviceState not found in reported properties.");
                    return null!;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving twin: {ex.Message}");
                return null!;
            }
        }
    }
}
