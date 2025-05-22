namespace LearningHub.NHS.OpenAPI.Authentication
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

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
        /// Initializes a new instance of the <see cref="ReadWriteHandler"/> class.
        /// Provide Authentication policy for Auth Service.
        /// </summary>
        /// <param name="contextAccessor">The context Accessor.</param>
        public ReadWriteHandler(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
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
