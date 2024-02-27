// <copyright file="BasicAccessibilityTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using Xunit;

    /// <summary>
    /// BasicAccessibilityTests.
    /// </summary>
    public class BasicAccessibilityTests : AccessibilityTestsBase, IClassFixture<AccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAccessibilityTests"/> class.
        /// BasicAccessibilityTests.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public BasicAccessibilityTests(AccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        [Theory]
        [InlineData("/Home/Index", "A platform for learning and sharing resources")]

#pragma warning disable SA1600 // Elements should be documented
        public void PageHasNoAccessibilityErrors(string url, string pageTitle)
#pragma warning restore SA1600 // Elements should be documented
        {
            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + url);

            // then
            this.AnalyzePageHeadingAndAccessibility(pageTitle);

            this.Driver.Dispose();
        }
    }
}
