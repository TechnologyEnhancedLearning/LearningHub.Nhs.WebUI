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

    ////app.Use(async (context, next) =>
    ////{
    ////    context.Response.Headers.Add("content-security-policy", "object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts allow-popups; base-uri 'self';");
    ////    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    ////    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    ////    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    ////    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    ////    context.Response.Headers.Add("X-XSS-protection", "0");
    ////    await next();
    ////});

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
