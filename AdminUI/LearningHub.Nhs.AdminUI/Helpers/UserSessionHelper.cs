// <copyright file="UserSessionHelper.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Helpers
{
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.AdminUI.Interfaces;

    /// <summary>
    /// Defines the <see cref="UserSessionHelper" />.
    /// </summary>
    public class UserSessionHelper : IUserSessionHelper
    {
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSessionHelper"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public UserSessionHelper(IUserService userService)
        {
            this.userService = userService;
        }

        /// <inheritdoc/>
        public async Task StartSession(int userId)
        {
            UserHistoryViewModel userHistoryViewModel = new UserHistoryViewModel()
            {
                Detail = "Learning Hub Admin user session started",
                UserId = userId,
            };
            await this.userService.StoreUserHistory(userHistoryViewModel);
        }
    }
}
