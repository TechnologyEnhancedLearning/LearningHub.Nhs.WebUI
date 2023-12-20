// <copyright file="Program.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly
using System;
using System.Diagnostics;
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

    builder.Configuration.AddUserSecrets(string.Empty);

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