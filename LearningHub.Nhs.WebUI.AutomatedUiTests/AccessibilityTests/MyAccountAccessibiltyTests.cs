namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// MyAccountAccessibiltyTests.
    /// </summary>
    public class MyAccountAccessibiltyTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyAccountAccessibiltyTests"/> class.
        /// MyAccountAccessibiltyTests.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public MyAccountAccessibiltyTests(AuthenticatedAccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// MyAccount Page Has Accessibility Errors.
        /// </summary>
        [Fact]
        public void MyAccountPageHasAccessibilityErrors()
        {
            // given
            const string myaccountsUrl = "/myaccount";

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + myaccountsUrl);
            var result = new AxeBuilder(this.Driver).Analyze();

            // then
            CheckAccessibilityResult(result);

            // Dispose driver
            this.Driver.LogOutUser(this.BaseUrl);
        }

        private static void CheckAccessibilityResult(AxeResult result)
        {
            // Expect axe violation
            result.Violations.Should().HaveCount(6);

            var violation = result.Violations[1];

            violation.Id.Should().Be("landmark-contentinfo-is-top-level");
            violation.Nodes.Should().HaveCount(1);
            violation.Nodes[0].Target.Should().HaveCount(1);
            violation.Nodes[0].Target[0].Selector.Should().Be("footer > footer");
        }
    }
}
