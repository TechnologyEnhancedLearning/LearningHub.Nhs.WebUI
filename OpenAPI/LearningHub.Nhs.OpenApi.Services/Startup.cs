// <copyright file="Startup.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Services
{
    using LearningHub.Nhs.OpenApi.Services.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Services.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Contains methods to configure the project to be called in an API startup class.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Registers the implementations in the project with ASP.NET DI.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IFindwiseClient, FindwiseClient>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<ILearningHubService, LearningHubService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<ICatalogueService, CatalogueService>();
            services.AddScoped<IBookmarkService, BookmarkService>();
            services.AddScoped<ILearningHubApiHttpClient, LearningHubApiHttpClient>();
        }
    }
}