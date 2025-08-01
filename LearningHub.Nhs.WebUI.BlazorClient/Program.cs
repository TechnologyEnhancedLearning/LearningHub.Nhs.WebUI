// Microsoft namespaces

//using LearningHub.Nhs.Models.Entities;
// Still required server side even if not used so components dont fail
using Blazored.LocalStorage;
using LearningHub.Nhs.Caching;
using LearningHub.Nhs.WebUI.BlazorClient.Services;
using LearningHub.Nhs.Shared.Configuration;
using LearningHub.Nhs.Shared.Interfaces;
using LearningHub.Nhs.Shared.Services;
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
using TELBlazor.Components.Core.Configuration;
using TELBlazor.Components.Core.Services.HelperServices;
using TELBlazor.Components.OptionalImplementations.Core.Services.HelperServices;
using TELBlazor.Components.OptionalImplementations.Test.TestComponents.SearchExperiment;
using LearningHub.Nhs.Shared.Interfaces.Services;




var builder = WebAssemblyHostBuilder.CreateDefault(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true);

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


    builder.Services.AddBlazoredLocalStorage(); //could make our own caching with this client side ... but needs careful consideration of managing two sets of cache for pages of mixed mvc and blazor components


    //this.findwiseSettings = findwiseSettings.Value;
    //this.publicSettings = publicSettings.Value;
    builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: true);
    builder.Services.Configure<PublicSettings>(builder.Configuration.GetSection("Settings"));

    //!!!!! builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));//qqqq we cannot do this in actuality there are security things inthsi
    //Scoped because being consumed with storage where singleton doesnt survive mvc page teardown
    //qqqq do we need it
    //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

    // Register your BFF-pointing clients

    //builder.Services.AddHttpClient<ILearningHubHttpClient, LearningHubBffClient>(client => {
    //    client.BaseAddress = new Uri("https://localhost:5001/bff/lh-web/"); // or whatever your BFF port is
    //});


    //we want to do this - oh but it wont be in domain!
    // it will be https://lh-web.dev.local/bff/ the api name so we need to set the name the same ... it doesnt need to be the same but may aswell
    // the other local ones are https://lh-web.dev.local/api/

    //"LearningHubUrl": "https://bff/lh-web.dev.local/",
    //"ELfhHubUrl": "https://bff/test-portal.e-lfhtech.org.uk ",
    //"LearningHubApiUrl": "https://bff/lh-api.dev.local/api/",
    //"UserApiUrl": "https://bff/lh-userapi.dev.local/api/",
    //"LearningHubAdminUrl": "https://bff/lh-admin.dev.local/",


    //"LearningHubUrl": "https://lh-web.dev.local/",
    //"ELfhHubUrl": "https://test-portal.e-lfhtech.org.uk ",
    //"LearningHubApiUrl": "https://lh-api.dev.local/api/",
    //"UserApiUrl": "https://lh-userapi.dev.local/api/",
    //"LearningHubAdminUrl": "https://lh-admin.dev.local/",



    //builder.Services.AddHttpClient<IUserApiHttpClient, UserApiBffClient>(client => {
    //    client.BaseAddress = new Uri("https://localhost:5001/bff/lh-userapi/");
    //});
    // LearningHubApiUrl -> //"LearningHubApiUrl": "https://lh-web.dev.local/bff/Catalogue/GetLatestCatalogueAccessRequest/500/lh-api.dev.local/api/", -->  //"LearningHubApiUrl": "https://lh-api.dev.local/api/",

    // qqqq put back in
    //builder.Services.AddHttpClient<ILearningHubHttpClient, GenericAPIHttpClient>(client => {
    //    client.BaseAddress = new Uri("https://lh-web.dev.local/bff/lh-api.dev.local/"); // or whatever your BFF port is
    //});

    //// //"UserApiUrl": "https://bff/lh-userapi.dev.local/api/",  --> //"UserApiUrl": "https://lh-userapi.dev.local/api/",
    //builder.Services.AddHttpClient<IUserApiHttpClient, GenericAPIHttpClient>(client => {
    //    client.BaseAddress = new Uri("https://lh-web.dev.local/bff/lh-userapi.dev.local/");
    //});

    builder.Services.AddScoped<LoggingLevelSwitch>(sp => levelSwitch);
    builder.Services.AddScoped<ILogLevelSwitcherService, SerilogLogLevelSwitcherService>();

    builder.Services.AddScoped<ICacheService, WasmCacheServiceStub>(); // had to change provider not to use it previously trying again
    builder.Services.AddScoped<IProviderService, ProviderService>();



    //qqqq Register IOptions<Settings> - requires manual binding in WASM
    // --> qqqq MONDAY Important: In Blazor WASM, IOptions<T> works only if the configuration is available at runtime.That means you must ensure that your appsettings.json or other configuration is correctly loaded into builder.Configuration.



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