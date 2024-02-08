namespace LearningHub.Nhs.Repository
{
    using System;
    using System.Data;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The event log repository.
    /// </summary>
    public class EventLogRepository : IEventLogRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        public EventLogRepository(LearningHubDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        protected LearningHubDbContext DbContext { get; }

        /// <summary>
        /// Create  event.
        /// </summary>
        /// <param name="eventLog">eventLog.</param>
        /// <param name="eventType">eventType.</param>
        /// <param name="hierarchyEditId">hierarchyEditId.</param>
        /// <param name="nodeId">nodeId.</param>
        /// <param name="resourceVersionId">resourceVersionId.</param>
        /// <param name="details">details.</param>
        /// <param name="userId">user id.</param>
        public void CreateEvent(EventLogEnum eventLog, EventLogEventTypeEnum eventType, int? hierarchyEditId, int? nodeId, int? resourceVersionId, string details, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = (int)eventLog };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = (int)eventType };
            SqlParameter param2;
            if (hierarchyEditId.HasValue)
            {
                param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = hierarchyEditId.Value };
            }
            else
            {
                param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = DBNull.Value };
            }

            SqlParameter param3;
            if (nodeId.HasValue)
            {
                param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = nodeId.Value };
            }
            else
            {
                param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = DBNull.Value };
            }

            SqlParameter param4;
            if (resourceVersionId.HasValue)
            {
                param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = resourceVersionId.Value };
            }
            else
            {
                param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = DBNull.Value };
            }

            var param5 = new SqlParameter("@p5", SqlDbType.NVarChar) { Value = details };
            var param6 = new SqlParameter("@p6", SqlDbType.Int) { Value = userId };

            this.DbContext.Database.ExecuteSqlRaw("hub.EventLogEventCreate @p0, @p1, @p2, @p3, @p4, @p5, @p6", param0, param1, param2, param3, param4, param5, param6);
        }
    }
}
