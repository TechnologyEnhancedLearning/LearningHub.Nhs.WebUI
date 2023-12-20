// <copyright file="IMigrationSourceRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Migrations
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Migration;

    /// <summary>
    /// The MigrationSourceRepository interface.
    /// </summary>
    public interface IMigrationSourceRepository : IGenericRepository<MigrationSource>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MigrationSource> GetByIdAsync(int id);
    }
}
