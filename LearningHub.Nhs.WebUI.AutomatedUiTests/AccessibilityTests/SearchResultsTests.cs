namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// SearchResultsTests.
    /// </summary>
    public class SearchResultsTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResultsTests"/> class.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public SearchResultsTests(AuthenticatedAccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// SearchResultPageHasNoAccessibilityErrors.
        /// </summary>
        [Fact]
        public void SearchResultPageHasNoAccessibilityErrors()
        {
            // given
            const string searchResultUrl = "/search/results?term=primary";
            const string catalogueSearchResultUrl = "/catalogues?term=primary";

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + searchResultUrl);
            this.ValidatePageHeading("Search results for primary");
            var searchResultPageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            this.Driver.Navigate().GoToUrl(this.BaseUrl + catalogueSearchResultUrl);
            ////this.ValidatePageHeading("Search results for primary");
            var catalogueSearchResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            // then
            searchResultPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            catalogueSearchResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
        }
    }
}
