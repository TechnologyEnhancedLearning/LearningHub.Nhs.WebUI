// <copyright file="ClientRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Report
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Reporting;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Report;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The report client repository.
    /// </summary>
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ClientRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<Client> GetByIdAsync(int id)
        {
            return this.DbContext.ReportClient
                .Where(n => n.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }
    }
}
