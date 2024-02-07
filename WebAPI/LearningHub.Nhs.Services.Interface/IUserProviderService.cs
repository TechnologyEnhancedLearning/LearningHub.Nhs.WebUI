// <copyright file="IUserProviderService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The User Provider Service interface.
    /// </summary>
    public interface IUserProviderService
    {
        /// <summary>
        /// The update provider list by user id async.
        /// </summary>
        /// <param name="userProviderUpdateModel">The user provider update model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateUserProviderAsync(UserProviderUpdateViewModel userProviderUpdateModel);
    }
}
