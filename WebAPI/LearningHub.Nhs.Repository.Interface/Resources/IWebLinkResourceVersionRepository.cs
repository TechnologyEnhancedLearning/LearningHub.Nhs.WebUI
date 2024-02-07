// <copyright file="IWebLinkResourceVersionRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The WebLinkResourceVersionRepository interface.
    /// </summary>
    public interface IWebLinkResourceVersionRepository
    {
        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<WebLinkResourceVersion> GetByResourceVersionIdAsync(int id);

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="webLinkResourceVersion">The web link resource version.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateAsync(int userId, WebLinkResourceVersion webLinkResourceVersion);
    }
}
