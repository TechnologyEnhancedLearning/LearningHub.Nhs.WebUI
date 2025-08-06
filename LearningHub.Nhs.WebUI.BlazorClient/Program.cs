//using LearningHub.Nhs.Models.Entities;
// Still required server side even if not used so components dont fail
using Blazored.LocalStorage;

// qqqq Test try without aspentcore library contamination
//using LearningHub.Nhs.Caching;
//using LearningHub.Nhs.Shared.Configuration;
//using LearningHub.Nhs.Shared.Interfaces;
//using LearningHub.Nhs.Shared.Interfaces.Configuration;
//using LearningHub.Nhs.Shared.Interfaces.Http;
//using LearningHub.Nhs.Shared.Interfaces.Services;
//using LearningHub.Nhs.Shared.Services;
using LearningHub.Nhs.WebUI.BlazorClient.Services;
using LearningHub.Nhs.WebUI.BlazorClient.TestDeleteMe.FromShared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
// Serilog core (used via appsettings, do not delete even if vs marks not in use)
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
// Serilog extensions and sinks (used via appsettings, do not delete even if vs marks not in use)
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
using Serilog.Settings.Configuration;
using Serilog.Sinks.BrowserConsole;
using System;
using TELBlazor.Components.Core.Configuration;
using TELBlazor.Components.Core.Services.HelperServices;
using TELBlazor.Components.OptionalImplementations.Core.Services.HelperServices;

// qqqq should be loading the appsettings apparently
var builder = WebAssemblyHostBuilder.CreateDefault(args);


var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var env = builder.HostEnvironment.Environment;
using var envSettings = await http.GetStreamAsync($"appsettings.{env}.json");

builder.Configuration.AddJsonStream(envSettings);

builder.Services.Configure<PublicSettings>(builder.Configuration.GetSection("Settings"));
builder.Logging.ClearProviders();

// Read default logging level from configuration
var logLevelString = builder.Configuration["Serilog:MinimumLevel:Default"];
// Convert string to LogEventLevel (with fallback)
if (!Enum.TryParse(logLevelString, true, out LogEventLevel defaultLogLevel))
{
    defaultLogLevel = LogEventLevel.Information; // Default if parsing fails
}

// Create a LoggingLevelSwitch that can be updated dynamically
LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch(defaultLogLevel); // Default: Information added this so in production can change the logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.ControlledBy(levelSwitch)
    .CreateLogger();

// Add Serilog to logging providers
builder.Logging.AddSerilog(Log.Logger, dispose: true);//qqqq may not need dispose for client

//for really bad fails
try
{
    // Candidates for DI collection
    builder.Services.AddSingleton<ITELBlazorBaseComponentConfiguration>(sp =>
    {
        return new TELBlazorBaseComponentConfiguration
        {
            JSEnabled = true, //if we are inject the client then it is true
            HostType = $"{builder.Configuration["Properties:Environment"]} {builder.Configuration["Properties:Application"]}"
        };
    });

    builder.Services.AddBlazoredLocalStorage();



    /* qqqq
        // "LearningHubApiUrl": "https://lh-api.dev.local/api/",
        // "LearningHubApiBFFUrl": "https://bff/lh-api.dev.local/api/",
        // IOptions<Settings> webSettings,
        // this.WebSettings.LearningHubApiUrl;
        client.BaseAddress = new Uri(publicSettings.LearningHubApiBFFUrl); // or whatever your BFF port is
    });
     */

    // Register your BFF - pointing clients
    builder.Services.AddHttpClient<ILearningHubHttpClientTest, GenericAPIHttpClient>((serviceProvider, client) =>
    {
        IPublicSettings publicSettings = serviceProvider.GetRequiredService<IOptions<PublicSettings>>().Value;

        // Log the URL to the console for debugging purposes.
        // qqqq
        Console.WriteLine($"[Configuration Log] Using Learning Hub API BFF URL: {publicSettings.LearningHubApiBFFUrl}");
        Console.WriteLine($"public settings: {publicSettings}");
        Uri apiUri = new Uri(publicSettings.LearningHubApiUrl);
        string apiHost = apiUri.Host;
        // qqqq may need forward slases
        client.BaseAddress = new Uri($"{publicSettings.LearningHubApiBFFUrl}{apiHost}/"); // or whatever your BFF port is
    });


    //builder.Services.AddHttpClient<IUserApiHttpClient, GenericAPIHttpClient>(client =>
    //{
    //    client.BaseAddress = new Uri("https://localhost:5001/bff/lh-userapi/");
    //});
    //// LearningHubApiUrl-> //"LearningHubApiUrl": "https://lh-web.dev.local/bff/Catalogue/GetLatestCatalogueAccessRequest/500/lh-api.dev.local/api/", -->  //"LearningHubApiUrl": "https://lh-api.dev.local/api/",

    //builder.Services.AddHttpClient<ILearningHubHttpClient, GenericAPIHttpClient>(client =>
    //{
    //    client.BaseAddress = new Uri("https://lh-web.dev.local/bff/lh-api.dev.local/"); // or whatever your BFF port is
    //});

    //// "UserApiUrl": "https://bff/lh-userapi.dev.local/api/",  --> //"UserApiUrl": "https://lh-userapi.dev.local/api/",
    //builder.Services.AddHttpClient<IUserApiHttpClient, GenericAPIHttpClient>(client =>
    //{
    //    client.BaseAddress = new Uri("https://lh-web.dev.local/bff/lh-userapi.dev.local/");
    //});


    builder.Services.AddScoped<LoggingLevelSwitch>(sp => levelSwitch);
    builder.Services.AddScoped<ILogLevelSwitcherService, SerilogLogLevelSwitcherService>();

    // qqqq will need back in post removing asp.net shared
    //builder.Services.AddScoped<ICacheService, WasmCacheServiceStub>(); // had to change provider not to use it previously trying again
    //builder.Services.AddScoped<IProviderService, ProviderService>();

    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    //If in production as requires sending to api we may never receive it
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush(); // Ensure logs are flushed before exit
}