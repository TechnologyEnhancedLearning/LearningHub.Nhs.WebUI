namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// Post login dashboard tests.
    /// </summary>
    public class DashboardTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardTests"/> class.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public DashboardTests(AuthenticatedAccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// DashboardPageHasNoAccessibilityErrors.
        /// </summary>
        [Fact]
        public void DashboardPageHasNoAccessibilityErrors()
        {
            // given
            const string dashboardUrl = "/?myLearningDashboard=my-in-progress&resourceDashboard=popular-resources&catalogueDashboard=popular-catalogues";

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + dashboardUrl);
            var axeResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            // then
            CheckAccessibilityResult(axeResult);
        }

        private static void CheckAccessibilityResult(AxeResult result)
        {
            result.Violations.Should().BeEmpty();
        }
    }
}
