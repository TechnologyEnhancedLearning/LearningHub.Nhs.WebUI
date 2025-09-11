namespace LearningHub.Nhs.WebUI.Startup
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.WebUtilitiesInterfaces;
    using LearningHub.Nhs.WebUI.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    /// <summary>
    /// Middleware class to capture user Timezone info.
    /// </summary>
    public class TimezoneInfoMiddleware
    {
        /// <summary>
        /// TimezoneInfo Url.
        /// </summary>
        public const string TimezoneInfoUrl = "/api/timezoneinfo";

        /// <summary>
        /// Initializes a new instance of the <see cref="TimezoneInfoMiddleware"/> class.
        /// </summary>
        /// <param name="next"><see cref="RequestDelegate"/>.</param>
        public TimezoneInfoMiddleware(RequestDelegate next)
        {
        }

        /// <summary>
        /// Invoke TimezoneInfoMiddleware.
        /// </summary>
        /// <param name="context"><see cref="RequestDelegate"/>.</param>
        /// <param name="cacheService"><see cref="ICacheService"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task InvokeAsync(HttpContext context, ICacheService cacheService)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                if (context.Request.Query.TryGetValue("tz", out StringValues queryVal))
                {
                    if (int.TryParse(queryVal, out int tzOffset))
                    {
                        await cacheService.SetAsync(context.User.GetTimezoneOffsetCacheKey(), tzOffset * -1, 30, true);
                    }
                }
            }

            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync("ok");
        }
    }
}