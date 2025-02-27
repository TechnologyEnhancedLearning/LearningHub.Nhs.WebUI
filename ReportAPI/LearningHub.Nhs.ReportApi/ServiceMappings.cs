namespace LearningHub.Nhs.ReportingApi
{
    using AutoMapper;
    using Azure.Messaging.ServiceBus;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.ReportApi.Authentication;
    using LearningHub.Nhs.ReportApi.Services;
    using LearningHub.Nhs.ReportApi.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

    /// <summary>
    /// Extension class for <see cref="IServiceCollection"/> service mappings.
    /// </summary>
    public static class ServiceMappings
    {
        /// <summary>
        /// The add mappings.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="env">The environment.</param>
        public static void AddMappings(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddServices(configuration, env);
            services.AddAuthentication();
            services.AddAutomapper();
        }

        /// <summary>
        /// The add services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="env">The environment.</param>
        private static void AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            // Http client
            if (env.IsDevelopment())
            {
                services.AddHttpClient<ILearningHubHttpClient, LearningHubHttpClient>()
                    .ConfigurePrimaryHttpMessageHandler(
                        () => new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                          HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        });
            }
            else
            {
                services.AddHttpClient<ILearningHubHttpClient, LearningHubHttpClient>();
            }

            services.AddScoped<ILearningHubApiFacade, LearningHubApiFacade>();
            services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
            services.AddScoped<IServiceBusMessageService, ServiceBusMessageService>();
            services.AddScoped<IPdfReportService, PdfReportService>();

            services.AddSingleton<IAuthorizationHandler, AuthorizeOrCallFromLHHandler>();

            services.AddSingleton((serviceProvider) =>
            {
                return new ServiceBusClient(configuration.GetConnectionString("AzureServiceBus"));
            });
        }

        private static void AddAuthentication(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AuthorizeOrCallFromLH", policy => policy.Requirements.Add(new AuthorizeOrCallFromLHRequirement()));
            });
        }

        private static void AddAutomapper(this IServiceCollection services)
        {
            services.AddSingleton(new MapperConfiguration(mc =>
            {
                mc.AllowNullCollections = true;
                mc.ShouldMapMethod = m => false;
                mc.AddProfile(new MappingProfile());
            }).CreateMapper());
        }
    }
}