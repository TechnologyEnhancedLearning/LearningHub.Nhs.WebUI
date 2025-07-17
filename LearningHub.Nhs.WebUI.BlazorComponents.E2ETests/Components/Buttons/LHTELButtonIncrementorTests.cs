using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using FluentAssertions;
using LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.BlazeWright;
using LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using Microsoft.Playwright.Xunit;


namespace LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.Components.Buttons
{

    public class LHTELButtonIncrementorTests :PageTest // qqqq for now no blazewright because mvc and now seperate test pages and host may need something for cicd : BlazorPageTest<Program>
    {
        private readonly bool _tracingEnabled;
        // qqqq worry about cicd and kestrel later take advantage of iis
        private const string BaseUrl = "https://lh-web.dev.local";  // or localhost:port
        public LHTELButtonIncrementorTests()
        {
            // qqqq does this work! and if se why is headless different!
            // qqqq hardcode for now _tracingEnabled = (bool.TryParse(Environment.GetEnvironmentVariable("E2ETracingEnabled"), out var result) && result);
            _tracingEnabled = false; //qqqq hardcode for now
        }



        // Axe needs js
        [Theory]
        [InlineData("chromium", true, ViewportHelper.ViewportType.Desktop)]
        //[InlineData("chromium", false, ViewportHelper.ViewportType.Desktop)]
        [InlineData("chromium", true, ViewportHelper.ViewportType.Mobile)]
        //[InlineData("chromium", false, ViewportHelper.ViewportType.Mobile)]

        [InlineData("firefox", true, ViewportHelper.ViewportType.Desktop)]
        //[InlineData("firefox", false, ViewportHelper.ViewportType.Desktop)]
        [InlineData("firefox", true, ViewportHelper.ViewportType.Mobile)]
        //[InlineData("firefox", false, ViewportHelper.ViewportType.Mobile)]

        [InlineData("webkit", true, ViewportHelper.ViewportType.Desktop)]
        //[InlineData("webkit", false, ViewportHelper.ViewportType.Desktop)]
        [InlineData("webkit", true, ViewportHelper.ViewportType.Mobile)]
        //[InlineData("webkit", false, ViewportHelper.ViewportType.Mobile)]

        public async Task LHTELButtonIncrementor_MeetsAxeAccesibilityStandards(string browserType, bool jsEnabled, ViewportHelper.ViewportType viewport)
        {

            using IPlaywright playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            IBrowserContext browserContext = await BrowserHelper.CreateBrowserContextAsync(playwright, browserType, jsEnabled, viewport, BaseUrl);
            //Debug option
            if (_tracingEnabled)
            {
                await browserContext.Tracing.StartAsync(new()
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true,
                });
            }

            IPage page = await browserContext.NewPageAsync();
            // qqqq this may work differently because not blazor so try something on anoth page
            Task<bool>? hydrationTask = null;
            bool hydrationResult = false;

            // Start listening for hydration message before navigation
            if (jsEnabled)
            {
                // Start listening for hydration message before navigation
                hydrationTask = page.ConsoleReportedHydrationWithinTimeAsync();
            }
            await page.GotoAsync($"{BaseUrl}/", new() { WaitUntil = WaitUntilState.NetworkIdle });
            if (jsEnabled && hydrationTask is not null)
            {
                hydrationResult = await hydrationTask;
                Console.WriteLine($"Hydration successful: {hydrationResult}");
            }
            await page.WaitForSelectorAsync("button[aria-label='Click Counter']");
            ILocator button = page.GetByRole(AriaRole.Button, new() { Name = "Click Counter" });

            AxeResult axeResults = await button.RunAxe();

            axeResults.Violations.Should().BeNullOrEmpty();

            if (_tracingEnabled)
            {
                string methodName = "LHTELButtonIncrementor_MeetsAxeAccesibilityStandards";
                string timestamp = DateTime.UtcNow.ToString("yy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture);
                string arguments = $"{browserType}_{$"jsEnabled_{jsEnabled.ToString()}"}_{viewport.ToString()}";
                string path = $"../../../Reports/{methodName}_{arguments}_{timestamp}.zip";
                await browserContext.Tracing.StopAsync(new()
                {
                    Path = path,
                });
            }

            // Clean up resources by closing the page and browser context
            await page.CloseAsync();
            await browserContext.CloseAsync();
        }


        [Theory]
        [InlineData("chromium", true, ViewportHelper.ViewportType.Desktop)]
        [InlineData("chromium", false, ViewportHelper.ViewportType.Desktop)]
        [InlineData("chromium", true, ViewportHelper.ViewportType.Mobile)]
        [InlineData("chromium", false, ViewportHelper.ViewportType.Mobile)]

        [InlineData("firefox", true, ViewportHelper.ViewportType.Desktop)]
        [InlineData("firefox", false, ViewportHelper.ViewportType.Desktop)]
        [InlineData("firefox", true, ViewportHelper.ViewportType.Mobile)]
        [InlineData("firefox", false, ViewportHelper.ViewportType.Mobile)]

        [InlineData("webkit", true, ViewportHelper.ViewportType.Desktop)]
        [InlineData("webkit", false, ViewportHelper.ViewportType.Desktop)]
        [InlineData("webkit", true, ViewportHelper.ViewportType.Mobile)]
        [InlineData("webkit", false, ViewportHelper.ViewportType.Mobile)]
        public async Task LHTELButtonIncrementor_InteractivityIsCorrectlySimulated(string browserType, bool jsEnabled, ViewportHelper.ViewportType viewport)
        {

            using IPlaywright playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            IBrowserContext browserContext = await BrowserHelper.CreateBrowserContextAsync(playwright, browserType, jsEnabled, viewport, BaseUrl);
            //Debug option
            if (_tracingEnabled)
            {
                await browserContext.Tracing.StartAsync(new()
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true,
                });
            }

            IPage page = await browserContext.NewPageAsync();

            var logs = new List<string>();

            page.Console += (_, msg) =>
            {
                logs.Add(msg.Text);
            };

            Task<bool>? hydrationTask = null;
            bool hydrationResult = false;

            // Start listening for hydration message before navigation
            if (jsEnabled)
            {
                // Start listening for hydration message before navigation
                hydrationTask = page.ConsoleReportedHydrationWithinTimeAsync();
            }

            await page.GotoAsync($"{BaseUrl}/", new() { WaitUntil = WaitUntilState.NetworkIdle });

            // Wait for hydration message or timeout
            if (jsEnabled && hydrationTask is not null)
            {
                hydrationResult = await hydrationTask;
                Console.WriteLine($"Hydration successful: {hydrationResult}");
            }
            await page.WaitForSelectorAsync("button[aria-label='Click Counter']");
            ILocator button = page.GetByRole(AriaRole.Button, new() { Name = "Click Counter" });
            await button.ScrollIntoViewIfNeededAsync();
            await Expect(button).ToContainTextAsync("(0)");
            // 1. Check visibility
            // await Expect(button).ToBeVisibleAsync();

            if (jsEnabled)
            {

                // Find the button and click it
                await button.ClickAsync();
                // Wait a bit if necessary for logs to flush
                await page.WaitForTimeoutAsync(500);
                // Check if JavaScript is enabled by verifying the text changes
                await Expect(button).ToContainTextAsync("(1)");
                //await Expect(button).ToContainTextAsync(jsEnabled ? "Button pressed 1 times" : "Button pressed 0 times");


  

                // ✅ Assert the log appeared
                logs.Should().Contain(log => log.Contains("Button clicked:"));

            }
            else
            {
                await page.RouteAsync("**/nojsfallback/mvcendpoint/telbuttonshowcase", async route =>
                {
                    var request = route.Request;
                    var postData = request.PostData;
                    Assert.Contains("increment=1", postData);
                    await route.FulfillAsync(new() { Status = 200, Body = "Intercepted OK" });
                });

                await button.ClickAsync();
            }

            if (_tracingEnabled)
            {
                string methodName = "TELButtonPage_InteractivityIsCorrectlySimulated";
                string timestamp = DateTime.UtcNow.ToString("yy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture);
                string arguments = $"{browserType}_{$"jsEnabled_{jsEnabled.ToString()}"}_{viewport.ToString()}";
                string path = $"../../../Reports/{methodName}_{arguments}_{timestamp}.zip";
                await browserContext.Tracing.StopAsync(new()
                {
                    Path = path,

                });
            }

            // Clean up resources by closing the page and browser context
            await page.CloseAsync();
            await browserContext.CloseAsync();

        }
    }
}

