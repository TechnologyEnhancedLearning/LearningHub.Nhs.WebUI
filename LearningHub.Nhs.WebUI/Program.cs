#pragma warning disable SA1200 // Using directives should be placed correctly
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using LearningHub.Nhs.WebUI;
using LearningHub.Nhs.WebUI.Interfaces;
using LearningHub.Nhs.WebUI.JsDetection;
using LearningHub.Nhs.WebUI.Middleware;
using LearningHub.Nhs.WebUI.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using tusdotnet;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;

#pragma warning restore SA1200 // Using directives should be placed correctly

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

try
{
    logger.Debug("Log Started");

    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddUserSecrets("a2ecb5d2-cf13-4551-9cb6-3d86dfbcf8ef");

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.AddHostedService<TimedHostedService>();
    builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

    var app = builder.Build();

    var appLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    var jsDetectionLogger = app.Services.GetRequiredService<IJsDetectionLogger>();
    appLifetime.ApplicationStopping.Register(async () => await jsDetectionLogger.FlushCounters());

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.Use((context, next) =>
        {
            var learningUri = new Uri(app.Configuration["Settings:LearningHubWebUiUrl"]);
            context.Request.Host = new HostString(learningUri.Host);
            return next();
        });

        app.UseForwardedHeaders();

        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

    // Custom redirect doesn't work when the port is part of the url explicitly,
    // as the http port will be different from the https port.
    // Will work on deployed servers with proper dns
    // Debugger check is to keep it working locally, but not perfect
    // ideally we would check to see what the https/http ports are
    if (!Debugger.IsAttached)
    {
        app.UseCustomHttpsRedirection();
    }
    else
    {
        app.UseHttpsRedirection();
    }

    // Proxy requests to the Blazor app qqqq
    // so we would need to properly set up cors cross domain so maybe change so name domain
    // https://lh-web.dev.local/SeperatelyHostedBlazor/books
    // Going to leave it for now maybe
    //app.MapWhen(context => context.Request.Path.StartsWithSegments("/SeperatelyHostedBlazor"), subApp =>
    //{
    //    subApp.Run(async context =>
    //    {
    //        var targetUri = new Uri("https://blazorecommerce.dev.local/" + context.Request.PathBase + context.Request.Path + context.Request.QueryString);

    //        // Reuse HttpClient
    //        using var handler = new HttpClientHandler
    //        {
    //            // Disable SSL certificate validation only in development
    //            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
    //                builder.Environment.IsDevelopment() || sslPolicyErrors == System.Net.Security.SslPolicyErrors.None,
    //        };

    //        using var client = new HttpClient(handler);
    //        var requestMessage = new HttpRequestMessage(HttpMethod.Get, targetUri);

    //        foreach (var header in context.Request.Headers)
    //        {
    //            // Add headers to the request, excluding certain headers
    //            if (!header.Key.Equals("Connection", StringComparison.OrdinalIgnoreCase) &&
    //                !header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
    //            {
    //                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
    //            }
    //        }

    //        var responseMessage = await client.SendAsync(requestMessage);

    //        context.Response.StatusCode = (int)responseMessage.StatusCode;
    //        foreach (var header in responseMessage.Headers)
    //        {
    //            context.Response.Headers[header.Key] = header.Value.ToArray();
    //        }

    //        await responseMessage.Content.CopyToAsync(context.Response.Body);
    //    });
    //});
    app.MapWhen(context => context.Request.Path.StartsWithSegments("/SeperatelyHostedBlazor"), subApp =>
    {
        subApp.Run(async context =>
        {
            var blazorBaseUrl = "https://blazorecommerce.dev.local";
            var targetUri = new Uri($"{blazorBaseUrl}{context.Request.Path}{context.Request.QueryString}");

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                    builder.Environment.IsDevelopment() || sslPolicyErrors == System.Net.Security.SslPolicyErrors.None,
            };

            using var client = new HttpClient(handler);

            // Clone the original request into the proxy request
            var proxyRequest = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUri);

            // Copy headers from original request, excluding some that should not be forwarded
            foreach (var header in context.Request.Headers)
            {
                if (!header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase) &&
                    !header.Key.Equals("Connection", StringComparison.OrdinalIgnoreCase))
                {
                    proxyRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // If the original request has a body, copy that to the proxy request
            if (context.Request.ContentLength.HasValue && context.Request.ContentLength.Value > 0)
            {
                proxyRequest.Content = new StreamContent(context.Request.Body);
            }

            // Send the request to the Blazor server
            var response = await client.SendAsync(proxyRequest);

            // Copy response status code
            context.Response.StatusCode = (int)response.StatusCode;

            // Copy response headers from the Blazor server response to the original response
            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in response.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            // Ensure chunked encoding is removed, as it's already handled
            context.Response.Headers.Remove("transfer-encoding");

            // Copy the response body to the client
            await response.Content.CopyToAsync(context.Response.Body);
        });
    });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<NLogMiddleware>();

    app.UseStaticFiles();

    app.Map(TimezoneInfoMiddleware.TimezoneInfoUrl, b => b.UseMiddleware<TimezoneInfoMiddleware>());

    app.UseEndpoints(endpoints => endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"));

    app.UseTus(httpContext =>
    {
        using var scope = app.Services.CreateScope();

        var store = scope.ServiceProvider.GetRequiredService<IPartialFileUploadService>();

        return new DefaultTusConfiguration
        {
            UrlPath = "/api/file-uploads",
            Store = store,
            Events = new Events
            {
                OnAuthorizeAsync = store.OnAuthoriseAsync,
                OnFileCompleteAsync = store.OnFileCompleteAsync,
            },
        };
    });

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Exception in Main (CreateWebHostBuilder) caused");
    throw;
}
finally
{
    LogManager.Shutdown();
}