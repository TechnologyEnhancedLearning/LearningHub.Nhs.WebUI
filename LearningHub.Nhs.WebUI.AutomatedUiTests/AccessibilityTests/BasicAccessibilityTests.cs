namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using Selenium.Axe;
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

        /// <summary>
        /// PageHasNoAccessibilityErrors.
        /// </summary>
        /// <param name="url">url to the page.</param>
        /// <param name="pageTitle">title of the page.</param>
        [Theory]
        [InlineData("/forgotten-password", "Forgotten your username or password")]
        [InlineData("/Login", "Access your Learning Hub account")]
        [InlineData("/Home/Contactus", "Contact us")]
        [InlineData("/Home/Aboutus", "About us")]
        [InlineData("/Updates", "Service updates and releases")]
        [InlineData("/Policies", "Our policies")]
        [InlineData("/Home/Accessibility", "Accessibility Statement for the NHS England Learning Hub")]
        [InlineData("/Home/NHSsites", "NHS sites")]
        [InlineData("/policies/terms-and-conditions", "NHS England Learning Hub Terms and Conditions of Use (‘Terms’)")]
        [InlineData("/policies/content-policy", "NHS England Learning Hub Content Policy")]
        [InlineData("/policies/privacy-policy", "PRIVACY NOTICE")]
        [InlineData("/policies/cookie-policy", "Cookie policy")]
        [InlineData("/policies/acceptable-use-policy", "ACCEPTABLE USE POLICY")]

        public void PageHasNoAccessibilityErrors(string url, string pageTitle)
        {
            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + url);

            // then
            this.AnalyzePageHeadingAndAccessibility(pageTitle);

            ////this.Driver.Dispose();
        }
    }
}
