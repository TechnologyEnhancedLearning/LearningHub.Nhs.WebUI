// <copyright file="NLogMiddleware.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Middleware
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Extensions;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Defines the <see cref="NLogMiddleware" />.
    /// </summary>
    public class NLogMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogMiddleware"/> class.
        /// Constructor.
        /// </summary>
        /// <param name="next">next.</param>
        public NLogMiddleware(
            RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Process the Request and Catch Exceptions.
        /// </summary>
        /// <param name="context">context.</param>
        /// <returns>Task.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            await this.next(context);

            if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                if (!NLog.MappedDiagnosticsContext.Contains("userid"))
                {
                    NLog.MappedDiagnosticsContext.Set("userid", context.User.Identity.GetCurrentUserId());
                }

                if (!NLog.MappedDiagnosticsContext.Contains("username"))
                {
                    NLog.MappedDiagnosticsContext.Set("username", context.User.Identity.GetCurrentElfhUserName());
                }
            }
        }
    }
}
