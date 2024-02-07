// <copyright file="IUserApiHttpClient.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
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
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<HttpClient> GetClientAsync();
    }
}
