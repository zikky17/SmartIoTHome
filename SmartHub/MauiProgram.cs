﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SharedResources.Handlers;
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
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<HomeVM>();
            builder.Services.AddTransient<AzureHub>();

            return builder.Build();
        }
    }
}
