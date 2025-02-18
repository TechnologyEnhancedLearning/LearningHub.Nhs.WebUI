namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Storage.Files.Shares;
    using Azure.Storage.Files.Shares.Models;
    using Azure.Storage.Sas;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="FileService" />.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly Settings settings;
        private ShareClient shareClient;
        private ShareClient archiveShareClient;
        private ShareClient contentArchiveShareClient;

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

        private ShareClient OutputArchiveShareClient
        {
            get
            {
                if (this.contentArchiveShareClient == null)
                {
                    var options = new ShareClientOptions();
                    options.Retry.MaxRetries = 3;
                    options.Retry.Delay = TimeSpan.FromSeconds(10);

                    this.contentArchiveShareClient = new ShareClient(this.settings.AzureContentArchiveStorageConnectionString, this.settings.AzureFileStorageResourceShareName, options);

                    if (!this.contentArchiveShareClient.Exists())
                    {
                        throw new Exception($"Unable to access azure content archive file storage resource {this.settings.AzureFileStorageResourceShareName}");
                    }
                }

                return this.contentArchiveShareClient;
            }
        }

        private ShareClient InputArchiveShareClient
        {
            get
            {
                if (this.archiveShareClient == null)
                {
                    var options = new ShareClientOptions();
                    options.Retry.MaxRetries = 3;
                    options.Retry.Delay = TimeSpan.FromSeconds(10);

                    this.archiveShareClient = new ShareClient(this.settings.AzureSourceArchiveStorageConnectionString, this.settings.AzureFileStorageResourceShareName, options);

                    if (!this.archiveShareClient.Exists())
                    {
                        throw new Exception($"Unable to access azure file storage resource {this.settings.AzureFileStorageResourceShareName}");
                    }
                }

                return this.archiveShareClient;
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
        /// <returns>The <see cref="Task{FileDownloadResponse}"/>.</returns>
        public async Task<FileDownloadResponse> DownloadFileAsync(string filePath, string fileName)
        {
           var file = await this.FindFileAsync(filePath, fileName);
            if (file == null)
            {
                return null;
            }

            var properties = await file.GetPropertiesAsync();
            long fileSize = properties.Value.ContentLength;

            try
            {
                var response = new FileDownloadResponse
                {
                    ContentType = properties.Value.ContentType,
                    ContentLength = fileSize,
                    Content = await file.OpenReadAsync(),
                };

                if (fileSize >= 999 * 1024 * 1024)
                {
                    response.DownloadUrl = this.GenerateSasUriForFile(file);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading file: {ex.Message}");
            }
        }

        /// <summary>
        /// The StreamFileAsync.
        /// </summary>
        /// <param name="filePath">The filePath.</param>
        /// <param name="fileName">The fileName.</param>
        /// <returns>The <see cref="Task{Stream}"/>.</returns>
        public async Task<Stream> StreamFileAsync(string filePath, string fileName)
        {
            var directory = this.ShareClient.GetDirectoryClient(filePath);

            if (await directory.ExistsAsync())
            {
                var file = directory.GetFileClient(fileName);

                if (await file.ExistsAsync())
                {
                    return await file.OpenReadAsync();
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

        /// <summary>
        /// The PurgeResourceFile.
        /// </summary>
        /// <param name="vm">The vm.<see cref="ResourceVersionExtendedViewModel"/>.</param>
        /// <param name="filePaths">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task PurgeResourceFile(ResourceVersionExtendedViewModel vm = null, List<string> filePaths = null)
        {
            if (filePaths != null
                && filePaths.Any())
            {
                await this.MoveInPutDirectoryToArchive(filePaths);
                return;
            }

            if (vm != null)
            {
                var allContentPath = new List<string>();
                var allFilePath = new List<string>();
                if (vm.ScormDetails != null && !string.IsNullOrWhiteSpace(vm.ScormDetails.ContentFilePath))
                {
                    allContentPath.Add(vm.ScormDetails.ContentFilePath);
                }
                else if (vm.HtmlDetails != null && !string.IsNullOrWhiteSpace(vm.HtmlDetails.ContentFilePath))
                {
                    allContentPath.Add(vm.HtmlDetails.ContentFilePath);
                }

                // audio and video to be added
                await this.MoveInPutDirectoryToArchive(allFilePath);
                await this.MoveOutPutDirectoryToArchive(allContentPath);
            }
        }

        private static async Task WaitForCopyAsync(ShareFileClient fileClient)
        {
            // Wait for the copy operation to complete
            ShareFileProperties copyInfo;
            do
            {
                await Task.Delay(500); // Delay before checking the status again
                copyInfo = await fileClient.GetPropertiesAsync().ConfigureAwait(false);
            }
            while (copyInfo.CopyStatus == CopyStatus.Pending);

            if (copyInfo.CopyStatus != CopyStatus.Success)
            {
                throw new InvalidOperationException($"Copy failed: {copyInfo.CopyStatus}");
            }
        }

        private async Task MoveOutPutDirectoryToArchive(List<string> allDirectoryRef)
        {
            try
            {
                if (allDirectoryRef.Any())
                {
                    foreach (var directoryRef in allDirectoryRef)
                    {
                        var directory = this.ShareClient.GetDirectoryClient(directoryRef);
                        var archiveDirectory = this.OutputArchiveShareClient.GetDirectoryClient(directoryRef);

                        await foreach (var fileItem in directory.GetFilesAndDirectoriesAsync())
                        {
                            var sourceFileClient = directory.GetFileClient(fileItem.Name);

                            archiveDirectory.CreateIfNotExists();
                            if (fileItem.IsDirectory)
                            {
                                if (await archiveDirectory.GetSubdirectoryClient(fileItem.Name).ExistsAsync() == false)
                                {
                                    archiveDirectory.CreateSubdirectory(fileItem.Name);
                                }

                                await this.MigrateSubdirectory(sourceFileClient.Path);
                            }
                            else
                            {
                            var destinationFileClient = archiveDirectory.GetFileClient(fileItem.Name);
                            var uri = sourceFileClient.GenerateSasUri(Azure.Storage.Sas.ShareFileSasPermissions.Read, DateTime.UtcNow.AddHours(24));

                            await destinationFileClient.StartCopyAsync(uri);

                            await WaitForCopyAsync(destinationFileClient);
                            }
                        }

                        if (await directory.ExistsAsync())
                        {
                            foreach (var fileItem in directory.GetFilesAndDirectories())
                            {
                                var sourceFileClient = directory.GetFileClient(fileItem.Name);
                                if (fileItem.IsDirectory)
                                {
                                    await this.DeleteSubdirectory(sourceFileClient.Path);
                                }
                                else
                                {
                                    await sourceFileClient.DeleteIfExistsAsync();
                                }
                            }

                            await directory.DeleteIfExistsAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async Task MigrateSubdirectory(string pathDirectory)
        {
            var sourceDirectory = this.ShareClient.GetDirectoryClient(pathDirectory);
            var archiveDirectory = this.OutputArchiveShareClient.GetDirectoryClient(pathDirectory);

            await foreach (var fileDirectory in sourceDirectory.GetFilesAndDirectoriesAsync())
            {
                if (fileDirectory.IsDirectory)
                {
                    if (await archiveDirectory.GetSubdirectoryClient(fileDirectory.Name).ExistsAsync() == false)
                    {
                        archiveDirectory.CreateSubdirectory(fileDirectory.Name);
                    }

                    var sourceFileClient = sourceDirectory.GetFileClient(fileDirectory.Name);
                    await this.MigrateSubdirectory(sourceFileClient.Path);
                }
                else
                {
                    var sourceFileClient = sourceDirectory.GetFileClient(fileDirectory.Name);
                    var destinationFileClient = archiveDirectory.GetFileClient(fileDirectory.Name);
                    var uri = sourceFileClient.GenerateSasUri(Azure.Storage.Sas.ShareFileSasPermissions.Read, DateTime.UtcNow.AddHours(24));
                    await destinationFileClient.StartCopyAsync(uri);
                    await WaitForCopyAsync(destinationFileClient);
                }
            }
        }

        private async Task DeleteSubdirectory(string pathDirectory)
        {
            var sourceDirectory = this.ShareClient.GetDirectoryClient(pathDirectory);

            await foreach (var fileDirectory in sourceDirectory.GetFilesAndDirectoriesAsync())
            {
                if (fileDirectory.IsDirectory)
                {
                    var sourceFileClient = sourceDirectory.GetFileClient(fileDirectory.Name);
                    await this.DeleteSubdirectory(sourceFileClient.Path);
                    await sourceFileClient.DeleteIfExistsAsync();
                }
                else
                {
                    var sourceFileClient = sourceDirectory.GetFileClient(fileDirectory.Name);
                    await sourceFileClient.DeleteIfExistsAsync();
                }
            }

            await sourceDirectory.DeleteIfExistsAsync();
        }

        private async Task MoveInPutDirectoryToArchive(List<string> allDirectoryRef)
        {
            if (allDirectoryRef.Any())
            {
                foreach (var directoryRef in allDirectoryRef)
                {
                    var directory = this.ShareClient.GetDirectoryClient(directoryRef);
                    var archiveDirectory = this.InputArchiveShareClient.GetDirectoryClient(directoryRef);
                    if (!directory.Exists())
                    {
                        continue;
                    }

                    if (directory.Exists())
                    {
                        var inputCheck = directory.GetFilesAndDirectories();
                        if (inputCheck.Count() > 1)
                        {
                            await this.MoveOutPutDirectoryToArchive(new List<string> { directoryRef });
                            continue;
                        }
                    }

                    await foreach (var fileItem in directory.GetFilesAndDirectoriesAsync())
                    {
                        var sourceFileClient = directory.GetFileClient(fileItem.Name);

                        archiveDirectory.CreateIfNotExists();
                        var destinationFileClient = archiveDirectory.GetFileClient(fileItem.Name);
                        var uri = sourceFileClient.GenerateSasUri(Azure.Storage.Sas.ShareFileSasPermissions.Read, DateTime.UtcNow.AddHours(24));

                        await destinationFileClient.StartCopyAsync(uri);

                        await WaitForCopyAsync(destinationFileClient);
                    }

                    if (await directory.ExistsAsync())
                    {
                        foreach (var fileItem in directory.GetFilesAndDirectories())
                        {
                            var sourceFileClient = directory.GetFileClient(fileItem.Name);
                            await sourceFileClient.DeleteIfExistsAsync();
                        }

                        await directory.DeleteAsync();
                    }
                }
            }
        }

        private async Task<ShareFileClient> FindFileAsync(string filePath, string fileName)
        {
            var directories = new[]
            {
                this.ShareClient.GetDirectoryClient(filePath),
                this.InputArchiveShareClient.GetDirectoryClient(filePath),
            };

            foreach (var directory in directories)
            {
                if (await directory.ExistsAsync())
                {
                    var file = directory.GetFileClient(fileName);
                    if (await file.ExistsAsync())
                    {
                        return file;
                    }
                }
            }

            return null;
        }

        private string GenerateSasUriForFile(ShareFileClient fileClient)
        {
            if (fileClient.CanGenerateSasUri)
            {
                ShareSasBuilder sasBuilder = new ShareSasBuilder(ShareFileSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(20))
                {
                    Protocol = SasProtocol.Https,
                };

                Uri sasUri = fileClient.GenerateSasUri(sasBuilder);
                return sasUri.ToString();
            }
            else
            {
                throw new InvalidOperationException("Unable to generate SAS URI for the file.");
            }
        }
    }
}