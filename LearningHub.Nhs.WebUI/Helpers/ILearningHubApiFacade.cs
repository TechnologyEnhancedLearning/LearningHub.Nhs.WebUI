// <copyright file="ILearningHubApiFacade.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Helpers
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
        /// <param name="url">The url.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        Task<T> GetAsync<T>(string url)
            where T : class, new();

        /// <summary>
        /// The PostAsync.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="url">The url.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task PostAsync<T>(string url, T body)
            where T : class, new();

        /// <summary>
        /// The PostAsync.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TBody">.</typeparam>
        /// <param name="url">The url.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task{T}"/>.</returns>
        Task<ApiResponse> PostAsync<T, TBody>(string url, TBody body)
            where T : class, new()
            where TBody : class, new();

        /// <summary>
        /// The PutAsync.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> PutAsync(string url);

        /// <summary>
        /// The PutAsync.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="url">The url.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> PutAsync<T>(string url, T body)
            where T : class, new();
    }
}
