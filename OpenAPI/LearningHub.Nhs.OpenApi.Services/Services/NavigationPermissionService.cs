namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Security.Principal;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;

    /// <summary>
    /// Defines the <see cref="NavigationPermissionService" />.
    /// </summary>
    public class NavigationPermissionService : INavigationPermissionService
    {
        private readonly IResourceService resourceService;
        private readonly IUserGroupService userGroupService;
        private readonly IDatabricksService databricksService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationPermissionService"/> class.
        /// </summary>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="userGroupService">userGroup service.</param>
        /// <param name="databricksService">databricksService.</param>
        public NavigationPermissionService(IResourceService resourceService, IUserGroupService userGroupService, IDatabricksService databricksService)
        {
            this.resourceService = resourceService;
            this.userGroupService = userGroupService;
            this.databricksService = databricksService;
        }

        /// <summary>
        /// The GetNavigationModelAsync.
        /// </summary>
        /// <param name="user">The user<see cref="IPrincipal"/>.</param>
        /// <param name="loginWizardComplete">The loginWizardComplete<see cref="bool"/>.</param>
        /// <param name="controllerName">The controller name.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task{NavigationModel}"/>.</returns>
        public async Task<NavigationModel> GetNavigationModelAsync(IPrincipal user, bool loginWizardComplete, string controllerName, int currentUserId)
        {
            if (!loginWizardComplete && (user.IsInRole("Administrator") || user.IsInRole("ReadOnly") || user.IsInRole("BlueUser") || user.IsInRole("BasicUser")))
            {
                return InLoginWizard();
            }
            else if (!user.Identity.IsAuthenticated)
            {
                return NotAuthenticated();
            }
            else if (user.IsInRole("Administrator"))
            {
                return await AuthenticatedAdministrator(controllerName, currentUserId);
            }
            else if (user.IsInRole("ReadOnly"))
            {
                return await AuthenticatedReadOnly(controllerName, currentUserId);
            }
            else if (user.IsInRole("BasicUser"))
            {
                return await AuthenticatedBasicUserOnly(currentUserId);
            }
            else if (user.IsInRole("BlueUser"))
            {
                return await AuthenticatedBlueUser(controllerName, currentUserId);
            }
            else
            {
                return AuthenticatedGuest();
            }
        }

        /// <summary>
        /// The NotAuthenticated.
        /// </summary>
        /// <returns>The <see cref="NavigationModel"/>.</returns>
        public NavigationModel NotAuthenticated()
        {
            return new NavigationModel()
            {
                ShowMyContributions = false,
                ShowMyLearning = false,
                ShowMyBookmarks = false,
                ShowSearch = false,
                ShowAdmin = false,
                ShowForums = false,
                ShowHelp = false,
                ShowMyRecords = false,
                ShowNotifications = false,
                ShowRegister = false,
                ShowSignOut = false,
                ShowMyAccount = false,
                ShowBrowseCatalogues = false,
                ShowReports = false,
            };
        }

        /// <summary>
        /// The AuthenticatedAdministrator.
        /// </summary>
        /// <param name="controllerName">The controller name.</param>
        /// <param name="userId">userId.</param>
        /// <returns>The <see cref="NavigationModel"/>.</returns>
        private async Task<NavigationModel> AuthenticatedAdministrator(string controllerName, int userId)
        {
            return new NavigationModel()
            {
                ShowMyContributions = true,
                ShowMyLearning = true,
                ShowMyBookmarks = false,
                ShowSearch = controllerName != "search" && controllerName != string.Empty,
                ShowAdmin = true,
                ShowForums = true,
                ShowHelp = false,
                ShowMyRecords = true,
                ShowNotifications = false,
                ShowRegister = false,
                ShowSignOut = true,
                ShowMyAccount = true,
                ShowBrowseCatalogues = true,
                ShowReports = await this.databricksService.IsUserReporter(userId),
            };
        }

        /// <summary>
        /// The AuthenticatedBlueUser.
        /// </summary>
        /// <param name="controllerName">The controller name.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="NavigationModel"/>.</returns>
        private async Task<NavigationModel> AuthenticatedBlueUser(string controllerName, int userId)
        {
            return new NavigationModel()
            {
                ShowMyContributions = await this.userGroupService.UserHasCatalogueContributionPermission(userId),
                ShowMyLearning = true,
                ShowMyBookmarks = false,
                ShowSearch = controllerName != "search" && controllerName != string.Empty,
                ShowAdmin = false,
                ShowForums = true,
                ShowHelp = false,
                ShowMyRecords = true,
                ShowNotifications = false,
                ShowRegister = false,
                ShowSignOut = true,
                ShowMyAccount = true,
                ShowBrowseCatalogues = true,
                ShowReports = await this.databricksService.IsUserReporter(userId),
            };
        }

        /// <summary>
        /// The AuthenticatedGuest.
        /// </summary>
        /// <returns>The <see cref="NavigationModel"/>.</returns>
        private NavigationModel AuthenticatedGuest()
        {
            return new NavigationModel()
            {
                ShowMyContributions = false,
                ShowMyLearning = false,
                ShowMyBookmarks = false,
                ShowSearch = false,
                ShowAdmin = false,
                ShowForums = false,
                ShowHelp = false,
                ShowMyRecords = false,
                ShowNotifications = false,
                ShowRegister = false,
                ShowSignOut = true,
                ShowMyAccount = false,
                ShowBrowseCatalogues = false,
                ShowReports = false,
            };
        }

        /// <summary>
        /// The AuthenticatedReadOnly.
        /// </summary>
        /// <param name="controllerName">The controller name.</param>
        /// <returns>The <see cref="Task{NavigationModel}"/>.</returns>
        private async Task<NavigationModel> AuthenticatedReadOnly(string controllerName,int userId)
        {
            return new NavigationModel()
            {
                ShowMyContributions = await resourceService.HasPublishedResourcesAsync(userId),
                ShowMyLearning = true,
                ShowMyBookmarks = false,
                ShowSearch = controllerName != "search" && controllerName != string.Empty,
                ShowAdmin = false,
                ShowForums = true,
                ShowHelp = false,
                ShowMyRecords = true,
                ShowNotifications = false,
                ShowRegister = false,
                ShowSignOut = true,
                ShowMyAccount = false,
                ShowBrowseCatalogues = true,
                ShowReports = await this.databricksService.IsUserReporter(userId),
            };
        }

        /// <summary>
        /// The AuthenticatedBasicUserOnly.
        /// </summary>
        /// <returns>The <see cref="Task{NavigationModel}"/>.</returns>
        private async Task<NavigationModel> AuthenticatedBasicUserOnly(int userId)
        {
            return new NavigationModel()
            {
                ShowMyContributions = await resourceService.HasPublishedResourcesAsync(userId),
                ShowMyLearning = true,
                ShowMyBookmarks = false,
                ShowSearch = true,
                ShowAdmin = false,
                ShowForums = true,
                ShowHelp = false,
                ShowMyRecords = true,
                ShowNotifications = false,
                ShowRegister = false,
                ShowSignOut = true,
                ShowMyAccount = true,
                ShowBrowseCatalogues = true,
                ShowReports = false,
            };
        }

        /// <summary>
        /// The InLoginWizard.
        /// </summary>
        /// <returns>The <see cref="NavigationModel"/>.</returns>
        private NavigationModel InLoginWizard()
        {
            return new NavigationModel()
            {
                ShowMyContributions = false,
                ShowMyLearning = false,
                ShowMyBookmarks = false,
                ShowSearch = false,
                ShowAdmin = false,
                ShowForums = false,
                ShowHelp = false,
                ShowMyRecords = false,
                ShowNotifications = false,
                ShowRegister = false,
                ShowSignOut = true,
                ShowMyAccount = false,
                ShowBrowseCatalogues = false,
                ShowReports = false,
            };
        }
    }
}
