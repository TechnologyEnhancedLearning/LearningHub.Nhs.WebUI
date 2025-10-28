using LearningHub.Nhs.WebUI.TelBlazorClient;
using LearningHub.Nhs.WebUI.TelBlazorClient.Shared.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.RootComponents.Add<TELAutoSuggest>("#tel-auto-suggest");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

///// Start Blazor
//var host = builder.Build();

//// Register multiple root components by selector
//var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();

//await host.RunAsync(); // Start the WASM runtime

// Attach multiple components (these are matched by selector from MVC HTML)
//await jsRuntime.InvokeVoidAsync("Blazor.rootComponents.add", "#tel-auto-suggest", typeof(AutoSuggest));
////await jsRuntime.InvokeVoidAsync("Blazor.rootComponents.add", "#date-picker", typeof(DatePicker));
////await jsRuntime.InvokeVoidAsync("Blazor.rootComponents.add", "#rich-editor", typeof(RichTextEditor));
