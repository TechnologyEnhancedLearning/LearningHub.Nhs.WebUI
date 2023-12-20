// <copyright file="ServiceCollectionExtension.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi
{
    using System;
    using System.IO;
    using System.Reflection;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.ReportApi.Authentication;
    using LearningHub.Nhs.ReportApi.Shared.Configuration;
    using LearningHub.Nhs.ReportingApi;
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
        /// <param name="env">IWebHostEnvironment.</param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddOptions();

            services.AddApplicationInsightsTelemetry();

            services.AddMappings(configuration, env);

            services.Configure<Settings>(configuration.GetSection("Settings"));

            // settings binding
            var settings = new Settings();
            configuration.Bind("Settings", settings);

            var swaggerTitle = configuration["Swagger:Title"];
            var swaggerVersion = configuration["Swagger:Version"];
            var swaggerDescription = $"Build Number: {configuration["Swagger:BuildNumber"]}";

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swaggerTitle, new OpenApiInfo { Title = swaggerTitle, Version = swaggerVersion, Description = swaggerDescription });
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
                });

            var environment = configuration.GetValue<EnvironmentEnum>("Environment");
            var envPrefix = environment.GetAbbreviation();
            if (environment == EnvironmentEnum.Local)
            {
                envPrefix += $"_{Environment.MachineName}";
            }

            services.AddSingleton(configuration);

            services.AddApplicationInsightsTelemetry();
        }
    }
}