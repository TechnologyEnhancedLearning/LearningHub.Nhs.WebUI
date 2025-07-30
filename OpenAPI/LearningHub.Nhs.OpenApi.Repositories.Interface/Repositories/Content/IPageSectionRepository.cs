namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Content
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Content;

    /// <summary>
    /// The IPageIdentifierRepository.
    /// </summary>
    public interface IPageSectionRepository : IGenericRepository<PageSection>
    {
        /// <summary>
        /// The CreateWithPositionAsync.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="pageSection">The pageSection<see cref="PageSection"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> CreateWithPositionAsync(int userId, PageSection pageSection);

        /// <summary>
        /// The CloneImageSectionAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task CloneImageSectionAsync(int pageSectionId, int currentUserId);

        /// <summary>
        /// The CloneVideoSectionAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task CloneVideoSectionAsync(int pageSectionId, int currentUserId);

        /// <summary>
        /// The HideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task HideAsync(int pageSectionId, int currentUserId);

        /// <summary>
        /// The UnHideAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UnHideAsync(int pageSectionId, int currentUserId);

        /// <summary>
        /// The DeleteAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int pageSectionId, int currentUserId);

        /// <summary>
        /// The ChangeOrderAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task ChangeOrderUpAsync(int pageId, int pageSectionId, int currentUserId);

        /// <summary>
        /// The ChangeOrderAsync.
        /// </summary>
        /// <param name="pageId">The pageId<see cref="int"/>.</param>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <param name="currentUserId">The currentUserId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task ChangeOrderDownAsync(int pageId, int pageSectionId, int currentUserId);

        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="pageSectionId">The pageSectionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PageSection> GetByIdAsync(int pageSectionId);
    }
}
