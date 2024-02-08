namespace LearningHub.Nhs.Repository
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The BookmarkRepository.
    /// </summary>
    public class BookmarkRepository : GenericRepository<UserBookmark>, IBookmarkRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public BookmarkRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetById.
        /// </summary>
        /// <param name="bookmarkId">The bookmarkId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserBookmark}"/>.</returns>
        public async Task<UserBookmark> GetById(int bookmarkId)
        {
            return await this.DbContext.UserBookmark.SingleAsync(ub => ub.Id == bookmarkId);
        }

        /// <summary>
        /// DeleteFolder.
        /// </summary>
        /// <param name="bookmarkId">bookmarkId.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DeleteFolder(int bookmarkId, int userId)
        {
            var bookmarks = this.DbContext.UserBookmark.Where(ub => ub.Id == bookmarkId || ub.ParentId == bookmarkId);

            foreach (var bookmark in bookmarks)
            {
                bookmark.ParentId = null;
                bookmark.Deleted = true;
                this.SetAuditFieldsForUpdate(userId, bookmark);
            }

            await this.DbContext.SaveChangesAsync();
        }
    }
}
