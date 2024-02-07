// <copyright file="ReportRepository.cs" company="NHS England">
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
    /// The report repository.
    /// </summary>
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ReportRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Create new record.
        /// </summary>
        /// <param name="userId">The id.</param>
        /// <param name="report">report.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task<int> CreateAsync(int userId, Report report)
        {
            foreach (var reportpage in report.ReportPages)
            {
                this.SetAuditFieldsForCreate(userId, reportpage);
            }

            return await base.CreateAsync(userId, report);
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeChildren">If the children entities should be loaded.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<Report> GetByIdAsync(int id, bool includeChildren = false)
        {
            if (includeChildren)
            {
                return this.DbContext.Report
                .Include(n => n.ReportPages)
                .Where(n => n.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
            }

            return this.DbContext.Report
                .Where(n => n.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// The get file name and hash async.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="hash">The hash.</param>
        /// <param name="includeChildren">Include children.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Report> GetByFileDetailAsync(string fileName, string hash, bool includeChildren = false)
        {
            if (includeChildren)
            {
                return await this.DbContext.Report.Include(n => n.ReportPages)
                    .Where(n => n.FileName == fileName && n.Hash == hash).AsNoTracking().SingleOrDefaultAsync();
            }
            else
            {
                return await this.DbContext.Report
                   .Where(n => n.FileName == fileName && n.Hash == hash).AsNoTracking().SingleOrDefaultAsync();
            }
        }
    }
}
