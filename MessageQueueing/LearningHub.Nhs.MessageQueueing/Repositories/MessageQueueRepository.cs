namespace LearningHub.Nhs.MessageQueueing.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using LearningHub.Nhs.MessageQueueing.EntityFramework;
    using LearningHub.Nhs.MessageQueueing.Helpers;
    using LearningHub.Nhs.Models.Entities.GovNotifyMessaging;
    using LearningHub.Nhs.Models.GovNotifyMessaging;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The MessageQueueRepository.
    /// </summary>
    public class MessageQueueRepository : IMessageQueueRepository
    {
        private readonly MessageQueueDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueueRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The context.</param>
        public MessageQueueRepository(MessageQueueDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// The QueueMessagesAsync.
        /// </summary>
        /// <param name="requests">The queue requests.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task QueueMessagesAsync(IEnumerable<QueueRequests> requests)
        {
            var dataTable = DataTableBuilder.ToQueueRequestDataTable(requests);
            var param0 = new SqlParameter("@p0", SqlDbType.Structured) { Value = dataTable, TypeName = "dbo.QueueRequestTableType" };
            await this.dbContext.Database.ExecuteSqlRawAsync("dbo.CreateQueueRequests @p0", param0);
        }

        /// <summary>
        /// The GetPendingEmailsAsync.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IEnumerable<PendingMessageRequests>> GetPendingEmailsAsync()
        {
            var result = await this.dbContext.PendingMessageRequests.FromSqlRaw("[dbo].[GetQueueRequests]")
              .AsNoTracking().ToListAsync();
            return result;
        }

        /// <summary>
        /// The Update Email request as success.
        /// </summary>
        /// <param name="response">Th response.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task MessageDeliverySuccess(GovNotifyResponse response)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = response.Id };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = response.NotificationId };
            await this.dbContext.Database.ExecuteSqlRawAsync("dbo.MessageDeliverySuccess @p0, @p1", param0, param1);
        }

        /// <summary>
        /// The Update Email request as failed.
        /// </summary>
        /// <param name="response">Th response.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task MessageDeliveryFailed(GovNotifyResponse response)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = response.Id };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = response.ErrorMessage };
            await this.dbContext.Database.ExecuteSqlRawAsync("dbo.MessageDeliveryFailed @p0, @p1", param0, param1);
        }

        /// <summary>
        /// The Save Single Emails.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task SaveSingleEmailTransactions(SingleEmailRequest request)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.NVarChar) { Value = request.Recipient };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = request.TemplateId };
            var param2 = new SqlParameter("@p2", SqlDbType.NVarChar) { Value = request.Personalisation == null ? DBNull.Value : request.Personalisation };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = request.Status };
            var param4 = new SqlParameter("@p4", SqlDbType.NVarChar) { Value = request.ErrorMessage == null ? DBNull.Value : request.ErrorMessage };
            await this.dbContext.Database.ExecuteSqlRawAsync("dbo.SaveSingleEmailTransactions @p0, @p1, @p2, @p3, @p4", param0, param1, param2, param3, param4);
        }
    }
}
