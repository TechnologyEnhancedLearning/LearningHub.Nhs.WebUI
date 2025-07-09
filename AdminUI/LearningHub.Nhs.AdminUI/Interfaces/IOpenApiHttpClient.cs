namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// The OpenApiHttpClient interface.
    /// </summary>
    public interface IOpenApiHttpClient
    {
        /// <summary>
        /// The get client.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HttpClient> GetClientAsync();
    }
}
