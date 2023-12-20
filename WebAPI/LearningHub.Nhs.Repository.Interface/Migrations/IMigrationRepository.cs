// <copyright file="IMigrationRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Migrations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Models.Entities.Migration;

    /// <summary>
    /// The MigrationRepository interface.
    /// </summary>
    public interface IMigrationRepository : IGenericRepository<Migration>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Migration> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new Resource based on the parameters supplied.
        /// </summary>
        /// <param name="resourceParams">The parameters to use when creating the resource.</param>
        /// <param name="resourceFileParamsList">The list of files that have already been copied from the Azure migration blob container
        /// to the Azure resources file share.</param>
        /// <returns>The ResourceVersionId.</returns>
        Task<int> CreateResourceAsync(ResourceParamsModel resourceParams, List<ResourceFileParamsModel> resourceFileParamsList);
    }
}
