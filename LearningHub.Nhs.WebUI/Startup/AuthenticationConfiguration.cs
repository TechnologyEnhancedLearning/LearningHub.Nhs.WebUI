namespace LearningHub.Nhs.WebUI.Startup
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityModel;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Handlers;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// The Authentication Configuration.
    /// </summary>
    public static class AuthenticationConfiguration
    {
        private const string AuthenticationScheme = "oidc";
        private const string AspNetCoreCookies = ".AspNetCore.Cookies";

        /// <summary>
        /// The Authentication Configuration Extension.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="config">The Learning Hub config.</param>
        public static void ConfigureAuthentication(this IServiceCollection services, LearningHubAuthServiceConfig config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(config.AuthTimeout);
                options.SlidingExpiration = true;
                options.EventsType = typeof(CookieEventHandler);
                options.AccessDeniedPath = "/Home/AccessDenied";
            })
            .AddOpenIdConnect(AuthenticationScheme, options =>
            {
                options.Authority = config.Authority;
                options.ClientId = config.ClientId;
                options.ClientSecret = config.ClientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("userapi");
                options.Scope.Add("learninghubapi");
                options.Scope.Add("offline_access"); // Enables refresh token even though Auth Service session has expired
                options.Scope.Add("roles");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Events.OnRemoteFailure = async context =>
                {
                    context.Response.Redirect("/"); // If login cancelled return to home page
                    context.HandleResponse();

                    await Task.CompletedTask;
                };

                options.ClaimActions.MapUniqueJsonKey("role", "role");
                options.ClaimActions.MapUniqueJsonKey("name", "elfh_userName");
                options.ClaimActions.MapUniqueJsonKey("moodle_username", "preferred_username");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,
                };

                options.Events.OnRedirectToIdentityProvider = OnRedirectToIdentityProvider;
                options.Events.OnTokenValidated = UserSessionBegins;
            });
        }

        private static async Task OnRedirectToIdentityProvider(RedirectContext context)
        {
            var referer = context.Request.Headers["Referer"].ToString();

            // if valid referer only
            if (!string.IsNullOrEmpty(referer) && Uri.TryCreate(referer, UriKind.RelativeOrAbsolute, out Uri uriReferer))
            {
                // only external referer
                if (uriReferer.IsAbsoluteUri && uriReferer.Host != context.Request.Host.Host)
                {
                    context.ProtocolMessage.SetParameter("ext_referer", referer);
                }
            }

            if (context.Request.Cookies.ContainsKey(AspNetCoreCookies))
            {
                context.ProtocolMessage.SetParameter("error", "auth_timeout");

                if (context.Request.Headers["x-requested-with"] == "XMLHttpRequest")
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.HandleResponse();
                }
            }

            await Task.Yield();
        }

        private static async Task UserSessionBegins(TokenValidatedContext context)
        {
            if (context.Principal != null)
            {
                var userIdString = context.Principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (!string.IsNullOrWhiteSpace(userIdString))
                {
                    var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
                    await cacheService.SetAsync($"{userIdString}:LoginWizard", "start");
                }
            }

            await Task.Yield();
        }
    }
}