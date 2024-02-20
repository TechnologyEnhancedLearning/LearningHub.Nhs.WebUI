// <copyright file="SeleniumServerFactory.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.AutomatedUiTests
{
    using System;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using LearningHub.Nhs.WebUI.Startup;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;
    using Serilog;

    /// <summary>
    /// SeleniumServerFactory.
    /// </summary>
    /// <typeparam name="TStartup">TStartup.</typeparam>
    public class SeleniumServerFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        /// <summary>
        /// Root Uri.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        public string RootUri;
#pragma warning restore SA1401 // Fields should be private
        private IWebHost host;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumServerFactory{TStartup}"/> class.
        /// </summary>
        public SeleniumServerFactory()
        {
            IConfiguration configuration = ConfigurationHelper.GetConfiguration();
            this.RootUri = configuration["LearningHubWebUiUrl"];

            // We are consuming from  IIS express
            // this.CreateServer(this.CreateWebHostBuilder());
            // this.CreateClient();
        }

        /// <summary>
        /// Create Server.
        /// </summary>
        /// <param name="builder">builder.</param>
        /// <returns>TestServer.</returns>
        protected sealed override TestServer CreateServer(IWebHostBuilder builder)
        {
           // base.ConfigureWebHost(builder);

            // Real TCP port
            var host = builder
                .UseStartup<TStartup>()
                .UseSerilog()
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddConfiguration(GetConfigForUiTests());
                })
                .Build();

            host.Start();
            var rootUri = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();

            // Fake Server to satisfy the return type
            return new TestServer(
                new WebHostBuilder()
                    .UseStartup<TStartup>()
                    .UseSerilog()
                    .ConfigureAppConfiguration(
                        configBuilder => { configBuilder.AddConfiguration(GetConfigForUiTests()); }));
        }

        /// <summary>
        /// Create Web Host Builder.
        /// </summary>
        /// <returns>CreateDefaultBuilder.</returns>
        protected sealed override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(Array.Empty<string>())
                .UseStartup<TStartup>()
                .UseSerilog()
                .UseUrls("http://127.0.0.1:0")
                .ConfigureAppConfiguration(
                    configBuilder =>
                    {
                        var jsonConfigSources = configBuilder.Sources
                            .Where(source => source.GetType() == typeof(JsonConfigurationSource))
                            .ToList();

                        foreach (var jsonConfigSource in jsonConfigSources)
                        {
                            configBuilder.Sources.Remove(jsonConfigSource);
                        }

                        configBuilder.AddConfiguration(GetConfigForUiTests());
                    });
        }

        /// <summary>
        /// Dispose implementation.
        /// </summary>
        /// <param name="disposing">disposing.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.host?.Dispose();
            }
        }

        /// <summary>
        /// Get Config ForUi Tests.
        /// </summary>
        /// <returns>ConfigurationBuilder.</returns>
        private static IConfigurationRoot GetConfigForUiTests()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .AddEnvironmentVariables("DlsRefactor_")
                .AddUserSecrets(typeof(Program).Assembly)
                .Build();
        }
    }
}
