// <copyright file="NLogSetup.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Setup
{
    using Microsoft.ApplicationInsights.NLogTarget;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using NLog.Web;
    using ConfigurationExtensions = LearningHub.NHS.OpenAPI.Configuration.ConfigurationExtensions;

    /// <summary>
    /// Static methods for configuring NLog on project startup.
    /// </summary>
    public static class NLogSetup
    {
        private const string NlogConfigFileName = "nlog.config";

        /// <summary>
        /// Carries out extra configuration of NLog which can't be done easily in nlog.config.
        /// </summary>
        /// <param name="config">Config.</param>
        public static void ConfigureNLog(IConfiguration config)
        {
            var appInsightsKey = config[ConfigurationExtensions.AppInsightsInstrumentationKeySettingName];

            // For more details see https://github.com/NLog/NLog/wiki/Configure-from-code
            NLogBuilder.ConfigureNLog(NlogConfigFileName);

            PassApplicationInsightsKeyToNlog(appInsightsKey);

            LogManager.ConfigurationReloaded += (_, _) =>
            {
                // Must be re-applied if config reloaded.
                PassApplicationInsightsKeyToNlog(appInsightsKey);
            };
        }

        private static void PassApplicationInsightsKeyToNlog(string instrumentationKey)
        {
            var nlogConfig = LogManager.Configuration;

            var applicationInsightsTarget = nlogConfig.FindTargetByName<ApplicationInsightsTarget>("appInsightsTarget");
            applicationInsightsTarget.InstrumentationKey = instrumentationKey;

            LogManager.Configuration = nlogConfig;
        }
    }
}
