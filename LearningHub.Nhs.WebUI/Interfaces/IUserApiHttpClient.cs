// <copyright file="IUserApiHttpClient.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// The User Api HttpClient interface.
    /// </summary>
    public interface IUserApiHttpClient
    {
        /// <summary>
        /// The get client.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HttpClient> GetClientAsync();
    }
}
