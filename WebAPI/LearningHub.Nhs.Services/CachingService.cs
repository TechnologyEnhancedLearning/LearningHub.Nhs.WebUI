// <copyright file="CachingService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The caching service.
    /// </summary>
    public class CachingService : ICachingService
    {
        private readonly ICacheService cacheService;
        private readonly Settings settings;
        private readonly ILogger<CacheService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingService"/> class.
        /// </summary>
        /// <param name="cacheService">The cache.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public CachingService(ICacheService cacheService, IOptions<Settings> settings, ILogger<CacheService> logger)
        {
            this.cacheService = cacheService;
            this.settings = settings.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Get cache.
        /// </summary>
        /// <typeparam name="T">Generic class type param.</typeparam>
        /// <param name="key">Cache key.</param>
        /// <returns>The generic task<see cref="Task{T}"/>.</returns>
        public async Task<CacheReadResponse<T>> GetAsync<T>(string key)
        {
            if (!this.settings.UseRedisCache)
            {
                return new CacheReadResponse<T>()
                {
                    ResponseEnum = CacheReadResponseEnum.NotFound,
                };
            }

            try
            {
                var value = await this.cacheService.GetAsync<T>(key);
                if (value == null)
                {
                    return new CacheReadResponse<T>()
                    {
                        ResponseEnum = CacheReadResponseEnum.NotFound,
                    };
                }

                return new CacheReadResponse<T>()
                {
                    ResponseEnum = CacheReadResponseEnum.Found,
                    Item = value,
                };
            }
            catch (CacheNotFoundException)
            {
                return new CacheReadResponse<T>()
                {
                    ResponseEnum = CacheReadResponseEnum.NotFound,
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error retrieving cache key: {key}");
                return new CacheReadResponse<T>()
                {
                    ResponseEnum = CacheReadResponseEnum.Error,
                    Exception = ex,
                };
            }
        }

        /// <summary>
        /// The remove async.
        /// </summary>
        /// <typeparam name="T">Generic class type param.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">Cache value.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<CacheWriteResponse> SetAsync<T>(string key, T value)
        {
            if (!this.settings.UseRedisCache)
            {
                return new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                };
            }

            try
            {
                await this.cacheService.SetAsync(key, value);
                return new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error setting cache key: {key}");
                return new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Error,
                    Exception = ex,
                };
            }
        }

        /// <summary>
        /// The remove async.
        /// </summary>
        /// <typeparam name="T">Generic class type param.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">Cache value.</param>
        /// <param name="expiryInMinutes">Expiry in minutes.</param>
        /// <param name="slidingExpiration">Is sliding expiration, default is true.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<CacheWriteResponse> SetAsync<T>(string key, T value, int? expiryInMinutes, bool slidingExpiration = true)
        {
            if (!this.settings.UseRedisCache)
            {
                return new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                };
            }

            try
            {
                await this.cacheService.SetAsync(key, value, expiryInMinutes, slidingExpiration);
                return new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error setting cache key: {key}");
                return new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Error,
                    Exception = ex,
                };
            }
        }

        /// <summary>
        /// The remove async.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<CacheWriteResponse> RemoveAsync(string key)
        {
            if (!this.settings.UseRedisCache)
            {
                return new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                };
            }

            try
            {
                await this.cacheService.RemoveAsync(key);
                return new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Success,
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error removing cache key: {key}");
                return new CacheWriteResponse
                {
                    ResponseEnum = CacheWriteResponseEnum.Error,
                    Exception = ex,
                };
            }
        }
    }
}
