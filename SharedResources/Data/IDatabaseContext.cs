using SharedResources.Models;

namespace SharedResources.Data;

public interface IDatabaseContext
{
    Task<ResultResponse<DeviceSettings>> GetSettingsAsync();

    Task<ResultResponse> ResetSettingsAsync();

    Task<ResultResponse> SaveSettingsAsync(DeviceSettings settings);
}
