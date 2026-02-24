namespace LearningHub.Nhs.MessageQueueing.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using LearningHub.Nhs.Models.Entities.GovNotifyMessaging;

    /// <summary>
    /// DataTable Builder.
    /// </summary>
    public static class DataTableBuilder
    {
        /// <summary>
        /// ToQueueRequestDataTable.
        /// </summary>
        /// <param name="requests">The requests list.</param>
        /// <returns>The table.</returns>
        public static DataTable ToQueueRequestDataTable(IEnumerable<QueueRequests> requests)
        {
            var table = new DataTable();
            table.Columns.Add("Recipient", typeof(string));
            table.Columns.Add("TemplateId", typeof(string));
            table.Columns.Add("Personalisation", typeof(string));
            table.Columns.Add("DeliverAfter", typeof(DateTimeOffset));

            foreach (var req in requests)
            {
                table.Rows.Add(req.Recipient, req.TemplateId, req.Personalisation, req.DeliverAfter);
            }

            return table;
        }
    }
}
