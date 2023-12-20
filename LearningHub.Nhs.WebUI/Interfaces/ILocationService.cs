// <copyright file="ILocationService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;

    /// <summary>
    /// Defines the <see cref="ILocationService" />.
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// The GetByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<LocationBasicViewModel> GetByIdAsync(int id);

        /// <summary>
        /// The GetFilteredAsync.
        /// </summary>
        /// <param name="criteria">Criteria.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<LocationBasicViewModel>> GetFilteredAsync(string criteria);

        /// <summary>
        /// The GetPagedFilteredAsync.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Tuple<int, List<LocationBasicViewModel>>> GetPagedFilteredAsync(string criteria, int page, int pageSize);
    }
}
