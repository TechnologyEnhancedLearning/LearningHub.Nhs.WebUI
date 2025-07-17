namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Specialized;
    using Azure.Storage.Files.Shares;
    using Azure.Storage.Files.Shares.Models;
    using LearningHub.Nhs.Models.Resource.Files;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using tusdotnet.Models;
    using tusdotnet.Models.Configuration;
    using LearningHub.Nhs.WebUI.Shared.Interfaces;
    using LearningHub.Nhs.WebUI.Shared.Services;
    using LearningHub.Nhs.WebUI.Shared.Models;
    /// <summary>
    /// Defines the <see cref="PartialFileUploadService" />.
    /// </summary>
    public class PartialFileUploadService : BaseService<PartialFileUploadService>, IPartialFileUploadService
    {
        private readonly Settings settings;
        private ShareClient shareClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialFileUploadService"/> class.
        /// </summary>
        /// <param name="settings">The Settings.</param>
        /// <param name="learningHubHttpClient">The learning hub http client.</param>
        /// <param name="logger">The logger.</param>
        public PartialFileUploadService(
            IOptions<Settings> settings,
            ILearningHubHttpClient learningHubHttpClient,
            ILogger<PartialFileUploadService> logger)
            : base(learningHubHttpClient, logger)
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
        /// Authorize the request.
        /// </summary>
        /// <param name="authoriseContext">The AuthorizeContext.</param>
        /// <returns>A Task.</returns>
        public async Task OnAuthoriseAsync(AuthorizeContext authoriseContext)
        {
            if (authoriseContext.HttpContext.User.IsInRole("ReadOnly") || authoriseContext.HttpContext.User.IsInRole("BasicUser"))
            {
                authoriseContext.FailRequest(HttpStatusCode.Forbidden);
            }

            if (authoriseContext.Intent != IntentType.CreateFile)
            {
                int fileId = int.Parse(authoriseContext.FileId);

                await this.GetPartialFile(fileId);
            }
        }

        /// <summary>
        /// Create a file upload reference that can later be used to upload data.
        /// </summary>
        /// <param name="uploadLength">The length of the upload in bytes.</param>
        /// <param name="metadata">The Upload-Metadata request header or null if no header was provided.</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling.</param>
        /// <returns>The reference to the file.</returns>
        public async Task<string> CreateFileAsync(long uploadLength, string metadata, CancellationToken cancellationToken)
        {
            string fileName = null;
            PartialFilePostProcessingOptions postProcessing = PartialFilePostProcessingOptions.None;

            string[] metadataParts = metadata.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (string metadataPart in metadataParts)
            {
                string[] keyAndValue = metadataPart.Split(" ", StringSplitOptions.None);
                string key = keyAndValue[0];
                string value = keyAndValue[1];

                if (key == "filename")
                {
                    string filenameBase64 = value;
                    fileName = Encoding.UTF8.GetString(Convert.FromBase64String(filenameBase64));
                }

                if (key == "postProcessing")
                {
                    string postProcessingStringBase64 = value;
                    string postProcessingString = Encoding.UTF8.GetString(Convert.FromBase64String(postProcessingStringBase64));
                    postProcessing = Enum.Parse<PartialFilePostProcessingOptions>(postProcessingString);
                }
            }

            if (fileName == null)
            {
                throw new Exception("Filename must be set when creating a file");
            }

            var requestViewModel = new PartialFileViewModel
            {
                FileName = fileName,
                TotalFileSize = uploadLength,
                PostProcessingOptions = postProcessing,
            };
            PartialFileViewModel responseViewModel = await this.CreatePartialFile(requestViewModel);

            return responseViewModel.FileId.ToString();
        }

        /// <summary>
        /// Creates the partial file.
        /// </summary>
        /// <param name="requestViewModel">The PartialFileViewModel.</param>
        /// <returns>A Task.</returns>
        public async Task<PartialFileViewModel> CreatePartialFile(PartialFileViewModel requestViewModel)
        {
            var json = JsonConvert.SerializeObject(requestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"partial-file";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                PartialFileViewModel responseViewModel = JsonConvert.DeserializeObject<PartialFileViewModel>(result);

                var blob = this.GetCloudAppendBlob(responseViewModel.FileId);

                await blob.CreateIfNotExistsAsync();

                return responseViewModel;
            }

            throw this.CreateExceptionForUnsuccessfulResponse(response, "Error creating partial file");
        }

        /// <summary>
        /// Check if a file exist.
        /// </summary>
        /// <param name="fileId">The id of the file to check.</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling.</param>
        /// <returns>True if the file exists.</returns>
        public async Task<bool> FileExistAsync(string fileId, CancellationToken cancellationToken)
        {
            int fileIdInt = int.Parse(fileId);
            var blob = this.GetCloudAppendBlob(fileIdInt);

            return await blob.ExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Returns the upload length specified when the file was created or null if Defer-Upload-Length was used.
        /// </summary>
        /// <param name="fileId">The id of the file to check.</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling.</param>
        /// <returns>The upload length of the file.</returns>
        public async Task<long?> GetUploadLengthAsync(string fileId, CancellationToken cancellationToken)
        {
            PartialFileViewModel partialFileViewModel = await this.GetPartialFile(int.Parse(fileId));
            return partialFileViewModel.TotalFileSize;
        }

        /// <summary>
        /// Returns the current size of the file a.k.a. the upload offset.
        /// </summary>
        /// <param name="fileId">The id of the file to check.</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling.</param>
        /// <returns>The size of the current file.</returns>
        public async Task<long> GetUploadOffsetAsync(string fileId, CancellationToken cancellationToken)
        {
            int fileIdInt = int.Parse(fileId);
            var blob = this.GetCloudAppendBlob(fileIdInt);

            var blobProps = await blob.GetPropertiesAsync(cancellationToken: cancellationToken);
            return blobProps.Value.ContentLength;
        }

        /// <summary>
        /// Write data to the file using the provided stream.
        /// The implementation must throw exception cref="TusStoreException" />
        /// if the streams length exceeds the upload length of the file.
        /// </summary>
        /// <param name="fileId">The id of the file to write.</param>
        /// <param name="stream">The request input stream from the client.</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling.</param>
        /// <returns>The number of bytes written.</returns>
        public async Task<long> AppendDataAsync(string fileId, Stream stream, CancellationToken cancellationToken)
        {
            int fileIdInt = int.Parse(fileId);
            var blob = this.GetCloudAppendBlob(fileIdInt);

            using var cloudBlobStream = await blob.OpenWriteAsync(false, cancellationToken: cancellationToken);

            return await PipeReadStreamToWriteStream(stream, cloudBlobStream, cancellationToken);
        }

        /// <summary>
        /// Get the Upload-Metadata header as it was provided to: <code>CreateFileAsync</code>.
        /// </summary>
        /// <param name="fileId">The id of the file to get the header for.</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling.</param>
        /// <returns>The Upload-Metadata header.</returns>
        public async Task<string> GetUploadMetadataAsync(string fileId, CancellationToken cancellationToken)
        {
            PartialFileViewModel partialFileViewModel = await this.GetPartialFile(int.Parse(fileId));

            string fileName = partialFileViewModel.FileName;
            string base64Filename = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileName));
            string metadata = $"filename {base64Filename}";

            return metadata;
        }

        /// <summary>
        /// Process the file once upload is complete.
        /// </summary>
        /// <param name="fileCompleteContext">The FileCompleteContext.</param>
        /// <returns>A Task.</returns>
        public async Task OnFileCompleteAsync(FileCompleteContext fileCompleteContext)
        {
            string fileId = fileCompleteContext.FileId;
            int fileIdInt = int.Parse(fileId);

            PartialFileViewModel partialFileViewModel = await this.GetPartialFile(fileIdInt);

            bool fileMovedSuccessfully = await this.MoveFileFromBlobStorageToFileStorage(partialFileViewModel);

            if (fileMovedSuccessfully)
            {
                await this.CompletePartialFile(fileIdInt);
            }
        }

        private static async Task<long> PipeReadStreamToWriteStream(
            Stream readStream,
            Stream writeStream,
            CancellationToken cancellationToken)
        {
            const int bufferSize = 10 * 1000 * 1000; // transfer up to 10MB at a time from one stream to the other
            byte[] buffer = new byte[bufferSize];
            bool stillReading = true;
            long totalBytesRead = 0;

            while (stillReading && !cancellationToken.IsCancellationRequested)
            {
                // This Stream is really a ClientDisconnectGuardedReadOnlyStream https://github.com/tusdotnet/tusdotnet/blob/master/Source/tusdotnet/Models/ClientDisconnectGuardedReadOnlyStream.cs
                // which extends a ReadOnlyStream https://github.com/tusdotnet/tusdotnet/blob/master/Source/tusdotnet/Models/ReadOnlyStream.cs
                // Most methods throw NotSupportedException
                // ReadAsync(byte[], int, int, CancellationToken) is the only method which works!
                int bytesRead = await readStream.ReadAsync(buffer, 0, bufferSize, cancellationToken);
                await writeStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                stillReading = bytesRead > 0;
                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }

        private async Task<bool> MoveFileFromBlobStorageToFileStorage(PartialFileViewModel partialFileViewModel)
        {
            var blob = this.GetCloudAppendBlob(partialFileViewModel.FileId);
            ShareFileClient file = null;

            using (var blobReadStream = await blob.OpenReadAsync())
            {
                file = await this.CreateCloudFile(partialFileViewModel.FileName, partialFileViewModel.FilePath, blobReadStream.Length);

                await file.UploadAsync(blobReadStream);
            }

            var blobProps = await blob.GetPropertiesAsync();
            var fileProps = await file.GetPropertiesAsync();

            bool fileMovedSuccessfully = fileProps.Value.ContentLength == blobProps.Value.ContentLength;
            if (fileMovedSuccessfully)
            {
                await blob.DeleteAsync();
            }

            return fileMovedSuccessfully;
        }

        private async Task<PartialFileViewModel> GetPartialFile(int fileId)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"partial-file/{fileId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                PartialFileViewModel responseViewModel = JsonConvert.DeserializeObject<PartialFileViewModel>(result);
                return responseViewModel;
            }

            throw this.CreateExceptionForUnsuccessfulResponse(response, "Error loading partial file");
        }

        private async Task CompletePartialFile(int fileId)
        {
            var stringContent = new StringContent(string.Empty, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"partial-file/{fileId}/complete";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw this.CreateExceptionForUnsuccessfulResponse(response, "Error completing partial file upload");
            }
        }

        private Exception CreateExceptionForUnsuccessfulResponse(HttpResponseMessage response, string errorMessage = "Error")
        {
            var result = response.Content.ReadAsStringAsync().Result;
            var message = response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden
                ? "Access Denied"
                : $"{errorMessage} - {result}";

            return new HttpRequestException(message, null, response.StatusCode);
        }

        private AppendBlobClient GetCloudAppendBlob(int fileId)
        {
            var blobServiceClient = new BlobServiceClient(this.settings.AzureBlobSettings.ConnectionString);

            var container = blobServiceClient.GetBlobContainerClient(this.settings.AzureBlobSettings.UploadContainer);

            var appendBlobClient = container.GetAppendBlobClient($"partial-files/{fileId}");

            return appendBlobClient;
        }

        private async Task<ShareFileClient> CreateCloudFile(string fileName, string directoryName, long length)
        {
            var directory = this.ShareClient.GetDirectoryClient(directoryName);

            await directory.CreateIfNotExistsAsync();

            var fileClient = directory.GetFileClient(fileName);

            if (!new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            await fileClient.CreateAsync(length, httpHeaders: new ShareFileHttpHeaders { ContentType = contentType });

            return fileClient;
        }
    }
}