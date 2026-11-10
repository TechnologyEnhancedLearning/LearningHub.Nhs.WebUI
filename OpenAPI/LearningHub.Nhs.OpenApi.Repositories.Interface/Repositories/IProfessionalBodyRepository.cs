using LearningHub.Nhs.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    /// <summary>
    /// The ProfessionalBodyRepository interface.
    /// </summary>
    public interface IProfessionalBodyRepository : IGenericRepository<ProfessionalBody>
    {
        /// <summary>
        /// Get all professional Bodies
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<ProfessionalBody>> GetAllAsync();

        /// <summary>
        /// Get a professional body by id.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns></returns>
        Task<ProfessionalBody?> GetByIdAsync(int id);
    }
}
