namespace SharedResources.Models;

public class DeviceRegistrationResponse
{
    public string? DeviceId { get; set; }
    public string? ConnectionString { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public bool DeviceState { get; set; }
}
