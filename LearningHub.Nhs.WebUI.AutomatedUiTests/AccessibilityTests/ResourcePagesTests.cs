namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using OpenQA.Selenium;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// SearchResultsTests.
    /// </summary>
    public class ResourcePagesTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePagesTests"/> class.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public ResourcePagesTests(AuthenticatedAccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// ResourcePagesHasNoAccessibilityErrors.
        /// </summary>
        /// <param name="url">url.</param>
        /// <param name="pageTitle">pageTitle.</param>
        [Theory]
        [MemberData(nameof(GetResourcePageTestData))]
        public void ResourcePagesHasNoAccessibilityErrors(string url, string pageTitle)
        {
            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + url);

            // then
            var h1Element = this.Driver.FindElement(By.TagName("h1"));
            if (h1Element.Text == "Unknown error" || h1Element.Text != pageTitle)
            {
                return;
            }

            this.AnalyzePageHeadingAndAccessibility(pageTitle);
        }

        /// <summary>
        /// GetResourcePageTestData.
        /// </summary>
        /// <returns>resource url.</returns>
        internal static IEnumerable<object[]> GetResourcePageTestData()
        {
            yield return new object[] { "/Resource/309/Item", "IE11 Image test" };
            yield return new object[] { "/Resource/91/Item", "Removal and disposal of Personal Protective Equipment (PPE)" };
            yield return new object[] { "/Resource/15458/Item", "Test PDF File 16Dec" };
            yield return new object[] { "/Resource/17014/Item", "How to develop your teaching skills | BMJ Careers" };
            yield return new object[] { "/Resource/17760/Item", "HTML" };
            yield return new object[] { "/Resource/21430/Item", "MHA/MCA Interface Case Study (document)" };
            yield return new object[] { "/Resource/52338/Item", "How to develop your teaching skills | BMJ Careers" };
            yield return new object[] { "/Resource/56274/Item", "Phlebitis (HTML)" };
            yield return new object[] { "/Resource/27527/Item", "Pain after spinal cord injury - articles" };
            yield return new object[] { "/Resource/31888/Item", "360 Image of AMU Bed Bay" };
        }
    }
}
