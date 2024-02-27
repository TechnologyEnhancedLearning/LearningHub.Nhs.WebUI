// <copyright file="DriverHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;

    /// <summary>
    /// Driver Helper.
    /// </summary>
    public static class DriverHelper
    {
        /// <summary>
        /// Create Headless ChromeDriver.
        /// </summary>
        /// <returns>Chrome Driver.</returns>
        public static ChromeDriver CreateHeadlessChromeDriver()
        {
            var chromeOptions = new ChromeOptions();

            chromeOptions.AddArgument("--headless");
            return new ChromeDriver(chromeOptions);
        }

        /// <summary>
        /// Fill Text Input.
        /// </summary>
        /// <param name="driver">WebDriver.</param>
        /// <param name="inputId">inputId.</param>
        /// <param name="inputText">inputText.</param>
        public static void FillTextInput(this IWebDriver driver, string inputId, string inputText)
        {
            var answer = driver.FindElement(By.Id(inputId));
            answer.Clear();
            answer.SendKeys(inputText);
        }

        /// <summary>
        /// ClickButtonByText.
        /// </summary>
        /// <param name="driver">WebDriver.</param>
        /// <param name="text">text.</param>
        public static void ClickButtonByText(this IWebDriver driver, string text)
        {
            var addButton = driver.FindElement(By.XPath($"//button[.='{text}']"));
            addButton.Click();
        }

        /// <summary>
        /// ClickLinkContainingText.
        /// </summary>
        /// <param name="driver">WebDriver.</param>
        /// <param name="text">text.</param>
        public static void ClickLinkContainingText(this IWebDriver driver, string text)
        {
            var foundLink = driver.FindElement(By.XPath($"//a[contains(., '{text}')]"));
            foundLink.Click();
        }

        /// <summary>
        /// SelectDropdownItemValue.
        /// </summary>
        /// <param name="driver">WebDriver.</param>
        /// <param name="dropdownId">dropdownId.</param>
        /// <param name="selectedValue">selectedValue.</param>
        public static void SelectDropdownItemValue(this IWebDriver driver, string dropdownId, string selectedValue)
        {
            var dropdown = new SelectElement(driver.FindElement(By.Id(dropdownId)));
            dropdown.SelectByValue(selectedValue);
        }

        /// <summary>
        /// SetCheckboxState.
        /// </summary>
        /// <param name="driver">WebDriver.</param>
        /// <param name="inputId">inputId.</param>
        /// <param name="checkState">checkState.</param>
        public static void SetCheckboxState(this IWebDriver driver, string inputId, bool checkState)
        {
            var answer = driver.FindElement(By.Id(inputId));
            if (answer.Selected != checkState)
            {
                answer.Click();
            }
        }

        /// <summary>
        /// Submit Form.
        /// </summary>
        /// <param name="driver">WebDriver.</param>
        public static void SubmitForm(this IWebDriver driver)
        {
            var selectPromptForm = driver.FindElement(By.TagName("form"));
            selectPromptForm.Submit();
        }

        /// <summary>
        /// Select Radio Option By Id.
        /// </summary>
        /// <param name="driver">WebDriver.</param>
        /// <param name="radioId">radio Id.</param>
        public static void SelectRadioOptionById(this IWebDriver driver, string radioId)
        {
            var radioInput = driver.FindElement(By.Id(radioId));
            radioInput.Click();
        }
    }
}
