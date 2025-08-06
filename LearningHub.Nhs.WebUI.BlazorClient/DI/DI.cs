using LearningHub.Nhs.WebUI.BlazorClient.TestDeleteMe.FromShared;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;


namespace LearningHub.Nhs.WebUI.BlazorClient.DI
{
    public static class DI
    {
        public static IHttpClientBuilder AddBffHttpClient<TInterface, TImplementation>(this IServiceCollection services, Func<PublicSettings, string> getApiUrl)
        where TInterface : class
        where TImplementation : class, TInterface
        {
            return services.AddHttpClient<TInterface, TImplementation>((serviceProvider, client) =>
            {
                var publicSettings = serviceProvider.GetRequiredService<IOptions<PublicSettings>>().Value;
                var apiUrl = getApiUrl(publicSettings);
                var apiUri = new Uri(apiUrl);
                var apiHost = apiUri.Host;
                string forwardSlash = "/";
                // Using the Uri class for robust path joining
                client.BaseAddress = new Uri($"{publicSettings.LearningHubApiBFFUrl}{apiHost}{forwardSlash}");
            });
        }
    }
}
