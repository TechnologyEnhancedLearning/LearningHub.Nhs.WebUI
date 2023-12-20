// <copyright file="ConfigurationExtensions.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Configuration
{
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Extensions for managing configuration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// The AppInsightsInstrumentationKeySettingName.
        /// </summary>
        public const string AppInsightsInstrumentationKeySettingName = "APPINSIGHTS_INSTRUMENTATIONKEY";

        /// <summary>
        /// The AuthSectionName.
        /// </summary>
        public const string AuthSectionName = "Auth";

        /// <summary>
        /// The FindwiseSectionName.
        /// </summary>
        public const string FindwiseSectionName = "Findwise";

        /// <summary>
        /// The LearningHubSectionName.
        /// </summary>
        public const string LearningHubSectionName = "LearningHub";

        /// <summary>
        /// The LearningHubApiSectionName.
        /// </summary>
        public const string LearningHubApiSectionName = "LearningHubAPIConfig";

        /// <summary>
        /// Adds config.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="config">The app config.</param>
        public static void AddConfig(this IServiceCollection services, IConfiguration config)
        {
            RegisterPostConfigure(
                services.AddOptions<AuthConfiguration>()
                    .Bind(config.GetSection(AuthSectionName)));

            services.AddOptions<FindwiseConfig>().Bind(config.GetSection(FindwiseSectionName));

            services.AddOptions<LearningHubConfig>().Bind(config.GetSection(LearningHubSectionName));

            services.AddOptions<LearningHubApiConfig>().Bind(config.GetSection(LearningHubApiSectionName));
        }

        private static OptionsBuilder<T> RegisterPostConfigure<T>(this OptionsBuilder<T> builder)
            where T : class, IHasPostConfigure
        {
            builder.PostConfigure(configSection => configSection.PostConfigure());
            return builder;
        }
    }
}
