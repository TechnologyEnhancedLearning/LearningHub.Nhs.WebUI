// <copyright file="IMediaResourcePlayedSegmentRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Activity
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;

    /// <summary>
    /// The MediaResourcePlayedSegmentRepository interface.
    /// </summary>
    public interface IMediaResourcePlayedSegmentRepository : IGenericRepository<MediaResourcePlayedSegment>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<MediaResourcePlayedSegment>> GetPlayedSegmentsAsync(int userId, int resourceId, int majorVersion);
    }
}
