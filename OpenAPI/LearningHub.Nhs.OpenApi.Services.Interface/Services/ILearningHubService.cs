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

        /// <summary>
        /// <see cref="GetExternalResourceLaunchUrl"/>.
        /// </summary>
        /// <param name="externalReference">The resource reference id.</param>
        /// <returns>The URL for launching an external resource.</returns>
        string GetExternalResourceLaunchUrl(string externalReference);
    }
}
