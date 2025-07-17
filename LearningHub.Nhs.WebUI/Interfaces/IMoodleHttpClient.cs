namespace LearningHub.Nhs.Services.Interface
{
    using LearningHub.Nhs.WebUI.Shared.Interfaces;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// The Moodle Http Client interface.
    /// </summary>
    public interface IMoodleHttpClient
    {
        /// <summary>
        /// The get cient async.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HttpClient> GetClient();

        /// <summary>
        /// GetDefaultParameters.
        /// </summary>
        /// <returns>defaultParameters.</returns>
        string GetDefaultParameters();
    }
}