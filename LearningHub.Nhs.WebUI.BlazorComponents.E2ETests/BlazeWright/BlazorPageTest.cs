using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright.Xunit;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.BlazeWright
{
    public class BlazorPageTest<TProgram> : PageTest, IAsyncLifetime
         where TProgram : class
    {
        protected string BaseUrl => Host.ServerAddress;
        private BlazorApplicationFactory<TProgram>? host;

        protected BlazorApplicationFactory<TProgram> Host
        {
            get
            {
                host ??= CreateHostFactory() ?? new BlazorApplicationFactory<TProgram>(ConfigureWebHost);
                return host;
            }
        }

        protected virtual BlazorApplicationFactory<TProgram> CreateHostFactory()
            => new BlazorApplicationFactory<TProgram>(ConfigureWebHost);

        protected virtual void ConfigureWebHost(IWebHostBuilder builder) { }

    }
}
