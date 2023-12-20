// <copyright file="IRoadMapService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.RoadMap;

    /// <summary>
    /// Defines the <see cref="IRoadMapService" />.
    /// </summary>
    public interface IRoadMapService
    {
        /// <summary>
        /// The GetUpdatesAsync.
        /// </summary>
        /// <param name="numberOfResults">numberOfResults.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<RoadMapResponseViewModel> GetUpdatesAsync(int numberOfResults);
    }
}
