// <copyright file="ScormResourceVersionRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The Scorm resource version repository.
    /// </summary>
    public class ScormResourceVersionRepository : GenericRepository<ScormResourceVersion>, IScormResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScormResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ScormResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ScormResourceVersion> GetByIdAsync(int id)
        {
            return await this.DbContext.ScormResourceVersion.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource versionid.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ScormResourceVersion> GetByResourceVersionIdAsync(int resourceVersionId, bool includeDeleted = false)
        {
            return await this.DbContext.ScormResourceVersion
            .Include(r => r.File)
            .Include(r => r.ScormResourceVersionManifest)
            .AsNoTracking().FirstOrDefaultAsync(r => r.ResourceVersionId == resourceVersionId && (includeDeleted || !r.Deleted));
        }

        /// <summary>
        /// Gets Scorm content details.
        /// </summary>
        /// <param name="resourceVersionId">resourceVersionId.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public ScormContentDetailsViewModel GetScormContentDetails(int resourceVersionId, int userId)
        {
            var param0 = new SqlParameter("@resourceVersionId", SqlDbType.Int) { Value = resourceVersionId };
            var param1 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };

            var scormContentDetailsViewModel = this.DbContext.ScormContentDetailsViewModel.FromSqlRaw("[resources].[GetScormContentDetails] @resourceVersionId, @userId", param0, param1).AsEnumerable().FirstOrDefault();

            return scormContentDetailsViewModel;
        }

        /// <summary>
        /// Gets the SCORM content details for a particular Learning Hub external reference (guid).
        /// </summary>
        /// <param name="externalReference">The external reference (guid).</param>
        /// <returns>A ScormContentServerViewModel.</returns>
        public ScormContentServerViewModel GetScormContentServerDetailsByLHExternalReference(string externalReference)
        {
            var param0 = new SqlParameter("@externalReference", SqlDbType.NVarChar) { Value = externalReference };

            var scormContentServerViewModel = this.DbContext.ScormContentServerViewModel.FromSqlRaw("[resources].[GetScormContentServerDetailsForLHExternalReference] @externalReference", param0).AsEnumerable().FirstOrDefault();

            return scormContentServerViewModel;
        }

        /// <summary>
        /// Gets the SCORM content details for a particular historic external URL. These historic URLs have to be supported to
        /// allow historic ESR links on migrated resources to continue to work.
        /// </summary>
        /// <param name="externalUrl">The external Url.</param>
        /// <returns>A ScormContentServerViewModel.</returns>
        public ScormContentServerViewModel GetScormContentServerDetailsByHistoricExternalUrl(string externalUrl)
        {
            var param0 = new SqlParameter("@externalUrl", SqlDbType.NVarChar) { Value = externalUrl };

            var scormContentServerViewModel = this.DbContext.ScormContentServerViewModel.FromSqlRaw("[resources].[GetScormContentServerDetailsForHistoricExternalUrl] @externalUrl", param0).ToList().FirstOrDefault<ScormContentServerViewModel>();

            return scormContentServerViewModel;
        }

        /// <inheritdoc/>
        public async Task<List<string>> GetExternalReferenceByResourceId(int resourceId)
        {
            return await (from rr in this.DbContext.ResourceReference
                          join er in this.DbContext.ExternalReference on rr.Id equals er.ResourceReferenceId
                          where rr.ResourceId == resourceId
                          select er.Reference)
                          .ToListAsync();
        }
    }
}