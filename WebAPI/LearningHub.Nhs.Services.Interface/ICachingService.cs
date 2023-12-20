// <copyright file="ICachingService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;

    /// <summary>
    /// The CachingService interface.
    /// </summary>
    public interface ICachingService
    {
        /// <summary>
        /// The get async.
        /// </summary>
        /// <typeparam name="T">Generic class type param.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<CacheReadResponse<T>> GetAsync<T>(string key);

        /// <summary>
        /// The remove async.
        /// </summary>
        /// <typeparam name="T">Generic class type param.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">Cache value.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<CacheWriteResponse> SetAsync<T>(string key, T value);

        /// <summary>
        /// The remove async.
        /// </summary>
        /// <typeparam name="T">Generic class type param.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">Cache value.</param>
        /// <param name="expiryInMinutes">Expiry in minutes.</param>
        /// <param name="slidingExpiration">Is sliding expiration, default is true.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<CacheWriteResponse> SetAsync<T>(string key, T value, int? expiryInMinutes, bool slidingExpiration = true);

        /// <summary>
        /// The remove async.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<CacheWriteResponse> RemoveAsync(string key);
    }
}