// <copyright file="ILearningHubService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    /// <summary>
    /// <see cref="ILearningHubService"/>.
    /// </summary>
    public interface ILearningHubService
    {
        /// <summary>
        /// <see cref="GetResourceLaunchUrl"/>.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The URL for launching a resource.</returns>
        string GetResourceLaunchUrl(int resourceReferenceId);
    }
}
