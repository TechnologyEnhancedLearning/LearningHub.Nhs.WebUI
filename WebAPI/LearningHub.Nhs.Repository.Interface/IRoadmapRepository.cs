// <copyright file="IRoadmapRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The IRoadmapRepository.
    /// </summary>
    public interface IRoadmapRepository : IGenericRepository<Roadmap>
    {
        /// <summary>
        /// The getupdates.
        /// </summary>
        /// <returns>The updates.</returns>
        List<Roadmap> GetUpdates();
    }
}
