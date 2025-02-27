namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using Microsoft.Azure.Management.Media.Models;
    using OpenQA.Selenium;
    using Xunit;

    /// <summary>
    /// BasicAuthenticatedAccessibilityTests.
    /// </summary>
    public class BasicAuthenticatedAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticatedAccessibilityTests"/> class.
        /// BasicAuthenticatedAccessibilityTests.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public BasicAuthenticatedAccessibilityTests(AuthenticatedAccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Authenticated Page Has no Accessibility Errors.
        /// </summary>
        /// <param name="url">url.</param>
        /// <param name="pageTitle">pageTitle.</param>
        [Theory]
        [InlineData("/myaccount", "My account details")]
        [InlineData("/MyLearning", "My learning")]
        [InlineData("/allcatalogue", "A-Z of catalogues")]
        [InlineData("/allcataloguesearch?term=primary#searchTab", "Search results for primary")]
        public void AuthenticatedPageHasNoAccessibilityErrors(string url, string pageTitle)
        {
            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + url);

            // then
            this.AnalyzePageHeadingAndAccessibility(pageTitle);

            // Dispose driver
            ////this.Driver.LogOutUser(this.BaseUrl);
        }
    }
}
