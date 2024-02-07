// <copyright file="CustomHttpsRedirectionMiddleware.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Startup
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project

    /// <summary>
    /// Defines the <see cref="CustomHttpsRedirectionMiddleware" />.
    /// </summary>
    public class CustomHttpsRedirectionMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHttpsRedirectionMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next.</param>
        public CustomHttpsRedirectionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// The Invoke.
        /// </summary>
        /// <param name="httpContext">Http context.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.IsHttps)
            {
                var request = httpContext.Request;
                var url = $"https://{request.Host}{request.Path}{request.QueryString}";
                httpContext.Response.Redirect(url);
                return Task.FromResult(0);
            }

            return this.next(httpContext);
        }
    }
}
