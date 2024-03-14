namespace LearningHub.Nhs.ReportApi.Services
{
    using System.IO;
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using LearningHub.Nhs.ReportApi.Services.Interface;
    using LearningHub.Nhs.ReportApi.Shared.Configuration;
    using LearningHub.Nhs.ReportApi.Shared.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The AzureBlobStorageService.
    /// </summary>
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly ILogger<AzureBlobStorageService> logger;
        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorageService"/> class.
        /// </summary>
        /// <param name="logger">The logger<see cref="ILogger{AzureBlobStorageService}"/>.</param>
        /// <param name="settings">The settings.</param>
        public AzureBlobStorageService(
            ILogger<AzureBlobStorageService> logger,
            IOptions<Settings> settings)
        {
            this.logger = logger;
            this.settings = settings.Value;
        }

        /// <summary>
        /// The download file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<BlobModel?> GetFile(string fileName)
        {
            var azureBlobStorageSettings = this.settings.AzureBlobStorageSettings;
            var blobServiceClient = new BlobServiceClient(azureBlobStorageSettings.ConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(azureBlobStorageSettings.Container);

            try
            {
                BlobClient file = blobContainerClient.GetBlobClient(fileName);

                // Check if the file exists in the container
                if (await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;

                    // Download the file details async
                    var content = await file.DownloadContentAsync();

                    // Create new BlobDto with blob data from variables
                    return new BlobModel { Content = blobContent, Name = fileName, ContentType = content.Value.Details.ContentType };
                }
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                // Log error
                this.logger.LogError($"File {fileName} was not found.");
            }

            // File does not exist, return null and handle that in requesting method
            return null;
        }
    }
}