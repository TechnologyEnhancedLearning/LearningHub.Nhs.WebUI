namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Models;

    /// <summary>
    /// Interface for Azure Search administration operations.
    /// </summary>
    public interface IAzureSearchAdminService
    {
        /// <summary>
        /// Gets the status of all indexers.
        /// </summary>
        /// <returns>A list of indexer statuses.</returns>
        Task<List<IndexerStatusViewModel>> GetIndexersStatusAsync();

        /// <summary>
        /// Gets the status of all indexes.
        /// </summary>
        /// <returns>A list of index statuses.</returns>
        Task<List<IndexStatusViewModel>> GetIndexesStatusAsync();

        /// <summary>
        /// Runs an indexer manually.
        /// </summary>
        /// <param name="indexerName">The name of the indexer to run.</param>
        /// <returns>True if successful, false otherwise.</returns>
        Task<bool> RunIndexerAsync(string indexerName);

        /// <summary>
        /// Resets an indexer (clears state and allows full reindex).
        /// </summary>
        /// <param name="indexerName">The name of the indexer to reset.</param>
        /// <returns>True if successful, false otherwise.</returns>
        Task<bool> ResetIndexerAsync(string indexerName);
    }
}