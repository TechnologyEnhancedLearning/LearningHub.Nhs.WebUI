// <copyright file="AccessibilityTestsFixture.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures
{
    using System;
    using System.Data.SqlClient;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using OpenQA.Selenium;

    /// <summary>
    /// Represents a fixture for accessibility tests.
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup class.</typeparam>
    public class AccessibilityTestsFixture<TStartup> : IDisposable
        where TStartup : class
    {
        /// <summary>
        /// Gets the base URL for the tests.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        internal readonly string BaseUrl;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Gets the WebDriver used for the tests.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        internal readonly IWebDriver Driver;
#pragma warning restore SA1401 // Fields should be private

        private readonly SeleniumServerFactory<TStartup> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessibilityTestsFixture{TStartup}"/> class.
        /// </summary>
        public AccessibilityTestsFixture()
        {
            this.factory = new SeleniumServerFactory<TStartup>();
            this.BaseUrl = this.factory.RootUri;
            this.Driver = DriverHelper.CreateHeadlessChromeDriver();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            this.Driver.Quit();
            this.Driver.Dispose();
            this.factory.Dispose();
        }
    }
}
