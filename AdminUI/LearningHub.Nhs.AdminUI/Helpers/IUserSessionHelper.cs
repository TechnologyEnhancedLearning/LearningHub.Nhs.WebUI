// <copyright file="IUserSessionHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Helpers
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IUserSessionHelper" />.
    /// </summary>
    public interface IUserSessionHelper
    {
        /// <summary>
        /// The Start User Session method.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task StartSession(int userId);
    }
}
