﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SharedResources.Data;
using SharedResources.Handlers;
using SharedResources.Services;
using SmartHub.Components.Pages;
using SmartHub.ViewModels;

namespace SmartHub
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                }
               
                );

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddSingleton<IDbContextMAUI>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<SQLiteContextMAUI>>();
                return new SQLiteContextMAUI(logger, () =>
                {
                    var databaseFolderPath = @"C:\Databases";

                    if (!Directory.Exists(databaseFolderPath))
                    {
                        Directory.CreateDirectory(databaseFolderPath);
                    }

                    return databaseFolderPath;
                });

            });

    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();

            builder.Services.AddTransient<HomeVM>();
            builder.Services.AddTransient<AzureHub>();
            builder.Services.AddTransient<SettingsVM>();
            builder.Services.AddTransient<Settings>();
            builder.Services.AddTransient<NewDeviceVM>();
            builder.Services.AddTransient<DeviceSettings>();
            builder.Services.AddTransient<DeviceSettingsVM>();
            builder.Services.AddScoped<DeviceStateService>();

            builder.Services.AddScoped(sp => new HttpClient());

            return builder.Build();
        }
    }
}
