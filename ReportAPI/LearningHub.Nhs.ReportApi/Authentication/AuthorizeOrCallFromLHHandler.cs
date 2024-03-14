namespace LearningHub.Nhs.ReportApi.Authentication
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.ReportApi.Shared.Configuration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Provide Authentication policy for Auth Service.
    /// </summary>
    public class AuthorizeOrCallFromLHHandler : AuthorizationHandler<AuthorizeOrCallFromLHRequirement>
    {
        private readonly IHttpContextAccessor contextAccessor;
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
            bool callFromValidClient = false;

            var headers = this.contextAccessor.HttpContext?.Request.Headers;

            if (headers != null && headers.ContainsKey("Client-Identity-Key"))
            {
                var clientKey = headers.Single(h => h.Key == "Client-Identity-Key").Value.ToString().ToUpperInvariant();

                callFromValidClient = clientKey == this.settings.Value.ClientIdentityKey.ToUpperInvariant();
            }

            if (!callFromValidClient)
            {
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
