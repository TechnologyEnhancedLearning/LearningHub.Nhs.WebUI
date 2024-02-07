// <copyright file="IAzureMediaService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Defines the <see cref="IAzureMediaService" />.
    /// </summary>
    public interface IAzureMediaService
    {
        /// <summary>
        /// The CreateMediaInputAsset.
        /// </summary>
        /// <param name="file">File.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> CreateMediaInputAsset(IFormFile file);

        /// <summary>
        /// The GetContentAuthenticationTokenAsync.
        /// </summary>
        /// <param name="encodedAssetId">Encoded asset id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetContentAuthenticationTokenAsync(string encodedAssetId);

        /// <summary>
        /// The GetTopLevelManifestForToken.
        /// </summary>
        /// <param name="manifestProxyUrl">The manifestProxyUrl.</param>
        /// <param name="topLeveLManifestUrl">The topLeveLManifestUrl.</param>
        /// <param name="token">The token.</param>
        /// <returns>The .</returns>
        string GetTopLevelManifestForToken(string manifestProxyUrl, string topLeveLManifestUrl, string token);
    }
}
