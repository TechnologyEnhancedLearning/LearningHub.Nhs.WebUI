namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// CreateAccountTests.
    /// </summary>
    public class CreateAccountTests : AccessibilityTestsBase, IClassFixture<AccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAccountTests"/> class.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public CreateAccountTests(AccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// CreateFullUserAccountPageHasNoAccessibilityErrors.
        /// </summary>
        [Fact]
        public void CreateFullUserAccountPageHasNoAccessibilityErrors()
        {
            // given
            const string createAccountUrl = "/Registration/CreateAccountRegInfo";

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl);
            this.Driver.ClickLinkContainingText("Create new account");
            this.ValidatePageHeading("What you need to set up an account");
            var accountPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.ClickButtonByText("Continue");
            this.Driver.FindElement(By.XPath("//button[normalize-space(text())='Continue']")).Click();

            // Verify the heading on the new page
            this.ValidatePageHeading("Create an account");
            var emailVerificationPageResult = new AxeBuilder(this.Driver).Analyze();

            this.Driver.FillTextInput("Email", "testuser4@nhs.net");
            this.Driver.FillTextInput("ComfirmEmail", "testuser4@nhs.net");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("Full User Account");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("Enter your personal details");
            var personalDetailsPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.FillTextInput("FirstName", "test");
            this.Driver.FillTextInput("LastName", "user");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("Search for your country");
            this.Driver.FillTextInput("FilterText", "England");
            this.Driver.SubmitForm();
            var countryPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.SelectRadioOptionById("CountryId-0");
            this.Driver.ClickButtonByText("Continue");

            this.ValidatePageHeading("Select your region");
            var regionPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.SelectRadioOptionById("RegionId-0");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("Search for your current role");
            this.Driver.FillTextInput("FilterText", "admin");
            this.Driver.SubmitForm();
            var rolePageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.SelectRadioOptionById("CurrentRole-0");
            this.Driver.ClickButtonByText("Continue");

            this.ValidatePageHeading("Select your grade");
            var gradePageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.SelectRadioOptionById("GradeId-0");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("Search for your primary specialty");
            var specialityPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.SelectRadioOptionById("PrimarySpecialtyId-0");
            this.Driver.ClickButtonByText("Continue");

            this.ValidatePageHeading("Enter your start date");
            var startDatePageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.FillTextInput("Day", "1");
            this.Driver.FillTextInput("Month", "1");
            this.Driver.FillTextInput("Year", "2022");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("Search for your place of work");
            this.Driver.FillTextInput("FilterText", "london");
            this.Driver.SubmitForm();
            var placeOfWorkPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.SelectRadioOptionById("LocationId-0");
            this.Driver.ClickButtonByText("Continue");

            this.ValidatePageHeading("Check your details");
            var summaryPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.Navigate().GoToUrl(this.BaseUrl);

            // then
            emailVerificationPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            personalDetailsPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            countryPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            regionPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            rolePageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            gradePageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            specialityPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            startDatePageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            placeOfWorkPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            summaryPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
        }

        /// <summary>
        /// CreateGeneralUserAccountPageHasNoAccessibilityErrors.
        /// </summary>
        [Fact]
        public void CreateGeneralUserAccountPageHasNoAccessibilityErrors()
        {
            // given
            const string createAccountUrl = "/Registration/CreateAccountRegInfo";

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl);
            ////this.Driver.Navigate().GoToUrl(this.BaseUrl + createAccountUrl);
            this.Driver.ClickLinkContainingText("Create new account");
            this.ValidatePageHeading("What you need to set up an account");
            var accountPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.ClickButtonByText("Continue");
            this.Driver.FindElement(By.XPath("//button[normalize-space(text())='Continue']")).Click();

            // Verify the heading on the new page
            this.ValidatePageHeading("Create an account");
            var emailVerificationPageResult = new AxeBuilder(this.Driver).Analyze();

            this.Driver.FillTextInput("Email", "testuser4@gmail.com");
            this.Driver.FillTextInput("ComfirmEmail", "testuser4@gmail.com");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("General User Account");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("Enter your personal details");
            var personalDetailsPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.FillTextInput("FirstName", "test");
            this.Driver.FillTextInput("LastName", "user");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("Search for your country");
            this.Driver.FillTextInput("FilterText", "England");
            this.Driver.SubmitForm();
            var countryPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.SelectRadioOptionById("CountryId-0");
            this.Driver.ClickButtonByText("Continue");

            this.ValidatePageHeading("Select your region");
            var regionPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.SelectRadioOptionById("RegionId-0");
            this.Driver.SubmitForm();

            this.ValidatePageHeading("Search for your current role");
            this.Driver.FillTextInput("FilterText", "admin");
            this.Driver.SubmitForm();
            var rolePageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.SelectRadioOptionById("CurrentRole-0");
            this.Driver.ClickButtonByText("Continue");

            this.ValidatePageHeading("Check your details");
            var summaryPageResult = new AxeBuilder(this.Driver).Analyze();
            this.Driver.Navigate().GoToUrl(this.BaseUrl);

            // then
            emailVerificationPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            personalDetailsPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            countryPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            regionPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            rolePageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            summaryPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
        }
    }
}
