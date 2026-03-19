using System.Net.Http;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Interface.HttpClients
{
    /// <summary>
    /// The Moodle Http Client interface.
    /// </summary>
    public interface IMoodleBridgeHttpClient
    {
        /// <summary>
        /// The get cient async.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HttpClient> GetClient();

        /// <summary>
        /// GETs data from Databricks API.
        /// </summary>
        /// <param name="requestUrl">The URL to make a get call to.</param>
        /// <param name="authHeader">Optional authorization header.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HttpResponseMessage> GetData(string requestUrl, string? authHeader);
    }
}
