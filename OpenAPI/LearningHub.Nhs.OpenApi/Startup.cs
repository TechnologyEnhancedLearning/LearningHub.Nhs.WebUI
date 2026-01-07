using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]

namespace LearningHub.NHS.OpenAPI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using AspNetCore.Authentication.ApiKey;
    using LearningHub.Nhs.Api.Authentication;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.NHS.OpenAPI.Auth;
    using LearningHub.NHS.OpenAPI.Authentication;
    using LearningHub.NHS.OpenAPI.Configuration;
    using LearningHub.NHS.OpenAPI.Middleware;
    using LearningHub.Nhs.OpenApi.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
            services.AddSingleton<IAuthorizationHandler, AuthorizeOrCallFromLHHandler>();

            services.AddRepositories(this.Configuration);
            services.AddServices();
            services.AddApplicationInsightsTelemetry();
            services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
            });

            services.AddMvc()
                  .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var swaggerTitle = this.Configuration["Swagger:Title"];
            var swaggerVersion = this.Configuration["Swagger:Version"];
            var swaggerDescription = $"A set of API endpoints for retrieving learning resource information from the Learning Hub learning platform. The [Learning Hub](https://learninghub.nhs.uk/) is a platform for hosting and sharing learning resources for health and social care provided by Technology Enhanced Learning (TEL) at NHS England. An application API key must be used to authorise calls to the API from external applications. To contact TEL to discuss connecting your external system to the Learning Hub, email england.tel@nhs.net.\n\n Build Number: {this.Configuration["Swagger:BuildNumber"]} \n\n";

            services.AddSwaggerGen(
                c =>
                {
                    // For docs see https://github.com/domaindrivendev/Swashbuckle.AspNetCore
                    c.SwaggerDoc("dev", new OpenApiInfo { Title = swaggerTitle, Version = swaggerVersion, Description = swaggerDescription });

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
                       "AuthorizeOrCallFromLH",
                       policy => policy.Requirements.Add(new AuthorizeOrCallFromLHRequirement()));

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
                option.KeyPrefix = $"{envPrefix}_WebUI";
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
                c.SwaggerEndpoint("/swagger/dev/swagger.json", "v1.4.0");

                ////c.SwaggerEndpoint("/SwaggerDefinitions/v1.3.0.json", "v1.3.0");
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
