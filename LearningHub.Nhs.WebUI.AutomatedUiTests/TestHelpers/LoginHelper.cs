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

            try
            {
                // Maximum time to wait for the element in seconds
                int maxWaitTimeInSeconds = 10;

                // Find the element using XPath
                IWebElement logoutLink = null;

                for (int i = 0; i < maxWaitTimeInSeconds; i++)
                {
                    try
                    {
                        logoutLink = driver.FindElement(By.XPath("//a[@class='nhsuk-account__login--link' and @href='/Home/Logout']"));
                        if (logoutLink.Displayed)
                        {
                            break; // Exit the loop if element is found and displayed
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        // Element not found yet, wait for a second and try again
                        Thread.Sleep(1000);
                    }
                }

                // Check if the element is found and displayed
                if (logoutLink != null && logoutLink.Displayed)
                {
                    // Perform an action on the element (e.g., click)
                    logoutLink.Click();
                }
            }
            finally
            {
                // Close the browser window
                driver.Quit();
            }
        }
    }
}
