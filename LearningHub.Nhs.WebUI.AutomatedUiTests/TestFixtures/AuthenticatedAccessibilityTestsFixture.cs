namespace LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures
{
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// AuthenticatedAccessibilityTestsFixture.
    /// </summary>
    /// <typeparam name="TStartup">TStartup.</typeparam>
    public class AuthenticatedAccessibilityTestsFixture : AccessibilityTestsFixture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatedAccessibilityTestsFixture"/> class.
        /// </summary>
        public AuthenticatedAccessibilityTestsFixture()
        {
            IConfiguration configuration = ConfigurationHelper.GetConfiguration();
            string adminUsername = configuration["AdminUser:Username"];
            string adminPassword = configuration["AdminUser:Password"];
            this.Driver.LogUserInAsAdmin(this.BaseUrl, adminUsername, adminPassword);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public new void Dispose()
        {
            this.Driver.LogOutUser(this.BaseUrl);
            this.Driver.Quit();
            base.Dispose();
        }
    }
}
