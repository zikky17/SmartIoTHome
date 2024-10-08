﻿using SharedResources.Models;

namespace SharedResources.Data;

public interface IDatabaseContext
{
    Task<ResultResponse<DeviceSettings>> GetSettingsAsync(string id);

    Task<ResultResponse> ResetSettingsAsync();

    Task<ResultResponse> SaveSettingsAsync(DeviceSettings settings, DeviceStateHistory history);

    Task<List<DeviceStateHistory>> GetDeviceHistory(string id);

    Task<ResultResponse> DeleteDeviceSettingsAsync(DeviceSettings device);

    Task<ResultResponse> RegisterEmailAddress(HubSettings settings);

    Task<string> GetRegisteredEmailAsync();
}
