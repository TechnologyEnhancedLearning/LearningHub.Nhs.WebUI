// <copyright file="LogoutController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityModel.Client;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Handlers;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines the <see cref="LogoutController" />.
    /// </summary>
    public class LogoutController : Controller
    {
        /// <summary>
        /// The http client..
        /// </summary>
        private readonly ILearningHubHttpClient httpClient;

        /// <summary>
        /// The logout users..
        /// </summary>
        private readonly LogoutUserManager logoutUsers;

        /// <summary>
        /// The wesettings config..
        /// </summary>
        private readonly IOptions<WebSettings> websettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutController"/> class.
        /// </summary>
        /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        /// <param name="logoutSessions">The logoutSessions<see cref="LogoutUserManager"/>.</param>
        /// <param name="httpClient">The httpClient<see cref="ILearningHubHttpClient"/>.</param>
        public LogoutController(
            IOptions<WebSettings> websettings,
            LogoutUserManager logoutSessions,
            ILearningHubHttpClient httpClient)
        {
            this.websettings = websettings;
            this.logoutUsers = logoutSessions;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="logout_token">The logout_token<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
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
        /// The validate jwt.
        /// </summary>
        /// <param name="jwt">The jwt<see cref="string"/>.</param>
        /// <param name="authorityUrl">The authorityUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{ClaimsPrincipal}"/>.</returns>
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
                ValidAudience = this.websettings.Value.ClientId,
                IssuerSigningKeys = keys,

                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role,
            };

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            var user = handler.ValidateToken(jwt, parameters, out var _);
            return user;
        }

        /// <summary>
        /// The validate logout token.
        /// </summary>
        /// <param name="logoutToken">The logoutToken<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{ClaimsPrincipal}"/>.</returns>
        private async Task<ClaimsPrincipal> ValidateLogoutToken(string logoutToken)
        {
            var claims = await this.ValidateJwt(logoutToken, this.websettings.Value.AuthenticationServiceUrl);

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
    }
}
