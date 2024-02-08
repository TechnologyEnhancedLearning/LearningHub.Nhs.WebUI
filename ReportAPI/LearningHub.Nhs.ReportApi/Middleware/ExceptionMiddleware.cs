namespace LearningHub.Nhs.ReportApi.Middleware
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
    /// The exception middleware.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment hostingEnv;
        private readonly ILogger<ExceptionMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="hostingEnv">The hosting env.</param>
        /// <param name="next">The next.</param>
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IWebHostEnvironment hostingEnv, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
            this.hostingEnv = hostingEnv;
        }

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            this.logger.LogError(exception, exception.Message);

            var response = context.Response;
            var statusCode = (int)HttpStatusCode.InternalServerError;

            var message = this.hostingEnv.IsDevelopment() ? exception.Message : "Unexpected error";
            var description = this.hostingEnv.IsDevelopment() ? exception.StackTrace : "Unexpected error";

            if (exception is LearningHubException customException)
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