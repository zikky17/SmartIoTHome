namespace SmartHub.ViewModels;

public class DeviceSettingsVM
{
    public string ConnectionString { get; set; } = null!;
    public bool ConnectionState { get; set; }

    public string DeviceId { get; set; } = null!;
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
}
