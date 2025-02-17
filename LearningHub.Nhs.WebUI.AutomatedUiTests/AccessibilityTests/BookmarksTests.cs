namespace LearningHub.Nhs.WebUI.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestFixtures;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using OpenQA.Selenium;
    using Selenium.Axe;
    using Xunit;

    /// <summary>
    /// Bookmarks Tests.
    /// </summary>
    public class BookmarksTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarksTests"/> class.
        /// </summary>
        /// <param name="fixture">fixture.</param>
        public BookmarksTests(AuthenticatedAccessibilityTestsFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// BookmarksPageHasNoAccessibilityErrors.
        /// </summary>
        [Fact]
        public void BookmarksPageHasNoAccessibilityErrors()
        {
            // given
            const string resourceUrl = "/Resource/91/Item";
            const string addBookmarkPageUrl = "/bookmark/resource?bookmarked=False&title=Understanding%20and%20managing%20conflict%20in%20children%27s%20healthcare&rri=16593&returnUrl=%2FResource%2F16593%2FItem";
            const string myBookmarksPage = "/bookmark";
            const string bookmarkname = "Removal and disposal of Personal Protective Equipment (PPE)";
            IWebElement renameBookmarkElement = null;
            IWebElement addBookmarkElement = null;
            IWebElement moveBookmarkElement = null;
            AxeResult addBookmarkPageResult = null;

            // when
            this.Driver.Navigate().GoToUrl(this.BaseUrl + resourceUrl);
            try
            {
                addBookmarkElement = this.Driver.FindElement(By.XPath("//a[contains(text(),'Add to  my bookmarks')]"));
                if (addBookmarkElement.Displayed)
                {
                    this.Driver.ClickLinkContainingText("Add to  my bookmarks");
                    this.ValidatePageHeading("Add bookmark");
                    addBookmarkPageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
                    this.Driver.ClickButtonByText("Continue");
                }
            }
            catch (NoSuchElementException)
            {
                this.Driver.Navigate().GoToUrl(this.BaseUrl + addBookmarkPageUrl);
                this.ValidatePageHeading("Add bookmark");
                addBookmarkPageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
                this.Driver.ClickLinkContainingText("Cancel");
            }

            this.Driver.Navigate().GoToUrl(this.BaseUrl + myBookmarksPage);
            this.ValidatePageHeading("Bookmarked learning");
            var myBookmarksPageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            this.Driver.ClickLinkContainingText("Add a folder");
            this.ValidatePageHeading("Add a folder");
            var addBookmarkFolderPageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Cancel");

            renameBookmarkElement = this.Driver.FindElement(By.XPath($"//tr[td//span[contains(text(), '{bookmarkname}')]]//td//div//form//span//button[contains(text(), 'Rename')]"));
            renameBookmarkElement.Click();
            this.ValidatePageHeading("Rename bookmark");
            var renameBookmarkPageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Cancel");

            moveBookmarkElement = this.Driver.FindElement(By.XPath($"//tr[td//span[contains(text(), '{bookmarkname}')]]//td//div//form//span//button[contains(text(), 'Move')]"));
            moveBookmarkElement.Click();
            this.ValidatePageHeading("Move your bookmark");
            var moveBookmarkPageResult = new AxeBuilder(this.Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            this.Driver.ClickLinkContainingText("Cancel");

            // then
            addBookmarkPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            myBookmarksPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            addBookmarkFolderPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            renameBookmarkPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
            moveBookmarkPageResult.Violations.Where(v => !v.Tags.Contains("best-practice")).Should().BeEmpty();
        }
    }
}
