namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Content
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Content;
    using LearningHub.Nhs.Models.Enums.Content;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Content;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The PageRepository.
    /// </summary>
    public class PageRepository : GenericRepository<Page>, IPageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext<see cref="LearningHubDbContext"/>.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public PageRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The DiscardAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DiscardAsync(int pageId, int currentUserId)
        {
            var page = await DbContext.Page.Include(p => p.PageSections).ThenInclude(ps => ps.PageSectionDetails)
                .Where(p => p.Id == pageId).SingleAsync();

            var discardStatusColl = new List<PageSectionStatus> { PageSectionStatus.Draft, PageSectionStatus.Processing, PageSectionStatus.ProcessingFailed, PageSectionStatus.Processed };

            var strategy = DbContext.Database.CreateExecutionStrategy();
            await strategy.Execute(
              async () =>
              {
                  using (var transaction = DbContext.Database.BeginTransaction())
                  {
                      try
                      {
                          foreach (var pageSection in page.PageSections)
                          {
                              foreach (var pageSectionDetail in pageSection.PageSectionDetails.Where(psd => psd.Deleted == false))
                              {
                                  if (discardStatusColl.Contains((PageSectionStatus)pageSectionDetail.PageSectionStatusId))
                                  {
                                      pageSectionDetail.Deleted = true;
                                      SetAuditFieldsForUpdate(currentUserId, pageSectionDetail);
                                  }
                              }

                              if (pageSection.PageSectionDetails.All(psd => psd.Deleted))
                              {
                                  SetAuditFieldsForUpdate(currentUserId, pageSection);
                                  pageSection.Deleted = true;
                              }
                          }

                          SetAuditFieldsForUpdate(currentUserId, page);
                          await DbContext.SaveChangesAsync();
                          transaction.Commit();
                      }
                      catch
                      {
                          transaction.Rollback();
                          throw;
                      }
                  }
              });
        }

        /// <summary>
        /// The GetPageById.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="publishedOnly">The published only<see cref="bool"/>.</param>
        /// <param name="preview">Preview mode.</param>
        /// <returns>The <see cref="Task{Page}"/>.</returns>
        public async Task<Page> GetPageByIdAsync(int id, bool publishedOnly, bool preview)
        {
            var pageData = await DbContext.Page
                    .Include(p => p.PageSections)
                    .ThenInclude(p => p.PageSectionDetails)
                    .ThenInclude(s => s.ImageAsset)
                    .ThenInclude(s => s.ImageFile)
                .Where(p => p.Id == id)
                .SingleAsync();

            if (publishedOnly)
            {
                foreach (var section in pageData.PageSections)
                {
                    section.PageSectionDetails.Where(p => p.PageSectionStatusId != (int)PageSectionStatus.Live).ToList()
                        .ForEach(e => section.PageSectionDetails.Remove(e));
                }

                pageData.PageSections.Where(p => p.PageSectionDetails.Count == 0).ToList()
                    .ForEach(e => pageData.PageSections.Remove(e));
            }

            if (preview)
            {
                var removeDeleted = pageData.PageSections.Where(ps => !ps.PageSectionDetails.Any(psd => psd.DeletePending.HasValue && psd.DeletePending.Value)).ToList();
                pageData.PageSections = removeDeleted;
            }

            return new Page
            {
                Id = pageData.Id,
                Name = pageData.Name,
                Url = pageData.Url,
                PageSections = pageData.PageSections.Where(ps => ps.Deleted == false).Select(ps =>
                     new PageSection
                     {
                         Id = ps.Id,
                         IsHidden = ps.IsHidden,
                         Position = ps.Position,
                         AmendUserId = ps.AmendUserId,
                         SectionTemplateTypeId = ps.SectionTemplateTypeId,
                         PageSectionDetails = ps.PageSectionDetails.Where(psd => psd.Deleted == false).OrderByDescending(psd => psd.Id).Take(1).ToList(),
                     }).ToList(),
            };
        }

        /// <summary>
        /// The GetPagesAsync.
        /// </summary>
        /// <returns>The <see cref="T:Task{List{Page}}"/>.</returns>
        public async Task<List<Page>> GetPagesAsync()
        {
            return await DbContext.Page.Select(p =>
                new Page
                {
                    Id = p.Id,
                    Name = p.Name,
                    Url = p.Url,
                    PageSections = p.PageSections.Where(ps => ps.Deleted == false).Select(ps =>
                         new PageSection
                         {
                             PageSectionDetails = ps.PageSectionDetails.Where(psd => psd.Deleted == false).OrderByDescending(psd => psd.Id).ToList(),
                         }).ToList(),
                }).ToListAsync();
        }

        /// <summary>
        /// The PublishAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task PublishAsync(int pageId, int currentUserId)
        {
            var page = await DbContext.Page.Include(p => p.PageSections).ThenInclude(ps => ps.PageSectionDetails.OrderBy(psd => psd.PageSectionStatusId))
                .Where(p => p.Id == pageId).SingleAsync();

            var publishStatusColl = new List<PageSectionStatus> { PageSectionStatus.Processed };

            var strategy = DbContext.Database.CreateExecutionStrategy();
            await strategy.Execute(
              async () =>
              {
                  using (var transaction = DbContext.Database.BeginTransaction())
                  {
                      try
                      {
                          foreach (var pageSection in page.PageSections)
                          {
                              var detailSetToLive = false;
                              foreach (var pageSectionDetail in pageSection.PageSectionDetails.Where(psd => psd.Deleted == false))
                              {
                                  if (detailSetToLive && (PageSectionStatus)pageSectionDetail.PageSectionStatusId == PageSectionStatus.Live)
                                  {
                                      pageSectionDetail.Deleted = true;
                                      SetAuditFieldsForUpdate(currentUserId, pageSectionDetail);
                                  }

                                  if (publishStatusColl.Contains((PageSectionStatus)pageSectionDetail.PageSectionStatusId))
                                  {
                                      pageSectionDetail.PageSectionStatusId = (int)PageSectionStatus.Live;
                                      detailSetToLive = true;
                                      SetAuditFieldsForUpdate(currentUserId, pageSectionDetail);
                                  }

                                  if (pageSectionDetail.PageSectionStatusId == (int)PageSectionStatus.Draft)
                                  {
                                      if (pageSectionDetail.DraftHidden.HasValue)
                                      {
                                          pageSection.IsHidden = pageSectionDetail.DraftHidden.Value;
                                      }

                                      if (pageSectionDetail.DraftPosition.HasValue)
                                      {
                                          pageSection.Position = pageSectionDetail.DraftPosition.Value;
                                      }

                                      if (pageSectionDetail.DeletePending.HasValue && pageSectionDetail.DeletePending.Value)
                                      {
                                          pageSection.Deleted = true;
                                      }

                                      pageSectionDetail.DraftHidden = null;
                                      pageSectionDetail.DraftPosition = null;
                                      pageSectionDetail.DeletePending = null;

                                      pageSectionDetail.PageSectionStatusId = (int)PageSectionStatus.Live;
                                      SetAuditFieldsForUpdate(currentUserId, pageSectionDetail);
                                      SetAuditFieldsForUpdate(currentUserId, pageSection);
                                  }
                              }
                          }

                          SetAuditFieldsForUpdate(currentUserId, page);
                          await DbContext.SaveChangesAsync();
                          transaction.Commit();
                      }
                      catch
                      {
                          transaction.Rollback();
                          throw;
                      }
                  }
              });
        }
    }
}
