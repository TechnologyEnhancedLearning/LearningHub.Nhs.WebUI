// <copyright file="IPartialFileUploadService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource.Files;
    using tusdotnet.Interfaces;
    using tusdotnet.Models.Configuration;

    /// <summary>
    /// Defines the <see cref="IPartialFileUploadService" />.
    /// </summary>
    public interface IPartialFileUploadService : ITusStore, ITusCreationStore
    {
        /// <summary>
        /// Creates the partial file.
        /// </summary>
        /// <param name="requestViewModel">The PartialFileViewModel.</param>
        /// <returns>A Task.</returns>
        Task<PartialFileViewModel> CreatePartialFile(PartialFileViewModel requestViewModel);

        /// <summary>
        /// Authorize the request.
        /// </summary>
        /// <param name="authoriseContext">The AuthorizeContext.</param>
        /// <returns>A Task.</returns>
        Task OnAuthoriseAsync(AuthorizeContext authoriseContext);

        /// <summary>
        /// Process the file once upload is complete.
        /// </summary>
        /// <param name="fileCompleteContext">The FileCompleteContext.</param>
        /// <returns>A Task.</returns>
        Task OnFileCompleteAsync(FileCompleteContext fileCompleteContext);
    }
}