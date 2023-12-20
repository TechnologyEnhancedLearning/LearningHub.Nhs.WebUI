// <copyright file="ILearningHubService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
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
