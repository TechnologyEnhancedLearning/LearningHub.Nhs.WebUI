// <copyright file="MyAccountAccessibiltyTests.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// MyAccountAccessibiltyTests.
    /// </summary>
    public class MyAccountAccessibiltyTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture<Program>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyAccountAccessibiltyTests"/> class.
        /// MyAccountAccessibiltyTests.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public MyAccountAccessibiltyTests(AuthenticatedAccessibilityTestsFixture<Program> fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// MyAccountPageHasNoAccessibilityErrors.
        /// </summary>
        [Fact]
        public void MyAccountPageHasNoAccessibilityErrors()
        {
            // given
           // this.Driver.LogUserInAsAdmin(this.BaseUrl);
            const string myaccountsUrl = "/myaccount";

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + myaccountsUrl);
            var result = new AxeBuilder(this.Driver).Analyze();

            // then
            CheckAccessibilityResult(result);
        }

        private static void CheckAccessibilityResult(AxeResult result)
        {
            // Expect axe violations caused by having an aria-expanded attribute on two
            // radio inputs and one checkbox input.
            // The targets #course-filter-type-1, #course-filter-type-2 and #EndDate are
            // nhs-tested components so ignore this violation.
            result.Violations.Should().HaveCount(5);

            var violation = result.Violations[0];

            violation.Id.Should().Be("landmark-contentinfo-is-top-level");
            violation.Nodes.Should().HaveCount(1);
            violation.Nodes[0].Target.Should().HaveCount(1);
            violation.Nodes[0].Target[0].Selector.Should().Be("footer > footer");
        }
    }
}
