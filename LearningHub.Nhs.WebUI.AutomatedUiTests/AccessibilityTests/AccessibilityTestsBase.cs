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
        internal readonly string BaseUrl;

        /// <summary>
        /// Gets the WebDriver used for the tests.
        /// </summary>
        internal readonly IWebDriver Driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessibilityTestsBase"/> class.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public AccessibilityTestsBase(AccessibilityTestsFixture fixture)
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
            // Exclude conditional radios, see: https://github.com/alphagov/govuk-frontend/issues/979#issuecomment-872300557
            var axeResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            axeResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
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
