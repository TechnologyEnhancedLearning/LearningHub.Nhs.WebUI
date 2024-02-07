// <copyright file="IGradeService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;

    /// <summary>
    /// Defines the <see cref="IGradeService" />.
    /// </summary>
    public interface IGradeService
    {
        /// <summary>
        /// The GetGradesForJobRoleAsync.
        /// </summary>
        /// <param name="jobRoleId">Job role id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<GenericListViewModel>> GetGradesForJobRoleAsync(int jobRoleId);

        /// <summary>
        /// The GetPagedGradesForJobRoleAsync.
        /// </summary>
        /// <param name="jobRoleId">The jobRoleId<see cref="int"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Tuple<int, List<GenericListViewModel>>> GetPagedGradesForJobRoleAsync(int jobRoleId, int page, int pageSize);
    }
}
