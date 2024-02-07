// <copyright file="ITermsAndConditionsService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Entities;

    /// <summary>
    /// Defines the <see cref="ITermsAndConditionsService" />.
    /// </summary>
    public interface ITermsAndConditionsService
    {
        /// <summary>
        /// The AcceptByUser.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="currentUserId">Current user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> AcceptByUser(int id, int currentUserId);

        /// <summary>
        /// The LatestVersionAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<TermsAndConditions> LatestVersionAsync(int id);
    }
}
