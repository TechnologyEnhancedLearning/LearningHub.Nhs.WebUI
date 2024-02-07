// <copyright file="ILogRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The LogRepository interface.
    /// </summary>
    public interface ILogRepository
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Log> GetByIdAsync(int id);

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        IQueryable<Log> GetAll();
    }
}
