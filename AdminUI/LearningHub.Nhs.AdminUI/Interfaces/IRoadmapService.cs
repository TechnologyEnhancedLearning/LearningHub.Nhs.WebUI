// <copyright file="IRoadmapService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// Defines the <see cref="IRoadmapService" />.
    /// </summary>
    public interface IRoadmapService
    {
        /// <summary>
        /// The AddRoadmap.
        /// </summary>
        /// <param name="roadmap">The roadmap<see cref="Roadmap"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> AddRoadmap(Roadmap roadmap);

        /// <summary>
        /// The DeleteRoadmapAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteRoadmapAsync(int id);

        /// <summary>
        /// The GetRoadmap.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{Roadmap}"/>.</returns>
        Task<Roadmap> GetRoadmap(int id);

        /// <summary>
        /// The GetUpdates.
        /// </summary>
        /// <returns>The <see cref="List{Roadmap}"/>.</returns>
        Task<List<Roadmap>> GetUpdates();

        /// <summary>
        /// The UpdateRoadmap.
        /// </summary>
        /// <param name="roadmap">The roadmap<see cref="Roadmap"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateRoadmap(Roadmap roadmap);
    }
}
