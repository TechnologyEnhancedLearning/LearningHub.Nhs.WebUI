using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.Models.Entities.Resource;
using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class ProfessionalBodyRepository : GenericRepository<ProfessionalBody>, IProfessionalBodyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfessionalBodyRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ProfessionalBodyRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        public async Task<IReadOnlyList<ProfessionalBody>> GetAllAsync()
        {
            return await this.DbContext.ProfessionalBody
                .AsNoTracking()
                .Where(x => x.RemoveDate == null)
                .OrderBy(x => x.OrderByNumber)
                .ThenBy(x => x.ProfessionalBodyName)
                .ToListAsync();
        }

        public async Task<ProfessionalBody?> GetByIdAsync(int id)
        {
            return await this.DbContext.ProfessionalBody
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         x.RemoveDate == null);
        }
    }
}
