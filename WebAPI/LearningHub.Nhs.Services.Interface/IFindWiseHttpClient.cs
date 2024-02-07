// <copyright file="IFindWiseHttpClient.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// The FindWise Http Client interface.
    /// </summary>
    public interface IFindWiseHttpClient
    {
        /// <summary>
        /// The get cient async.
        /// </summary>
        /// <param name="httpClientUrl">The url of the client.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HttpClient> GetClient(string httpClientUrl);
    }
}