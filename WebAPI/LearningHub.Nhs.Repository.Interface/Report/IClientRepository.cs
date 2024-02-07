// <copyright file="IClientRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Report
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Reporting;

    /// <summary>
    /// The ClientRepository interface.
    /// </summary>
    public interface IClientRepository : IGenericRepository<Client>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Client> GetByIdAsync(int id);
    }
}
