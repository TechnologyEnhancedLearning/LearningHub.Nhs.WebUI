namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// Catalogue Folder Content Tests.
    /// </summary>
    public class CatalogueFolderContentTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueFolderContentTests"/> class.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public CatalogueFolderContentTests(AuthenticatedAccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// DashboardPageHasNoAccessibilityErrors.
        /// </summary>
        [Fact]
        public void CatalogueFolderContentPageHasNoAccessibilityErrors()
        {
            // given
            const string catalogueUrl = "/catalogue/Neonatal-AHP";

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + catalogueUrl);
            this.ValidatePageHeading("Enhanced Modules for Allied Health Professionals Working in Neonatal Care");
            var cataloguePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            ////this.Driver.ClickLinkContainingText("Manage this catalogue");
            ////this.ValidatePageHeading("Catalogue Management");
            ////var catalogueManagementPageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            // then
            cataloguePageResult.Violations.Should().BeEmpty();
        }
    }
}
