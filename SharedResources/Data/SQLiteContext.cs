﻿using Microsoft.Extensions.Logging;
using SharedResources.Factories;
using SharedResources.Models;
using SQLite;
using System.Diagnostics;

namespace SharedResources.Data
{
    public class SQLiteContext : IDatabaseContext
    {
        private readonly ILogger<SQLiteContext> _logger;
        private readonly SQLiteAsyncConnection? _context;

        public SQLiteContext(ILogger<SQLiteContext> logger, Func<string> directoryPath, string databaseName = "database.db3")
        {
            _logger = logger;

            try
            {
                var databasePath = Path.Combine(directoryPath(), databaseName);
                if (string.IsNullOrWhiteSpace(databasePath))
                    throw new ArgumentException("The database path cannot be null or empty.");

                _context = new SQLiteAsyncConnection(databasePath);
                if (_context != null)
                {
                    _logger.LogInformation("Database connection was initialized");
                }

                CreateTablesAsync().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while creating the database connection.");
            }
        }

        public async Task CreateTablesAsync()
        {
            try
            {
                if (_context == null)
                {
                    throw new ArgumentException("The database has not been initialized.");

                }
                else
                {
                    await _context.CreateTableAsync<DeviceSettings>();
                    await _context.CreateTableAsync<HubSettings>();
                    await _context.CreateTableAsync<DeviceStateHistory>();

                    _logger.LogInformation("Database tables were created successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while creating the database tables.");
            }
        }

        public async Task<ResultResponse<DeviceSettings>> GetSettingsAsync(string id)
        {
            try
            {
                var deviceSettings = await _context!.Table<DeviceSettings>().Where(sd => sd.Id == id).FirstOrDefaultAsync();

                if (deviceSettings != null)
                {
                    return ResultResponseFactory<DeviceSettings>.Success($"Settings for Id {deviceSettings.Id} were retrieved.", deviceSettings);
                }

                return ResultResponseFactory<DeviceSettings>.Failed("No device settings were found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while getting device settings.");
                return ResultResponseFactory<DeviceSettings>.Failed("An error occured while getting device settings.");
            }
        }

        public async Task<List<DeviceStateHistory>> GetDeviceHistory(string id)
        {
            try
            {
                if (id != null)
                {
                    var history = await _context!.Table<DeviceStateHistory>().Where(h => h.Id == id)
                        .OrderBy(h => h.TimeStamp)
                        .Take(10)
                        .ToListAsync();
                    return history;
                }
                else
                {
                    return null!;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null!;
            }

        }

        public async Task<ResultResponse> ResetSettingsAsync()
        {
            try
            {
                await _context!.DeleteAllAsync<DeviceSettings>();
                var deviceSettings = await _context.Table<DeviceSettings>().ToListAsync();
                if (deviceSettings.Count == 0)
                {
                    return ResultResponseFactory.Success("Settings were reset successfully!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to reset.");
                return ResultResponseFactory<DeviceSettings>.Failed("Settings were not reset successfully.");
            }

            _logger.LogError("Failed to reset.");
            return ResultResponseFactory<DeviceSettings>.Failed("");
        }

        public async Task<ResultResponse> SaveSettingsAsync(DeviceSettings settings, DeviceStateHistory history)
        {
            try
            {
                if (string.IsNullOrEmpty(settings.Id))
                {
                    _logger.LogError("Failed to save settings: ID is null or empty.");
                    return ResultResponseFactory.Failed("Settings ID is null or empty.");
                }

                var response = await GetSettingsAsync(settings.Id);

                if (response.Content != null)
                {
                    response.Content.Location = settings.Location;
                    response.Content.ConnectionString = settings.ConnectionString;
                    response.Content.Type = settings.Type;
                    response.Content.DeviceState = settings.DeviceState;

                    await _context!.UpdateAsync(settings);

                    if (history != null)
                    {
                        await _context!.InsertAsync(history);
                    }

                    return ResultResponseFactory.Success("Settings were updated successfully!");


                }
                await _context!.InsertAsync(settings);



                return ResultResponseFactory.Success("Settings were inserted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save settings: {ex.Message}");
                return ResultResponseFactory<DeviceSettings>.Failed("Failed to save settings.");
            }

        }

        public async Task<ResultResponse> DeleteDeviceSettingsAsync(DeviceSettings device)
        {
            try
            {
                if (device != null)
                {
                    await _context!.DeleteAsync(device);
                    return ResultResponseFactory.Success("Device was succecsfully deleted.");
                }
                return ResultResponseFactory.Failed("Failed to delete.");
            }
            catch (Exception ex)
            {
                return ResultResponseFactory<DeviceSettings>.Failed("Id was not found.");
            }
        }

        public async Task<ResultResponse> RegisterEmailAddress(HubSettings settings)
        {
            var existingSettings = await _context!.Table<HubSettings>().FirstOrDefaultAsync();

            if (existingSettings != null)
            {
                var query = "UPDATE HubSettings SET Email = ?";
                await _context.ExecuteAsync(query, settings.Email);
                return ResultResponseFactory.Success("Email updated.");
            }
            else
            {
                await _context.InsertAsync(settings);
                return ResultResponseFactory.Failed("Update failed.");
            }
        }



        public async Task<string> GetRegisteredEmailAsync()
        {
            try
            {
                var email = await _context!.Table<HubSettings>().FirstOrDefaultAsync();
                if (email != null)
                {
                    return email.Email;
                }
                else
                {
                    return "Example.live.se";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }

        }
    }
}
