// <copyright file="IMediaResourceActivityRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Activity
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;

    /// <summary>
    /// The MediaResourceActivity interface.
    /// </summary>
    public interface IMediaResourceActivityRepository : IGenericRepository<MediaResourceActivity>
    {
        /// <summary>
        /// Get Media Resource Activity By Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>MediaResourceActivity.</returns>
        Task<MediaResourceActivity> GetByIdAsync(int id);
    }
}
