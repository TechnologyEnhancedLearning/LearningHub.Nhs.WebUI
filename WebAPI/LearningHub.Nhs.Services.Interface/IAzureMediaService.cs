// <copyright file="IAzureMediaService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to define methods for interacting with Azure Media Services.
    /// </summary>
    public interface IAzureMediaService
    {
        /// <summary>
        /// Create AzureMedia InputAsset from file upload.
        /// </summary>
        /// <param name="fileName">The .</param>
        /// <param name="sourceBlobConntectionString">The sourceBlobConntectionString.</param>
        /// <param name="sourceBlobContainerName">The sourceBlobContainerName.</param>
        /// <param name="sourceBlobName">The sourceBlobName.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<BlobCopyResult> CreateMediaInputAssetFromBlob(
            string fileName,
            string sourceBlobConntectionString,
            string sourceBlobContainerName,
            string sourceBlobName);
    }

    /// <summary>
    /// BlobCopyResult.
    /// </summary>
    public class BlobCopyResult
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the FileSizeKb.
        /// </summary>
        public int FileSizeKb { get; set; }
    }
}
