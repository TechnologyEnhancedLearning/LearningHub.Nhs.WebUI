namespace LearningHub.Nhs.OpenApi.Services.Interface.HttpClients
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// The Bookmark Http Client interface.
    /// </summary>
    public interface IDatabricksApiHttpClient : IDisposable
    {
        /// <summary>
        /// GETs data from Databricks API.
        /// </summary>
        /// <param name="requestUrl">The URL to make a get call to.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HttpResponseMessage> GetData(string requestUrl);

        /// <summary>
        /// The Get Client method.
        /// </summary>
        /// <returns>The <see cref="HttpClient"/>.</returns>
        Task<HttpClient> GetClient();
    }
}
