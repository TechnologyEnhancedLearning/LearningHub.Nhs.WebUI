namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The GenericRepository interface.
    /// </summary>
    /// <typeparam name="TEntity">Imput type.</typeparam>
    public interface IGenericRepository<TEntity>
        where TEntity : EntityBase
    {
        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> CreateAsync(int userId, TEntity entity);

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateAsync(int userId, TEntity entity);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entity">The entity.</param>
        void Update(int userId, TEntity entity);
    }
}
