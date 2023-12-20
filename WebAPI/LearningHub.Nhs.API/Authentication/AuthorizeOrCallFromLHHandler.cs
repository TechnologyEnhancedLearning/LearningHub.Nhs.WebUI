// <copyright file="AuthorizeOrCallFromLHHandler.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Authentication
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Provide Authentication policy for Auth Service.
    /// </summary>
    public class AuthorizeOrCallFromLHHandler : AuthorizationHandler<AuthorizeOrCallFromLHRequirement>
    {
        /// <summary>
        /// The context accessor.
        /// </summary>
        private readonly IHttpContextAccessor contextAccessor;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly IOptions<Settings> settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeOrCallFromLHHandler"/> class.
        /// Provide Authentication policy for Auth Service.
        /// </summary>
        /// <param name="contextAccessor">The context Accessor.</param>
        /// <param name="settings">The settings.</param>
        public AuthorizeOrCallFromLHHandler(IHttpContextAccessor contextAccessor, IOptions<Settings> settings)
        {
            this.contextAccessor = contextAccessor;
            this.settings = settings;
        }

        /// <summary>
        /// Handle Authentication policy Requirement.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="requirement">The requirement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizeOrCallFromLHRequirement requirement)
        {
            bool callFromAuthService = false;
            bool callFromLearningHubClient = false;
            bool callFromLHContentServerClient = false;
            bool callFromReportApiClient = false;

            if (!context.User.Identity.IsAuthenticated)
            {
                var headers = this.contextAccessor.HttpContext.Request.Headers;

                // Note: headers.ContainsKey and headers.TryGetValue are case-insensitive.
                if (headers.ContainsKey("Client-Identity-Key"))
                {
                    Microsoft.Extensions.Primitives.StringValues clientKeyValues;
                    if (headers.TryGetValue("Client-Identity-Key", out clientKeyValues))
                    {
                        string clientKey = clientKeyValues.First().ToUpperInvariant();

                        callFromAuthService = clientKey
                                          == this.settings.Value.AuthClientIdentityKey.ToUpperInvariant();

                        callFromLearningHubClient = clientKey
                                                == this.settings.Value.LHClientIdentityKey.ToUpperInvariant();

                        callFromLHContentServerClient = clientKey
                                                == this.settings.Value.ContentServerClientIdentityKey.ToUpperInvariant();

                        callFromReportApiClient = clientKey
                                               == this.settings.Value.ReportApiClientIdentityKey.ToUpperInvariant();
                    }
                }
            }

            if (!callFromAuthService && !callFromLearningHubClient && !callFromLHContentServerClient && !callFromReportApiClient && !context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
