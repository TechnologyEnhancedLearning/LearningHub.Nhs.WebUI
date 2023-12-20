// <copyright file="ILearningHubHttpClient.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// The LearningHubHttpClient interface.
    /// </summary>
    public interface ILearningHubHttpClient
    {
        /// <summary>
        /// The get client async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<HttpClient> GetClientAsync();
    }
}