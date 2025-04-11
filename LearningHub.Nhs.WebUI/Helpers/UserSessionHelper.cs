namespace LearningHub.Nhs.WebUI.Helpers
{
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Interfaces;

    /// <summary>
    /// Defines the <see cref="UserSessionHelper" />.
    /// </summary>
    public class UserSessionHelper : IUserSessionHelper
    {
        private readonly IUserService userService;
        private readonly IActivityService activityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSessionHelper"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="activityService">The activity service.</param>
        public UserSessionHelper(IUserService userService, IActivityService activityService)
        {
            this.userService = userService;
            this.activityService = activityService;
        }

        /// <inheritdoc/>
        public async Task StartSession(int userId)
        {
            UserHistoryViewModel userHistoryViewModel = new UserHistoryViewModel()
            {
                Detail = "Learning Hub user session started",
                UserId = userId,
            };
            await this.userService.StoreUserHistory(userHistoryViewModel);

           // await this.userService.SyncLHUserAsync(userId);
            await this.activityService.ResolveScormActivity(userId);
        }
    }
}
