namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Hierarchy
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The CatalogueAccessRequestRepository class.
    /// </summary>
    public class CatalogueAccessRequestRepository : GenericRepository<CatalogueAccessRequest>, ICatalogueAccessRequestRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueAccessRequestRepository"/> class.
        /// </summary>
        /// <param name="context">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public CatalogueAccessRequestRepository(LearningHubDbContext context, ITimezoneOffsetManager tzOffsetManager)
            : base(context, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetByUserIdAndCatalogueId.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogueAccessRequest.</returns>
        public CatalogueAccessRequest GetByUserIdAndCatalogueId(int catalogueNodeId, int userId)
        {
            return GetAll().OrderByDescending(x => x.CreateDate).FirstOrDefault(x => x.UserId == userId && x.CatalogueNodeId == catalogueNodeId);
        }

        /// <summary>
        /// The GetAllByUserIdAndCatalogueId.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogueAccessRequest.</returns>
        public IQueryable<CatalogueAccessRequest> GetAllByUserIdAndCatalogueId(int catalogueNodeId, int userId)
        {
            return GetAll().Where(x => x.UserId == userId && x.CatalogueNodeId == catalogueNodeId).OrderByDescending(x => x.CreateDate);
        }

        /// <summary>
        /// The CreateCatalogueAccessRequestAsync.
        /// </summary>
        /// <param name="currentUserId">The currentUserId.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="message">The message.</param>
        /// <param name="roleId">The roleId.</param>
        /// <param name="catalogueManageAccessUrl">The catalogueManageAccessUrl.</param>
        /// <param name="accessType">The accessType.</param>
        /// <returns>The task.</returns>
        public async Task CreateCatalogueAccessRequestAsync(
            int currentUserId,
            string reference,
            string message,
            int roleId,
            string catalogueManageAccessUrl,
            string accessType)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = currentUserId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = reference };
            var param2 = new SqlParameter("@p2", SqlDbType.NVarChar) { Value = message };
            var param3 = new SqlParameter("@p3", SqlDbType.NVarChar) { Value = catalogueManageAccessUrl };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param5 = new SqlParameter("@p5", SqlDbType.NVarChar) { Value = accessType };
            var param6 = new SqlParameter("@p6", SqlDbType.Int) { Value = roleId };
            await DbContext.Database.ExecuteSqlRawAsync("exec [hierarchy].[CatalogueAccessRequestCreate] @p0, @p1, @p2, @p3, @p4, @p5, @p6", param0, param1, param2, param3, param4, param5, param6);
        }
    }
}
