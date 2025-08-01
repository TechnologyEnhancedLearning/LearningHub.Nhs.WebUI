namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityModel.Client;
    using LearningHub.Nhs.Shared.Interfaces.Http;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Handlers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The logout controller.
    /// </summary>
    public class LogoutController : Controller
    {
        private readonly LearningHubAuthServiceConfig authConfig;
        private readonly LogoutUserManager logoutUsers;
        private readonly ILearningHubHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutController"/> class.
        /// </summary>
        /// <param name="authConfig">The auth config.</param>
        /// <param name="logoutSessions">The logout sessions.</param>
        /// <param name="httpClient">The http fac.</param>
        public LogoutController(
            LearningHubAuthServiceConfig authConfig,
            LogoutUserManager logoutSessions,
            ILearningHubHttpClient httpClient)
        {
            this.authConfig = authConfig;
            this.logoutUsers = logoutSessions;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="logout_token">The logout_token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string logout_token)
        {
            // response headers are added, part of OpenId Connect Front-Channel specifications
            this.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            this.Response.Headers.Add("Pragma", "no-cache");

            try
            {
                var user = await this.ValidateLogoutToken(logout_token);

                // these are the sup & sid to sign out
                var sub = user.FindFirst("sub")?.Value;
                var sid = user.FindFirst("sid")?.Value;

                this.logoutUsers.Add(sub, sid);
                return this.Ok();
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }

        /// <summary>
        /// The validate logout token.
        /// </summary>
        /// <param name="logoutToken">The logout token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<ClaimsPrincipal> ValidateLogoutToken(string logoutToken)
        {
            var claims = await this.ValidateJwt(logoutToken, this.authConfig.Authority);

            if (claims.FindFirst("sub") == null && claims.FindFirst("sid") == null)
            {
                throw new Exception("Invalid logout token");
            }

            var nonce = claims.FindFirstValue("nonce");
            if (!string.IsNullOrWhiteSpace(nonce))
            {
                throw new Exception("Invalid logout token");
            }

            var eventsJson = claims.FindFirst("events")?.Value;
            if (string.IsNullOrWhiteSpace(eventsJson))
            {
                throw new Exception("Invalid logout token");
            }

            var events = JObject.Parse(eventsJson);
            var logoutEvent = events.TryGetValue("http://schemas.openid.net/event/backchannel-logout");
            if (logoutEvent == null)
            {
                throw new Exception("Invalid logout token");
            }

            return claims;
        }

        /// <summary>
        /// The validate jwt.
        /// </summary>
        /// <param name="jwt">The jwt.</param>
        /// <param name="authorityUrl">The authority url.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<ClaimsPrincipal> ValidateJwt(string jwt, string authorityUrl)
        {
            var client = await this.httpClient.GetClientAsync();
            var disco = await client.GetDiscoveryDocumentAsync(authorityUrl);
            var keys = new List<SecurityKey>();
            foreach (var webKey in disco.KeySet.Keys)
            {
                var e = Base64Url.Decode(webKey.E);
                var n = Base64Url.Decode(webKey.N);

                var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
                {
                    KeyId = webKey.Kid,
                };

                keys.Add(key);
            }

            var parameters = new TokenValidationParameters
            {
                ValidIssuer = disco.Issuer,
                ValidAudience = this.authConfig.ClientId,
                IssuerSigningKeys = keys,

                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role,
            };

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            var user = handler.ValidateToken(jwt, parameters, out var _);
            return user;
        }
    }
}