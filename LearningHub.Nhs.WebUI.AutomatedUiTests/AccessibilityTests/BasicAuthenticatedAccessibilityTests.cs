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
        [InlineData("/?myLearningDashboard=my-in-progress&resourceDashboard=popular-resources&catalogueDashboard=popular-catalogues", "Learning Hub - Home")]
        [InlineData("/myaccount", "My account details")]
        [InlineData("/MyLearning", "My learning")]
        [InlineData("/allcatalogue", "A-Z of catalogues")]
        [InlineData("/allcataloguesearch?term=test#searchTab", "Search results for test")]
        [InlineData("/my-contributions", "Community contributions")]
        [InlineData("/bookmark", "Bookmarked learning")]
        [InlineData("/Resource/309/Item", "IE11 Image test")]
        [InlineData("/Resource/91/Item", "Removal and disposal of Personal Protective Equipment (PPE)")]
        [InlineData("/Resource/15458/Item", "Test PDF File 16Dec")]
        [InlineData("/Resource/17014/Item", "How to develop your teaching skills | BMJ Careers")]
        [InlineData("/Resource/17760/Item", "HTML")]
        public void AuthenticatedPageHasNoAccessibilityErrors(string url, string pageTitle)
        {
            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + url);
            ////string currentUrl = this.Driver.Url;
            ////if (currentUrl.Contains("/myaccount"))
            ////{
            ////    var submitButton = this.Driver.FindElement(By.TagName("form"));
            ////    submitButton.Submit();
            ////}

            // then
            this.AnalyzePageHeadingAndAccessibility(pageTitle);

            // Dispose driver
            this.Driver.LogOutUser(this.BaseUrl);
        }
    }
}
