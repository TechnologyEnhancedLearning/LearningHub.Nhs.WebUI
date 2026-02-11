namespace LearningHub.Nhs.WebUI.Services
{
    using System.Security.Principal;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using Microsoft.FeatureManagement;

    /// <summary>
    /// Defines the <see cref="NavigationPermissionService" />.
    /// </summary>
    public class NavigationPermissionService : INavigationPermissionService
    {
        private readonly IResourceService resourceService;
        private readonly IUserGroupService userGroupService;
        private readonly IReportService reportService;
        private IFeatureManager featureManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationPermissionService"/> class.
        /// </summary>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="userGroupService">UserGroup service.</param>
        /// <param name="reportService">Report Service.</param>
        /// <param name="featureManager">Feature Manager.</param>
        public NavigationPermissionService(
        IResourceService resourceService,
        IUserGroupService userGroupService,
        IReportService reportService,
        IFeatureManager featureManager)
        {
            this.resourceService = resourceService;
            this.userGroupService = userGroupService;
            this.reportService = reportService;
            this.featureManager = featureManager;
        }

        /// <summary>
        /// The GetNavigationModelAsync.
        /// </summary>
        /// <param name="user">The user<see cref="IPrincipal"/>.</param>
        /// <param name="loginWizardComplete">The loginWizardComplete<see cref="bool"/>.</param>
        /// <param name="controllerName">The controller name.</param>
        /// <returns>The <see cref="Task{NavigationModel}"/>.</returns>
        public async Task<NavigationModel> GetNavigationModelAsync(IPrincipal user, bool loginWizardComplete, string controllerName)
        {
            if (!loginWizardComplete && (user.IsInRole("Administrator") || user.IsInRole("ReadOnly") || user.IsInRole("BlueUser") || user.IsInRole("BasicUser")))
            {
                return this.InLoginWizard();
            }
            else if (!user.Identity.IsAuthenticated)
            {
                return this.NotAuthenticated();
            }
            else if (user.IsInRole("Administrator"))
            {
                return await this.AuthenticatedAdministrator(controllerName);
            }
            else if (user.IsInRole("ReadOnly"))
            {
                return await this.AuthenticatedReadOnly(controllerName);
            }
            else if (user.IsInRole("BasicUser"))
            {
                return await this.AuthenticatedBasicUserOnly();
            }
            else if (user.IsInRole("BlueUser"))
            {
                return await this.AuthenticatedBlueUser(controllerName);
            }
            else
            {
                return this.AuthenticatedGuest();
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
                ShowHome = false,
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
        /// <returns>The <see cref="NavigationModel"/>.</returns>
        private async Task<NavigationModel> AuthenticatedAdministrator(string controllerName)
        {
            return new NavigationModel()
            {
                ShowHome = true,
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
                ShowReports = this.DisplayReportMenu() ? await this.reportService.GetReporterPermission() : false,
            };
        }

        /// <summary>
        /// The AuthenticatedBlueUser.
        /// </summary>
        /// <param name="controllerName">The controller name.</param>
        /// <returns>The <see cref="NavigationModel"/>.</returns>
        private async Task<NavigationModel> AuthenticatedBlueUser(string controllerName)
        {
            return new NavigationModel()
            {
                ShowHome = true,
                ShowMyContributions = await this.userGroupService.UserHasCatalogueContributionPermission(),
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
                ShowReports = this.DisplayReportMenu() ? await this.reportService.GetReporterPermission() : false,
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
                ShowHome = true,
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
        private async Task<NavigationModel> AuthenticatedReadOnly(string controllerName)
        {
            return new NavigationModel()
            {
                ShowHome = true,
                ShowMyContributions = await this.resourceService.UserHasPublishedResourcesAsync(),
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
                ShowReports = this.DisplayReportMenu() ? await this.reportService.GetReporterPermission() : false,
            };
        }

        /// <summary>
        /// The AuthenticatedBasicUserOnly.
        /// </summary>
        /// <returns>The <see cref="Task{NavigationModel}"/>.</returns>
        private async Task<NavigationModel> AuthenticatedBasicUserOnly()
        {
            return new NavigationModel()
            {
                ShowHome = true,
                ShowMyContributions = await this.resourceService.UserHasPublishedResourcesAsync(),
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
                ShowReports = this.DisplayReportMenu() ? await this.reportService.GetReporterPermission() : false,
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
                ShowHome = true,
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

        private bool DisplayReportMenu()
        {
            return Task.Run(() => this.featureManager.IsEnabledAsync(FeatureFlags.InPlatformReport)).Result;
        }
    }
}
