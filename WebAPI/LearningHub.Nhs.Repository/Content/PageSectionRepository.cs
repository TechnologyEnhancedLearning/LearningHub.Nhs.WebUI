// <copyright file="PageSectionRepository.cs" company="NHS England">
// Copyright (c) NHS England.
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
    /// The PageSectionRepository.
    /// </summary>
    public class PageSectionRepository : GenericRepository<PageSection>, IPageSectionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageSectionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext<see cref="LearningHubDbContext"/>.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public PageSectionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="pageSection">The PageSection.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreateWithPositionAsync(int userId, PageSection pageSection)
        {
            var strategy = this.DbContext.Database.CreateExecutionStrategy();
            var pageSectionId = 0;
            await strategy.Execute(
              async () =>
              {
                  using (var transaction = this.DbContext.Database.BeginTransaction())
                  {
                      try
                      {
                          foreach (var section in this.DbContext.PageSection.Where(p => p.Position >= pageSection.Position && p.PageId == pageSection.PageId))
                          {
                              section.Position += 1;
                              this.SetAuditFieldsForUpdate(userId, section);
                          }

                          pageSectionId = await this.CreateAsync(userId, pageSection);

                          transaction.Commit();

                          return pageSectionId;
                      }
                      catch
                      {
                          transaction.Rollback();
                          throw;
                      }
                  }
              });

            return pageSectionId;
        }

        /// <summary>
        /// The CloneImageSectionAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task CloneImageSectionAsync(int pageSectionId, int currentUserId)
        {
            var pageSection = await this.DbContext.PageSection.Where(ps => ps.Id == pageSectionId).AsNoTracking().Include(p => p.PageSectionDetails).ThenInclude(psd => psd.ImageAsset).AsNoTracking().SingleAsync();
            pageSection.Id = 0;
            var pageSectionDetail = pageSection.PageSectionDetails.Where(psd => psd.Deleted == false).OrderByDescending(psd => psd.Id).Take(1).Single();
            pageSectionDetail.Id = 0;
            this.SetAuditFieldsForCreate(currentUserId, pageSectionDetail);

            var imageAsset = pageSectionDetail.ImageAsset;
            imageAsset.Id = 0;
            this.SetAuditFieldsForCreate(currentUserId, imageAsset);
            pageSectionDetail.ImageAsset = imageAsset;

            // The new page section must be made in a status of draft
            pageSectionDetail.PageSectionStatusId = 1;

            pageSection.PageSectionDetails = new List<PageSectionDetail>
            {
                pageSectionDetail,
            };
            this.SetAuditFieldsForCreate(currentUserId, pageSection);

            var strategy = this.DbContext.Database.CreateExecutionStrategy();
            await strategy.Execute(
              async () =>
              {
                  using (var transaction = this.DbContext.Database.BeginTransaction())
                  {
                      try
                      {
                          var pageSections = await this.DbContext.PageSection
                              .Where(ps => ps.PageId == pageSection.PageId && ps.Deleted == false)
                              .OrderBy(ps => ps.Position).ToListAsync();
                          var pageSectionIndex = pageSections.FindIndex(ps => ps.Id == pageSectionId);
                          for (int i = pageSectionIndex; i <= pageSections.Count() - 1; i++)
                          {
                              pageSections[i].Position += 1;
                              this.SetAuditFieldsForUpdate(currentUserId, pageSections[i]);
                          }

                          await this.CreateAsync(currentUserId, pageSection);
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
        /// The CloneVideoSectionAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task CloneVideoSectionAsync(int pageSectionId, int currentUserId)
        {
            var pageSection = await this.DbContext.PageSection.Where(ps => ps.Id == pageSectionId).AsNoTracking().Include(p => p.PageSectionDetails).ThenInclude(psd => psd.VideoAsset).AsNoTracking().SingleAsync();
            pageSection.Id = 0;
            var pageSectionDetail = pageSection.PageSectionDetails.Where(psd => psd.Deleted == false).OrderByDescending(psd => psd.Id).Take(1).Single();
            pageSectionDetail.Id = 0;
            this.SetAuditFieldsForCreate(currentUserId, pageSectionDetail);

            var videoAsset = pageSectionDetail.VideoAsset;
            videoAsset.Id = 0;
            this.SetAuditFieldsForCreate(currentUserId, videoAsset);
            pageSectionDetail.VideoAsset = videoAsset;

            // The new page section must be made in a status of draft
            pageSectionDetail.PageSectionStatusId = 1;

            pageSection.PageSectionDetails = new List<PageSectionDetail>
            {
                pageSectionDetail,
            };
            this.SetAuditFieldsForCreate(currentUserId, pageSection);

            var strategy = this.DbContext.Database.CreateExecutionStrategy();
            await strategy.Execute(
              async () =>
              {
                  using (var transaction = this.DbContext.Database.BeginTransaction())
                  {
                      try
                      {
                          var pageSections = await this.DbContext.PageSection.Where(ps => ps.PageId == pageSection.PageId && ps.Deleted == false).OrderBy(ps => ps.Position).ToListAsync();
                          var pageSectionIndex = pageSections.FindIndex(ps => ps.Id == pageSectionId);
                          for (int i = pageSectionIndex; i <= pageSections.Count() - 1; i++)
                          {
                              pageSections[i].Position += 1;
                              this.SetAuditFieldsForUpdate(currentUserId, pageSections[i]);
                          }

                          await this.CreateAsync(currentUserId, pageSection);
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
        /// The ChangeOrderUpAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ChangeOrderUpAsync(int pageId, int pageSectionId, int currentUserId)
        {
            var pageSections = this.DbContext.PageSection
                .Include(x => x.PageSectionDetails)
                .Where(ps => ps.PageId == pageId && !ps.Deleted && !ps.IsHidden)
                .ToList()
                .OrderBy(ps =>
                {
                    var latestDetails = ps.PageSectionDetails.OrderByDescending(x => x.Id).First();
                    if (latestDetails.PageSectionStatusId == (int)PageSectionStatus.Draft
                        && latestDetails.DraftPosition != null)
                    {
                        return latestDetails.DraftPosition;
                    }

                    return ps.Position;
                }).ToList();

            var pageSectionIndex = pageSections.FindIndex(ps => ps.Id == pageSectionId);

            if (pageSections.Count >= pageSectionIndex + 1)
            {
                var strategy = this.DbContext.Database.CreateExecutionStrategy();
                await strategy.Execute(
                  async () =>
                  {
                      using (var transaction = this.DbContext.Database.BeginTransaction())
                      {
                          try
                          {
                              var moveDownSection = pageSections[pageSectionIndex + 1];
                              var moveUpSection = pageSections[pageSectionIndex];

                              var (moveDownSectionDetails, moveDownExistingDraft) = await this.EnsureDraft(moveDownSection.Id, currentUserId);
                              var (moveUpSectionDetails, moveUpExistingDraft) = await this.EnsureDraft(moveUpSection.Id, currentUserId);

                              var currentMoveDownPosition = moveDownExistingDraft
                                  ? moveDownSectionDetails.DraftPosition ?? moveDownSection.Position
                                  : moveDownSection.Position;

                              var currentMoveUpPosition = moveUpExistingDraft
                                  ? moveUpSectionDetails.DraftPosition ?? moveUpSection.Position
                                  : moveUpSection.Position;

                              moveDownSectionDetails.DraftPosition = currentMoveDownPosition - 1;

                              if (moveDownExistingDraft)
                              {
                                  this.SetAuditFieldsForUpdate(currentUserId, moveDownSectionDetails);
                              }

                              moveUpSectionDetails.DraftPosition = currentMoveUpPosition + 1;

                              if (moveUpExistingDraft)
                              {
                                  this.SetAuditFieldsForUpdate(currentUserId, moveUpSectionDetails);
                              }

                              await this.DbContext.SaveChangesAsync();
                              if (moveDownExistingDraft)
                              {
                                  this.DbContext.Entry(moveDownSectionDetails).State = EntityState.Detached;
                              }

                              if (moveUpExistingDraft)
                              {
                                  this.DbContext.Entry(moveUpSectionDetails).State = EntityState.Detached;
                              }

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

        /// <summary>
        /// The ChangeOrderUpAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ChangeOrderDownAsync(int pageId, int pageSectionId, int currentUserId)
        {
            var pageSections = this.DbContext.PageSection
                .Include(x => x.PageSectionDetails)
                .Where(ps => ps.PageId == pageId && !ps.Deleted && !ps.IsHidden)
                .ToList()
                .OrderBy(ps =>
                {
                    var latestDetails = ps.PageSectionDetails.OrderByDescending(x => x.Id).First();
                    if (latestDetails.PageSectionStatusId == (int)PageSectionStatus.Draft
                        && latestDetails.DraftPosition != null)
                    {
                        return latestDetails.DraftPosition;
                    }

                    return ps.Position;
                }).ToList();

            var pageSectionIndex = pageSections.FindIndex(ps => ps.Id == pageSectionId);

            if (pageSectionIndex > 0)
            {
                var strategy = this.DbContext.Database.CreateExecutionStrategy();
                await strategy.Execute(
                  async () =>
                  {
                      using (var transaction = this.DbContext.Database.BeginTransaction())
                      {
                          try
                          {
                              var moveDownSection = pageSections[pageSectionIndex];
                              var moveUpSection = pageSections[pageSectionIndex - 1];

                              var (moveDownSectionDetails, moveDownExistingDraft) = await this.EnsureDraft(moveDownSection.Id, currentUserId);
                              var (moveUpSectionDetails, moveUpExistingDraft) = await this.EnsureDraft(moveUpSection.Id, currentUserId);

                              var currentMoveDownPosition = moveDownExistingDraft
                                  ? moveDownSectionDetails.DraftPosition ?? moveDownSection.Position
                                  : moveDownSection.Position;

                              var currentMoveUpPosition = moveUpExistingDraft
                                  ? moveUpSectionDetails.DraftPosition ?? moveUpSection.Position
                                  : moveUpSection.Position;

                              moveDownSectionDetails.DraftPosition = currentMoveDownPosition - 1;

                              if (moveDownExistingDraft)
                              {
                                  this.SetAuditFieldsForUpdate(currentUserId, moveDownSectionDetails);
                              }

                              moveUpSectionDetails.DraftPosition = currentMoveUpPosition + 1;

                              if (moveUpExistingDraft)
                              {
                                  this.SetAuditFieldsForUpdate(currentUserId, moveUpSectionDetails);
                              }

                              await this.DbContext.SaveChangesAsync();
                              if (moveDownExistingDraft)
                              {
                                  this.DbContext.Entry(moveDownSectionDetails).State = EntityState.Detached;
                              }

                              if (moveUpExistingDraft)
                              {
                                  this.DbContext.Entry(moveUpSectionDetails).State = EntityState.Detached;
                              }

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

        /// <summary>
        /// The HideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task HideAsync(int pageSectionId, int currentUserId)
        {
            var strategy = this.DbContext.Database.CreateExecutionStrategy();
            await strategy.Execute(
              async () =>
              {
                  using (var transaction = this.DbContext.Database.BeginTransaction())
                  {
                      try
                      {
                          var (pageSectionDetails, ed) = await this.EnsureDraft(pageSectionId, currentUserId);
                          pageSectionDetails.DraftHidden = true;
                          if (ed)
                          {
                              this.SetAuditFieldsForUpdate(currentUserId, pageSectionDetails);
                          }

                          await this.DbContext.SaveChangesAsync();
                          if (ed)
                          {
                              this.DbContext.Entry(pageSectionDetails).State = EntityState.Detached;
                          }

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
        /// The UnHideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UnHideAsync(int pageSectionId, int currentUserId)
        {
            var strategy = this.DbContext.Database.CreateExecutionStrategy();
            await strategy.Execute(
              async () =>
              {
                  using (var transaction = this.DbContext.Database.BeginTransaction())
                  {
                      try
                      {
                          var (pageSectionDetails, ed) = await this.EnsureDraft(pageSectionId, currentUserId);
                          pageSectionDetails.DraftHidden = false;
                          if (ed)
                          {
                              this.SetAuditFieldsForUpdate(currentUserId, pageSectionDetails);
                          }

                          await this.DbContext.SaveChangesAsync();

                          if (ed)
                          {
                              this.DbContext.Entry(pageSectionDetails).State = EntityState.Detached;
                          }

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
        /// The DeleteAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int pageSectionId, int currentUserId)
        {
            var strategy = this.DbContext.Database.CreateExecutionStrategy();
            await strategy.Execute(
              async () =>
              {
                  using (var transaction = this.DbContext.Database.BeginTransaction())
                  {
                      try
                      {
                          var (pageSectionDetails, ed) = await this.EnsureDraft(pageSectionId, currentUserId);
                          pageSectionDetails.DeletePending = true;
                          if (ed)
                          {
                              this.DbContext.Set<PageSectionDetail>().Update(pageSectionDetails);
                              this.SetAuditFieldsForUpdate(currentUserId, pageSectionDetails);
                          }

                          await this.DbContext.SaveChangesAsync();
                          if (ed)
                          {
                              this.DbContext.Entry(pageSectionDetails).State = EntityState.Detached;
                          }

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
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PageSection> GetByIdAsync(int pageSectionId)
        {
            return await this.DbContext.PageSection
                .Include(ps => ps.PageSectionDetails)
                    .ThenInclude(s => s.ImageAsset).ThenInclude(s => s.ImageFile)
                .SingleAsync(ps => ps.Id == pageSectionId);
        }

        /// <summary>
        /// The EnsureDraft method. This makes sure the latest PageSectionDetails for the PageSection
        /// is a draft, and creates a new draft if not.
        /// Used when moving/hiding/etc PageSections, so that the PageSectionDetails which
        /// are updated are not the Live versions.
        /// </summary>
        /// <returns>The task.</returns>
        private async Task<(PageSectionDetail, bool)> EnsureDraft(int pageSectionId, int currentUserId)
        {
            var pageSection = this.DbContext.PageSection
                .Where(ps => ps.Id == pageSectionId)
                .Include(p => p.PageSectionDetails).ThenInclude(x => x.VideoAsset)
                .Include(p => p.PageSectionDetails).ThenInclude(x => x.ImageAsset)
                .Single();

            var pageSectionDetail = pageSection.PageSectionDetails
                .Where(psd => psd.Deleted == false)
                .OrderByDescending(psd => psd.Id).Take(1).Single();

            var existingDraft = true;
            if (pageSectionDetail.PageSectionStatusId != (int)PageSectionStatus.Draft)
            {
                this.DbContext.Entry(pageSectionDetail).State = EntityState.Detached;
                pageSectionDetail.Id = 0;
                pageSectionDetail.PageSectionStatusId = (int)PageSectionStatus.Draft;

                if (pageSectionDetail.VideoAsset != null)
                {
                    var va = pageSectionDetail.VideoAsset;
                    this.DbContext.Entry(va).State = EntityState.Detached;
                    va.Id = 0;
                    this.SetAuditFieldsForCreate(currentUserId, va);
                    pageSectionDetail.VideoAsset = va;
                }

                if (pageSectionDetail.ImageAsset != null)
                {
                    var ia = pageSectionDetail.ImageAsset;
                    this.DbContext.Entry(ia).State = EntityState.Detached;
                    ia.Id = 0;
                    this.SetAuditFieldsForCreate(currentUserId, ia);
                    pageSectionDetail.ImageAsset = ia;
                }

                await this.DbContext.Set<PageSectionDetail>().AddAsync(pageSectionDetail);
                this.SetAuditFieldsForCreate(currentUserId, pageSectionDetail);

                existingDraft = false;
            }

            return (pageSectionDetail, existingDraft);
        }
    }
}
