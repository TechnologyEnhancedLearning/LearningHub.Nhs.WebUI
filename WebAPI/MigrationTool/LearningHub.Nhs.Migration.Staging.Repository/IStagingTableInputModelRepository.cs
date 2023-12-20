// <copyright file="IStagingTableInputModelRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Staging.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Migration.Models;

    /// <summary>
    /// The StagingTableInputModelRepository interface.
    /// </summary>
    public interface IStagingTableInputModelRepository
    {
        /// <summary>
        /// The all staging table input models from the staging database.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<StagingTableInputModel>> GetAllStagingTableInputModels();
    }
}
