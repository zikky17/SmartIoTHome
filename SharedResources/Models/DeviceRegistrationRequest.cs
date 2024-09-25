namespace SharedResources.Models;

public class DeviceRegistrationRequest
{
    public string DeviceId { get; set; } = Guid.NewGuid().ToString();
    public string DeviceName { get; set; } = null!;
    public string DeviceType { get; set; } = null!;
}
