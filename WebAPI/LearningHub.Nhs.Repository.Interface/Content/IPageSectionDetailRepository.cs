// <copyright file="IPageSectionDetailRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Content
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Content;
    using LearningHub.Nhs.Models.Enums.Content;

    /// <summary>
    /// The IPageIdentifierRepository.
    /// </summary>
    public interface IPageSectionDetailRepository : IGenericRepository<PageSectionDetail>
    {
        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetail}"/>.</returns>
        Task<PageSectionDetail> GetPageSectionDetailImageAssetByIdAsync(int id);

        /// <summary>
        /// The GetPageSectionDetailVideoAssetById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetail}"/>.</returns>
        Task<PageSectionDetail> GetPageSectionDetailVideoAssetByIdAsync(int id);

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="pageSectionDetail">The pageSectionDetail.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        new Task<int> CreateAsync(int userId, PageSectionDetail pageSectionDetail);

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="pageSectionDetail">The pageSectionDetail.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        new Task UpdateAsync(int userId, PageSectionDetail pageSectionDetail);

        /// <summary>
        /// The CloneSectionDetailAsync.
        /// </summary>
        /// <param name="pageSectionDetailId">The pageSectionDetailId<see cref="int"/>.</param>
        /// <param name="sectionTemplateType">sectionTemplateType.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PageSectionDetail> CloneSectionDetailAsync(int pageSectionDetailId, SectionTemplateType sectionTemplateType, int currentUserId);
    }
}