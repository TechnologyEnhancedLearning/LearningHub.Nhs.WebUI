namespace LearningHub.Nhs.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Azure.Management.Media;
    using Microsoft.Azure.Management.Media.Models;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure.Authentication;

    /// <summary>
    /// The azure media service.
    /// </summary>
    public class AzureMediaService : IAzureMediaService, IDisposable
    {
        private Settings settings;
        private IAzureMediaServicesClient azureMediaServicesClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureMediaService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public AzureMediaService(IOptions<Settings> settings)
        {
            this.settings = settings.Value;
        }

        /// <summary>
        /// Create Azure Media Input Asset from Azure Blob. Used by the migration tool to create a draft video/audio resource from a blob located in the
        /// migration's Azure blob container in the migration Azure storage account. Process is similar to that in the WebUI AzureMediaService class,
        /// but it is not a local file being processed.
        /// </summary>
        /// <param name="fileName">The destination file name.</param>
        /// <param name="sourceBlobConntectionString">The sourceBlobConntectionString.</param>
        /// <param name="sourceBlobContainerName">The sourceBlobContainerName.</param>
        /// <param name="sourceBlobName">The sourceBlobName.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<BlobCopyResult> CreateMediaInputAssetFromBlob(
            string fileName,
            string sourceBlobConntectionString,
            string sourceBlobContainerName,
            string sourceBlobName)
        {
            string uniqueness = Guid.NewGuid().ToString().Substring(0, 13);
            string inputAssetName = $"input-{uniqueness}";

            IAzureMediaServicesClient client = await this.CreateMediaServicesClientAsync();

            Asset asset = await client.Assets.CreateOrUpdateAsync(this.settings.AzureMediaResourceGroup, this.settings.AzureMediaAccountName, inputAssetName, new Asset());

            ListContainerSasInput input = new ListContainerSasInput()
            {
                Permissions = AssetContainerPermission.ReadWrite,
                ExpiryTime = DateTime.Now.AddHours(2).ToUniversalTime(),
            };

            var response = client.Assets.ListContainerSasAsync(this.settings.AzureMediaResourceGroup, this.settings.AzureMediaAccountName, inputAssetName, input.Permissions, input.ExpiryTime).Result;
            string uploadSasUrl = response.AssetContainerSasUrls.First();
            var sasUri = new Uri(uploadSasUrl);

            var blobServiceClient = new BlobServiceClient(sourceBlobConntectionString);
            var container = blobServiceClient.GetBlobContainerClient(sourceBlobContainerName);
            var blob = container.GetBlockBlobClient(sourceBlobName);
            var sourceUri = blob.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(2));

            // Create reference to destination blob container for AMS asset.
            var destContainer = new BlobContainerClient(sasUri);

            var destBlob = destContainer.GetBlockBlobClient(fileName);

            // Start the copy operation from source to destination.
            await destBlob.StartCopyFromUriAsync(sourceUri);

            var destProperties = await destBlob.GetPropertiesAsync();

            // Copy may not be finished at this point, check on the status of the copy.
            while (destProperties.Value.CopyStatus == CopyStatus.Pending)
            {
                await Task.Delay(500);
                destProperties = await destBlob.GetPropertiesAsync();
            }

            if (destProperties.Value.CopyStatus != CopyStatus.Success)
            {
                throw new InvalidOperationException($"Copy failed: {destProperties.Value.CopyStatus}");
            }

            var fileSizeKb = Math.Ceiling((double)destProperties.Value.ContentLength / 1000);

            return new BlobCopyResult
            {
                Name = asset.Name,
                FileSizeKb = Convert.ToInt32(fileSizeKb),
            };
        }

        /// <summary>
        /// Creates the AzureMediaServicesClient object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync()
        {
            if (this.azureMediaServicesClient != null)
            {
                return this.azureMediaServicesClient;
            }

            var credentials = await this.GetCredentialsAsync();

            this.azureMediaServicesClient = new AzureMediaServicesClient(this.settings.AzureMediaArmEndpoint, credentials)
            {
                SubscriptionId = this.settings.AzureMediaSubscriptionId,
            };

            return this.azureMediaServicesClient;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispoase.
        /// </summary>
        /// <param name="disposing">disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.azureMediaServicesClient.Dispose();
            }
        }

        /// <summary>
        /// Get AzureMedia Credentials.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<ServiceClientCredentials> GetCredentialsAsync()
        {
            ClientCredential clientCredential = new ClientCredential(this.settings.AzureMediaAadClientId, this.settings.AzureMediaAadSecret);
            return await ApplicationTokenProvider.LoginSilentAsync(this.settings.AzureMediaAadTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure);
        }
    }
}
