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
            ////const string changeFirstNameUrl = "/myaccount/ChangeFirstname";
            ////const string changeLastNameUrl = "/myaccount/ChangeLastName";
            ////const string changePreferredNameUrl = "/myaccount/ChangePreferredName";
            ////const string changeCountryUrl = "/myaccount/ChangeCountry";
            ////const string changeSecEmailUrl = "/myaccount/ChangeSecondaryEmail";
            ////const string changeFirstQuestionUrl = "/MyAccount/SecurityQuestionsDetails?questionIndex=0";
            ////const string changeSecQuestionUrl = "/MyAccount/SecurityQuestionsDetails?questionIndex=1";
            ////const string changeCurrentRoleUrl = "/my-account/change-current-role";
            ////const string changeGradeUrl = "/myaccount";
            ////const string changePrimarySpecialityUrl = "/myaccount";
            ////const string changeStartDateUrl = "/myaccount";
            ////const string changePlaceOfWorkUrl = "/my-account/change-work-place";

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + myaccountsUrl);
            this.ValidatePageHeading("My account details");
            var myAccountPageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            this.Driver.ClickLinkContainingText("first name");
            this.ValidatePageHeading("Update your first name");
            var firstNameChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");
            this.ValidatePageHeading("My account details");
            this.Driver.ClickLinkContainingText("last name");
            this.ValidatePageHeading("Update your last name");
            var lastNameChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("preferred name");
            this.ValidatePageHeading("Update your preferred name");
            var preferredNameChangeResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("country");
            this.ValidatePageHeading("Update your country");
            var countryChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            ////this.Driver.ClickLinkContainingText("region");
            ////this.ValidatePageHeading("Update your region");
            ////var regionChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            ////this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("secondry email");
            this.ValidatePageHeading("Enter your new secondary email address");
            var emailChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("security question 1");
            this.ValidatePageHeading("Choose your first security question");
            var securityQuestChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("security question 2");
            this.ValidatePageHeading("Choose your second security question");
            var securityQuest2ChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("current role");
            this.ValidatePageHeading("Update current job role");
            var roleChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("grade");
            this.ValidatePageHeading("Update grade");
            var gradeChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("primary specialty");
            this.ValidatePageHeading("Update primary specialty");
            var primarySpecialityChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("start date");
            this.ValidatePageHeading("Update start date");
            var startDateChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            this.Driver.ClickLinkContainingText("place of work");
            this.ValidatePageHeading("Update place of work");
            var placeOfWorkChangePageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Go back");

            // then
            myAccountPageResult.Violations.Should().BeEmpty();
            firstNameChangePageResult.Violations.Should().BeEmpty();
            lastNameChangePageResult.Violations.Should().BeEmpty();
            preferredNameChangeResult.Violations.Should().BeEmpty();
            countryChangePageResult.Violations.Should().BeEmpty();
            ////regionChangePageResult.Violations.Should().BeEmpty();
            emailChangePageResult.Violations.Should().BeEmpty();
            securityQuestChangePageResult.Violations.Should().BeEmpty();
            securityQuest2ChangePageResult.Violations.Should().BeEmpty();
            roleChangePageResult.Violations.Should().BeEmpty();
            gradeChangePageResult.Violations.Should().BeEmpty();
            primarySpecialityChangePageResult.Violations.Should().BeEmpty();
            startDateChangePageResult.Violations.Should().BeEmpty();
            placeOfWorkChangePageResult.Violations.Should().BeEmpty();
            ////CheckAccessibilityResult(result);

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
