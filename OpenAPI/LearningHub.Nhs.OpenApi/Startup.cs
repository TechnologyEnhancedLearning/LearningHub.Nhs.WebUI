using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]

namespace LearningHub.NHS.OpenAPI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using AspNetCore.Authentication.ApiKey;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.NHS.OpenAPI.Auth;
    using LearningHub.NHS.OpenAPI.Configuration;
    using LearningHub.NHS.OpenAPI.Middleware;
    using LearningHub.Nhs.OpenApi.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Microsoft.AspNetCore.Authorization;
    using LearningHub.NHS.OpenAPI.Authentication;
    using System.Configuration;
    using LearningHub.Nhs.MessagingService;
    using LearningHub.Nhs.MessageQueueing;
    using Microsoft.AspNetCore.Authentication.Cookies;

    /// <summary>
    /// The Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The IConfiguration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets app config.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures services.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfig(this.Configuration);

            services.AddApiKeyAuth();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = this.Configuration.GetValue<string>("LearningHUbAuthServiceConfig:Authority");
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    NameClaimType = "given_name",
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ValidateAudience = true,
                    ValidAudiences = new List<string> { "learninghubopenapi", "learninghubapi" },
                };
            });

            services.AddCustomMiddleware();
            services.AddSingleton<IAuthorizationHandler, ReadWriteHandler>();

            services.AddRepositories(this.Configuration);
            services.AddServices();

            services.AddDbContext<LearningHubDbContext>(
                options =>
                    options.UseSqlServer(this.Configuration.GetConnectionString("LearningHub")));
            services.AddApplicationInsightsTelemetry();
            services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
                options.Filters.Add(new AuthorizeFilter());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
            services.AddMessagingServices(this.Configuration);
            services.AddQueueingRepositories(this.Configuration);
            services.AddSwaggerGen(
                c =>
                {
                    // For docs see https://github.com/domaindrivendev/Swashbuckle.AspNetCore
                    c.SwaggerDoc("dev", new OpenApiInfo { Title = "LearningHub.NHS.OpenAPI", Version = "dev" });
                    c.CustomSchemaIds(type => type.FullName);
                    c.AddSecurityDefinition(
                        "ApiKey",
                        new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Name = AuthConstants.ApiKeyHeaderName,
                        });
                    c.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                        { Type = ReferenceType.SecurityScheme, Id = "ApiKey" },
                                },
                                new string[] { } // Must be empty since not oauth2 - see https://github.com/domaindrivendev/Swashbuckle.AspNetCore#add-security-definitions-and-requirements
                            },
                        });
                    c.AddSecurityDefinition(
                        "OAuth",
                        new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            In = ParameterLocation.Header,
                            Name = "Authorization",
                            Flows = new OpenApiOAuthFlows
                            {
                                AuthorizationCode = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new System.Uri(this.Configuration.GetValue<string>("LearningHUbAuthServiceConfig:AuthorizationUrl")),
                                    TokenUrl = new System.Uri(this.Configuration.GetValue<string>("LearningHubAuthServiceConfig:TokenUrl")),
                                    Scopes = new Dictionary<string, string>
                                    {
                                        { "learninghubapi", string.Empty },

                                    },
                                },
                            },
                        });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Id = "OAuth", Type = ReferenceType.SecurityScheme },
                            },
                            new string[] { }
                        },
                    });
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "ReadWrite",
                    policy => policy.Requirements.Add(new ReadWriteRequirement()));
            });

            var environment = this.Configuration.GetValue<EnvironmentEnum>("Environment");
            var envPrefix = environment.GetAbbreviation();
            if (environment == EnvironmentEnum.Local)
            {
                envPrefix += $"_{Environment.MachineName}";
            }

            services.AddDistributedCache(option =>
            {
                option.RedisConnectionString = this.Configuration.GetConnectionString("LearningHubRedis");
                option.KeyPrefix = envPrefix;
                option.DefaultExpiryInMinutes = 60;
            });

        }

        /// <summary>
        /// Configures the app.
        /// </summary>
        /// <param name="app">The IApplication Builder.</param>
        /// <param name="env">The IWebHostEnvironment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                // Check context headers to determine which authentication scheme is appropriate
                string scheme = ApiKeyDefaults.AuthenticationScheme;
                if (context.Request.Headers.Keys.Contains("Authorization"))
                {
                    scheme = JwtBearerDefaults.AuthenticationScheme;
                }

                var result = await context.AuthenticateAsync(scheme);
                if (result.Succeeded)
                {
                    context.User = result.Principal;
                }

                await next();
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "SwaggerDefinitions")),
                RequestPath = "/SwaggerDefinitions",
            });

            app.UseCustomMiddleware();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                if (env.IsDevelopment())
                {
                    c.SwaggerEndpoint("/swagger/dev/swagger.json", "Auto-generated");
                }

                c.SwaggerEndpoint("/SwaggerDefinitions/v1.3.0.json", "v1.3.0");
                c.OAuthClientId(this.Configuration.GetValue<string>("LearningHubAuthServiceConfig:ClientId"));
                c.OAuthClientSecret(this.Configuration.GetValue<string>("LearningHubAuthServiceConfig:ClientSecret"));
                c.OAuthScopes(this.Configuration.GetValue<string>("LearningHubAuthServiceConfig:Scopes"));
                c.OAuthUsePkce();
            });

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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuth();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
