// <copyright file="ISpecialtyService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;

    /// <summary>
    /// Defines the <see cref="ISpecialtyService" />.
    /// </summary>
    public interface ISpecialtyService
    {
        /// <summary>
        /// The GetSpecialtiesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<GenericListViewModel>> GetSpecialtiesAsync();

        /// <summary>
        /// The GetPagedSpecialtiesAsync.
        /// </summary>
        /// <param name="filter">The filter<see cref="string"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Tuple<int, List<GenericListViewModel>>> GetPagedSpecialtiesAsync(string filter, int page, int pageSize);
    }
}
