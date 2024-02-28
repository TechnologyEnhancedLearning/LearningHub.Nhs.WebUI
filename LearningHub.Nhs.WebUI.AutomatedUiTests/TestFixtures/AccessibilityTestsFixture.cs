namespace LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures
{
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using OpenQA.Selenium;

    /// <summary>
    /// Represents a fixture for accessibility tests.
    /// </summary>
    public class AccessibilityTestsFixture
    {
        /// <summary>
        /// Gets the base URL for the tests.
        /// </summary>
        internal readonly string BaseUrl;

        /// <summary>
        /// Gets the WebDriver used for the tests.
        /// </summary>
        internal readonly IWebDriver Driver;

        private readonly SeleniumServerFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessibilityTestsFixture"/> class.
        /// </summary>
        public AccessibilityTestsFixture()
        {
            this.factory = new SeleniumServerFactory();
            this.BaseUrl = this.factory.RootUri;
            this.Driver = DriverHelper.CreateHeadlessChromeDriver();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            this.Driver.Quit();
            this.Driver.Dispose();
        }
    }
}
