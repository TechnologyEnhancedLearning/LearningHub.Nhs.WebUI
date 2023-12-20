// <copyright file="PageExtension.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Entities.Content;
    using LearningHub.Nhs.Models.Enums.Content;

    /// <summary>
    /// Defines the <see cref="PageExtension" />.
    /// </summary>
    public static class PageExtension
    {
        /// <summary>
        /// The CanPublish.
        /// </summary>
        /// <param name="pageModel">The pageModel<see cref="Page"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool CanPublish(this Page pageModel)
        {
            var excludedPublishStatusColl = new List<PageSectionStatus> { PageSectionStatus.Processing, PageSectionStatus.ProcessingFailed };
            var pageSections = pageModel.PageSections.Where(ps =>
            {
                var psd = ps.PageSectionDetails.First();
                return !ps.IsHidden || (psd.DraftHidden.HasValue && !psd.DraftHidden.Value);
            });
            var canPublish = pageSections.All(ps => !excludedPublishStatusColl.Contains((PageSectionStatus)ps.PageSectionDetails.First().PageSectionStatusId));
            if (canPublish)
            {
                canPublish = !pageSections.All(ps => (PageSectionStatus)ps.PageSectionDetails.First().PageSectionStatusId == PageSectionStatus.Live);
            }

            return pageModel.PageSections.Count() > 0 && canPublish;
        }

        /// <summary>
        /// The CanPreview.
        /// </summary>
        /// <param name="pageModel">The pageModel<see cref="Page"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool CanPreview(this Page pageModel)
        {
            var canPreviewStatusCol = new List<PageSectionStatus> { PageSectionStatus.Draft, PageSectionStatus.Processed };
            var dissallowPreviewStatusCol = new List<PageSectionStatus> { PageSectionStatus.Processing, PageSectionStatus.ProcessingFailed };
            var pageSections = pageModel.PageSections.Where(ps =>
            {
                var psd = ps.PageSectionDetails.First();
                return !ps.IsHidden || (psd.DraftHidden.HasValue && !psd.DraftHidden.Value);
            });
            return pageSections.Count() > 0
                && !pageSections.All(ps => dissallowPreviewStatusCol.Contains((PageSectionStatus)ps.PageSectionDetails.First().PageSectionStatusId))
                && pageSections.Any(ps => canPreviewStatusCol.Contains((PageSectionStatus)ps.PageSectionDetails.First().PageSectionStatusId));
        }

        /// <summary>
        /// The CanDiscard.
        /// </summary>
        /// <param name="pageModel">The pageModel<see cref="Page"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool CanDiscard(this Page pageModel)
        {
            var allowedDiscardStatusColl = new List<PageSectionStatus> { PageSectionStatus.Draft, PageSectionStatus.Processing, PageSectionStatus.ProcessingFailed, PageSectionStatus.Processed };
            var pageSections = pageModel.PageSections.Where(ps => ps.IsHidden == false);
            return pageSections.Count() > 0 && pageModel.PageSections.Any(ps => allowedDiscardStatusColl.Contains((PageSectionStatus)ps.PageSectionDetails.First().PageSectionStatusId));
        }

        /// <summary>
        /// The GetStatus.
        /// </summary>
        /// <param name="pageModel">The pageModel<see cref="Page"/>.</param>
        /// <returns>The <see cref="PageStatus"/>.</returns>
        public static PageStatus GetStatus(this Page pageModel)
        {
            return pageModel.PageSections.Count() > 0 && pageModel.PageSections.Any(ps => (PageSectionStatus)ps.PageSectionDetails.First().PageSectionStatusId != PageSectionStatus.Live) ? PageStatus.EditsPending : PageStatus.Live;
        }

        /// <summary>
        /// The HasHiddenSections.
        /// </summary>
        /// <param name="pageModel">The pageModel<see cref="Page"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool HasHiddenSections(this Page pageModel)
        {
            return pageModel.PageSections.Count() > 0
                && (pageModel.PageSections.Any(ps => ps.IsHidden)
                    || pageModel.PageSections.Any(ps =>
                    {
                        var dh = ps.PageSectionDetails.OrderBy(x => x.Id).First().DraftHidden;
                        return dh.HasValue && dh.Value;
                    }));
        }
    }
}
