using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using LearningHub.Nhs.Shared.Configuration;


namespace LearningHub.Nhs.WebUI.BlazorClient.DI
{
    public static class DI
    {
        public static IHttpClientBuilder AddBffHttpClient<TInterface, TImplementation>(this IServiceCollection services, Func<ExposableSettings, string> getApiUrl)
        where TInterface : class
        where TImplementation : class, TInterface
        {
            return services.AddHttpClient<TInterface, TImplementation>((serviceProvider, client) =>
            {
                var ExposableSettings = serviceProvider.GetRequiredService<IOptions<ExposableSettings>>().Value;
                var apiUrl = getApiUrl(ExposableSettings);
                var apiUri = new Uri(apiUrl);
                var apiHost = apiUri.Host;
                string forwardSlash = "/";
                // Using the Uri class for robust path joining
                client.BaseAddress = new Uri($"{ExposableSettings.LearningHubApiBFFUrl}{apiHost}{forwardSlash}");
            });
        }
    }
}
