namespace LearningHub.Nhs.AdminUI.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// View model for Azure Search Admin page.
    /// </summary>
    public class AzureSearchAdminViewModel
    {
        /// <summary>
        /// Gets or sets the list of indexer statuses.
        /// </summary>
        public List<IndexerStatusViewModel> Indexers { get; set; } = new List<IndexerStatusViewModel>();

        /// <summary>
        /// Gets or sets the list of index statuses.
        /// </summary>
        public List<IndexStatusViewModel> Indexes { get; set; } = new List<IndexStatusViewModel>();

        /// <summary>
        /// Gets or sets a message to display to the user.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message is an error.
        /// </summary>
        public bool IsError { get; set; }
    }
}