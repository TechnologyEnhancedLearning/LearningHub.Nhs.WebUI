using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.Helpers
{
    public static class HydrationDetectionHelper
    {
        /// <summary>
        /// Waits until the Blazor WebAssembly runtime logs its hydration message to the browser console.
        /// </summary>
        /// <param name="page">The Playwright page instance.</param>
        /// <param name="timeout">Maximum wait time in milliseconds (default 10000ms).</param>
        /// <returns>A Task that completes when the hydration console message is observed.</returns>
        public static async Task<bool> ConsoleReportedHydrationWithinTimeAsync(this IPage page, int timeout = 30_000)
        {
            var tcs = new TaskCompletionSource<bool>();
            using var cts = new CancellationTokenSource(timeout);

            // qqqq this is from our logger we inject it in client services
            const string hydrationMessage = "Development LearningHub Nhs WebUI Blazor Client";
            // qqqq chrome one doesnt have console so need hydration check element but then it would be in production

            void ConsoleHandler(object? sender, IConsoleMessage message)
            {
            
                if (message.Text.Contains(hydrationMessage))
                {
                    tcs.TrySetResult(true);
                }
            }

            try
            {
                page.Console += ConsoleHandler;

                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeout, cts.Token));
                return completedTask == tcs.Task && tcs.Task.Result;
            }
            catch
            {
                return false;
            }
            finally
            {
                page.Console -= ConsoleHandler;
            }
        }
    }
}