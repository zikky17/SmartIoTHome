﻿using Microsoft.Extensions.Logging;
using SharedResources.Factories;
using SharedResources.Models;
using SQLite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Data
{
    internal class SQLiteContext
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

                    _logger.LogInformation("Database tables were created successfully.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while creating the database tables.");
            }
        }

        public async Task<ResultResponse<DeviceSettings>> GetSettingsAsync()
        {
            try
            {
                var deviceSettings = (await _context!.Table<DeviceSettings>().ToListAsync()).SingleOrDefault();
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

        public async Task<ResultResponse> SaveSettingsAsync(DeviceSettings settings)
        {
            try
            {
                if (!string.IsNullOrEmpty(settings.Id))
                {
                    var response = await GetSettingsAsync();

                    if (response.Content != null)
                    {
                        response.Content.Location = settings.Location;
                        response.Content.ConnectionString = settings.ConnectionString;
                        response.Content.Type = settings.Type;

                        await _context!.UpdateAsync(settings);
                        return ResultResponseFactory.Success("Settings were updated successfully!");
                    }
                    else
                    {
                        await _context!.InsertAsync(settings);
                    }

                }

                _logger.LogError($"Failed to save settings: ID is null or enmpty.");
                return ResultResponseFactory.Success("Settingss were inserted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to reset.");
                return ResultResponseFactory<DeviceSettings>.Failed("");
            }
        }
    }
}
