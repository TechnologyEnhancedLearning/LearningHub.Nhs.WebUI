// <copyright file="LoginHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>
namespace LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    /// <summary>
    /// LoginHelper.
    /// </summary>
    public static class LoginHelper
    {
        /// <summary>
        /// Get LogUserInAsAdmin.
        /// </summary>
        /// <param name="driver">WebDriver.</param>
        /// <param name="baseUrl">baseUrl.</param>
        /// <param name="adminName">adminName.</param>
        /// <param name="adminPassword">adminPassword.</param>
        public static void LogUserInAsAdmin(this IWebDriver driver, string baseUrl, string adminName, string adminPassword)
        {
            driver.Navigate().GoToUrl(baseUrl + "/Login");
            var username = driver.FindElement(By.Id("Username"));
            username.SendKeys(adminName);

            var password = driver.FindElement(By.Id("Password"));
            password.SendKeys(adminPassword);

            var submitButton = driver.FindElement(By.TagName("form"));
            submitButton.Submit();
        }

        /// <summary>
        /// LogOutUser.
        /// </summary>
        /// <param name="driver">WebDriver.</param>
        /// <param name="baseUrl">baseUrl.</param>
        public static void LogOutUser(this IWebDriver driver, string baseUrl)
        {
            driver.Navigate().GoToUrl(baseUrl);

            // Wait for the element to be present on the page
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement logoutLink = driver.FindElement(By.XPath("//a[@class='nhsuk-account__login--link' and @href='/Home/Logout']"));

            // Perform an action on the element (e.g., click)
            logoutLink.Click();

            // var submitButton = driver.FindElement(By.TagName("form"));
            // submitButton.Submit();
        }
    }
}
