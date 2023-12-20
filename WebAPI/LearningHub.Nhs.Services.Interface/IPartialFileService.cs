// <copyright file="IPartialFileService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource.Files;

    /// <summary>
    /// The PartialFileService interface.
    /// </summary>
    public interface IPartialFileService
    {
        /// <summary>
        /// CreatePartialFile.
        /// </summary>
        /// <param name="viewModel">viewModel.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<PartialFileViewModel> CreatePartialFile(PartialFileViewModel viewModel, int userId);

        /// <summary>
        /// GetFile.
        /// </summary>
        /// <param name="fileId">fileId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<File> GetFile(int fileId);

        /// <summary>
        /// GetPartialFile.
        /// </summary>
        /// <param name="fileId">fileId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<PartialFile> GetPartialFile(int fileId);

        /// <summary>
        /// The CompletePartialFile.
        /// </summary>
        /// <param name="file">file.</param>
        /// <param name="partialFile">partialFile.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task CompletePartialFile(File file, PartialFile partialFile, int userId);
    }
}
