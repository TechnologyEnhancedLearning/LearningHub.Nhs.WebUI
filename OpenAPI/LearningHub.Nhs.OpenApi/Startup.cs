using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]

namespace LearningHub.NHS.OpenAPI
{
    using System.Collections.Generic;
    using System.IO;
    using LearningHub.NHS.OpenAPI.Auth;
    using LearningHub.NHS.OpenAPI.Configuration;
    using LearningHub.NHS.OpenAPI.Middleware;
    using LearningHub.Nhs.OpenApi.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Services;
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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
              options.Authority = this.Configuration.GetValue<string>("LearningHUbAuthServiceConfig:Authority");
              options.TokenValidationParameters = new TokenValidationParameters()
              {
                NameClaimType = "given_name",
                RoleClaimType = "role",
                ValidateAudience = true,
                ValidAudiences = new List<string> { "learninghubopenapi", "learninghubapi" },
              };
            });

            services.AddCustomMiddleware();

            services.AddRepositories(this.Configuration);
            services.AddServices();

            services.AddDbContext<LearningHubDbContext>(
                options =>
                    options.UseSqlServer(this.Configuration.GetConnectionString("LearningHub")));
            services.AddApplicationInsightsTelemetry();
            services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()));
            services.AddControllers(opt => { opt.Filters.Add(new AuthorizeFilter()); });
            services.AddSwaggerGen(
                c =>
                {
                    // For docs see https://github.com/domaindrivendev/Swashbuckle.AspNetCore
                    c.SwaggerDoc("dev", new OpenApiInfo { Title = "LearningHub.NHS.OpenAPI", Version = "dev" });
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuth();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
