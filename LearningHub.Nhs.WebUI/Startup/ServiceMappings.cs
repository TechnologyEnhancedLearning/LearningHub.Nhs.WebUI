namespace LearningHub.Nhs.WebUI.Startup
{
    using System.Net.Http;
    using GDS.MultiPageFormData;
    using LearningHub.Nhs.Models.OpenAthens;
    using LearningHub.Nhs.Services;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.JsDetection;
    using LearningHub.Nhs.WebUI.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;



    using Blazored.LocalStorage;
    using LearningHub.Nhs.WebUI.BlazorComponents.Services.HelperServices;
    using LearningHub.Nhs.WebUI.Shared.Interfaces;
    using LearningHub.Nhs.WebUI.Shared.Services;
    using LearningHub.Nhs.WebUI.Shared.Configuration;
    using LearningHub.Nhs.WebUI.Shared.Helpers;
    using LearningHub.Nhs.WebUI.Shared.Models;

    using TELBlazor.Components.Core.Configuration;
    using TELBlazor.Components.Core.Services.HelperServices;
    using TELBlazor.Components.OptionalImplementations.Test.TestComponents.SearchExperiment;

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
                services.AddHttpClient<IUserApiHttpClient, UserApiHttpClient>();
                services.AddHttpClient<ILearningHubReportApiClient, LearningHubReportApiClient>();
                services.AddHttpClient<IMoodleHttpClient, MoodleHttpClient>();
            }

       
            // Config
            services.Configure<OpenAthensScopes>(configuration.GetSection("OpenAthensScopes"));



            // Learning Hub Services
            services.AddTransient<INavigationPermissionService, NavigationPermissionService>();
            services.AddTransient <LearningHub.Nhs.WebUI.Interfaces.IActivityService, LearningHub.Nhs.WebUI.Services.ActivityService>();
            services.AddTransient<ICardService, CardService>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.IUserService, LearningHub.Nhs.WebUI.Services.UserService>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.IUserGroupService, LearningHub.Nhs.WebUI.Services.UserGroupService>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.INotificationService, LearningHub.Nhs.WebUI.Services.NotificationService>();

            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.ICatalogueService, LearningHub.Nhs.WebUI.Services.CatalogueService>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.IHierarchyService, LearningHub.Nhs.WebUI.Services.HierarchyService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IJobRoleService, JobRoleService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ITermsAndConditionsService, TermsAndConditionsService>();
            services.AddScoped<ILoginWizardService, LoginWizardService>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.IResourceService, LearningHub.Nhs.WebUI.Services.ResourceService>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.IRatingService, LearningHub.Nhs.WebUI.Services.RatingService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IContributeService, ContributeService>();
            services.AddSingleton<LearningHub.Nhs.WebUI.Interfaces.IAzureMediaService, MKIOMediaService>();
            services.AddSingleton<IRoadMapService, RoadMapService>();
            services.AddSingleton<LearningHub.Nhs.WebUI.Interfaces.IMyLearningService, LearningHub.Nhs.WebUI.Services.MyLearningService>();
            services.AddSingleton<LearningHub.Nhs.WebUI.Interfaces.IDashboardService, LearningHub.Nhs.WebUI.Services.DashboardService>();
            services.AddSingleton<IContentService, ContentService>();
            services.AddScoped<IPartialFileUploadService, PartialFileUploadService>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.IRoleService, LearningHub.Nhs.WebUI.Services.RoleService>();
            services.AddScoped<ILearningHubApiFacade, LearningHubApiFacade>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.IBookmarkService, BoomarkService>();
            services.AddScoped<IUserSessionHelper, UserSessionHelper>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.IDetectJsLogService, LearningHub.Nhs.WebUI.Services.DetectJsLogService>();
            services.AddScoped<IMultiPageFormService, MultiPageFormService>();
            services.AddSingleton<IPDFReportService, PDFReportService>();

            services.AddSingleton<IJsDetectionLogger, JsDetectionLogger>();
            services.AddScoped<LearningHub.Nhs.WebUI.Interfaces.IInternalSystemService, LearningHub.Nhs.WebUI.Services.InternalSystemService>();
            services.AddScoped<LearningHub.Nhs.WebUI.Shared.Interfaces.IProviderService, ProviderService>();

            // Filters (that require DI)
            services.AddScoped<LoginWizardFilter>();
            services.AddScoped<SsoLoginFilterAttribute>();
            services.AddScoped<OfflineCheckFilter>();


            // <!--qqqq blazor architecture keep at bottom so can use existing services and map them-->
            services.AddRazorComponents()
                    .AddInteractiveServerComponents()
                    .AddCircuitOptions(opt => opt.DetailedErrors = true)
                    .AddInteractiveWebAssemblyComponents();

            // <!--qqqq blazor services maybe collection so all together and update show whats missing-->
            // Future candidates for DI collection
            services.AddBlazoredLocalStorage();
            services.AddSingleton<ITELBlazorBaseComponentConfiguration>(provider =>
            {
                return new TELBlazorBaseComponentConfiguration
                {
                    JSEnabled = true, // qqqq may need to implement as do from prototype if lh doesnt have a way

                    // HostType = $"{builder.Configuration["Properties:Environment"]} {builder.Configuration["Properties:Application"]}"
                    // qqqq back to this one
                    HostType = $"{configuration["Properties:Environment"]} {configuration["Properties:Application"]}",

                    // HostType = "Server",
                };
            });

            services.AddScoped<ILogLevelSwitcherService, NLogLogLevelSwitcherService>();

            // services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(configuration["Settings:LearningHubWebUiUrl"]) });
            services.AddScoped<LearningHub.Nhs.WebUI.Shared.Interfaces.ISearchService, LearningHub.Nhs.WebUI.Shared.Services.SearchService>();
            services.AddScoped<ISearchExperimentService>(provider => provider.GetRequiredService<ISearchService>());
        }
    }
}
