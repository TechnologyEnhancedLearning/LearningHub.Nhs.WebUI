namespace LearningHub.Nhs.Repository.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The IScopeRepository interface.
    /// </summary>
    public interface IScopeRepository : IGenericRepository<Scope>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Scope> GetByIdAsync(int id);

        /// <summary>
        /// The get by catalogueNodeId async.
        /// </summary>
        /// <param name="catalogueNodeId">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Scope> GetByCatalogueNodeIdAsync(int? catalogueNodeId);
    }
}
