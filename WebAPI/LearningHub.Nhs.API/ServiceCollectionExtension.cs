namespace LearningHub.Nhs.Api
{
    using System;
    using System.IO;
    using System.Reflection;
    using AutoMapper;
    using elfhHub.Nhs.Models.Automapper;
    using IdentityServer4.AccessTokenValidation;
    using LearningHub.Nhs.Api.Authentication;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// ServiceCollectionExtension.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// ConfigureServices.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configuration">IConfiguration.</param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.AddApplicationInsightsTelemetry();

            services.AddLearningHubMappings(configuration);

            services.Configure<Settings>(configuration.GetSection("Settings"));

            // settings binding
            var settings = new Settings();
            configuration.Bind("Settings", settings);

            services.AddAuthentication(
                IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = settings.AuthenticationServiceUrl;
                    options.ApiName = "learninghubapi";
                });

            services.AddSingleton<IAuthorizationHandler, AuthorizeOrCallFromLHHandler>();
            services.AddSingleton<IAuthorizationHandler, ReadWriteHandler>();

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AllowNullCollections = true;
                mc.ShouldMapMethod = m => false;
                mc.AddProfile(new MappingProfile());
                mc.AddProfile(new ElfhMappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            var buildNumber = $"Build Number: {configuration["Swagger:BuildNumber"]}";
            var swaggerTitle = configuration["Swagger:Title"];
            var swaggerVersion = configuration["Swagger:Version"];
            var swaggerDescription = "This is the API documentation for " + buildNumber + " and version ";
            services.AddSwaggerGen(c =>
            {
                var version = typeof(Program).Assembly.GetName().Version;
                var versionString = $"{version?.Major}.{version?.Minor}.{version?.Build}";
                c.SwaggerDoc(swaggerTitle, new OpenApiInfo { Title = swaggerTitle, Version = swaggerVersion, Description = $"{swaggerDescription}{versionString}" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.XML"));
                c.CustomSchemaIds(type => type.ToString());
            });

            services.AddMvc()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddAuthorization(options =>
                {
                    options.AddPolicy(
                        "AuthorizeOrCallFromLH",
                        policy => policy.Requirements.Add(new AuthorizeOrCallFromLHRequirement()));

                    options.AddPolicy(
                        "ReadWrite",
                        policy => policy.Requirements.Add(new ReadWriteRequirement()));
                });

            var environment = configuration.GetValue<EnvironmentEnum>("Environment");
            var envPrefix = environment.GetAbbreviation();
            if (environment == EnvironmentEnum.Local)
            {
                envPrefix += $"_{Environment.MachineName}";
            }

            services.AddDistributedCache(option =>
            {
                option.RedisConnectionString = configuration.GetConnectionString("LearningHubRedis");
                option.KeyPrefix = envPrefix;
                option.DefaultExpiryInMinutes = 60;
            });

            services.AddSingleton(configuration);
            services.AddApplicationInsightsTelemetry();
        }
    }
}