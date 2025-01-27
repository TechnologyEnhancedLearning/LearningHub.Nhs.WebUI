namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
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

        [Theory]
        [InlineData("/Home/Index", "Learning Hub - Home")]
        [InlineData("/forgotten-password", "Forgotten your username or password")]
        [InlineData("/Login", "Access your Learning Hub account")]

        public void PageHasNoAccessibilityErrors(string url, string pageTitle)
        {
            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + url);

            // then
            this.AnalyzePageHeadingAndAccessibility(pageTitle);

            this.Driver.Dispose();
        }
    }
}
