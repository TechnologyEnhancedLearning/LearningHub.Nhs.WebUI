//using LearningHub.Nhs.Models.Entities;
// Still required server side even if not used so components dont fail
using Blazored.LocalStorage;
using LearningHub.Nhs.WebUI.BlazorClient.DI;

using LearningHub.Nhs.Caching;
using LearningHub.Nhs.Shared.Configuration;
using LearningHub.Nhs.Shared.Interfaces;
using LearningHub.Nhs.Shared.Interfaces.Configuration;
using LearningHub.Nhs.Shared.Interfaces.Http;
using LearningHub.Nhs.Shared.Interfaces.Services;
using LearningHub.Nhs.Shared.Services;
using LearningHub.Nhs.WebUI.BlazorClient.Services;
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


var builder = WebAssemblyHostBuilder.CreateDefault(args);

var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var env = builder.HostEnvironment.Environment;
using var envSettings = await http.GetStreamAsync($"appsettings.{env}.json");

builder.Configuration.AddJsonStream(envSettings);

builder.Services.Configure<ExposableSettings>(builder.Configuration.GetSection("Settings"));
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


    // Register your BFF using httpclient ILearningHubHttpClient
    builder.Services.AddBffHttpClient<ILearningHubHttpClient, GenericAPIHttpClient>(settings => settings.LearningHubApiUrl);

    // Register your BFF using httpclient IUserApiHttpClient
    builder.Services.AddBffHttpClient<IUserApiHttpClient, GenericAPIHttpClient>(settings => settings.UserApiUrl);

    // Register your BFF using httpclient IOpenApiHttpClient
    builder.Services.AddBffHttpClient<IOpenApiHttpClient, GenericAPIHttpClient>(settings => settings.OpenApiUrl);


    builder.Services.AddScoped<LoggingLevelSwitch>(sp => levelSwitch);
    builder.Services.AddScoped<ILogLevelSwitcherService, SerilogLogLevelSwitcherService>();

    builder.Services.AddScoped<ICacheService, WasmCacheServiceStub>();

    // qqqq will go in a shared DI service collection extension
    builder.Services.AddScoped<IProviderService, ProviderService>();
    builder.Services.AddScoped<ISearchService, SearchService>();

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