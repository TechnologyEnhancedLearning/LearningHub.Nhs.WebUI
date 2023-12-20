// <copyright file="JsDetectionLogger.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.JsDetection
{
    using System.Threading;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// JsDetectionLogger class.
    /// </summary>
    public class JsDetectionLogger : IJsDetectionLogger
    {
        private static long jsEnabled = 0;
        private static long jsDisabled = 0;
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsDetectionLogger"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory">IServiceScopeFactory.</param>
        public JsDetectionLogger(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Flush.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task FlushCounters()
        {
            var jsEnabledRequest = Interlocked.Exchange(ref jsEnabled, 0);
            var jsDisabledRequest = Interlocked.Exchange(ref jsDisabled, 0);

            if (jsEnabledRequest != 0 || jsDisabledRequest != 0)
            {
                using var scope = this.serviceScopeFactory.CreateScope();

                var detectJsLogService = scope.ServiceProvider.GetService<IDetectJsLogService>();

                await detectJsLogService.UpdateAsync(jsEnabledRequest, jsDisabledRequest);
            }
        }

        /// <summary>
        /// IncrementEnabled.
        /// </summary>
        public void IncrementEnabled()
        {
            Interlocked.Increment(ref jsEnabled);
        }

        /// <summary>
        /// IncrementEnabled.
        /// </summary>
        public void IncrementDisabled()
        {
            Interlocked.Increment(ref jsDisabled);
        }
    }
}