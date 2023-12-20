// <copyright file="IUrlRewritingRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;

    /// <summary>
    /// The UrlRewritingRepository interface.
    /// </summary>
    public interface IUrlRewritingRepository
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="fullHistoricUrl">The fullHistoricUrl.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        bool Exists(string fullHistoricUrl);
    }
}
