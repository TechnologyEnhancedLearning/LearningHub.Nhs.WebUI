// <copyright file="AccessibilityTestsBase.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using LearningHub.Nhs.WebUI.Startup;
    using OpenQA.Selenium;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// Accessibility Tests Base.
    /// </summary>
    [Collection("Selenium test collection")]
    public class AccessibilityTestsBase
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessibilityTestsBase"/> class.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public AccessibilityTestsBase(AccessibilityTestsFixture<Program> fixture)
        {
            this.BaseUrl = fixture.BaseUrl;
            this.Driver = fixture.Driver;
        }

        /// <summary>
        /// Analyze Page Heading And Accessibility.
        /// </summary>
        /// <param name="pageTitle">Page Title.</param>
        public void AnalyzePageHeadingAndAccessibility(string pageTitle)
        {
            this.ValidatePageHeading(pageTitle);

            // then
            var axeResult = new AxeBuilder(this.Driver).Analyze();
            axeResult.Violations.Should().BeEmpty();
        }

        /// <summary>
        /// ValidatePageHeading.
        /// </summary>
        /// <param name="pageTitle">Page Title.</param>
        public void ValidatePageHeading(string pageTitle)
        {
            var h1Element = this.Driver.FindElement(By.TagName("h1"));
            h1Element.Text.Should().BeEquivalentTo(pageTitle);
        }
    }
}
