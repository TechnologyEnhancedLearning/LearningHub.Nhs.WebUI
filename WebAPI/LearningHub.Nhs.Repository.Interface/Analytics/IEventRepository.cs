// <copyright file="IEventRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Analytics
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Analytics;

    /// <summary>
    /// The IEventRepository interface.
    /// </summary>
    public interface IEventRepository : IGenericRepository<Event>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Event> GetByIdAsync(int id);
    }
}
