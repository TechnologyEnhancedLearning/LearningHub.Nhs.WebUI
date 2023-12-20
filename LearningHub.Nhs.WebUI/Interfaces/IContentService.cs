// <copyright file="IContentService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Content;

    /// <summary>
    /// Defines the <see cref="IContentService" />.
    /// </summary>
    public interface IContentService
    {
        /// <summary>
        /// The GetPageByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="preview">The preview<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{PageViewModel}"/>.</returns>
        Task<PageViewModel> GetPageByIdAsync(int id, bool preview);

        /// <summary>
        /// GetPageSectionDetailVideoAssetById.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<PageSectionDetailViewModel> GetPageSectionDetailVideoAssetByIdAsync(int id);
    }
}