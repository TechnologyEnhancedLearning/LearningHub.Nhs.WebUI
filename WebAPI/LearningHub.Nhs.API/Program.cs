#pragma warning disable SA1200 // Using directives should be placed correctly
using System;
using LearningHub.Nhs.Api;
using LearningHub.Nhs.Api.Middleware;
using Microsoft.AspNetCore.Builder;
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
    builder.Host.UseNLog();

    builder.Services.ConfigureServices(builder.Configuration);

    var app = builder.Build();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<NLogMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/{app.Configuration["Swagger:Title"]}/swagger.json", app.Configuration["Swagger:Version"]);
    });

    app.UseMiddleware<ExceptionMiddleware>();

    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("content-security-policy", "default-src 'self'; " + $"script-src 'self' 'nonce-random772362' https://script.hotjar.com https://www.google-analytics.com https://static.hotjar.com https://www.googletagmanager.com https://cdnjs.cloudflare.com 'unsafe-hashes' 'sha256-oywvD6W6okwID679n4cvPJtWLowSS70Pz87v1ryS0DU=' 'sha256-kbHtQyYDQKz4SWMQ8OHVol3EC0t3tHEJFPCSwNG9NxQ' 'sha256-YoDy5WvNzQHMq2kYTFhDYiGnEgPrvAY5Il6eUu/P4xY=' 'sha256-/n13APBYdqlQW71ZpWflMB/QoXNSUKDxZk1rgZc+Jz8='   'sha256-+6WnXIl4mbFTCARd8N3COQmT3bJJmo32N8q8ZSQAIcU=' 'sha256-VQKp2qxuvQmMpqE/U/ASQ0ZQ0pIDvC3dgQPPCqDlvBo=';" + "style-src 'self' 'unsafe-inline' https://use.fontawesome.com; " + "font-src https://script.hotjar.com https://assets.nhs.uk/; " + "connect-src 'self' http: ws:; " + "img-src 'self' data: https:; " + "frame-src 'self' https:");
        context.Response.Headers.Add("Referrer-Policy", "no-referrer");
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "deny");
        context.Response.Headers.Add("X-XSS-protection", "0");
        await next();
    });

    app.UseEndpoints(endpoints => endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"));

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