using SharedResources.Models;

namespace SharedResources.Data;

public interface IDbContextWPF
{
    Task<ResultResponse<DeviceSettings>> GetSettingsAsync(string id);

    Task<ResultResponse> ResetSettingsAsync();

    Task<ResultResponse> SaveSettingsAsync(DeviceSettings settings, DeviceStateHistory history);

    Task<List<DeviceStateHistory>> GetDeviceHistory(string id);

    Task<ResultResponse> DeleteDeviceSettingsAsync(DeviceSettings device);

    Task<ResultResponse> SaveHistoryAsync(DeviceStateHistory history);
}
