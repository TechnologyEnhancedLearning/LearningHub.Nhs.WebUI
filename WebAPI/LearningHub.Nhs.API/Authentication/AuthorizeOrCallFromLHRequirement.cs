namespace LearningHub.Nhs.Api.Authentication
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Provide Authentication policy for Auth Service.
    /// </summary>
    public class AuthorizeOrCallFromLHRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeOrCallFromLHRequirement"/> class.
        /// Provide Authentication policy for Auth Service.
        /// </summary>
        public AuthorizeOrCallFromLHRequirement()
        {
        }
    }
}
