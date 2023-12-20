// <copyright file="ResourceVersionFlagRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource version flag repository.
    /// </summary>
    public class ResourceVersionFlagRepository : GenericRepository<ResourceVersionFlag>, IResourceVersionFlagRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionFlagRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionFlagRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public IQueryable<ResourceVersionFlag> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            return this.DbContext.ResourceVersionFlag.Where(r => r.ResourceVersionId == resourceVersionId && !r.Deleted).AsNoTracking();
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionFlag">The entity.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public virtual async Task<LearningHubValidationResult> UpdateResourceVersionFlagAsync(int userId, ResourceVersionFlag resourceVersionFlag)
        {
            var retVal = new LearningHubValidationResult();
            var update = this.DbContext.ResourceVersionFlag
                .SingleOrDefault(r => r.Id == resourceVersionFlag.Id);

            if (update == null)
            {
                retVal.IsValid = false;
                retVal.Details.Add($"Unable to update ResourceVersionFlag. Could not find ResourceVersionFlag for Id={resourceVersionFlag.Id}");
            }
            else
            {
                // Update ResourceVersionFlag
                this.DbContext.Entry(update).CurrentValues.SetValues(resourceVersionFlag);
                this.SetAuditFieldsForUpdate(userId, update);

                await this.DbContext.SaveChangesAsync();

                this.DbContext.Entry(update).State = EntityState.Detached;

                retVal.IsValid = true;
            }

            return retVal;
        }

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionFlagId">The resource version Flag id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int userId, int resourceVersionFlagId)
        {
            var resourceVersionFlag = this.DbContext.ResourceVersionFlag
                .SingleOrDefault(r => r.Id == resourceVersionFlagId);

            if (resourceVersionFlag != null)
            {
                this.SetAuditFieldsForDelete(userId, resourceVersionFlag);

                await this.DbContext.SaveChangesAsync();
            }
        }
    }
}
