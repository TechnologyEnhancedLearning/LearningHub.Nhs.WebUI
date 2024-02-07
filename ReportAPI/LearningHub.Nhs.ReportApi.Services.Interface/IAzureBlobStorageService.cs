// <copyright file="IAzureBlobStorageService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.ReportApi.Shared.Models;

    /// <summary>
    /// The IAzureBlobStorageService interface.
    /// </summary>
    public interface IAzureBlobStorageService
    {
        /// <summary>
        /// The get file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<BlobModel?> GetFile(string fileName);
    }
}