// <copyright file="CustomHttpsRedirectionMiddlewareExtensions.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Startup
{
    using Microsoft.AspNetCore.Builder;

    // Extension method used to add the middleware to the HTTP request pipeline.

    /// <summary>
    /// Defines the <see cref="CustomHttpsRedirectionMiddlewareExtensions" />.
    /// </summary>
    public static class CustomHttpsRedirectionMiddlewareExtensions
    {
        /// <summary>
        /// The UseCustomHttpsRedirection.
        /// </summary>
        /// <param name="builder">The builder<see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseCustomHttpsRedirection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomHttpsRedirectionMiddleware>();
        }
    }
}
