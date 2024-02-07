// <copyright file="IAzureMediaService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Threading.Tasks;
    using Azure.Storage.Blobs.Models;
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
        /// <param name="manifestProxyUrl">The manifestProxyUrl<see cref="string"/>.</param>
        /// <param name="topLeveLManifestUrl">The topLeveLManifestUrl<see cref="string"/>.</param>
        /// <param name="token">The token<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        string GetTopLevelManifestForToken(string manifestProxyUrl, string topLeveLManifestUrl, string token);

        /// <summary>
        /// Download the input media file from Azure Media Services.
        /// </summary>
        /// <param name="inputAssetName">The name of the input asset.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>.</returns>
        Task<BlobDownloadResult> DownloadMediaInputAsset(string inputAssetName, string fileName);
    }
}
