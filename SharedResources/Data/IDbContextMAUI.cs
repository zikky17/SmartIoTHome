﻿using SharedResources.Models;

namespace SharedResources.Data
{
    public interface IDbContextMAUI
    {
        Task CreateTablesAsync();
        Task<ResultResponse> RegisterEmailAddress(HubSettings settings);
        Task<string> GetRegisteredEmailAsync();
        Task<string> GetHubConnectionString();
        Task SaveDeviceHistory(DeviceStateHistory history);
        Task SaveConnectionString(HubSettings settings);
    }
}
