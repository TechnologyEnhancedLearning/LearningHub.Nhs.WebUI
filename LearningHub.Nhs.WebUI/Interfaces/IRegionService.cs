namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;

    /// <summary>
    /// Defines the <see cref="IRegionService" />.
    /// </summary>
    public interface IRegionService
    {
        /// <summary>
        /// The GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<GenericListViewModel>> GetAllAsync();

        /// <summary>
        /// The GetAllPagedAsync.
        /// </summary>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{Region}"/>.</returns>
        Task<Tuple<int, List<GenericListViewModel>>> GetAllPagedAsync(int page, int pageSize);

        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{Region}"/>.</returns>
        Task<GenericListViewModel> GetByIdAsync(int id);
    }
}
