// <copyright file="ResourceActivityRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>
namespace LearningHub.Nhs.Repository.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The activity repository.
    /// </summary>
    public class ResourceActivityRepository : GenericRepository<ResourceActivity>, IResourceActivityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceActivityRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceActivityRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<ResourceActivity> GetByIdAsync(int id)
        {
            return this.DbContext.ResourceActivity
                .Include(n => n.ResourceVersion)
                .Include(n => n.NodePath)
                .Include(n => n.LaunchResourceActivity)
                .Where(n => n.Id == id).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Create activity record against a ResourceVersion.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="launchResourceActivityId">The launch resource activity id.</param>
        /// <param name="activityStatusEnum">The activity Status Enum.</param>
        /// <param name="activityStart">The activity Start.</param>
        /// <param name="activityEnd">The activity End.</param>
        /// <returns>Activity Id.</returns>
        public int CreateActivity(
           int userId,
           int resourceVersionId,
           int nodePathId,
           int? launchResourceActivityId,
           ActivityStatusEnum activityStatusEnum,
           DateTimeOffset? activityStart,
           DateTimeOffset? activityEnd)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = userId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = resourceVersionId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = nodePathId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = (int)activityStatusEnum };
            var param4 = new SqlParameter("@p4", SqlDbType.DateTimeOffset) { Value = activityStart ?? (object)DBNull.Value };
            var param5 = new SqlParameter("@p5", SqlDbType.DateTimeOffset) { Value = activityEnd ?? (object)DBNull.Value };
            var param6 = new SqlParameter("@p6", SqlDbType.Decimal) { Value = DBNull.Value };
            var param7 = new SqlParameter("@p7", SqlDbType.Int) { Value = launchResourceActivityId ?? (object)DBNull.Value };
            var param8 = new SqlParameter("@p8", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param9 = new SqlParameter("@p9", SqlDbType.Int) { Direction = ParameterDirection.Output };

            this.DbContext.Database.ExecuteSqlRaw("activity.ResourceActivityCreate @p0, @p1, @p2, @p3, @p4, @p5,@p6, @p7, @p8, @p9 output", param0, param1, param2, param3, param4, param5, param6, param7, param8, param9);

            return (int)param9.Value;
        }

        /// <summary>
        /// Get Resource Activity by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>ResourceActivity.</returns>
        public IQueryable<ResourceActivity> GetByUserId(int userId)
        {
            // For video/audio activities, only include them if they are finished - i.e. there is a second ResourceActivity record with status = 3 - completed.
            //
            // For assessment activities, only include the original activities that were created when starting the assessment. The created end record is only for consistency.
            // It's easier to get the real assessment resource activity from the original resource activity, so only fetch that one.
             return this.DbContext.ResourceActivity
                .Include(r => r.Resource)
                .ThenInclude(r => r.ResourceReference)
                .Include(r => r.MediaResourceActivity)
                .Include(r => r.ScormActivity)
                .Include(r => r.ResourceVersion.VideoResourceVersion)
                .Include(r => r.ResourceVersion.AudioResourceVersion)
                .Include(r => r.ResourceVersion.ScormResourceVersion)
                .Include(r => r.ResourceVersion.ScormResourceVersion.ScormResourceVersionManifest)
                .Include(r => r.ResourceVersion.AssessmentResourceVersion.AssessmentContent.Blocks)
                .Include(r => r.AssessmentResourceActivity)
                .ThenInclude(a => a.AssessmentResourceActivityInteractions)
                .Include(r => r.NodePath)
                .AsNoTracking()
                .Where(r =>
                      r.UserId == userId && r.ScormActivity.First().CmiCoreLessonStatus != (int)ActivityStatusEnum.Completed &&
                     ((r.Resource.ResourceTypeEnum != ResourceTypeEnum.Video && r.Resource.ResourceTypeEnum != ResourceTypeEnum.Audio && !r.InverseLaunchResourceActivity.Any()) ||
                        r.InverseLaunchResourceActivity.Any(y => y.ActivityStatusId == (int)ActivityStatusEnum.Completed)) &&
                     (r.Resource.ResourceTypeEnum != ResourceTypeEnum.Assessment || r.ActivityStatusId == (int)ActivityStatusEnum.Launched))
                .OrderByDescending(r => r.ActivityStart);
        }

        /// <summary>
        /// Gets a list of incomplete media activities. Those that for any reason, the end of the user's activity was not recorded normally. For example - browser crash, power loss, connection loss.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<ResourceActivity>> GetIncompleteMediaActivities(int userId)
        {
            var incompleteActivities = await this.DbContext.ResourceActivity
                .Include(x => x.MediaResourceActivity)
                .ThenInclude(x => x.MediaResourceActivityInteraction)
                .Where(x =>
                    x.MediaResourceActivity != null &&
                    x.UserId == userId &&
                    !x.LaunchResourceActivityId.HasValue &&
                    !x.InverseLaunchResourceActivity.Any(y => y.ActivityStatusId == (int)ActivityStatusEnum.Completed))
                .OrderBy(x => x.ActivityStart)
                .ToListAsync();

            return incompleteActivities;
        }

        /// <summary>
        /// Gets a list of all the user's activities for a given resource version.
        /// </summary>
        /// <param name="userId">The user id.</param>>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<ResourceActivity>> GetAllTheActivitiesFor(int userId, int resourceVersionId)
        {
            var activities = await this.GetByUserId(userId)
                .Where(r => r.ResourceVersionId == resourceVersionId)
                .OrderBy(r => r.AmendDate)
                .ToListAsync();

            return activities;
        }

        /// <summary>
        /// Check if scorm activity has been completed.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="scormActivityId">The scormActivityId id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> IsScormActivityFinished(int userId, int scormActivityId)
        {
            var finishedActivity = await this.DbContext.ResourceActivity
               .Include(r => r.ScormActivity)
               .FirstOrDefaultAsync(x =>
                   x.UserId == userId &&
                   x.ScormActivity.Any(x => x.Id == scormActivityId) &&
                   x.LaunchResourceActivityId.HasValue &&
                   x.InverseLaunchResourceActivity.Any(y => y.ActivityStatusId == (int)ActivityStatusEnum.Completed ||
                   y.ActivityStatusId == (int)ActivityStatusEnum.Passed || y.ActivityStatusId == (int)ActivityStatusEnum.Failed));
            return finishedActivity != null;
        }
    }
}
