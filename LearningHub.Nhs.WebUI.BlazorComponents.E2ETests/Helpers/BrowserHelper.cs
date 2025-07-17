using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.Helpers.ViewportHelper;

namespace LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.Helpers
{
    public static class BrowserHelper
    {
        // qqqq Setting value using PackageSetting.props potentially replace appsettings.Test.json in future especially if using apis during testing
//        static bool headless =>
//#if HEADLESS_TESTING
//                        true;
//#else
//                        false;
//#endif

        public static async Task<IBrowserContext> CreateBrowserContextAsync(IPlaywright playwright, string browserType, bool jsEnabled, ViewportType viewport, string baseUrl)
        {
            bool headless = true; // qqqqq hardcode for now
            IBrowser browser;

            switch (browserType.ToLower())
            {
                case "chromium":
                    browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });
                    break;
                case "firefox":
                    browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });
                    break;
                case "webkit":
                    browser = await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });
                    break;
                default:
                    throw new ArgumentException($"Unsupported browser type: {browserType}");
            }


            BrowserNewContextOptions contextOptions = new BrowserNewContextOptions
            {

                JavaScriptEnabled = jsEnabled,
                BaseURL = baseUrl,
                IgnoreHTTPSErrors = true,
                ViewportSize = ViewportHelper.Viewports[viewport]
            };
            IBrowserContext context = await browser.NewContextAsync(contextOptions);

            return context;

        }

    }
}
