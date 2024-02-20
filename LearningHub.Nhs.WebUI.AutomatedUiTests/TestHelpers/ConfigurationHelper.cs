// <copyright file="ConfigurationHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// ConfigurationHelper.
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// GetConfiguration.
        /// </summary>
        /// <returns>IConfiguration.</returns>
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();
        }
    }
}
