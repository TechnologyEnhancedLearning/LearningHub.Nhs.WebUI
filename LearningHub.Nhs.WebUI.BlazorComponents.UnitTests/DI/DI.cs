using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Sinks.InMemory;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
//using Serilog.Sinks.XUnit; //This defines testoutput
using Microsoft.Extensions.Logging.Abstractions;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Formatting.Json;
using Serilog.Formatting.Compact;
using Serilog.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LearningHub.Nhs.WebUI.BlazorComponents.UnitTests.DI
{
    public static class DI
    {
        /// <summary>
        /// AddLogging XUNIT this is Test Explorer logging, not for Asserts
        /// InMemory for Asserting
        /// </summary>
        /// <param name="services"></param>
        /// <param name="outputHelper"></param>
        /// <returns></returns>

        public static IServiceCollection AddLogging(this IServiceCollection services, ITestOutputHelper outputHelper, InMemorySink inMemorySink)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Create configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                .Build();


            Log.Logger = new LoggerConfiguration()
                .WriteTo.TestOutput(outputHelper)
                .WriteTo.Sink(inMemorySink)
                .ReadFrom.Configuration(configuration) //this does not apply to custom sinks so custom sinks need to be dot notation configured. The config can still be used for setting up the serilog mainstream packages settings
                .CreateLogger();

            //qqqq
            //They are not Serilog packages! so config may not work unless directly mapped.
            //.WriteTo.TestOutput(testOutputHelper: outputHelper, //WORKS if we want to specify
            //    formatter: new ExpressionTemplate("[{UtcDateTime(@t):mm:ss.ffffff} | {@l:u3} | {Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)} | {Coalesce(EventId.Name, '<none>')}] {@m}\n{@x}"),
            //    restrictedToMinimumLevel: LogEventLevel.Verbose)
            //.WriteTo.Sink<InMemorySink>(inMemorySink, // DOESNT WORK this package cant be set like this
            //   new ExpressionTemplate("[{UtcDateTime(@t):mm:ss.ffffff} | {@l:u3} | {Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)} | {Coalesce(EventId.Name, '<none>')}] {@m}\n{@x}"),
            //   LogEventLevel.Verbose)

            services.AddSingleton<ILoggerFactory>(_ => new LoggerFactory().AddSerilog(Log.Logger, dispose: true));
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            return services;
        }
    }
}
