using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.Helpers.HydrationDetectionHelper;

namespace LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.Infrastructure
{

        public class BlazorHydrationTests // qqqq for now no blazewright because mvc and now seperate test pages and host may need something for cicd : BlazorPageTest<Program>
        {
            private readonly bool _tracingEnabled;
            // qqqq worry about cicd and kestrel later take advantage of iis
            private const string BaseUrl = "https://lh-web.dev.local";  // or localhost:port
            public BlazorHydrationTests()
            {
                // qqqq does this work! and if se why is headless different!
                // qqqq hardcode for now _tracingEnabled = (bool.TryParse(Environment.GetEnvironmentVariable("E2ETracingEnabled"), out var result) && result);
                _tracingEnabled = false;
            }



            // Axe needs js
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
            public async Task BlazorClientLogs(string browserType, bool jsEnabled, ViewportHelper.ViewportType viewport)
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

            // qqqq this may work differently because not blazor so try something on another page
            // await page.GotoOnceNetworkIsIdleAsync("TELButton");
            // Start listening for hydration message before navigation
            Task<bool>? hydrationTask = null;
            bool hydrationResult = false;

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
            }

            if (_tracingEnabled)
                {
                    string methodName = "BlazorClientLogs";
                    string timestamp = DateTime.UtcNow.ToString("yy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture);
                    string arguments = $"{browserType}_{$"jsEnabled_{jsEnabled.ToString()}"}_{viewport.ToString()}";
                    string path = $"../../../Reports/{methodName}_{arguments}_{timestamp}.zip";
                    await browserContext.Tracing.StopAsync(new()
                    {
                        Path = path,
                    });
                }

                Assert.True(jsEnabled == hydrationResult, jsEnabled?"Blazor WASM did not hydrate within the expected 60 seconds.":"JS false so no hydration");

                // Clean up resources by closing the page and browser context
                await page.CloseAsync();
                await browserContext.CloseAsync();
            }

        }
    }
