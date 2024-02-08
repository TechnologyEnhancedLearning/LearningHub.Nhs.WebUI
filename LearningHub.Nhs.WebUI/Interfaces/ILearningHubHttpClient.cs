namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// The LearningHubHttpClient interface.
    /// </summary>
    public interface ILearningHubHttpClient
    {
        /// <summary>
        /// The get client.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HttpClient> GetClientAsync();
    }
}
