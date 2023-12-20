// <copyright file="FileService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Services
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using Azure.Storage.Files.Shares;
  using Azure.Storage.Files.Shares.Models;
  using LearningHub.Nhs.AdminUI.Configuration;
  using LearningHub.Nhs.AdminUI.Interfaces;
  using Microsoft.AspNetCore.StaticFiles;
  using Microsoft.Extensions.Options;

  /// <summary>
  /// Defines the <see cref="FileService" />.
  /// </summary>
  public class FileService : IFileService
  {
    private readonly WebSettings settings;
    private ShareClient shareClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    /// <param name="settings">Settings.</param>
    public FileService(IOptions<WebSettings> settings)
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
    /// <param name="directoryRef">The directoryRef<see cref="string"/>.</param>
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
    /// <param name="filePath">The filePath<see cref="string"/>.</param>
    /// <param name="fileName">The fileName<see cref="string"/>.</param>
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

    /// <inheritdoc/>
    public async Task<string> ProcessFile(Stream fileBytes, string fileName, string directoryRef = "", string originalFileName = null)
    {
      if (directoryRef == string.Empty)
      {
        directoryRef = Guid.NewGuid().ToString();
      }

      var directory = this.ShareClient.GetDirectoryClient(directoryRef);

      await directory.CreateIfNotExistsAsync();

      var fileClient = directory.GetFileClient(fileName);

      if (!new FileExtensionContentTypeProvider().TryGetContentType(originalFileName ?? fileName, out string contentType))
      {
        contentType = "application/octet-stream";
      }

      await fileClient.CreateAsync(fileBytes.Length, httpHeaders: new ShareFileHttpHeaders { ContentType = contentType });
      await fileClient.UploadAsync(fileBytes);

      return directoryRef;
    }
  }
}