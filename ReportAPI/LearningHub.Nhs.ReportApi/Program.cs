// <copyright file="Program.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly
using LearningHub.Nhs.ReportApi;
using LearningHub.Nhs.ReportApi.Middleware;
using NLog;
using NLog.Web;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

try
{
    logger.Debug("Log Started");

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

    var app = builder.Build();

    app.UseRouting();
    app.UseAuthorization();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/{app.Configuration["Swagger:Title"]}/swagger.json", app.Configuration["Swagger:Version"]);
    });

    app.UseMiddleware<ExceptionMiddleware>();

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
