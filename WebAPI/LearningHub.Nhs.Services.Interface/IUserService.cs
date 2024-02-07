// <copyright file="IUserService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The UserService interface.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// The get by username async.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<User> GetByUsernameAsync(string userName);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserLHBasicViewModel> GetByIdAsync(int id);

        /// <summary>
        /// The get active content async.
        /// </summary>
        /// <param name="userId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActiveContentViewModel}"/>.</returns>
        Task<List<ActiveContentViewModel>> GetActiveContentAsync(int userId);

        /// <summary>
        /// The add active content.
        /// </summary>
        /// <param name="activeContentViewModel">The active content view model<see cref="ActiveContentViewModel"/>.</param>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> AddActiveContent(ActiveContentViewModel activeContentViewModel, int userId);

        /// <summary>
        /// The release active content.
        /// </summary>
        /// <param name="activeContentReleaseViewModel">The active content view model<see cref="ActiveContentReleaseViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> ReleaseActiveContent(ActiveContentReleaseViewModel activeContentReleaseViewModel);

        /// <summary>
        /// Returns a list of basic user info - filtered, sorted and paged as required.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The preset filter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<PagedResultSet<UserAdminBasicViewModel>> GetUserAdminBasicPageAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "");

        /// <summary>
        /// Returns indication of whether the user in an Admin.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool IsAdminUser(int userId);

        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="userCreateViewModel">The userCreateViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateUserAsync(int userId, UserCreateViewModel userCreateViewModel);

        /// <summary>
        /// Update the lh user async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userUpdateViewModel">The userUpdate ViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateUserAsync(int userId, UserUpdateViewModel userUpdateViewModel);
    }
}
