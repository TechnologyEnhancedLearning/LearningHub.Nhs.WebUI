// <copyright file="ILearningHubApiHttpClient.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Services.Interface.HttpClients
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// The Bookmark Http Client interface.
    /// </summary>
    public interface ILearningHubApiHttpClient : IDisposable
    {
        /// <summary>
        /// GETs data from LearningHub API.
        /// </summary>
        /// <param name="requestUrl">The URL to make a get call to.</param>
        /// <param name="authHeader">Optional authorization header.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HttpResponseMessage> GetData(string requestUrl, string? authHeader);
    }
}
