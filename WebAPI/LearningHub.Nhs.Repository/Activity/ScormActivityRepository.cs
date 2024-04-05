// <copyright file="ScormActivityRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>
namespace LearningHub.Nhs.Repository.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dto;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The activity repository.
    /// </summary>
    public class ScormActivityRepository : GenericRepository<ScormActivity>, IScormActivityRepository
    {
        private const int IndexKeyResetValue = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormActivityRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ScormActivityRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<ScormActivity> GetByIdAsync(int id)
        {
            return this.DbContext.ScormActivity.AsNoTracking().Where(s => s.Id == id)
                   .Include(sao => sao.ScormActivityInteraction)
                   .ThenInclude(sai => sai.ScormActivityInteractionCorrectResponse).AsNoTracking()
                   .Include(sai => sai.ScormActivityInteraction)
                   .ThenInclude(sai => sai.ScormActivityInteractionObjective).AsNoTracking()
                   .Include(sao => sao.ScormActivityObjective).AsNoTracking().SingleOrDefaultAsync();
        }

        /// <summary>
        /// Create scorm activity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>Scorm Activity Id.</returns>
        public int Create(int userId, int resourceReferenceId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = userId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = resourceReferenceId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Direction = ParameterDirection.Output };

            this.DbContext.Database.ExecuteSqlRaw("[activity].[ScormActivityCreate] @p0, @p1, @p2, @p3 output", param0, param1, param2, param3);

            return (int)param3.Value;
        }

        /// <summary>
        /// Complete scorm activity.
        /// Returns the resource activity id of the completion event.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="scormActivityId">The scorm activity id.</param>
        /// <returns>Resource Activity Id.</returns>
        public int Complete(int userId, int scormActivityId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = userId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = scormActivityId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Direction = ParameterDirection.Output };

            this.DbContext.Database.ExecuteSqlRaw("[activity].[ScormActivityComplete] @p0, @p1, @p2, @p3 output", param0, param1, param2, param3);

            return (int)param3.Value;
        }

        /// <summary>
        /// Resolve scorm activity.
        /// </summary>
        /// <param name="scormActivityId">The scorm activity id.</param>
        public void Resolve(int scormActivityId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = scormActivityId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            this.DbContext.Database.ExecuteSqlRaw("[activity].[ScormActivityResolve] @p0, @p1", param0, param1);
        }

        /// <summary>
        /// Gets the previously launched incomplete scrom activity summary.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="resourceReferenceId">resourceReferenceId.</param>
        /// <returns>ScormActivitySummaryDto.</returns>
        public ScormActivitySummaryDto GetScormActivitySummary(int userId, int resourceReferenceId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = userId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = resourceReferenceId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var param3 = new SqlParameter("@p3", SqlDbType.NVarChar) { Direction = ParameterDirection.Output, Size = 20 };

            this.DbContext.Database.ExecuteSqlRaw("[activity].[ScormActivityGetSummary] @p0, @p1, @p2 output, @p3 output", param0, param1, param2, param3);
            var scormActivitySummaryDto = new ScormActivitySummaryDto
            {
                IncompleteActivityId = (int?)(param2.Value == DBNull.Value ? null : param2.Value),
                TotalTime = (string)(param3.Value == DBNull.Value ? null : param3.Value),
            };
            return scormActivitySummaryDto;
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="updatedScormActivity">The scorm activity entity.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task UpdateAsync(int userId, ScormActivity updatedScormActivity)
        {
            try
            {
                await this.UpdateScormActivityAsync(userId, updatedScormActivity);
            }

            // Re-try one more time if sql Unique constraint error
            catch (DbUpdateException e) when (e.InnerException is SqlException sqlEx && sqlEx?.Number == 2627)
            {
                this.DbContext.ChangeTracker.Clear();
                await this.UpdateScormActivityAsync(userId, updatedScormActivity);
            }
        }

        /// <summary>
        /// Clones the incomplete scrom activity session with the newly created session.
        /// </summary>
        /// <param name="incompleteScormActivityId">incompleteScromActivityId.</param>
        /// <param name="scormActivityId">scromActivityId.</param>
        /// <returns>Cloned ScormActivity.</returns>
        public ScormActivity Clone(int incompleteScormActivityId, int scormActivityId)
        {
            var incompleteScormActivity = this.GetByIdAsync(incompleteScormActivityId).Result;

            var scormActivity = this.DbContext.ScormActivity
               .Where(s => s.Id == scormActivityId).SingleOrDefault();

            foreach (var interaction in incompleteScormActivity.ScormActivityInteraction)
            {
                interaction.Id = IndexKeyResetValue;
                interaction.ScormActivityId = IndexKeyResetValue;

                foreach (var correctResponse in interaction.ScormActivityInteractionCorrectResponse)
                {
                    correctResponse.Id = IndexKeyResetValue;
                    correctResponse.ScormActivityInteractionId = IndexKeyResetValue;
                    this.SetAuditFieldsForCreate(scormActivity.CreateUserId, correctResponse);
                }

                foreach (var objective in interaction.ScormActivityInteractionObjective)
                {
                    objective.Id = IndexKeyResetValue;
                    objective.ScormActivityInteractionId = IndexKeyResetValue;
                    this.SetAuditFieldsForCreate(scormActivity.CreateUserId, objective);
                }

                this.SetAuditFieldsForCreate(scormActivity.CreateUserId, interaction);
                scormActivity.ScormActivityInteraction.Add(interaction);
            }

            foreach (var objective in incompleteScormActivity.ScormActivityObjective)
            {
                objective.ScormActivityId = IndexKeyResetValue;
                objective.Id = IndexKeyResetValue;
                this.SetAuditFieldsForCreate(scormActivity.CreateUserId, objective);
                scormActivity.ScormActivityObjective.Add(objective);
            }

            scormActivity.CmiCoreLessonStatus = incompleteScormActivity.CmiCoreLessonStatus;
            scormActivity.CmiCoreLessonLocation = incompleteScormActivity.CmiCoreLessonLocation;
            scormActivity.CmiCoreScoreRaw = incompleteScormActivity.CmiCoreScoreRaw;
            scormActivity.CmiCoreScoreMin = incompleteScormActivity.CmiCoreScoreMin;
            scormActivity.CmiCoreScoreMax = incompleteScormActivity.CmiCoreScoreMax;
            scormActivity.CmiCoreExit = incompleteScormActivity.CmiCoreExit;
            scormActivity.CmiSuspendData = incompleteScormActivity.CmiSuspendData;

            this.SetAuditFieldsForUpdate(scormActivity.CreateUserId, scormActivity);
            this.DbContext.SaveChangesAsync();

            return scormActivity;
        }

        /// <summary>
        /// Check user scorm activity data suspend data need to be cleared.
        /// </summary>
        /// <param name="lastScormActivityId">last scorm activity id.</param>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <returns>boolean.</returns>
        public async Task<bool> CheckUserScormActivitySuspendDataToBeCleared(int lastScormActivityId, int resourceVersionId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = lastScormActivityId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = resourceVersionId };
            var param2 = new SqlParameter("@p2", SqlDbType.Bit) { Direction = ParameterDirection.Output };

            string sql = "activity.UserScormActivitySuspendDataToBeCleared @p0, @p1, @p2 output";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);

            return (bool)param2.Value;
        }

        private async Task UpdateScormActivityAsync(int userId, ScormActivity updatedScormActivity)
        {
            var existingScormActivity = this.DbContext.ScormActivity.Where(s => s.Id == updatedScormActivity.Id)
                        .Include(sao => sao.ScormActivityObjective)
                        .Include(sao => sao.ScormActivityInteraction)
                        .ThenInclude(sai => sai.ScormActivityInteractionCorrectResponse)
                        .Include(sai => sai.ScormActivityInteraction)
                        .ThenInclude(sai => sai.ScormActivityInteractionObjective)
                        .SingleOrDefault();

            updatedScormActivity.ResourceActivityId = existingScormActivity.ResourceActivityId;

            this.DbContext.Entry(existingScormActivity).CurrentValues.SetValues(updatedScormActivity);
            this.SetAuditFieldsForUpdate(userId, existingScormActivity);

            foreach (var interaction in updatedScormActivity.ScormActivityInteraction)
            {
                var existingInteraction = existingScormActivity.ScormActivityInteraction
                    .Where(i => i.ScormActivityId == existingScormActivity.Id && i.SequenceNumber == interaction.SequenceNumber).OrderByDescending(i => i.CreateDate).FirstOrDefault();
                if (existingInteraction != null)
                {
                    interaction.Id = existingInteraction.Id;
                    this.DbContext.Entry(existingInteraction).CurrentValues.SetValues(interaction);
                    this.SetAuditFieldsForUpdate(userId, existingInteraction);
                    foreach (var updatedCorrectResponse in interaction.ScormActivityInteractionCorrectResponse)
                    {
                        var existingInteractionCorrectResponse = existingInteraction.ScormActivityInteractionCorrectResponse.Where(cr => cr.Index == updatedCorrectResponse.Index).SingleOrDefault();
                        if (existingInteractionCorrectResponse != null)
                        {
                            existingInteractionCorrectResponse.Pattern = updatedCorrectResponse.Pattern;
                            this.SetAuditFieldsForUpdate(userId, existingInteractionCorrectResponse);
                        }
                    }

                    foreach (var updatedObjective in interaction.ScormActivityInteractionObjective)
                    {
                        var existingInteractionObjective = existingInteraction.ScormActivityInteractionObjective.Where(o => o.Index == updatedObjective.Index).SingleOrDefault();
                        if (existingInteractionObjective != null)
                        {
                            existingInteractionObjective.ObjectiveId = updatedObjective.ObjectiveId;
                            this.SetAuditFieldsForUpdate(userId, existingInteractionObjective);
                        }
                    }
                }
                else
                {
                    this.SetAuditFieldsForCreate(userId, interaction);
                    foreach (var correctResponse in interaction.ScormActivityInteractionCorrectResponse)
                    {
                        this.SetAuditFieldsForCreate(userId, correctResponse);
                    }

                    foreach (var objective in interaction.ScormActivityInteractionObjective)
                    {
                        this.SetAuditFieldsForCreate(userId, objective);
                    }

                    existingScormActivity.ScormActivityInteraction.Add(interaction);
                }
            }

            foreach (var objective in updatedScormActivity.ScormActivityObjective)
            {
                var existingObjective = existingScormActivity.ScormActivityObjective
                .Where(i => i.ScormActivityId == existingScormActivity.Id && i.SequenceNumber == objective.SequenceNumber).OrderByDescending(o => o.CreateDate).FirstOrDefault();
                if (existingObjective != null)
                {
                    objective.Id = existingObjective.Id;
                    this.DbContext.Entry(existingObjective).CurrentValues.SetValues(objective);
                    this.SetAuditFieldsForUpdate(userId, existingObjective);
                }
                else
                {
                    this.SetAuditFieldsForCreate(userId, objective);
                    existingScormActivity.ScormActivityObjective.Add(objective);
                }
            }

            await this.DbContext.SaveChangesAsync();
        }
    }
}
