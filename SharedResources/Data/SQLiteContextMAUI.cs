﻿using Microsoft.Extensions.Logging;
using SharedResources.Factories;
using SharedResources.Models;
using SQLite;
using System.Diagnostics;

namespace SharedResources.Data
{
    public class SQLiteContextMAUI : IDbContextMAUI
    {
        private readonly ILogger<SQLiteContextMAUI> _logger;
        private readonly SQLiteAsyncConnection? _context;

        public SQLiteContextMAUI(ILogger<SQLiteContextMAUI> logger, Func<string> directoryPath, string databaseName = "SmartIoTHome_MAUI_DB.db3")
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
                    _logger.LogInformation(databasePath);
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
                    await _context.CreateTableAsync<HubSettings>();
                    await _context.CreateTableAsync<DeviceStateHistory>();

                    _logger.LogInformation("Database tables were created successfully.");
                    var hubConnectionString = "HostName=gurra-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=/6xdlTOp1WhRRbgMsWuAS+FCnQSBLRI9BAIoTAU4LdE=";

                    try
                    {
                        var settings = await _context.Table<HubSettings>().FirstOrDefaultAsync();

                        if (settings == null || settings.HubConnectionString == null)
                        {
                            var newSettings = new HubSettings
                            {
                                HubConnectionString = hubConnectionString
                            };

                            await _context.InsertAsync(newSettings);
                            _logger.LogInformation("HubConnectionString has been inserted successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while creating the database tables.");
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

        public async Task<string> GetHubConnectionString()
        {
            try
            {
                var latestHubSettings = await _context!.Table<HubSettings>()
                        .FirstOrDefaultAsync();
                if (latestHubSettings != null)
                {
                    return latestHubSettings.HubConnectionString;
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
    }
}
