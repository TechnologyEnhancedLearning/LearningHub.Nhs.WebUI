// <copyright file="RequestClientLoggingMiddleware.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Middleware
{
    using System;
    using System.Threading.Tasks;
    using AspNetCore.Authentication.ApiKey;
    using LearningHub.NHS.OpenAPI.Auth;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    public class RequestClientLoggingMiddleware : IMiddleware
    {
        private readonly IApiKeyProvider apiKeyProvider;
        private readonly ILogger<RequestClientLoggingMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestClientLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="apiKeyProvider">The ApiKeyProvider.</param>
        /// <param name="logger">The logger.</param>
        public RequestClientLoggingMiddleware(
            IApiKeyProvider apiKeyProvider,
            ILogger<RequestClientLoggingMiddleware> logger)
        {
            this.apiKeyProvider = apiKeyProvider;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // TODO-10504: We kind of duplicate some logic here by extracting the API key from the header manually.
                // TODO-10504 (cont.): It would be nice if we could get the ApiKey object off the Http context directly.
                var attachedKey = context.Request.Headers[AuthConstants.ApiKeyHeaderName];
                var apiKeyBeingUsed = await this.apiKeyProvider.ProvideAsync(attachedKey);
                var clientName = apiKeyBeingUsed?.OwnerName;

                context.Features.Get<RequestTelemetry?>()?.Properties.Add("Client name", clientName);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Could not add api key client name to request telemetry");
            }

            await next(context);
        }
    }
}
