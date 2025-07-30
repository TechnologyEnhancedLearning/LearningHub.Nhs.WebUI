namespace LearningHub.Nhs.WebUI.Startup
{
    using System.Net.Http;
    using GDS.MultiPageFormData;
    using LearningHub.Nhs.Models.OpenAthens;
    using LearningHub.Nhs.Services;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Shared.Interfaces.Http;
    using LearningHub.Nhs.Shared.Interfaces.Services;
    using LearningHub.Nhs.Shared.Services;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.JsDetection;
    using LearningHub.Nhs.WebUI.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The service mappings.
    /// </summary>
    public static class ServiceMappings
    {
        /// <summary>
        /// The add learning hub mappings.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="env">The env.</param>
        public static void AddLearningHubMappings(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
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

                services.AddHttpClient<IOpenApiHttpClient, OpenApiHttpClient>()
                 .ConfigurePrimaryHttpMessageHandler(
                     () => new HttpClientHandler
                     {
                         ServerCertificateCustomValidationCallback =
                                       HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                     });

                services.AddHttpClient<IUserApiHttpClient, UserApiHttpClient>()
                    .ConfigurePrimaryHttpMessageHandler(
                        () => new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                          HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        });

                services.AddHttpClient<ILearningHubReportApiClient, LearningHubReportApiClient>()
                 .ConfigurePrimaryHttpMessageHandler(
                     () => new HttpClientHandler
                     {
                         ServerCertificateCustomValidationCallback =
                                       HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                     });
                services.AddHttpClient<IMoodleHttpClient, MoodleHttpClient>()
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
                services.AddHttpClient<IOpenApiHttpClient, OpenApiHttpClient>();
                services.AddHttpClient<IUserApiHttpClient, UserApiHttpClient>();
                services.AddHttpClient<ILearningHubReportApiClient, LearningHubReportApiClient>();
                services.AddHttpClient<IMoodleHttpClient, MoodleHttpClient>();
            }

            // Config
            services.Configure<OpenAthensScopes>(configuration.GetSection("OpenAthensScopes"));

            // services.Configure<BFFPathValidationOptions>(configuration.GetSection(BFFPathValidationOptions.SectionName));

            // Learning Hub Services
            services.AddTransient<INavigationPermissionService, NavigationPermissionService>();
            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<ICardService, CardService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<ICatalogueService, CatalogueService>();
            services.AddScoped<IHierarchyService, HierarchyService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IJobRoleService, JobRoleService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ITermsAndConditionsService, TermsAndConditionsService>();
            services.AddScoped<ILoginWizardService, LoginWizardService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IContributeService, ContributeService>();
            services.AddSingleton<IAzureMediaService, MKIOMediaService>();
            services.AddSingleton<IRoadMapService, RoadMapService>();
            services.AddSingleton<IMyLearningService, MyLearningService>();
            services.AddSingleton<IDashboardService, DashboardService>();
            services.AddSingleton<IContentService, ContentService>();
            services.AddScoped<IPartialFileUploadService, PartialFileUploadService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ILearningHubApiFacade, LearningHubApiFacade>();
            services.AddScoped<IOpenApiFacade, OpenApiFacade>();
            services.AddScoped<IBookmarkService, BoomarkService>();
            services.AddScoped<IUserSessionHelper, UserSessionHelper>();
            services.AddScoped<IDetectJsLogService, DetectJsLogService>();
            services.AddScoped<IMultiPageFormService, MultiPageFormService>();
            services.AddSingleton<IPDFReportService, PDFReportService>();

            services.AddSingleton<IJsDetectionLogger, JsDetectionLogger>();
            services.AddScoped<IInternalSystemService, InternalSystemService>();
            services.AddScoped<IProviderService, ProviderService>();

            // Filters (that require DI)
            services.AddScoped<LoginWizardFilter>();
            services.AddScoped<SsoLoginFilterAttribute>();
            services.AddScoped<OfflineCheckFilter>();
        }
    }
}
