// <copyright file="LogRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The log repository.
    /// </summary>
    public class LogRepository : ILogRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogRepository"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The nlog db context.
        /// </param>
        public LogRepository(NLogDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        protected NLogDbContext DbContext { get; }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Log> GetByIdAsync(int id)
        {
            return await this.DbContext.Log
                            .AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// The get page async.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<List<Log>> GetPageAsync(int page, int pageSize)
        {
            return await this.DbContext.Set<Log>().AsNoTracking()
                .OrderByDescending(l => l.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        public IQueryable<Log> GetAll()
        {
            return this.DbContext.Set<Log>()
                .OrderByDescending(l => l.Id)
                .AsNoTracking();
        }
    }
}
