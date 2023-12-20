// <copyright file="FileService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.Storage.Files.Shares;
    using Azure.Storage.Files.Shares.Models;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="FileService" />.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly Settings settings;
        private ShareClient shareClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        /// <param name="settings">Settings.</param>
        public FileService(IOptions<Settings> settings)
        {
            this.settings = settings.Value;
        }

        private ShareClient ShareClient
        {
            get
            {
                if (this.shareClient == null)
                {
                    var options = new ShareClientOptions();
                    options.Retry.MaxRetries = 3;
                    options.Retry.Delay = TimeSpan.FromSeconds(10);

                    this.shareClient = new ShareClient(this.settings.AzureFileStorageConnectionString, this.settings.AzureFileStorageResourceShareName, options);

                    if (!this.shareClient.Exists())
                    {
                        throw new Exception($"Unable to access azure file storage resource {this.settings.AzureFileStorageResourceShareName}");
                    }
                }

                return this.shareClient;
            }
        }

        /// <summary>
        /// The DeleteChunkDirectory.
        /// </summary>
        /// <param name="directoryRef">The directoryRef.</param>
        /// <param name="chunks">The chunks<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteChunkDirectory(string directoryRef, int chunks)
        {
            var directory = this.ShareClient.GetDirectoryClient(directoryRef);

            if (await directory.ExistsAsync())
            {
                for (int i = 0; i < chunks; i++)
                {
                    var file = directory.GetFileClient("Chunk_" + i.ToString());
                    await file.DeleteIfExistsAsync();
                }

                await directory.DeleteAsync();
            }
        }

        /// <summary>
        /// The DownloadFileAsync.
        /// </summary>
        /// <param name="filePath">The filePath.</param>
        /// <param name="fileName">The fileName.</param>
        /// <returns>The <see cref="Task{CloudFile}"/>.</returns>
        public async Task<ShareFileDownloadInfo> DownloadFileAsync(string filePath, string fileName)
        {
            var directory = this.ShareClient.GetDirectoryClient(filePath);

            if (await directory.ExistsAsync())
            {
                var file = directory.GetFileClient(fileName);

                if (await file.ExistsAsync())
                {
                    return await file.DownloadAsync();
                }
            }

            return null;
        }

        /// <summary>
        /// The ProcessFile.
        /// </summary>
        /// <param name="fileBytes">The fileBytes<see cref="Stream"/>.</param>
        /// <param name="fileName">The fileName.</param>
        /// <param name="directoryRef">The directoryRef.</param>
        /// <returns>The .</returns>
        public async Task<string> ProcessFile(Stream fileBytes, string fileName, string directoryRef = "")
        {
            if (directoryRef == string.Empty)
            {
                directoryRef = Guid.NewGuid().ToString();
            }

            var directory = this.ShareClient.GetDirectoryClient(directoryRef);

            await directory.CreateIfNotExistsAsync();

            var fileClient = directory.GetFileClient(fileName);

            if (!new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            await fileClient.CreateAsync(fileBytes.Length, httpHeaders: new ShareFileHttpHeaders { ContentType = contentType });
            await fileClient.UploadAsync(fileBytes);

            return directoryRef;
        }
    }
}