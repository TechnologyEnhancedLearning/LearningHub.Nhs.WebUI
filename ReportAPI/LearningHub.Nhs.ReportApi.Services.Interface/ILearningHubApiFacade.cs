// <copyright file="ILearningHubApiFacade.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;

    /// <summary>
    /// Defines the <see cref="ILearningHubApiFacade" />.
    /// </summary>
    public interface ILearningHubApiFacade
    {
        /// <summary>
        /// The GetAsync.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        Task<T> GetAsync<T>(string url)
            where T : class, new();

        /// <summary>
        /// The PostAsync.
        /// </summary>
        /// <typeparam name="TBody">.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body<see cref="V"/>.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        Task<ApiResponse> PostAsync<TBody>(string url, TBody body)
            where TBody : class, new();

        /// <summary>
        /// The PostAsync.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TBody">.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body<see cref="V"/>.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        Task<T> PostAsync<T, TBody>(string url, TBody body)
            where TBody : class, new()
            where T : class, new();

        /// <summary>
        /// The PutAsync.
        /// </summary>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> PutAsync(string url);

        /// <summary>
        /// The PutAsync.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body<see cref="T"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> PutAsync<T>(string url, T body)
            where T : class, new();

        /// <summary>
        /// The PutAsync.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TBody">.</typeparam>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="body">The body<see cref="V"/>.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        Task<T> PutAsync<T, TBody>(string url, TBody body)
            where TBody : class, new()
            where T : class, new();
    }
}
