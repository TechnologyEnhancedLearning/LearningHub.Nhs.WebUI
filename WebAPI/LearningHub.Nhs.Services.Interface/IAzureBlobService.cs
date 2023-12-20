// <copyright file="IAzureBlobService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;

    /// <summary>
    /// Interface to define methods for interacting with an Azure blob storage container.
    /// </summary>
    public interface IAzureBlobService
    {
        /// <summary>
        /// Check whether an Azure blob storage container exists.
        /// </summary>
        /// <param name="blobConnectionString">The Azure storage account connection string.</param>
        /// <param name="blobContainerName">The name of the container to check exists or not.</param>
        /// <returns>Returns true if the container exists, otherwise false.</returns>
        Task<bool> BlobContainerExists(string blobConnectionString, string blobContainerName);

        /// <summary>
        /// Uploads a file to an Azure blob storage container.
        /// </summary>
        /// <param name="blobConnectionString">The Azure storage account connection string.</param>
        /// <param name="blobContainerName">The name of the container within the storage account.</param>
        /// <param name="filename">The name to give the file when it's created within the Azure blob storage container.</param>
        /// <param name="fileContents">The contents of the file.</param>
        /// <returns>Returns the full URL of the file created within Azure.</returns>
        Task<string> UploadBlobToContainer(string blobConnectionString, string blobContainerName, string filename, byte[] fileContents);

        /// <summary>
        /// Download the text contents of a blob (file) from a blob container.
        /// </summary>
        /// <param name="blobConnectionString">The blob connection string.</param>
        /// <param name="blobContainerName">The blob container name.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>The <see cref="string"/>.</returns>
        Task<string> DownloadBlobAsText(string blobConnectionString, string blobContainerName, string filename);

        /// <summary>
        /// Gets the metadata of a blob within an Azure blob container, e.g. blob size.
        /// </summary>
        /// <param name="blobConnectionString">The blobConnectionString.</param>
        /// <param name="blobContainerName">The blobContainerName.</param>
        /// <param name="blobName">The blobName.</param>
        /// <returns>Returns a BlobMetadataViewModel or null if the blob does not exist.</returns>
        Task<BlobMetadataViewModel> GetBlobMetadata(string blobConnectionString, string blobContainerName, string blobName);

        /// <summary>
        /// Copies a blob from an Azure blob container to an Azure file share.
        /// </summary>
        /// <param name="blobConnectionString">The blobConnectionString.</param>
        /// <param name="blobContainerName">The blobContainerName.</param>
        /// <param name="blobName">The blobName.</param>
        /// <param name="fileShareConnectionString">The fileShareConnectionString.</param>
        /// <param name="fileShareName">The fileShareName.</param>
        /// <param name="fileShareDirectoryName">The fileShareDirectoryName.</param>
        /// <param name="fileShareFileName">The fileShareFileName.</param>
        /// <returns>The size (in Kb) of the file being copied.</returns>
        Task<int> CopyBlobToFileShare(
            string blobConnectionString,
            string blobContainerName,
            string blobName,
            string fileShareConnectionString,
            string fileShareName,
            string fileShareDirectoryName,
            string fileShareFileName);
    }
}
