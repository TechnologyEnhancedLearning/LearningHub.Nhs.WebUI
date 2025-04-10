namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// Contributions Tests.
    /// </summary>
    public class MyContributionsTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyContributionsTests"/> class.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public MyContributionsTests(AuthenticatedAccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// MyContributionsPageHasNoAccessibilityErrors.
        /// </summary>
        [Fact]
        public void MyContributionsPageHasNoAccessibilityErrors()
        {
            // given
            const string myContributionUrl = "/my-contributions";

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + myContributionUrl);
            var axeResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            // then
            CheckAccessibilityResult(axeResult);
        }

        private static void CheckAccessibilityResult(AxeResult result)
        {
            result.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
        }
    }
}
