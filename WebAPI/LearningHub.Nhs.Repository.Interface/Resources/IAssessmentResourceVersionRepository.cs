// <copyright file="IAssessmentResourceVersionRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The AssessmentResourceVersionRepository interface.
    /// </summary>
    public interface IAssessmentResourceVersionRepository : IGenericRepository<AssessmentResourceVersion>
    {
        /// <summary>
        /// The get by Assessment Version Id async.
        /// </summary>
        /// <param name="assessmentVersionId">The Assessment Version Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentResourceVersion> GetByResourceVersionIdAsync(int assessmentVersionId);

        /// <summary>
        /// The get by Assessment Content Id async.
        /// </summary>
        /// <param name="assessmentContentId">The Assessment Content Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<AssessmentResourceVersion> GetByAssessmentContentBlockCollectionIdAsync(int assessmentContentId);
    }
}