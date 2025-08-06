namespace LearningHub.Nhs.WebUI.BlazorClient.TestDeleteMe.FromShared
{
    /// <summary>
    /// Marker interface for the LearningHub API HttpClient.
    ///
    /// <para>
    /// Inherits from <see cref="IAPIHttpClient"/> to enable
    /// dependency injection of a specific implementation configured with
    /// different API endpoints or settings specific to LH API.
    /// </para>
    ///
    /// <para>
    /// Currently, this interface is empty and used solely to differentiate implementations
    /// that connect to different endpoints via configuration, but it may be extended in the future
    /// with LearningHub-specific functionality or properties.
    /// </para>
    /// </summary>
    public interface ILearningHubHttpClient : IAPIHttpClient
    {

    }
}
