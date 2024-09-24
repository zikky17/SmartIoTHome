using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices;

namespace SharedResources.Models
{
    public class DeviceInstance
    {
        public string? ConnectionString { get; set; }
        public Device? Device { get; set; }
        public Twin? Twin { get; set; }
    }
}
