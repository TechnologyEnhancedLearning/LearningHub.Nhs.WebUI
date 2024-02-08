namespace LearningHub.Nhs.AdminUI
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Validation;
    using FluentValidation;
    using FluentValidation.AspNetCore;
    using IdentityModel;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Filters;
    using LearningHub.Nhs.AdminUI.Handlers;
    using LearningHub.Nhs.AdminUI.Helpers;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Services;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;

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

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSession();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddDistributedMemoryCache();
            services.AddMvc()
                .AddFluentValidation()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.Configure<WebSettings>(configuration.GetSection("WebSettings"));
            services.AddSingleton(configuration);

            services.AddScoped<ILearningHubApiFacade, LearningHubApiFacade>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IExternalSystemService, ExternalSystemService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IRoadmapService, RoadmapService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAzureMediaService, AzureMediaService>();
            services.AddScoped<IResourceSyncService, ResourceSyncService>();
            services.AddScoped<ICatalogueService, CatalogueService>();
            services.AddScoped<IContentService, ContentService>();
            services.AddScoped<IHierarchyService, HierarchyService>();
            services.AddScoped<IEventService, EventService>();
            services.AddTransient<IValidator<UserAdminDetailViewModel>, UserValidator>();
            services.AddTransient<IUserSessionHelper, UserSessionHelper>();
            services.AddTransient<IInternalSystemService, InternalSystemService>();
            services.AddScoped<IProviderService, ProviderService>();

            // web settings binding
            var webSettings = new WebSettings();
            configuration.Bind("WebSettings", webSettings);

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an HttpClients
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
            }
            else
            {
                services.AddHttpClient<ILearningHubHttpClient, LearningHubHttpClient>();
                services.AddHttpClient<IUserApiHttpClient, UserApiHttpClient>();
            }

            services.AddTransient<CookieEventHandler>();
            services.AddSingleton<LogoutUserManager>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie(
            "Cookies",
            options =>
                {
                    options.AccessDeniedPath = "/Authorisation/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(webSettings.AuthTimeout);
                    options.SlidingExpiration = true;
                    options.EventsType = typeof(CookieEventHandler);
                }).AddOpenIdConnect(
                "oidc",
                options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = webSettings.AuthenticationServiceUrl;
                    options.ClientId = webSettings.ClientId;
                    options.ClientSecret = webSettings.LearningHubSecret;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("userapi");
                    options.Scope.Add("learninghubapi");
                    options.Scope.Add("offline_access"); // Enables refresh token even though Auth Service session has expired
                    options.Scope.Add("roles");
                    options.SaveTokens = true;
                    options.UsePkce = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Events.OnRemoteFailure = async context =>
                                         {
                                             context.Response.Redirect("/"); // If login cancelled return to home page
                                             context.HandleResponse();

                                             await Task.CompletedTask;
                                         };

                    options.ClaimActions.MapUniqueJsonKey("role", "role");
                    options.ClaimActions.MapUniqueJsonKey("name", "elfh_userName");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                    };
                    options.Events.OnTokenValidated = UserSessionBegins;
                });

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // policy requires user to have administrator role
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "RequireAdministratorRole",
                    policy => policy.RequireRole("Administrator"));
            });

            services.Configure<ForwardedHeadersOptions>(
                options =>
                    {
                        options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
                        options.KnownNetworks.Clear();
                        options.KnownProxies.Clear();
                    });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(CheckInitialLogonFilter));
            });
        }

        private static async Task UserSessionBegins(TokenValidatedContext context)
        {
            if (context.Principal != null)
            {
                var userIdString = context.Principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (!string.IsNullOrWhiteSpace(userIdString))
                {
                    var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
                    await cacheService.SetAsync($"{userIdString}:LoginProcess", "true");
                }
            }

            await Task.Yield();
        }
    }
}