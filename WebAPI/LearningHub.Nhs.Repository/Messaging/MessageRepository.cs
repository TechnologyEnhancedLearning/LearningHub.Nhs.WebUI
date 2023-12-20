// <copyright file="MessageRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Messaging;
    using LearningHub.Nhs.Models.Messaging;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Messaging;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The MessageRepository class.
    /// </summary>
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public MessageRepository(LearningHubDbContext context, ITimezoneOffsetManager tzOffsetManager)
            : base(context, tzOffsetManager)
        {
        }

        /// <summary>
        /// The CreateEmailAsync.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="recipientUserId">The recipient user id.</param>
        /// <returns>The task.</returns>
        public async Task CreateEmailAsync(int userId, string subject, string body, int recipientUserId)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.NVarChar) { Value = subject };
                var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = body };
                var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = recipientUserId };
                var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
                var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

                await this.DbContext.Database.ExecuteSqlRawAsync("messaging.CreateEmailForUser @p0, @p1, @p2, @p3, @p4", param0, param1, param2, param3, param4);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// The CreateEmailAsync.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="recipientEmailAddress">The recipientEmailAddress.</param>
        /// <returns>The task.</returns>
        public async Task CreateEmailAsync(int userId, string subject, string body, string recipientEmailAddress)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.NVarChar) { Value = subject };
                var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = body };
                var param2 = new SqlParameter("@p2", SqlDbType.NVarChar) { Value = recipientEmailAddress };
                var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
                var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

                await this.DbContext.Database.ExecuteSqlRawAsync("messaging.CreateEmailForEmailAddress @p0, @p1, @p2, @p3, @p4", param0, param1, param2, param3, param4);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// The CreateNotificationForUserAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="recipientUserId">The recipientUserId.</param>
        /// <param name="notificationStartDate">The notificationStartDate.</param>
        /// <param name="notificationEndDate">The notificationEndDate.</param>
        /// <param name="notificationPriority">The notificationPriority.</param>
        /// <param name="notificationType">The notificationType.</param>
        /// <returns>The task.</returns>
        public async Task CreateNotificationForUserAsync(
            int userId,
            string subject,
            string body,
            int recipientUserId,
            DateTimeOffset notificationStartDate,
            DateTimeOffset notificationEndDate,
            int notificationPriority,
            int notificationType)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.NVarChar) { Value = subject };
                var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = body };
                var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = recipientUserId };
                var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
                var param4 = new SqlParameter("@p4", SqlDbType.DateTimeOffset) { Value = notificationStartDate };
                var param5 = new SqlParameter("@p5", SqlDbType.DateTimeOffset) { Value = notificationEndDate };
                var param6 = new SqlParameter("@p6", SqlDbType.Int) { Value = notificationPriority };
                var param7 = new SqlParameter("@p7", SqlDbType.Int) { Value = notificationType };
                var param8 = new SqlParameter("@p8", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

                await this.DbContext.Database.ExecuteSqlRawAsync("messaging.CreateNotificationForUser @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8", param0, param1, param2, param3, param4, param5, param6, param7, param8);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of all messages which have a message send which hasn't been sent.
        /// </summary>
        /// <returns>The messages.</returns>
        public IQueryable<FullMessageDto> GetPendingMessages()
        {
            // MarkAsSent = true
            var param0 = new SqlParameter("@p0", SqlDbType.Bit) { Value = 1 };

            return this.DbContext.FullMessageDto.FromSqlRaw("messaging.GetPendingMessages @p0", param0);
        }

        /// <summary>
        /// Marks a message send as having been successful.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="messageSends">The messageSends.</param>
        /// <returns>The task.</returns>
        public async Task MessageSendSuccess(int userId, List<int> messageSends)
        {
            try
            {
                var ids = string.Join(',', messageSends);
                var param0 = new SqlParameter("@p0", SqlDbType.NVarChar) { Value = ids };
                var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };

                await this.DbContext.Database.ExecuteSqlRawAsync("messaging.MessageSendSuccess @p0, @p1", param0, param1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Either marks a message as failed, or queues it for a retry.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="messageSends">The messageSends.</param>
        /// <returns>The task.</returns>
        public async Task MessageSendFailure(int userId, List<int> messageSends)
        {
            try
            {
                var ids = string.Join(',', messageSends);
                var param0 = new SqlParameter("@p0", SqlDbType.NVarChar) { Value = ids };
                var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };

                await this.DbContext.Database.ExecuteSqlRawAsync("messaging.MessageSendFailed @p0, @p1", param0, param1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
