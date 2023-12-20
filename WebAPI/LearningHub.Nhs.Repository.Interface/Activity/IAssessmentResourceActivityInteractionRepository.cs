// <copyright file="IAssessmentResourceActivityInteractionRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Activity
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;

    /// <summary>
    /// The AssessmentResourceActivityInteraction interface.
    /// </summary>
    public interface IAssessmentResourceActivityInteractionRepository : IGenericRepository<AssessmentResourceActivityInteraction>
    {
        /// <summary>
        /// Get Assessment Resource Activity Interaction By Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>AssessmentResourceActivityInteraction.</returns>
        Task<AssessmentResourceActivityInteraction> GetByIdAsync(int id);

        /// <summary>
        /// Get the assessment resource activity interaction for the given user, activity, and question block.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <param name="questionBlockId">The question block id.</param>
        /// <returns>AssessmentResourceActivityInteraction.</returns>
        Task<AssessmentResourceActivityInteraction> GetInteractionForQuestion(int userId, int assessmentResourceActivityId, int questionBlockId);

        /// <summary>
        /// Creates an assessment activity interaction.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="interaction">The interaction.</param>
        /// <returns>The task.</returns>
        Task<int> CreateInteraction(int userId, AssessmentResourceActivityInteraction interaction);

        /// <summary>
        /// Gets all the interactions for a given assessment resource activity.
        /// </summary>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <returns>The list of interactions.</returns>
        Task<List<AssessmentResourceActivityInteraction>> GetInteractionsForAssessmentResourceActivity(int assessmentResourceActivityId);
    }
}
