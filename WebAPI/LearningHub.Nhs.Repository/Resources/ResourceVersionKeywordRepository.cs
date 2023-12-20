// <copyright file="ResourceVersionKeywordRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource version keyword repository.
    /// </summary>
    public class ResourceVersionKeywordRepository : GenericRepository<ResourceVersionKeyword>, IResourceVersionKeywordRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionKeywordRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionKeywordRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionKeywordId">The resource version keyword id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int userId, int resourceVersionKeywordId)
        {
            var amendDate = DateTimeOffset.Now;

            var resourceVersionKeyword = this.DbContext.ResourceVersionKeyword
                    .Where(r => r.Id == resourceVersionKeywordId)
                    .SingleOrDefault();

            if (resourceVersionKeyword != null)
            {
                this.SetAuditFieldsForDelete(userId, resourceVersionKeyword);

                await this.DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="keywordId">The resource version author id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersionKeyword> GetByResourceVersionAndKeywordAsync(int resourceVersionId, int keywordId)
        {
            return await this.DbContext.ResourceVersionKeyword.AsNoTracking()
                                .FirstOrDefaultAsync(r => r.ResourceVersionId == resourceVersionId && r.Id == keywordId);
        }

        /// <summary>
        /// Checks if a specific keyword exists for the Resource Version.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> DoesResourceVersionKeywordAlreadyExistAsync(int resourceVersionId, string keyword)
        {
          return await this.DbContext.ResourceVersionKeyword.AsNoTracking()
                              .AnyAsync(r => r.ResourceVersionId == resourceVersionId && r.Keyword.ToLower() == keyword);
        }
    }
}
