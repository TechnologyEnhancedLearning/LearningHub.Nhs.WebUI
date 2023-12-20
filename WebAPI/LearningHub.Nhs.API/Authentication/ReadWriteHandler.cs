// <copyright file="ReadWriteHandler.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Authentication
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Provide Authentication policy for Auth Service.
    /// </summary>
    public class ReadWriteHandler : AuthorizationHandler<ReadWriteRequirement>
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
        /// Initializes a new instance of the <see cref="ReadWriteHandler"/> class.
        /// Provide Authentication policy for Auth Service.
        /// </summary>
        /// <param name="contextAccessor">The context Accessor.</param>
        /// <param name="settings">The settings.</param>
        public ReadWriteHandler(IHttpContextAccessor contextAccessor, IOptions<Settings> settings)
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
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ReadWriteRequirement requirement)
        {
            if (requirement.HasReadWriteRole(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
