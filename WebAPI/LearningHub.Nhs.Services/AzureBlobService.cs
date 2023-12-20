// <copyright file="AzureBlobService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Specialized;
    using Azure.Storage.Files.Shares;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Services.Interface;

    /// <summary>
    /// The azure blob service.
    /// </summary>
    public class AzureBlobService : IAzureBlobService
    {
        /// <summary>
        /// The blob container exists.
        /// </summary>
        /// <param name="blobConnectionString">The blob connection string.</param>
        /// <param name="blobContainerName">The blob container name.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public async Task<bool> BlobContainerExists(string blobConnectionString, string blobContainerName)
        {
            var client = new BlobServiceClient(blobConnectionString);

            var container = client.GetBlobContainerClient(blobContainerName);

            return await container.ExistsAsync();
        }

        /// <summary>
        /// The upload blob to container.
        /// </summary>
        /// <param name="blobConnectionString">The blob connection string.</param>
        /// <param name="blobContainerName">The blob container name.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="fileContents">The file contents.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public async Task<string> UploadBlobToContainer(string blobConnectionString, string blobContainerName, string filename, byte[] fileContents)
        {
            var blobServiceClient = new BlobServiceClient(blobConnectionString);

            var container = blobServiceClient.GetBlobContainerClient(blobContainerName);

            var client = container.GetBlockBlobClient(filename);

            await client.UploadAsync(new MemoryStream(fileContents));

            return client.Uri.ToString();
        }

        /// <summary>
        /// Download the text contents of a blob (file) from a blob container.
        /// </summary>
        /// <param name="blobConnectionString">The blob connection string.</param>
        /// <param name="blobContainerName">The blob container name.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public async Task<string> DownloadBlobAsText(string blobConnectionString, string blobContainerName, string filename)
        {
            var blobServiceClient = new BlobServiceClient(blobConnectionString);

            var container = blobServiceClient.GetBlobContainerClient(blobContainerName);

            var result = await container.GetBlockBlobClient(filename).DownloadContentAsync();

            return result.Value.Content.ToString();
        }

        /// <summary>
        /// Gets the metadata of a blob within an Azure blob container, e.g. blob size.
        /// </summary>
        /// <param name="blobConnectionString">The blobConnectionString.</param>
        /// <param name="blobContainerName">The blobContainerName.</param>
        /// <param name="blobName">The blobName.</param>
        /// <returns>Returns a BlobMetadataViewModel or null if the blob does not exist.</returns>
        public async Task<BlobMetadataViewModel> GetBlobMetadata(string blobConnectionString, string blobContainerName, string blobName)
        {
            var blobServiceClient = new BlobServiceClient(blobConnectionString);

            var container = blobServiceClient.GetBlobContainerClient(blobContainerName);
            if (!await container.ExistsAsync())
            {
                return null;
            }

            var blobClient = container.GetBlockBlobClient(blobName);
            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var props = await blobClient.GetPropertiesAsync();

            return new BlobMetadataViewModel
            {
                Name = blobName,
                SizeInBytes = props.Value.ContentLength,
            };
        }

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
        /// <returns>The size of the file being copied.</returns>
        public async Task<int> CopyBlobToFileShare(
            string blobConnectionString,
            string blobContainerName,
            string blobName,
            string fileShareConnectionString,
            string fileShareName,
            string fileShareDirectoryName,
            string fileShareFileName)
        {
            var shareClient = GetShareClient(fileShareConnectionString, fileShareName);
            var directory = shareClient.GetDirectoryClient(fileShareDirectoryName);
            await directory.CreateIfNotExistsAsync();
            var destFile = directory.GetFileClient(fileShareFileName);

            // Generate blob sas url
            var blobServiceClient = new BlobServiceClient(blobConnectionString);
            var container = blobServiceClient.GetBlobContainerClient(blobContainerName);
            var blob = container.GetBlockBlobClient(blobName);
            var sasUri = blob.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(2));

            // Start the copy operation from blob to file.
            await destFile.StartCopyAsync(sasUri);

            var destProperties = await destFile.GetPropertiesAsync();

            // Copy may not be finished at this point, check on the status of the copy.
            while (destProperties.Value.CopyStatus == Azure.Storage.Files.Shares.Models.CopyStatus.Pending)
            {
                await Task.Delay(500);
                destProperties = await destFile.GetPropertiesAsync();
            }

            if (destProperties.Value.CopyStatus != Azure.Storage.Files.Shares.Models.CopyStatus.Success)
            {
                throw new InvalidOperationException($"Copy failed: {destProperties.Value.CopyStatus}");
            }

            var fileSizeKb = Math.Ceiling((double)destProperties.Value.ContentLength / 1000);
            return Convert.ToInt32(fileSizeKb);
        }

        private static ShareClient GetShareClient(string azureFileStorageConnectionString, string azureFileStorageResourceShareName)
        {
            var options = new ShareClientOptions();
            options.Retry.MaxRetries = 3;
            options.Retry.Delay = TimeSpan.FromSeconds(10);

            var shareClient = new ShareClient(azureFileStorageConnectionString, azureFileStorageResourceShareName, options);

            if (!shareClient.Exists())
            {
                throw new Exception($"Unable to access azure file storage resource {azureFileStorageResourceShareName}");
            }

            return shareClient;
        }
    }
}