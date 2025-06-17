namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The generic repository.
    /// </summary>
    /// <typeparam name="TEntity">Input type.</typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public GenericRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
        {
            this.DbContext = dbContext;
            this.TimezoneOffsetManager = tzOffsetManager;
        }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        protected LearningHubDbContext DbContext { get; }

        /// <summary>
        /// Gets the TimezoneOffset manager.
        /// </summary>
        protected ITimezoneOffsetManager TimezoneOffsetManager { get; }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        public IQueryable<TEntity> GetAll()
        {
            return this.DbContext.Set<TEntity>().AsNoTracking();
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public virtual async Task<int> CreateAsync(int userId, TEntity entity)
        {
            await this.DbContext.Set<TEntity>().AddAsync(entity);
            this.SetAuditFieldsForCreate(userId, entity);
            try
            {
                await this.DbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            this.DbContext.Entry(entity).State = EntityState.Detached;

            return entity.Id;
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public virtual async Task UpdateAsync(int userId, TEntity entity)
        {
            this.DbContext.Set<TEntity>().Update(entity);

            this.SetAuditFieldsForUpdate(userId, entity);

            await this.DbContext.SaveChangesAsync();

            this.DbContext.Entry(entity).State = EntityState.Detached;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        public virtual void Update(int userId, TEntity entity)
        {
            this.DbContext.Set<TEntity>().Update(entity);

            this.SetAuditFieldsForUpdate(userId, entity);

            this.DbContext.SaveChanges();

            this.DbContext.Entry(entity).State = EntityState.Detached;
        }

        /// <summary>
        /// The set audit fields for create.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        public void SetAuditFieldsForCreate(int userId, EntityBase entity)
        {
            var amendDate = this.GetAmendDate();

            entity.Deleted = false;
            entity.CreateUserId = userId;
            entity.CreateDate = amendDate;
            entity.AmendUserId = userId;
            entity.AmendDate = amendDate;
        }

        /// <summary>
        /// The set audit fields for create or delete.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="isCreate">Flag used to specify the journey.</param>
        public void SetAuditFieldsForCreateOrDelete(int userId, EntityBase entity, bool isCreate)
        {
            if (isCreate)
            {
                this.SetAuditFieldsForCreate(userId, entity);
            }
            else
            {
                this.SetAuditFieldsForDelete(userId, entity);
            }
        }

        /// <summary>
        /// The set audit fields for update.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        public void SetAuditFieldsForUpdate(int userId, EntityBase entity)
        {
            entity.AmendUserId = userId;
            entity.AmendDate = this.GetAmendDate();
            this.DbContext.Entry(entity).Property("CreateUserId").IsModified = false;
            this.DbContext.Entry(entity).Property("CreateDate").IsModified = false;
            if (entity.GetType() == typeof(User))
            {
                this.DbContext.Entry(entity).Property("VersionStartTime").IsModified = false;
                this.DbContext.Entry(entity).Property("VersionEndTime").IsModified = false;
            }
        }

        /// <summary>
        /// The set audit fields for delete.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        public void SetAuditFieldsForDelete(int userId, EntityBase entity)
        {
            entity.Deleted = true;
            entity.AmendUserId = userId;
            entity.AmendDate = this.GetAmendDate();
            this.DbContext.Entry(entity).Property("CreateUserId").IsModified = false;
            this.DbContext.Entry(entity).Property("CreateDate").IsModified = false;
        }

        private DateTimeOffset GetAmendDate()
        {
            var tzOffset = this.TimezoneOffsetManager.UserTimezoneOffset;
            return tzOffset.HasValue ? new DateTimeOffset(DateTime.UtcNow.AddMinutes(tzOffset.Value).Ticks, TimeSpan.FromMinutes(tzOffset.Value)) : DateTimeOffset.Now;
        }
    }
}