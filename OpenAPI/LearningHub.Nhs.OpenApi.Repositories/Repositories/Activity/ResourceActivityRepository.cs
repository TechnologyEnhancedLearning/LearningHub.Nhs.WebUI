// <copyright file="ResourceActivityRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>
namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Helpers;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
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
            return DbContext.ResourceActivity
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
            var param8 = new SqlParameter("@p8", SqlDbType.Int) { Value = TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param9 = new SqlParameter("@p9", SqlDbType.Int) { Direction = ParameterDirection.Output };

            DbContext.Database.ExecuteSqlRaw("activity.ResourceActivityCreate @p0, @p1, @p2, @p3, @p4, @p5,@p6, @p7, @p8, @p9 output", param0, param1, param2, param3, param4, param5, param6, param7, param8, param9);

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
            // TD-4047: As part of this defect bringing back the removed code which then used for the new stored procedure created as part of performance improvement.
            return DbContext.ResourceActivity
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
                           (!r.InverseLaunchResourceActivity.Any() ||
                              r.InverseLaunchResourceActivity.Any()))
               .OrderByDescending(r => r.ActivityStart);
        }

        /// <summary>
        /// Gets a list of incomplete media activities. Those that for any reason, the end of the user's activity was not recorded normally. For example - browser crash, power loss, connection loss.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<ResourceActivity>> GetIncompleteMediaActivities(int userId)
        {
            var incompleteActivities = await DbContext.ResourceActivity
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
            var activities = await GetByUserId(userId)
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
            var finishedActivity = await DbContext.ResourceActivity
               .Include(r => r.ScormActivity)
               .FirstOrDefaultAsync(x =>
                   x.UserId == userId &&
                   x.ScormActivity.Any(x => x.Id == scormActivityId) &&
                   x.LaunchResourceActivityId.HasValue &&
                   x.InverseLaunchResourceActivity.Any(y => y.ActivityStatusId == (int)ActivityStatusEnum.Completed ||
                   y.ActivityStatusId == (int)ActivityStatusEnum.Passed || y.ActivityStatusId == (int)ActivityStatusEnum.Failed));
            return finishedActivity != null;
        }

        /// <summary>
        /// Get Resource Activity by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requestModel">requestModel.</param>
        /// <param name="detailedMediaActivityRecordingStartDate">detailedMediaActivityRecordingStartDate.</param>
        /// <returns>ResourceActivity.</returns>
        public async Task<IQueryable<ResourceActivity>> GetByUserIdFromSP(int userId, Nhs.Models.MyLearning.MyLearningRequestModel requestModel, DateTimeOffset detailedMediaActivityRecordingStartDate)
        {
            (DateTimeOffset? startDate, DateTimeOffset? endDate) = this.ApplyDatesFilter(requestModel);
            (string strResourceTypes, bool resourceTypeFlag) = this.ApplyResourceTypesfilters(requestModel);
            (string strActivityStatus, bool activityStatusEnumFlag) = this.ApplyActivityStatusFilter(requestModel);
            var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
            var param1 = new SqlParameter("@searchText", SqlDbType.NVarChar) { Value = requestModel.SearchText == null ? DBNull.Value : requestModel.SearchText };
            var param2 = new SqlParameter("@activityStatuses", SqlDbType.NVarChar) { Value = activityStatusEnumFlag == false ? DBNull.Value : strActivityStatus };
            var param3 = new SqlParameter("@resourceTypes", SqlDbType.NVarChar) { Value = resourceTypeFlag == false ? DBNull.Value : strResourceTypes };
            var param4 = new SqlParameter("@activityStartDate", SqlDbType.DateTimeOffset) { Value = startDate == null ? DBNull.Value : startDate };
            var param5 = new SqlParameter("@activityEndDate", SqlDbType.DateTimeOffset) { Value = endDate == null ? DBNull.Value : endDate };
            var param6 = new SqlParameter("@mediaActivityRecordingStartDate", SqlDbType.DateTimeOffset) { Value = detailedMediaActivityRecordingStartDate };
            var param7 = new SqlParameter("@certificateEnabled", SqlDbType.Bit) { Value = requestModel.CertificateEnabled == false ? DBNull.Value : requestModel.CertificateEnabled };
            var param8 = new SqlParameter("@offSet", SqlDbType.Int) { Value = requestModel.Skip };
            var param9 = new SqlParameter("@fetchRows", SqlDbType.Int) { Value = requestModel.Take };
            var result = await DbContext.MyLearningActivity.FromSqlRaw("[activity].[GetUserLearningActivities] @userId, @searchText, @activityStatuses, @resourceTypes,@activityStartDate,@activityEndDate,@mediaActivityRecordingStartDate,@certificateEnabled,@offSet,@fetchRows ", param0, param1, param2, param3, param4, param5, param6, param7, param8, param9)
                .AsNoTracking().ToListAsync();

            // Bind result to ResourceActivity model
            this.BindNestedData(result);
            List<ResourceActivity> listOfresourceActivities = result.Select(i => new ResourceActivity()
            {
                ActivityEnd = i.ActivityEnd,
                ActivityStart = i.ActivityStart,
                ActivityStatusId = i.ActivityStatusId,
                AmendDate = i.AmendDate,
                AmendUserId = i.AmendUserId,
                AssessmentResourceActivity = i.AssessmentResourceActivity,
                CreateDate = i.CreateDate,
                CreateUserId = i.CreateUserId,
                Deleted = i.Deleted,
                DurationSeconds = i.DurationSeconds ?? 0,
                Id = i.Id,
                InverseLaunchResourceActivity = i.Resource_InverseLaunchResourceActivity,
                LaunchResourceActivityId = i.LaunchResourceActivityId,
                MajorVersion = i.MajorVersion,
                MediaResourceActivity = i.MediaResourceActivity,
                MinorVersion = i.MinorVersion,
                NodePath = i.NodePath,
                NodePathId = i.NodePathId,
                Resource = i.Resource,
                ResourceId = i.ResourceId,
                ResourceVersion = i.ResourceVersion,
                ResourceVersionId = i.ResourceVersionId,
                Score = i.Score,
                ScormActivity = i.ScormActivity,
                UserId = i.UserId,
            }).ToList();

            return listOfresourceActivities.OrderByDescending(r => r.ActivityStart).AsQueryable();
        }

        /// <summary>
        /// Get User Recent My LearningActivities.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requestModel">requestModel.</param>
        /// <returns></returns>
        public async Task<List<MyLearningActivitiesViewModel>> GetUserRecentMyLearningActivities(int userId, Nhs.Models.MyLearning.MyLearningRequestModel requestModel)
        {
            try
            {
                (string strActivityStatus, bool activityStatusEnumFlag) = this.GetActivityStatusFilter(requestModel);
                var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
                var param1 = new SqlParameter("@activityStatuses", SqlDbType.NVarChar) { Value = activityStatusEnumFlag == false ? DBNull.Value : strActivityStatus };
                var result = await DbContext.MyLearningActivitiesViewModel.FromSqlRaw("EXEC activity.GetUserRecentLearningActivities @userId, @activityStatuses", param0, param1).AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get User Recent My LearningActivities.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requestModel">requestModel.</param>
        /// <returns></returns>
        public async Task<List<MyLearningActivitiesViewModel>> GetUserLearningHistory(int userId, Nhs.Models.MyLearning.MyLearningRequestModel requestModel)
        {
                (string strActivityStatus, bool activityStatusEnumFlag) = this.GetActivityStatusFilter(requestModel);
                (string strResourceTypes, bool resourceTypeFlag) = this.ApplyResourceTypesfilters(requestModel);
                var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
                var param1 = new SqlParameter("@activityStatuses", SqlDbType.NVarChar) { Value = activityStatusEnumFlag == false ? DBNull.Value : strActivityStatus };
                var param2 = new SqlParameter("@resourceTypes", SqlDbType.NVarChar) { Value = resourceTypeFlag == false ? DBNull.Value : strResourceTypes };
                var result = await DbContext.MyLearningActivitiesViewModel.FromSqlRaw("EXEC activity.GetUsersLearningHistory @userId, @activityStatuses,@resourceTypes", param0, param1, param2).AsNoTracking().ToListAsync();
                return result;
        }

        /// <summary>
        /// Get User Recent My LearningActivities.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requestModel">requestModel.</param>
        /// <returns></returns>
        public async Task<List<MyLearningActivitiesViewModel>> GetUserLearningHistoryBasedonSearchText(int userId, Nhs.Models.MyLearning.MyLearningRequestModel requestModel)
        {
            try
            {
                (string strActivityStatus, bool activityStatusEnumFlag) = this.GetActivityStatusFilter(requestModel);
                (string strResourceTypes, bool resourceTypeFlag) = this.ApplyResourceTypesfilters(requestModel);
                var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
                var param1 = new SqlParameter("@searchText", SqlDbType.NVarChar) { Value = requestModel.SearchText };
                var param2 = new SqlParameter("@activityStatuses", SqlDbType.NVarChar) { Value = activityStatusEnumFlag == false ? DBNull.Value : strActivityStatus };
                var param3 = new SqlParameter("@resourceTypes", SqlDbType.NVarChar) { Value = resourceTypeFlag == false ? DBNull.Value : strResourceTypes };
                var result = await DbContext.MyLearningActivitiesViewModel.FromSqlRaw("EXEC activity.GetUsersLearningHistory_Search @userId,@searchText, @activityStatuses,@resourceTypes", param0,param1, param2,param3).AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get User's in progress My LearningActivities.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="pageNumber">pageNumber.</param>
        /// <returns></returns>
        public async Task<List<MyLearningActivitiesViewModel>> GetUserInprogressLearningActivities(int userId, int pageNumber)
        {
            var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
            var result = await DbContext.MyLearningActivitiesViewModel.FromSqlRaw("EXEC activity.GetUserInProgressLearningActivities @userId", param0).AsNoTracking().ToListAsync();
            return result;
        }

        /// <summary>
        /// Get Resource Activity by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requestModel">requestModel.</param>
        /// <param name="detailedMediaActivityRecordingStartDate">detailedMediaActivityRecordingStartDate.</param>
        /// <returns>ResourceActivity.</returns>
        public int GetTotalCount(int userId, Nhs.Models.MyLearning.MyLearningRequestModel requestModel, DateTimeOffset detailedMediaActivityRecordingStartDate)
        {
            (DateTimeOffset? startDate, DateTimeOffset? endDate) = this.ApplyDatesFilter(requestModel);
            (string strResourceTypes, bool resourceTypeFlag) = this.ApplyResourceTypesfilters(requestModel);
            (string strActivityStatus, bool activityStatusEnumFlag) = this.ApplyActivityStatusFilter(requestModel);
            var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
            var param1 = new SqlParameter("@searchText", SqlDbType.NVarChar) { Value = requestModel.SearchText == null ? DBNull.Value : requestModel.SearchText };
            var param2 = new SqlParameter("@activityStatuses", SqlDbType.NVarChar) { Value = activityStatusEnumFlag == false ? DBNull.Value : strActivityStatus };
            var param3 = new SqlParameter("@resourceTypes", SqlDbType.NVarChar) { Value = resourceTypeFlag == false ? DBNull.Value : strResourceTypes };
            var param4 = new SqlParameter("@activityStartDate", SqlDbType.DateTimeOffset) { Value = startDate == null ? DBNull.Value : startDate };
            var param5 = new SqlParameter("@activityEndDate", SqlDbType.DateTimeOffset) { Value = endDate == null ? DBNull.Value : endDate };
            var param6 = new SqlParameter("@mediaActivityRecordingStartDate", SqlDbType.DateTimeOffset) { Value = detailedMediaActivityRecordingStartDate };
            var param7 = new SqlParameter("@certificateEnabled", SqlDbType.Bit) { Value = requestModel.CertificateEnabled == false ? DBNull.Value : requestModel.CertificateEnabled };
            var param8 = new SqlParameter("@totalcount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            DbContext.Database.ExecuteSqlRaw("[activity].[GetUserLearningActivitiesCount] @userId, @searchText, @activityStatuses, @resourceTypes,@activityStartDate,@activityEndDate,@mediaActivityRecordingStartDate,@certificateEnabled,@totalcount output", param0, param1, param2, param3, param4, param5, param6, param7, param8);

            return (int)param8.Value;
        }

        /// <summary>
        /// Gets a list of all the user's activities for a given resource version.
        /// </summary>
        /// <param name="userId">The user id.</param>>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<ResourceActivity>> GetAllTheActivitiesFromSP(int userId, int resourceVersionId)
        {
            var activities = GetByUserIdWithResourceVersionId(userId, resourceVersionId).Result.OrderBy(r => r.AmendDate).ToList();
            return activities;
        }

        /// <summary>
        /// Gets a list of all the user's activities for a given resource version.
        /// </summary>
        /// <param name="userId">The user id.</param>>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="activityId">The resource activity id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AssessmentActivityCompletionViewModel> GetAssessmentActivityCompletionPercentage(int userId, int resourceVersionId, int activityId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = userId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = resourceVersionId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = activityId };

            var retVal = await DbContext.AssessmentActivityCompletionViewModel
                .FromSqlRaw("EXEC activity.GetAssessmentActivityCompletionPercentage @p0, @p1, @p2", param0, param1, param2)
                .AsNoTracking()
                .ToListAsync();

            AssessmentActivityCompletionViewModel assessmentResourceActivityQuestion = retVal.FirstOrDefault();
            return assessmentResourceActivityQuestion;
        }

        /// <summary>
        /// Get Resource Activity by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionId">The resource Version Id.</param>
        /// <returns>ResourceActivity.</returns>
        public async Task<IQueryable<ResourceActivity>> GetByUserIdWithResourceVersionId(int userId, int resourceVersionId)
        {
            var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
            var param1 = new SqlParameter("@resourceVersionId", SqlDbType.Int) { Value = resourceVersionId };
            var result = await DbContext.MyLearningActivity.FromSqlRaw("[activity].[GetUserLatestActivityCheck] @userId, @resourceVersionId ", param0, param1)
                .AsNoTracking().ToListAsync();

            // Bind result to ResourceActivity model
            this.BindNestedData(result);
            List<ResourceActivity> listOfresourceActivities = result.Select(i => new ResourceActivity()
            {
                ActivityEnd = i.ActivityEnd,
                ActivityStart = i.ActivityStart,
                ActivityStatusId = i.ActivityStatusId,
                AmendDate = i.AmendDate,
                AmendUserId = i.AmendUserId,
                AssessmentResourceActivity = i.AssessmentResourceActivity,
                CreateDate = i.CreateDate,
                CreateUserId = i.CreateUserId,
                Deleted = i.Deleted,
                DurationSeconds = i.DurationSeconds ?? 0,
                Id = i.Id,
                InverseLaunchResourceActivity = i.Resource_InverseLaunchResourceActivity,
                LaunchResourceActivityId = i.LaunchResourceActivityId,
                MajorVersion = i.MajorVersion,
                MediaResourceActivity = i.Resource_MediaResourceActivity,
                MinorVersion = i.MinorVersion,
                NodePath = i.NodePath,
                NodePathId = i.NodePathId,
                Resource = i.Resource,
                ResourceId = i.ResourceId,
                ResourceVersion = i.ResourceVersion,
                ResourceVersionId = i.ResourceVersionId,
                Score = i.Score,
                ScormActivity = i.ScormActivity,
                UserId = i.UserId,
            }).ToList();
            return listOfresourceActivities.OrderByDescending(r => r.ActivityStart).AsQueryable();
        }

        private void BindNestedData(List<MyLearningActivity> result)
        {
            BindNodePathNestedData(result);
            BindResourceNestedData(result);
            BindResourceVersionData(result);
            BindScormActivityNestedData(result);
            BindAssessmentResourceActivityNestedData(result);
            BindMediaResourceActivityNestedData(result);
            BindResourceVersionBlockData(result);
            BindScormResourceVersionManifestNestedData(result);
        }

        private void BindScormResourceVersionManifestNestedData(List<MyLearningActivity> result)
        {
            result.ToList().ForEach(i =>
            {
                i.ResourceVersion.ScormResourceVersion = new ScormResourceVersion();
                i.ResourceVersion.ScormResourceVersion.ScormResourceVersionManifest = new ScormResourceVersionManifest();
                i.ResourceVersion.ScormResourceVersion.ScormResourceVersionManifest.MasteryScore = i.ScormResourceVersionManifest_MasteryScore;
            });
        }

        private void BindResourceVersionBlockData(List<MyLearningActivity> result)
        {
            result.ToList().ForEach(i =>
            {
                i.ResourceVersion.AssessmentResourceVersion = new AssessmentResourceVersion();
                int assessmentType = i.ResourceVersion_AssessmentResourceVersion_AssessmentType ?? 0;
                i.ResourceVersion.AssessmentResourceVersion.AssessmentType = (AssessmentTypeEnum)assessmentType;
                i.ResourceVersion.AssessmentResourceVersion.PassMark = i.ResourceVersion_PassMark;
            });
        }

        private void BindMediaResourceActivityNestedData(List<MyLearningActivity> result)
        {
            result.ToList().ForEach(i =>
            {
                if (i.MediaResourceActivity_ResourceActivityId != null)
                {
                    MediaResourceActivity mediaResourceActivity = new MediaResourceActivity();

                    mediaResourceActivity.ResourceActivityId = i.MediaResourceActivity_ResourceActivityId ?? 0;
                    mediaResourceActivity.Id = i.MediaResourceActivity_ResourceActivityId ?? 0;
                    mediaResourceActivity.ActivityStart = i.MediaResourceActivity_ActivityStart ?? DateTimeOffset.MinValue;
                    mediaResourceActivity.SecondsPlayed = i.MediaResourceActivity_SecondsPlayed;
                    mediaResourceActivity.PercentComplete = i.MediaResourceActivity_PercentComplete;
                    i.MediaResourceActivity.Add(mediaResourceActivity);
                }
            });
        }

        private void BindAssessmentResourceActivityNestedData(List<MyLearningActivity> result)
        {
            result.ToList().ForEach(i =>
            {
                if (i.AssessmentResourceActivity_ResourceActivityId != null)
                {
                    AssessmentResourceActivity assessmentResourceActivity = new AssessmentResourceActivity();
                    assessmentResourceActivity.ResourceActivityId = i.AssessmentResourceActivity_ResourceActivityId ?? 0;
                    assessmentResourceActivity.Id = i.AssessmentResourceActivity_Id ?? 0;
                    assessmentResourceActivity.Score = i.AssessmentResourceActivity_Score;
                    assessmentResourceActivity.Reason = i.AssessmentResourceActivity_Reason;
                    i.AssessmentResourceActivity.Add(assessmentResourceActivity);
                }
            });
        }

        private void BindScormActivityNestedData(List<MyLearningActivity> result)
        {
            result.ToList().ForEach(i =>
            {
                ScormActivity scormActivity = new ScormActivity();
                scormActivity.CmiCoreExit = i.ScormActivity_CmiCoreExit;
                scormActivity.CmiCoreLessonLocation = i.ScormActivity_CmiCoreLessonLocation;
                scormActivity.CmiCoreLessonStatus = i.ScormActivity_CmiCoreLessonStatus;
                scormActivity.CmiCoreScoreMin = i.ScormActivity_CmiCoreScoreMin;
                scormActivity.CmiCoreScoreMax = i.ScormActivity_CmiCoreScoreMax;
                scormActivity.CmiCoreScoreRaw = i.ScormActivity_CmiCoreScoreRaw;
                scormActivity.CmiCoreSessionTime = i.ScormActivity_CmiCoreSessionTime;
                scormActivity.CmiSuspendData = i.ScormActivity_CmiSuspendData;
                scormActivity.DurationSeconds = i.ScormActivity_DurationSeconds ?? 0;
                i.ScormActivity.Add(scormActivity);
            });
        }

        private void BindResourceVersionData(List<MyLearningActivity> result)
        {
            result.ToList().ForEach(i =>
            {
                i.ResourceVersion.AdditionalInformation = i.ResourceVersion_AdditionalInformation;
                i.ResourceVersion.AmendDate = i.ResourceVersion_AmendDate;
                i.ResourceVersion.AmendUserId = i.ResourceVersion_AmendUserId;
                i.ResourceVersion.ArticleResourceVersion = i.ResourceVersion_ArticleResourceVersion;
                AudioResourceVersion audioResourceVersion = new AudioResourceVersion();
                audioResourceVersion.DurationInMilliseconds = i.AudioResourceVersion_DurationInMilliseconds;
                i.ResourceVersion.AudioResourceVersion = audioResourceVersion;
                i.ResourceVersion.CaseResourceVersion = i.ResourceVersion_CaseResourceVersion;
                i.ResourceVersion.CertificateEnabled = i.ResourceVersion_CertificateEnabled;
                i.ResourceVersion.Cost = i.ResourceVersion_Cost;
                i.ResourceVersion.Deleted = i.ResourceVersion_Deleted;
                i.ResourceVersion.Description = i.ResourceVersion_Description;
                i.ResourceVersion.EmbeddedResourceVersion = i.ResourceVersion_EmbeddedResourceVersion;
                i.ResourceVersion.EquipmentResourceVersion = i.ResourceVersion_EquipmentResourceVersion;
                i.ResourceVersion.FileChunkDetail = i.ResourceVersion_FileChunkDetail;
                i.ResourceVersion.GenericFileResourceVersion = i.ResourceVersion_GenericFileResourceVersion;
                i.ResourceVersion.HasCost = i.ResourceVersion_HasCost;
                i.ResourceVersion.HtmlResourceVersion = i.ResourceVersion_HtmlResourceVersion;
                i.ResourceVersion.ImageResourceVersion = i.ResourceVersion_ImageResourceVersion;
                i.ResourceVersion.MajorVersion = i.ResourceVersion_MajorVersion;
                i.ResourceVersion.MinorVersion = i.ResourceVersion_MinorVersion;
                i.ResourceVersion.Publication = i.ResourceVersion_Publication;
                i.ResourceVersion.PublicationId = i.ResourceVersion_PublicationId;
                i.ResourceVersion.Resource = i.ResourceVersion_Resource;
                i.ResourceVersion.ResourceAccessibilityEnum = i.ResourceVersion_ResourceAccessibilityEnum;
                i.ResourceVersion.ResourceId = i.ResourceVersion_ResourceId;
                i.ResourceVersion.ResourceLicence = i.ResourceVersion_ResourceLicence;
                i.ResourceVersion.ResourceLicenceId = i.ResourceVersion_ResourceLicenceId;
                i.ResourceVersion.ResourceVersionAuthor = i.ResourceVersion_ResourceVersionAuthor;
                i.ResourceVersion.ResourceVersionEvent = i.ResourceVersion_ResourceVersionEvent;
                i.ResourceVersion.ResourceVersionFlag = i.ResourceVersion_ResourceVersionFlag;
                i.ResourceVersion.ResourceVersionKeyword = i.ResourceVersion_ResourceVersionKeyword;
                i.ResourceVersion.ResourceVersionProvider = i.ResourceVersion_ResourceVersionProvider;
                i.ResourceVersion.ResourceVersionRatingSummary = i.ResourceVersion_ResourceVersionRatingSummary;
                i.ResourceVersion.ResourceVersionRatings = i.ResourceVersion_ResourceVersionRatings;
                int versionStatusid = i.ResourceVersion_VersionStatusId ?? 0;
                i.ResourceVersion.VersionStatusEnum = (VersionStatusEnum)versionStatusid;
                i.ResourceVersion.ResourceVersionValidationResult = i.ResourceVersion_ResourceVersionValidationResult;
                i.ResourceVersion.ResourceWhereCurrent = i.ResourceVersion_ResourceWhereCurrent;
                i.ResourceVersion.ReviewDate = i.ResourceVersion_ReviewDate;
                i.ResourceVersion.ScormResourceVersion = i.ResourceVersion_ScormResourceVersion;
                i.ResourceVersion.SensitiveContent = i.ResourceVersion_SensitiveContent;
                i.ResourceVersion.Title = i.ResourceVersion_Title;
                i.ResourceVersion.VersionStatusEnum = i.ResourceVersion_VersionStatusEnum;
                VideoResourceVersion vedioResourceVersion = new VideoResourceVersion();
                vedioResourceVersion.DurationInMilliseconds = i.VideoResourcVersion_DurationInMilliseconds;
                i.ResourceVersion.VideoResourceVersion = vedioResourceVersion;
                i.ResourceVersion.WebLinkResourceVersion = i.ResourceVersion_WebLinkResourceVersion;
            });
        }

        private void BindResourceNestedData(List<MyLearningActivity> result)
        {
            result.ToList().ForEach(i =>
            {
                i.Resource.AmendDate = i.Resource_AmendDate;
                i.Resource.AmendUserId = i.Resource_AmendUserId;
                i.Resource.CreateUserId = i.Resource_CreateUserId;
                i.Resource.CreateDate = i.Resource_CreateDate;
                i.Resource.CurrentResourceVersionId = i.Resource_CurrentResourceVersionId;
                i.Resource.Deleted = i.Resource_Deleted;
                int resourceTypeId = i.Resource_ResourceTypeId;
                i.Resource.ResourceTypeEnum = (ResourceTypeEnum)resourceTypeId;
                var resourceReferences = result.Where(x => x.Id == i.Id && x.ResourceVersionId == i.ResourceVersionId).ToList();
                List<ResourceReference> resourceReferenceList = new List<ResourceReference>();
                foreach (var b in resourceReferences)
                {
                    ResourceReference resourceReference = new ResourceReference();
                    resourceReference.OriginalResourceReferenceId = b.ResourceReference_OriginalResourceReferenceId;
                    resourceReference.NodePathId = b.ResourceReference_NodePathId;
                    resourceReference.ResourceId = b.ResourceReference_ResourceId;
                    resourceReferenceList.Add(resourceReference);
                }

                i.Resource.ResourceReference = resourceReferenceList;
            });
        }

        private void BindNodePathNestedData(List<MyLearningActivity> result)
        {
            result.ToList().ForEach(i =>
            {
                i.NodePath.AmendDate = i.NodePath_AmendDate;
                i.NodePath.AmendUserId = i.NodePath_AmendUserId;
                i.NodePath.CatalogueNodeId = i.NodePath_CatalogueNode;
                i.NodePath.CreateDate = i.NodePath_CreateDate;
                i.NodePath.CreateUserId = i.NodePath_CreateUserId;
                i.NodePath.Deleted = i.NodePath_Deleted;
                i.NodePath.Id = i.NodePath_NodeId;
                i.NodePath.IsActive = i.NodePath_IsActive;
                i.NodePath.NodeId = i.NodePath_NodeId;
                i.NodePath.NodePathString = i.NodePath_NodePathString;
            });
        }

        private (DateTimeOffset? startDate, DateTimeOffset? endDate) ApplyDatesFilter(MyLearningRequestModel requestModel)
        {
            DateTimeOffset? startDate = null;
            DateTimeOffset? endDate = null;
            if (!string.IsNullOrEmpty(requestModel.TimePeriod))
            {
                var now = DateTime.Now.Date;

                if (requestModel.TimePeriod == "dateRange")
                {
                    if (requestModel.StartDate.HasValue || requestModel.EndDate.HasValue)
                    {
                        if (requestModel.StartDate.HasValue)
                        {
                            startDate = requestModel.StartDate;
                        }

                        if (requestModel.EndDate.HasValue)
                        {
                            endDate = requestModel.EndDate.Value.AddDays(1);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("If RequestModel.TimePeriod is set to 'dateRange', the RequestModel.StartDate and/or EndDate must also be specified.");
                    }
                }
                else if (requestModel.TimePeriod == "thisWeek")
                {
                    // Definition of this week is anything from the Monday prior, or today if today is Monday.
                    var firstDateOfWeek = now.FirstDateInWeek(DayOfWeek.Monday);
                    startDate = firstDateOfWeek;
                }
                else if (requestModel.TimePeriod == "thisMonth")
                {
                    // Definition of this month is anything from the 1st of the current month.
                    startDate = new DateTime(now.Year, now.Month, 1);
                }
                else if (requestModel.TimePeriod == "last12Months")
                {
                    // Definition of the last 12 months is anything from the same date 12 months ago. e.g. if today is 7th Oct 2020, then return anything from 7th Oct 2019.
                    startDate = now.AddMonths(-12);
                }
            }

            return (startDate, endDate);
        }

        /// <summary>
        /// ApplyResourceTypesfilters.
        /// </summary>
        /// <param name="requestModel">The requestModel.</param>
        /// <returns></returns>
        public (string strResourceTypes, bool resourceTypeFlag) ApplyResourceTypesfilters(MyLearningRequestModel requestModel)
        {
            var listOfResourceTypeEnum = Enum.GetValues(typeof(ResourceTypeEnum)).Cast<int>().ToList();
            listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Undefined);
            listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Embedded);
            listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Equipment);
            bool resourceTypeFlag = false;

            // Resource Type filter.
            if (requestModel.Article || requestModel.Audio || requestModel.Elearning || requestModel.Html || requestModel.File || requestModel.Image || requestModel.Video || requestModel.Weblink || requestModel.Assessment || requestModel.Case)
            {
                if (!requestModel.Article)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Article);
                }

                if (!requestModel.Audio)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Audio);
                }

                if (!requestModel.Elearning)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Scorm);
                }

                if (!requestModel.Html)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Html);
                }

                if (!requestModel.File)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.GenericFile);
                }

                if (!requestModel.Image)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Image);
                }

                if (!requestModel.Video)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Video);
                }

                if (!requestModel.Weblink)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.WebLink);
                }

                if (!requestModel.Assessment)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Assessment);
                }

                if (!requestModel.Case)
                {
                    listOfResourceTypeEnum.Remove((int)ResourceTypeEnum.Case);
                }

                resourceTypeFlag = true;
            }

            return (string.Join(",", listOfResourceTypeEnum), resourceTypeFlag);
        }

        private (string strActivityStatus, bool activityStatusEnumFlag) ApplyActivityStatusFilter(MyLearningRequestModel requestModel)
        {
            var listOfactivityStatusesEnum = Enum.GetValues(typeof(ActivityStatusEnum)).Cast<int>().ToList();
            var activityStatusEnumFlag = false;
            if (requestModel.Complete || requestModel.Incomplete || requestModel.Passed || requestModel.Failed || requestModel.Downloaded || requestModel.Launched || requestModel.Viewed)
            {
                activityStatusEnumFlag = true;
                if (!requestModel.Complete)
                {
                    listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Completed);
                }

                if (!requestModel.Passed)
                {
                    listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Passed);
                }

                if (!requestModel.Failed)
                {
                    listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Failed);
                }

                if (!requestModel.Downloaded)
                {
                    listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Downloaded);
                }

                if (!requestModel.Incomplete)
                {
                    listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Incomplete);
                    listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.InProgress);
                }

                if (!requestModel.Launched)
                {
                    listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Launched);
                }

                if (!requestModel.Viewed)
                {
                    listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Viewed);
                }
            }

            return (string.Join(",", listOfactivityStatusesEnum), activityStatusEnumFlag);
        }

        /// <summary>
        /// GetActivityStatusFilter.
        /// </summary>
        /// <param name="requestModel">The requestModel.</param>
        /// <returns></returns>
        public (string strActivityStatus, bool activityStatusEnumFlag) GetActivityStatusFilter(MyLearningRequestModel requestModel)
        {
            var listOfactivityStatusesEnum = Enum.GetValues(typeof(ActivityStatusEnum)).Cast<int>().ToList();
            var activityStatusEnumFlag = false;
            if(requestModel.Complete && !requestModel.Incomplete)
            {
                activityStatusEnumFlag = true;
                listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Incomplete);
                listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.InProgress);
                listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Failed);

            }
            else if(!requestModel.Complete && requestModel.Incomplete)
            {
                activityStatusEnumFlag = true;
                listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Completed);
                listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Passed);
                listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Launched);
                listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Downloaded);
                listOfactivityStatusesEnum.Remove((int)ActivityStatusEnum.Viewed);
            }         
            return (string.Join(",", listOfactivityStatusesEnum), activityStatusEnumFlag);
        }
    }
}
