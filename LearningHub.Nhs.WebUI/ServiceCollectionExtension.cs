namespace LearningHub.Nhs.WebUI
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Binders;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.EventSource;
    using LearningHub.Nhs.WebUI.Handlers;
    using LearningHub.Nhs.WebUI.Services;
    using LearningHub.Nhs.WebUI.Startup;
    using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.FeatureManagement;
    using Microsoft.IdentityModel.Logging;

    /// <summary>
    /// The ServiceCollectionExtension.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// ConfigureServices.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="env">IWebHostEnvironment.</param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddApplicationInsightsTelemetry();

            services.ConfigureTelemetryModule<EventCounterCollectionModule>(
                    (module, o) =>
                    {
                        module.Counters.Clear();
                        module.Counters.Add(new EventCounterCollectionRequest(ScormContentEventSource.SourceName, "request-count"));
                        module.Counters.Add(new EventCounterCollectionRequest(ScormContentEventSource.SourceName, "request-count-delta"));
                        module.Counters.Add(new EventCounterCollectionRequest(ScormContentEventSource.SourceName, "request-duration"));
                        module.Counters.Add(new EventCounterCollectionRequest(ScormContentEventSource.SourceName, "request-byte-served"));
                        module.Counters.Add(new EventCounterCollectionRequest(ScormContentEventSource.SourceName, "mp4-request-duration"));
                        module.Counters.Add(new EventCounterCollectionRequest(ScormContentEventSource.SourceName, "mp4-request-byte-served"));
                    });

            services.AddOptions();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .AddMvcOptions(options =>
                {
                    var readerFactory = services.BuildServiceProvider().GetRequiredService<IHttpRequestStreamReaderFactory>();
                    options.ModelBinderProviders.Insert(0, new SanitizedHtmlModelBinderProvider(options.InputFormatters, readerFactory));
                    options.CacheProfiles.Add(
                           "Never",
                           new CacheProfile()
                           {
                               Location = ResponseCacheLocation.None,
                               NoStore = true,
                           });
                })
                  .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var settingsSection = configuration.GetSection("Settings");

            services.Configure<Settings>(settingsSection);
            services.AddSingleton(settingsSection.Get<Settings>());

            services.AddLearningHubMappings(configuration, env);

            // Get Auth service config and add OpenID Connect Authentication to this client
            var learningHubAuthSvcConf = configuration.GetSection(nameof(LearningHubAuthServiceConfig))
                .Get<LearningHubAuthServiceConfig>();
            services.AddTransient<CookieEventHandler>();
            services.AddSingleton<LogoutUserManager>();
            services.AddSingleton<VersionService>();

            services.ConfigureAuthentication(learningHubAuthSvcConf);

            services.Configure<ForwardedHeadersOptions>(
            options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();

                var knownProxies = settingsSection.GetValue<string>("KnownProxies");
                if (!string.IsNullOrEmpty(knownProxies))
                {
                    foreach (var ipAddress in knownProxies.Split(new char[] { ',' }))
                    {
                        options.KnownProxies.Add(IPAddress.Parse(ipAddress));
                    }
                }
            });

            // this method setup so httpcontext is available from controllers
            services.AddHttpContextAccessor();
            services.AddSingleton(learningHubAuthSvcConf);

            IdentityModelEventSource.ShowPII = true;

            var environment = configuration.GetValue<EnvironmentEnum>("Environment");
            var envPrefix = environment.GetAbbreviation();
            if (environment == EnvironmentEnum.Local)
            {
                envPrefix += $"_{Environment.MachineName}";
            }

            // Set up redis caching.
            services.AddDistributedCache(opt =>
            {
                opt.RedisConnectionString = configuration.GetConnectionString("LearningHubRedis");
                opt.KeyPrefix = $"{envPrefix}_WebUI";
                opt.DefaultExpiryInMinutes = 60;
            });

            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddControllersWithViews();

            services.AddFeatureManagement();
        }
    }
}