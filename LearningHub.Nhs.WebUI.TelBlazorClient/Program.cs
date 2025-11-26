using LearningHub.Nhs.WebUI.TelBlazorClient;
using LearningHub.Nhs.WebUI.TelBlazorClient.Configuration;
using LearningHub.Nhs.WebUI.TelBlazorClient.Services;
using LearningHub.Nhs.WebUI.TelBlazorClient.Shared.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");

builder.RootComponents.Add<TELAutoSuggest>("#tel-auto-suggest");

// Step 1: Create temporary HttpClient for config loading
using var tempClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

// Step 2: Load appsettings.json
//var apiSettings = await tempClient.GetFromJsonAsync<Settings>("appsettings.development.json")
//                  ?? new Settings();

var apiSettings = new Settings
{
    BaseUrl = "https://lh-openapi.dev.local/",
    ApiKey = ""
};

// Step 3: Register services with DI
builder.Services.AddSingleton(apiSettings);
builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddScoped<AppSettingsService>();

await builder.Build().RunAsync();