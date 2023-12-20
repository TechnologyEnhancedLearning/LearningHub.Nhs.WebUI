// <copyright file="PageSectionDetailRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Content
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Content;
    using LearningHub.Nhs.Models.Enums.Content;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Content;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The PageSectionDetailRepository.
    /// </summary>
    public class PageSectionDetailRepository : GenericRepository<PageSectionDetail>, IPageSectionDetailRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageSectionDetailRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public PageSectionDetailRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetPageSectionDetailById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetail}"/>.</returns>
        public async Task<PageSectionDetail> GetPageSectionDetailImageAssetByIdAsync(int id)
        {
            return await this.DbContext.PageSectionDetail
                .Include(s => s.ImageAsset).ThenInclude(s => s.ImageFile)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// The GetPageSectionDetailVideoAssetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{PageSectionDetail}"/>.</returns>
        public async Task<PageSectionDetail> GetPageSectionDetailVideoAssetByIdAsync(int id)
        {
            return await this.DbContext.PageSectionDetail
                .Include(v => v.VideoAsset).ThenInclude(v => v.VideoFile)
                .Include(v => v.VideoAsset).ThenInclude(a => a.AzureMediaAsset)
                .Include(v => v.VideoAsset).ThenInclude(t => t.TranscriptFile)
                .Include(v => v.VideoAsset).ThenInclude(c => c.ClosedCaptionsFile)
                .Include(v => v.VideoAsset).ThenInclude(t => t.ThumbnailImageFile)
                .SingleOrDefaultAsync(psd => psd.Id == id);
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="pageSectionDetail">The pageSectionDetail.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async new Task<int> CreateAsync(int userId, PageSectionDetail pageSectionDetail)
        {
            if (pageSectionDetail.ImageAsset != null)
            {
                this.SetAuditFieldsForCreate(userId, pageSectionDetail.ImageAsset);
            }

            return await base.CreateAsync(userId, pageSectionDetail);
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="pageSectionDetail">The pageSectionDetail.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async new Task UpdateAsync(int userId, PageSectionDetail pageSectionDetail)
        {
            if (pageSectionDetail.ImageAsset != null)
            {
                this.SetAuditFieldsForUpdate(userId, pageSectionDetail.ImageAsset);
            }

            await base.UpdateAsync(userId, pageSectionDetail);
        }

        /// <inheritdoc/>
        public async Task<PageSectionDetail> CloneSectionDetailAsync(int pageSectionDetailId, SectionTemplateType sectionTemplateType, int currentUserId)
        {
            PageSectionDetail pageSectionDetail = null;

            if (sectionTemplateType == SectionTemplateType.Video)
            {
                pageSectionDetail = await this.DbContext.PageSectionDetail.Where(ps => ps.Id == pageSectionDetailId).AsNoTracking().Include(psd => psd.VideoAsset).AsNoTracking().SingleAsync();
                var videoAsset = pageSectionDetail.VideoAsset;
                videoAsset.Id = 0;
                pageSectionDetail.VideoAsset = videoAsset;
            }
            else
            {
                pageSectionDetail = await this.DbContext.PageSectionDetail.Where(ps => ps.Id == pageSectionDetailId).AsNoTracking().Include(psd => psd.ImageAsset).AsNoTracking().SingleAsync();
                var imageAsset = pageSectionDetail.ImageAsset;
                imageAsset.Id = 0;
                pageSectionDetail.ImageAsset = imageAsset;
            }

            pageSectionDetail.Id = 0;

            // The new page section must be made in a status of draft
            pageSectionDetail.PageSectionStatusId = (int)PageSectionStatus.Draft;

            pageSectionDetail.Id = await this.CreateAsync(currentUserId, pageSectionDetail);

            if (sectionTemplateType == SectionTemplateType.Video)
            {
                pageSectionDetail = await this.GetPageSectionDetailVideoAssetByIdAsync(pageSectionDetail.Id);
            }
            else
            {
                pageSectionDetail = await this.GetPageSectionDetailImageAssetByIdAsync(pageSectionDetail.Id);
            }

            return pageSectionDetail;
        }
    }
}
