// <copyright file="Program.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly
using System;
using LearningHub.Nhs.AdminUI;
using LearningHub.Nhs.AdminUI.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
#pragma warning restore SA1200 // Using directives should be placed correctly

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

try
{
    logger.Debug("Log Started");

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Logging.AddConsole();
    builder.Host.UseNLog();

    builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.Use((context, next) =>
        {
            var hostUri = new Uri(app.Configuration["WebSettings:LearningHubAdminUrl"]);
            context.Request.Host = new HostString(hostUri.Host);
            return next();
        });

        app.UseForwardedHeaders();

        app.UseExceptionHandler("/Home/Error");
    }

    app.UseCookiePolicy();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<NLogMiddleware>();

    app.Map(TimezoneInfoMiddleware.TimezoneInfoUrl, b => b.UseMiddleware<TimezoneInfoMiddleware>());

    app.UseEndpoints(
        endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
        });

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Exception in caused");
    throw;
}
finally
{
    LogManager.Shutdown();
}