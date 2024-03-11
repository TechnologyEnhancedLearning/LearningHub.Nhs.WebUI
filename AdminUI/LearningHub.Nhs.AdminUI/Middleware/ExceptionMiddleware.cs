namespace LearningHub.Nhs.AdminUI.Middleware
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Exceptions;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ExceptionMiddleware" />.
    /// </summary>
    public class ExceptionMiddleware
    {
        /// <summary>
        /// Defines the _hostingEnv.
        /// </summary>
        private readonly IWebHostEnvironment hostingEnv;

        /// <summary>
        /// Defines the _next.
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private ILogger<ExceptionMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="logger">The logger<see cref="ILogger{ExceptionMiddleware}"/>.</param>
        /// <param name="hostingEnv">The hostingEnv<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="next">The next<see cref="RequestDelegate"/>.</param>
        public ExceptionMiddleware(
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment hostingEnv,
            RequestDelegate next)
        {
            this.next = next;
            this.logger = logger;
            this.hostingEnv = hostingEnv;
        }

        /// <summary>
        /// The Invoke.
        /// </summary>
        /// <param name="httpContext">The httpContext<see cref="HttpContext"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await this.next(httpContext);
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// The HandleExceptionAsync.
        /// </summary>
        /// <param name="context">The context<see cref="HttpContext"/>.</param>
        /// <param name="exception">The exception<see cref="Exception"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            this.logger.LogError(exception, exception.Message);

            var response = context.Response;
            var customException = exception as LearningHubException;
            var statusCode = (int)HttpStatusCode.InternalServerError;

            var message = this.hostingEnv.IsDevelopment() ? exception.Message : "Unexpected error";
            var description = this.hostingEnv.IsDevelopment() ? exception.StackTrace : "Unexpected error";

            if (customException != null)
            {
                message = customException.Message;
                description = customException.Description;
                statusCode = customException.Code;
            }

            response.ContentType = "application/json";
            response.StatusCode = statusCode;
            await response.WriteAsync(JsonConvert.SerializeObject(new ErrorResponse(message, description)));
        }
    }
}
