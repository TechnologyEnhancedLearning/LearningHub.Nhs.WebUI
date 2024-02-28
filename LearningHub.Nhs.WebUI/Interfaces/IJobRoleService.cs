namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;

    /// <summary>
    /// Defines the <see cref="IJobRoleService" />.
    /// </summary>
    public interface IJobRoleService
    {
        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<JobRoleBasicViewModel> GetByIdAsync(int id);

        /// <summary>
        /// The GetFilteredAsync.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<JobRoleBasicViewModel>> GetFilteredAsync(string filter);

        /// <summary>
        /// The GetPagedFilteredAsync.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The pageSize.</param>
        /// <returns>The <see cref="T:Task{List{JobRoleBasicViewModel}}"/>.</returns>
        Task<Tuple<int, List<JobRoleBasicViewModel>>> GetPagedFilteredAsync(string filter, int page, int pageSize);

        /// <summary>
        /// The ValidateMedicalCouncilNumber.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <param name="medicalCouncilId">Medical council id.</param>
        /// <param name="medicalCouncilNumber">Medical council name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> ValidateMedicalCouncilNumber(string lastName, int medicalCouncilId, string medicalCouncilNumber);
    }
}
