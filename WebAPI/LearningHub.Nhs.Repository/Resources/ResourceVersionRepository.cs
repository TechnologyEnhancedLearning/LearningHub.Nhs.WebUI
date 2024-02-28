// <copyright file="ResourceVersionRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource version repository.
    /// </summary>
    public class ResourceVersionRepository : GenericRepository<ResourceVersion>, IResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get all admin search.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        public IQueryable<ResourceVersion> GetAllAdminSearch(int userId)
        {
            return this.DbContext.Set<ResourceVersion>()
                .Include(r => r.Resource)
                .ThenInclude(r => r.ResourceReference)
                .Include(n => n.CreateUser)
                .Where(r => !r.Deleted
                        && (r.Resource.CurrentResourceVersionId.Value == r.Id
                            || (r.VersionStatusEnum == VersionStatusEnum.Draft
                            || r.VersionStatusEnum == VersionStatusEnum.Publishing
                            || r.VersionStatusEnum == VersionStatusEnum.SubmittedToPublishingQueue
                            || r.VersionStatusEnum == VersionStatusEnum.FailedToPublish)))
                .OrderByDescending(r => r.Id)
                .AsNoTracking();
        }

        /// <summary>
        /// The GetResourceVersionsForSearchSubmission.
        /// </summary>
        /// <param name="resourceVersionIds">List of resource version ids.</param>
        /// <returns>The resource version list.</returns>
        public async Task<List<ResourceVersion>> GetResourceVersionsForSearchSubmission(List<int> resourceVersionIds)
        {
            return await this.DbContext.ResourceVersion
                    .Include(x => x.Resource).ThenInclude(r => r.ResourceReference)
                    .Include(x => x.ResourceVersionKeyword)
                    .Include(x => x.ResourceVersionAuthor)
                    .Include(x => x.ResourceVersionRatingSummary)
                    .Include(x => x.Publication)
                    .Include(r => r.ResourceVersionProvider)
                    .Include(x => x.Resource)
                        .ThenInclude(x => x.NodeResource)
                        .ThenInclude(x => x.Node)
                        .ThenInclude(x => x.NodePaths)
                    .AsNoTracking()
                    .Where(r => resourceVersionIds.Any(resourceVersionId => resourceVersionId == r.Id)).ToListAsync();
        }

        /// <summary>
        /// The get basic by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersion> GetBasicByIdAsync(int id)
        {
            return await this.DbContext.ResourceVersion
                .Include(rv => rv.Resource)
                    .ThenInclude(r => r.CurrentResourceVersion)
                .Include(rv => rv.Resource)
                    .ThenInclude(r => r.NodeResource)
                .Include(rv => rv.ResourceVersionAuthor)
                .Include(rv => rv.ResourceVersionKeyword)
                .Include(rv => rv.ResourceVersionFlag)
                .Include(rv => rv.ResourceVersionProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync(rv => rv.Id == id && !rv.Deleted);
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeEvents">Flag to include ResourceVersionEvents.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersion> GetByIdAsync(int id, bool includeEvents)
        {
            ResourceVersion retVal;
            if (includeEvents)
            {
                retVal = await this.DbContext.ResourceVersion.AsNoTracking()
                             .Include(r => r.Resource)
                             .Include(r => r.Publication).AsNoTracking()
                             .Include(r => r.ResourceVersionKeyword).AsNoTracking()
                             .Include(r => r.ResourceVersionAuthor).AsNoTracking()
                             .Include(r => r.ResourceVersionFlag).AsNoTracking()
                             .Include(r => r.ResourceVersionEvent).AsNoTracking()
                             .Include(r => r.ResourceLicence).AsNoTracking()
                             .Include(r => r.ResourceVersionProvider).ThenInclude(r => r.Provider).AsNoTracking()
                             .Include(r => r.CreateUser).AsNoTracking()
                             .Include(r => r.ResourceVersionValidationResult).AsNoTracking()
                             .FirstOrDefaultAsync(r => r.Id == id
                                                       && !r.Deleted);
            }
            else
            {
                retVal = await this.DbContext.ResourceVersion.AsNoTracking()
                             .Include(r => r.Resource)
                             .Include(r => r.Publication).AsNoTracking()
                             .Include(r => r.ResourceVersionKeyword).AsNoTracking()
                             .Include(r => r.ResourceVersionAuthor).AsNoTracking()
                             .Include(r => r.ResourceVersionFlag).AsNoTracking()
                             .Include(r => r.ResourceLicence).AsNoTracking()
                             .Include(r => r.ResourceVersionProvider).ThenInclude(r => r.Provider).AsNoTracking()
                             .Include(r => r.CreateUser).AsNoTracking()
                             .Include(r => r.ResourceVersionValidationResult).AsNoTracking()
                             .FirstOrDefaultAsync(r => r.Id == id
                                                       && !r.Deleted);
            }

            return retVal;
        }

        /// <summary>
        /// The get current for resource async.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersion> GetCurrentForResourceAsync(int resourceId)
        {
            return await this.DbContext.ResourceVersion.AsNoTracking()
                       .Include(r => r.Resource)
                       .Include(r => r.Publication).AsNoTracking()
                       .Include(r => r.ResourceVersionKeyword).AsNoTracking()
                       .Include(r => r.ResourceVersionAuthor).AsNoTracking()
                       .Include(r => r.ResourceVersionFlag).AsNoTracking()
                       .Include(r => r.ResourceVersionEvent).AsNoTracking()
                       .Include(r => r.ResourceLicence).AsNoTracking()
                       .Include(r => r.CreateUser).AsNoTracking()
                       .Include(r => r.ResourceVersionProvider).ThenInclude(r => r.Provider).AsNoTracking()
                       .OrderByDescending(r => r.Id)
                       .FirstOrDefaultAsync(r => r.ResourceId == resourceId
                                                 && r.VersionStatusEnum != VersionStatusEnum.FailedToPublish
                                                 && !r.Deleted);
        }

        /// <summary>
        /// The get current published for resource async.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersion> GetCurrentPublicationForResourceAsync(int resourceId)
        {
            return await this.DbContext.ResourceVersion.AsNoTracking()
                       .Include(r => r.Resource)
                       .OrderByDescending(r => r.Id)
                       .FirstOrDefaultAsync(r => r.ResourceId == resourceId
                                                 && !r.Deleted
                                                 && r.Resource.CurrentResourceVersionId.Value == r.Id
                                                 && (r.VersionStatusEnum == VersionStatusEnum.Published || r.VersionStatusEnum == VersionStatusEnum.Unpublished));
        }

        /// <summary>
        /// The resource cards.
        /// </summary>
        /// <param name="includeEvents">Flag to include ResourceVersionEvents.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<ResourceVersion>> GetResourceCards(bool includeEvents)
        {
            List<ResourceVersion> retVal;

            if (includeEvents)
            {
                retVal = await this.DbContext.ResourceVersion.AsNoTracking()
                                        .Include(r => r.Resource).AsNoTracking()
                                        .Include(r => r.Publication).AsNoTracking()
                                        .Include(r => r.ResourceVersionKeyword).AsNoTracking()
                                        .Include(r => r.ResourceVersionAuthor).AsNoTracking()
                                        .Include(r => r.ResourceVersionFlag).AsNoTracking()
                                        .Include(r => r.ResourceVersionEvent).AsNoTracking()
                                        .Include(r => r.CreateUser).AsNoTracking()
                                        .Include(r => r.ResourceVersionProvider).AsNoTracking()
                                        .Where(r => !r.Deleted && r.Resource.CurrentResourceVersionId.Value == r.Id && r.VersionStatusEnum == VersionStatusEnum.Published).ToListAsync();
            }
            else
            {
                retVal = await this.DbContext.ResourceVersion.AsNoTracking()
                                        .Include(r => r.Resource).AsNoTracking()
                                        .Include(r => r.Publication).AsNoTracking()
                                        .Include(r => r.ResourceVersionKeyword).AsNoTracking()
                                        .Include(r => r.ResourceVersionAuthor).AsNoTracking()
                                        .Include(r => r.ResourceVersionFlag).AsNoTracking()
                                        .Include(r => r.CreateUser).AsNoTracking()
                                        .Include(r => r.ResourceVersionProvider).AsNoTracking()
                                        .Where(r => !r.Deleted && r.Resource.CurrentResourceVersionId.Value == r.Id && r.VersionStatusEnum == VersionStatusEnum.Published).ToListAsync();
            }

            return retVal;
        }

        /// <summary>
        /// The get current for resource reference id async.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersion> GetCurrentForResourceReferenceIdAsync(int resourceReferenceId)
        {
            ResourceVersion retVal = null;

            var or = this.DbContext.ResourceReference.OrderByDescending(r => r.Id).FirstOrDefault(x => x.OriginalResourceReferenceId == resourceReferenceId);

            if (or != null)
            {
                retVal = await this.GetCurrentForResourceAsync(or.ResourceId);
            }

            return retVal;
        }

        /// <summary>
        /// The get current published for resource reference id async.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersion> GetCurrentPublicationForResourceReferenceIdAsync(int resourceReferenceId)
        {
            ResourceVersion retVal = null;

            var rr = this.DbContext.ResourceReference.FirstOrDefault(x => x.OriginalResourceReferenceId == resourceReferenceId);

            if (rr != null)
            {
                retVal = await this.GetCurrentPublicationForResourceAsync(rr.ResourceId);
            }

            return retVal;
        }

        /// <summary>
        /// The get resource versions.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<ResourceVersion>> GetResourceVersionsAsync(int resourceId)
        {
            return await this.DbContext.ResourceVersion.AsNoTracking()
                                                    .Include(r => r.Resource).AsNoTracking()
                                                    .Include(r => r.Publication).AsNoTracking()
                                                    .Include(r => r.ResourceVersionKeyword).AsNoTracking()
                                                    .Include(r => r.ResourceVersionAuthor).AsNoTracking()
                                                    .Include(r => r.ResourceVersionFlag).AsNoTracking()
                                                    .Include(r => r.ResourceVersionEvent).AsNoTracking()
                                                    .Include(r => r.CreateUser).AsNoTracking()
                                                    .Include(r => r.ResourceVersionProvider).AsNoTracking()
                                                    .Where(r => r.ResourceId == resourceId)
                                                    .OrderBy(r => r.Id).ToListAsync();
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersion">The resource version.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public new async Task UpdateAsync(int userId, ResourceVersion resourceVersion)
        {
            var resourceVersionUpdate = this.DbContext.ResourceVersion
                .SingleOrDefault(r => r.Id == resourceVersion.Id);

            if (resourceVersionUpdate != null)
            {
                // Update Resource Version
                resourceVersionUpdate.AdditionalInformation = resourceVersion.AdditionalInformation ?? string.Empty;
                resourceVersionUpdate.Title = resourceVersion.Title;
                resourceVersionUpdate.ResourceAccessibilityEnum = resourceVersion.ResourceAccessibilityEnum;
                resourceVersionUpdate.Description = resourceVersion.Description ?? string.Empty;
                resourceVersionUpdate.ReviewDate = resourceVersion.ReviewDate;
                resourceVersionUpdate.ResourceLicenceId = resourceVersion.ResourceLicenceId == 0 ? null : resourceVersion.ResourceLicenceId;
                resourceVersionUpdate.SensitiveContent = resourceVersion.SensitiveContent;
                resourceVersionUpdate.CertificateEnabled = resourceVersion.CertificateEnabled;
                this.SetAuditFieldsForUpdate(userId, resourceVersionUpdate);
            }

            await this.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// The set resource type.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="resourceTypeEnum">The resource type enum.</param>
        /// <param name="userId">The user id.</param>
        public void SetResourceType(int resourceVersionId, ResourceTypeEnum resourceTypeEnum, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = (int)resourceTypeEnum };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = userId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            this.DbContext.Database.ExecuteSqlRaw("resources.ResourceVersionSetResourceType @p0, @p1, @p2, @p3", param0, param1, param2, param3);
        }

        /// <summary>
        /// Gets the resource type of a ResourceVersion.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceTypeEnum> GetResourceType(int resourceVersionId)
        {
            return await this.DbContext.ResourceVersion
               .Include(rv => rv.Resource).AsNoTracking()
               .Where(rv => rv.Id == resourceVersionId)
               .Select(rv => rv.Resource.ResourceTypeEnum)
               .FirstOrDefaultAsync();
        }

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="isMajorRevision">The is major revision.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="publicationDate">The publication date. Set to null if not giving the resource a publication date in the past. This parameter is intended for use by the migration tool.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The publication id.</returns>
        public int Publish(int resourceVersionId, bool isMajorRevision, string notes, DateTimeOffset? publicationDate, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = new SqlParameter("@p1", SqlDbType.Bit) { Value = isMajorRevision };
            var param2 = new SqlParameter("@p2", SqlDbType.VarChar) { Value = notes };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param5 = new SqlParameter("@p5", SqlDbType.Int) { Direction = ParameterDirection.Output };

            string sql = "resources.ResourceVersionPublish @p0, @p1, @p2, @p3, @p4, @p5 output";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3, param4, param5 };

            if (publicationDate.HasValue)
            {
                sqlParams.Add(new SqlParameter("@p6", SqlDbType.DateTimeOffset) { Value = publicationDate });
                sql += ", @PublicationDate=@p6";
            }

            this.DbContext.Database.ExecuteSqlRaw(sql, sqlParams);

            return (int)param5.Value;
        }

        /// <summary>
        /// The unpublish.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="details">The details.</param>
        /// <param name="userId">The user id.</param>
        public void Unpublish(int resourceVersionId, string details, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = string.IsNullOrEmpty(details)
                ? new SqlParameter("@p1", SqlDbType.VarChar) { Value = DBNull.Value }
                : new SqlParameter("@p1", SqlDbType.VarChar) { Value = details };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = userId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            this.DbContext.Database.ExecuteSqlRaw("resources.ResourceVersionUnpublish @p0, @p1, @p2, @p3", param0, param1, param2, param3);
        }

        /// <summary>
        /// The revert to draft.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        public void RevertToDraft(int resourceVersionId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            this.DbContext.Database.ExecuteSqlRaw("resources.ResourceVersionRevertToDraft @p0, @p1, @p2", param0, param1, param2);
        }

        /// <summary>
        /// Deletes a resource version.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        public void Delete(int resourceVersionId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            this.DbContext.Database.ExecuteSqlRaw("resources.ResourceVersionDelete @p0, @p1, @p2", param0, param1, param2);
        }

        /// <summary>
        /// Deletes a resource version.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="status">The status of the resource version.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<bool> DoesVersionExist(int resourceId, VersionStatusEnum status)
        {
            return this.DbContext.ResourceVersion.AnyAsync(rv => rv.ResourceId == resourceId && rv.VersionStatusEnum == status);
        }

        /// <summary>
        /// Creates the next a resource version.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreateNextVersionAsync(int resourceId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Direction = ParameterDirection.Output };

            await this.DbContext.Database.ExecuteSqlRawAsync("resources.ResourceVersionCreateNext @p0, @p1, @p2, @p3 output", param0, param1, param2, param3);

            return (int)param3.Value;
        }

        /// <summary>
        /// Duplicates a resource version.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Returns the new resource version id.</returns>
        public async Task<int> CreateDuplicateVersionAsync(int resourceId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Direction = ParameterDirection.Output };

            await this.DbContext.Database.ExecuteSqlRawAsync("resources.ResourceVersionCreateDuplicate @p0, @p1, @p2, @p3 output", param0, param1, param2, param3);

            return (int)param3.Value;
        }

        ///// <summary>
        ///// Create activity record against a ResourceVersion.
        ///// </summary>
        ///// <param name="userId">
        ///// The user id.
        ///// </param>
        ///// <param name="resourceVersionId">
        ///// The resource version id.
        ///// </param>
        ///// <param name="activityStatusEnum">
        ///// The activity Status Enum.
        ///// </param>
        ///// <param name="activityStart">
        ///// The activity Start.
        ///// </param>
        ///// <param name="activityEnd">
        ///// The activity End.
        ///// </param>
        ////public void CreateActivity(int userId, int resourceVersionId, ActivityStatusEnum activityStatusEnum, DateTimeOffset activityStart, DateTimeOffset activityEnd)
        ////{
        ////    var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = userId };
        ////    var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = resourceVersionId };
        ////    var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = DBNull.Value };
        ////    var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = (int)activityStatusEnum };
        ////    var param4 = new SqlParameter("@p4", SqlDbType.DateTimeOffset) { Value = activityStart };
        ////    var param5 = new SqlParameter("@p5", SqlDbType.DateTimeOffset) { Value = activityEnd };

        ////    this.DbContext.Database.ExecuteSqlCommand("activity.ResourceActivityCreate @p0, @p1, @p2, @p3, @p4, @p5", param0, param1, param2, param3, param4, param5);
        ////}

        /// <summary>
        /// Check if a user has completed the activity corresponding to the resource version.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>A boolean.</returns>
        public async Task<bool> HasUserCompletedActivity(int userId, int resourceVersionId)
        {
            return await this.DbContext.ResourceActivity.AnyAsync(x => x.UserId == userId && x.ResourceVersionId == resourceVersionId);
        }

        /// <summary>
        /// The publishing.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        public void SubmitForPublishing(int resourceVersionId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            this.DbContext.Database.ExecuteSqlRaw("resources.ResourceVersionSubmitForPublishing @p0, @p1, @p2", param0, param1, param2);
        }

        /// <summary>
        /// The publishing.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        public void Publishing(int resourceVersionId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };

            this.DbContext.Database.ExecuteSqlRaw("resources.ResourceVersionPublishing @p0, @p1", param0, param1);
        }

        /// <summary>
        /// Set resource version to "failed to publish".
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        public void FailedToPublish(int resourceVersionId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };

            this.DbContext.Database.ExecuteSqlRaw("resources.ResourceVersionFailedToPublish @p0, @p1", param0, param1);
        }

        /// <summary>
        /// Create resource version event.
        /// </summary>
        /// <param name="resourceVersionId">resourceVersionId.</param>
        /// <param name="resourceVersionEventType">resourceVersionEventType.</param>
        /// <param name="details">details.</param>
        /// <param name="userId">user id.</param>
        public void CreateResourceVersionEvent(int resourceVersionId, ResourceVersionEventTypeEnum resourceVersionEventType, string details, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = (int)resourceVersionEventType };
            var param2 = new SqlParameter("@p2", SqlDbType.NVarChar) { Value = details };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            this.DbContext.Database.ExecuteSqlRaw("resources.ResourceVersionEventCreate @p0, @p1, @p2, @p3, @p4", param0, param1, param2, param3, param4);
        }

        /// <summary>
        /// Create resource version event.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A list of contributed resources.</returns>
        public MyContributionsTotalsViewModel GetMyContributionTotals(int catalogueNodeId, int userId)
        {
            var param0 = new SqlParameter("@catalogueNodeId", SqlDbType.Int) { Value = catalogueNodeId };
            var param1 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@ActionRequiredCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var param3 = new SqlParameter("@UserActionRequiredCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var param4 = new SqlParameter("@DraftCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var param5 = new SqlParameter("@UserDraftCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var param6 = new SqlParameter("@PublishedCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var param7 = new SqlParameter("@UserPublishedCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var param8 = new SqlParameter("@UnpublishedCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var param9 = new SqlParameter("@UserUnpublishedCount", SqlDbType.Int) { Direction = ParameterDirection.Output };

            this.DbContext.Database.ExecuteSqlRaw("resources.GetContributionTotals @catalogueNodeId, @userId, @ActionRequiredCount output, @UserActionRequiredCount output, @DraftCount output, @UserDraftCount output, @PublishedCount output, @UserPublishedCount output, @UnpublishedCount output, @UserUnpublishedCount output", param0, param1, param2, param3, param4, param5, param6, param7, param8, param9);

            var myContributionsTotalsViewModel = new MyContributionsTotalsViewModel();

            myContributionsTotalsViewModel.ActionRequiredCount = (int)param2.Value;
            myContributionsTotalsViewModel.UserActionRequiredCount = (int)param3.Value;
            myContributionsTotalsViewModel.DraftCount = (int)param4.Value;
            myContributionsTotalsViewModel.UserDraftCount = (int)param5.Value;
            myContributionsTotalsViewModel.PublishedCount = (int)param6.Value;
            myContributionsTotalsViewModel.UserPublishedCount = (int)param7.Value;
            myContributionsTotalsViewModel.UnpublishedCount = (int)param8.Value;
            myContributionsTotalsViewModel.UserUnpublishedCount = (int)param9.Value;

            return myContributionsTotalsViewModel;
        }

        /// <summary>
        /// Create resource version event.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceContributionsRequestViewModel">The ResourceContributionsRequestViewModel.</param>
        /// <returns>A list of contributed resources.</returns>
        public List<ResourceContributionDto> GetContributions(int userId, ResourceContributionsRequestViewModel resourceContributionsRequestViewModel)
        {
            var param0 = new SqlParameter("@catalogueNodeId", SqlDbType.Int) { Value = resourceContributionsRequestViewModel.CatalogueNodeId };
            var param1 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@offset", SqlDbType.Int) { Value = resourceContributionsRequestViewModel.Offset };
            var param3 = new SqlParameter("@take", SqlDbType.Int) { Value = resourceContributionsRequestViewModel.Take };
            var param4 = new SqlParameter("@restrictToCurrentUser", SqlDbType.Bit) { Value = resourceContributionsRequestViewModel.RestrictToCurrentUser };

            // Stored procedure has been split into separate ones for each status type to improve SQL performance. Suspicion that the top level IF statements
            // were causing problems with SQL query execution plan efficiency. Different statusId param values would need a quite different execution plan.
            string sql;
            switch (resourceContributionsRequestViewModel.StatusId)
            {
                case 1:
                    sql = "resources.GetContributionsActionRequired";
                    break;
                case 2:
                    sql = "resources.GetContributionsDraft";
                    break;
                case 3:
                    sql = "resources.GetContributionsPublished";
                    break;
                case 4:
                    sql = "resources.GetContributionsUnpublished";
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid statusId - {resourceContributionsRequestViewModel.StatusId}. Must be in range 1 to 4.");
            }

            sql += " @catalogueNodeId, @userId, @offset, @take, @restrictToCurrentUser";

            var contributions = this.DbContext.ResourceContributionDto.FromSqlRaw(sql, param0, param1, param2, param3, param4).ToList();
            return contributions;
        }

        /// <summary>
        /// Gets resources for dashboard based on type.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The number of rows to return.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>resources.</returns>
        public (int resourceCount, List<DashboardResourceDto> resources) GetResources(string dashboardType, int pageNumber, int userId)
        {
            var param0 = new SqlParameter("@dashboardType", SqlDbType.NVarChar, 30) { Value = dashboardType };
            var param1 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber };
            var param3 = new SqlParameter("@totalRows", SqlDbType.Int) { Direction = ParameterDirection.Output };

            var dashboardResources = this.DbContext.DashboardResourceDto.FromSqlRaw("resources.GetDashboardResources @dashboardType, @userId, @pageNumber, @totalRows output", param0, param1, param2, param3).ToList();

            return (resourceCount: (int)param3.Value, resources: dashboardResources);
        }

        /// <summary>
        /// Copy the blocks from source to destination.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="sourceBlockCollectionId">The source block collection id.</param>
        /// <param name="destinationBlockCollectionId">The destination block collection id.</param>
        /// <param name="blocks">The blocks to be duplicated.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> FractionalDuplication(int userId, int sourceBlockCollectionId, int destinationBlockCollectionId, List<int> blocks)
        {
            DataTable ids = new DataTable();
            ids.Columns.Add(new DataColumn("ID", typeof(int)));
            blocks.ForEach(blockId => ids.Rows.Add(blockId));
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = sourceBlockCollectionId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = destinationBlockCollectionId };
            var param2 = new SqlParameter("@p2", SqlDbType.Structured) { Value = ids, TypeName = "resources.IDList" };
            var param5 = new SqlParameter("@p5", SqlDbType.Int) { Direction = ParameterDirection.Output };
            await this.DbContext.Database.ExecuteSqlRawAsync(
                "resources.BlockCollectionWithBlocksCreateDuplicate @p0, @p1, @p2, @p3, @p4, @p5 output", param0, param1, param2, param3, param4, param5);
            return param5.Value == DBNull.Value ? -1 : (int)param5.Value;
        }

        /// <summary>
        /// Gets Generic File content details.
        /// </summary>
        /// <param name="resourceVersionId">resourceVersionId.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public ExternalContentDetailsViewModel GetExternalContentDetails(int resourceVersionId, int userId)
        {
            try
            {
                var param0 = new SqlParameter("@resourceVersionId", SqlDbType.Int) { Value = resourceVersionId };
                var param1 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };

                var externalContentDetailsViewModel = this.DbContext.ExternalContentDetailsViewModel.FromSqlRaw("[resources].[GetExternalContentDetails] @resourceVersionId, @userId", param0, param1).AsEnumerable().FirstOrDefault();

                return externalContentDetailsViewModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
