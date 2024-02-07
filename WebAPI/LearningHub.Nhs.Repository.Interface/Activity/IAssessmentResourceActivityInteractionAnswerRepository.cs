// <copyright file="IAssessmentResourceActivityInteractionAnswerRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Activity
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;

    /// <summary>
    /// The AssessmentResourceActivityInteractionAnswer interface.
    /// </summary>
    public interface IAssessmentResourceActivityInteractionAnswerRepository : IGenericRepository<AssessmentResourceActivityInteractionAnswer>
    {
        /// <summary>
        /// Get Assessment Resource Activity Interaction Answer By Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>AssessmentResourceActivityInteractionAnswer.</returns>
        Task<AssessmentResourceActivityInteractionAnswer> GetByIdAsync(int id);
    }
}
