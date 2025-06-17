namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Extensions;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="UserGroupController" />.
    /// </summary>
    public class UserGroupController : BaseController
    {
        /// <summary>
        /// Defines the userGroupService.
        /// </summary>
        private readonly IUserGroupService userGroupService;

        /// <summary>
        /// Defines the userService.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Defines the websettings.
        /// </summary>
        private readonly IOptions<WebSettings> websettings;

        /// <summary>
        /// Defines the logger.
        /// </summary>
        private ILogger<LogController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="userGroupService">The userService<see cref="IUserGroupService"/>.</param>
        /// <param name="userService">The userService<see cref="IUserService"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{LogController}"/>.</param>
        /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        public UserGroupController(
            IWebHostEnvironment hostingEnvironment,
            IUserGroupService userGroupService,
            IUserService userService,
            ILogger<LogController> logger,
            IOptions<WebSettings> websettings)
        : base(hostingEnvironment)
        {
            this.userService = userService;
            this.userGroupService = userGroupService;
            this.logger = logger;
            this.websettings = websettings;
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// The UserGroupList.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> UserGroupList()
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.websettings.Value.DefaultPageSize;
            pagingRequestModel.SortColumn = "Id";
            pagingRequestModel.SortDirection = "A";
            pagingRequestModel.Filter = new List<PagingColumnFilter>();

            var model = new TablePagingViewModel<UserGroupAdminBasicViewModel>
            {
                Results = await this.userGroupService.GetUserGroupAdminBasicPageAsync(pagingRequestModel),
                SortColumn = "Id",
                SortDirection = "A",
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = 1,
                HasItems = model.Results != null && model.Results.Items.Any(),
                PageSize = this.websettings.Value.DefaultPageSize,
                TotalItems = model.Results != null ? model.Results.TotalItemCount : 0,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results != null ? model.Results.TotalItemCount : 0,
                DisplayedCount = model.Results != null ? model.Results.Items.Count() : 0,
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = 0,
                CreateRequired = false,
            };
            return this.PartialView("_UserGroups", model);
        }

        /// <summary>
        /// The UserGroupList.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> UserGroupList(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            if (string.IsNullOrEmpty(requestModel.SortColumn))
            {
                requestModel.SortColumn = "Id";
            }

            if (string.IsNullOrEmpty(requestModel.SortDirection))
            {
                requestModel.SortDirection = "A";
            }

            requestModel.Sanitize();
            requestModel.PageSize = this.websettings.Value.DefaultPageSize;

            var model = new TablePagingViewModel<UserGroupAdminBasicViewModel>
            {
                Results = await this.userGroupService.GetUserGroupAdminBasicPageAsync(requestModel),
                SortColumn = requestModel.SortColumn,
                SortDirection = requestModel.SortDirection,
                Filter = requestModel.Filter,
            };

            model.Paging = new PagingViewModel
            {
                CurrentPage = requestModel.Page,
                HasItems = model.Results.Items.Any(),
                PageSize = this.websettings.Value.DefaultPageSize,
                TotalItems = model.Results.TotalItemCount,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = model.Filter.Count(),
                CreateRequired = false,
            };

            return this.PartialView("_UserGroups", model);
        }

        /// <summary>
        /// The Details.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return this.View(new UserGroupAdminDetailViewModel());
            }

            var userGroup = await this.userGroupService.GetUserGroupAdminDetailbyIdAsync(id);
            return this.View(userGroup);
        }

        /// <summary>
        /// The Details.
        /// </summary>
        /// <param name="userGroup">The userGroup<see cref="UserGroupAdminDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Details(UserGroupAdminDetailViewModel userGroup)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.ErrorMessage = "Update failed. Please fix the following error(s):";
                return this.View("Details", userGroup);
            }

            LearningHubValidationResult validationResult;

            if (userGroup.IsNew())
            {
                validationResult = await this.userGroupService.CreateUserGroup(userGroup);
                if (validationResult.IsValid)
                {
                    userGroup = await this.userGroupService.GetUserGroupAdminDetailbyIdAsync(validationResult.CreatedId.Value);
                }
                else
                {
                    this.ViewBag.ErrorMessage = $"Update failed: {string.Join(Environment.NewLine, validationResult.Details)}";
                    return this.View("Details", userGroup);
                }
            }
            else
            {
                validationResult = await this.userGroupService.UpdateUserGroup(userGroup);
            }

            if (validationResult.IsValid)
            {
                userGroup.OriginalName = userGroup.Name;
                userGroup.OriginalDescription = userGroup.Description;
                this.ViewBag.SuccessMessage = "Update completed successfully";
                return this.View("Details", userGroup);
            }
            else
            {
                this.ViewBag.ErrorMessage = $"Update failed: {string.Join(Environment.NewLine, validationResult.Details)}";
                return this.View("Details", userGroup);
            }
        }

        /// <summary>
        /// The DeleteUserGroup.
        /// </summary>
        /// <param name="userGroupId">The userGroupId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteUserGroup(int userGroupId)
        {
            var vr = await this.userGroupService.DeleteUserGroup(userGroupId);
            if (vr.IsValid)
            {
                return this.Json(new
                {
                    success = true,
                    details = $"Deleted user group id={userGroupId}",
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error deleting user group id={userGroupId}",
                });
            }
        }

        /// <summary>
        /// The AddUserGroupsToUser.
        /// </summary>
        /// <param name="userGroupId">The userId<see cref="int"/>.</param>
        /// <param name="userIdList">The userIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> AddUsersToUserGroup(int userGroupId, string userIdList)
        {
            var vr = await this.userGroupService.AddUsersToUserGroup(userGroupId, userIdList);
            if (vr.IsValid)
            {
                this.ClearUserCachedPermissions(userIdList);
                return this.Json(new
                {
                    success = true,
                    details = $"Update completed successfully",
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error adding users to user group",
                });
            }
        }

        /// <summary>
        /// The RemoveUserFromUserGroup.
        /// </summary>
        /// <param name="userUserGroupId">The userUserGroupId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveUserFromUserGroup(int userUserGroupId)
        {
            var vr = await this.userGroupService.DeleteUserUserGroup(userUserGroupId);
            if (vr.IsValid)
            {
                return this.Json(new
                {
                    success = true,
                    details = $"User removed from user group",
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error removing user from user group",
                });
            }
        }

        /// <summary>
        /// The RemoveRoleUserGroup.
        /// </summary>
        /// <param name="roleUserGroupId">The roleUserGroupId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveRoleUserGroup(int roleUserGroupId)
        {
            var vr = await this.userGroupService.DeleteRoleUserGroup(roleUserGroupId);
            if (vr.IsValid)
            {
                return this.Json(new
                {
                    success = true,
                    details = $"Role - User group successfully removed.",
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error removing Role - User group",
                });
            }
        }

        /// <summary>
        /// The UserUserGroupList.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> UserUserGroupList(int id)
        {
            var userGroup = await this.userGroupService.GetUserGroupAdminDetailbyIdAsync(id);

            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.websettings.Value.DefaultPageSize;
            pagingRequestModel.SortColumn = "UserId";
            pagingRequestModel.SortDirection = "A";
            pagingRequestModel.PresetFilter = new List<PagingColumnFilter>
            {
                new PagingColumnFilter() { Column = "usergroupid", Value = id.ToString() },
            };

            var model = new TablePagingViewModel<UserUserGroupViewModel>
            {
                Results = await this.userGroupService.GetUserUserGroupPageAsync(pagingRequestModel),
                SortColumn = "UserId",
                SortDirection = "A",
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = 1,
                HasItems = model.Results != null && model.Results.Items.Any(),
                PageSize = this.websettings.Value.DefaultPageSize,
                TotalItems = model.Results != null ? model.Results.TotalItemCount : 0,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results != null ? model.Results.TotalItemCount : 0,
                DisplayedCount = model.Results != null ? model.Results.Items.Count() : 0,
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = 0,
                CreateRequired = false,
            };

            this.ViewBag.CanEdit = userGroup.CanEdit();
            return this.PartialView("_UserUserGroups", model);
        }

        /// <summary>
        /// The UserUserGroupList.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> UserUserGroupList(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            var userGroupPresetFilter = requestModel.PresetFilter.Where(pf => pf.Column.ToLower() == "usergroupid").First();
            int userGroupId = int.Parse(userGroupPresetFilter.Value);

            var userGroup = await this.userGroupService.GetUserGroupAdminDetailbyIdAsync(userGroupId);

            if (string.IsNullOrEmpty(requestModel.SortColumn))
            {
                requestModel.SortColumn = "UserId";
            }

            if (string.IsNullOrEmpty(requestModel.SortDirection))
            {
                requestModel.SortDirection = "A";
            }

            requestModel.Sanitize();
            requestModel.PageSize = this.websettings.Value.DefaultPageSize;

            var model = new TablePagingViewModel<UserUserGroupViewModel>
            {
                Results = await this.userGroupService.GetUserUserGroupPageAsync(requestModel),
                SortColumn = requestModel.SortColumn,
                SortDirection = requestModel.SortDirection,
                Filter = requestModel.Filter,
            };

            model.Paging = new PagingViewModel
            {
                CurrentPage = requestModel.Page,
                HasItems = model.Results.Items.Any(),
                PageSize = this.websettings.Value.DefaultPageSize,
                TotalItems = model.Results.TotalItemCount,
            };

            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = model.Filter.Count(),
                CreateRequired = false,
            };

            this.ViewBag.CanEdit = userGroup.CanEdit();
            return this.PartialView("_UserUserGroups", model);
        }

        /// <summary>
        /// The SelectUserList.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> SelectUserList(int id)
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.websettings.Value.DefaultPageSize;
            pagingRequestModel.SortColumn = "Id";
            pagingRequestModel.SortDirection = "A";
            pagingRequestModel.Filter = new List<PagingColumnFilter>();
            pagingRequestModel.PresetFilter = new List<PagingColumnFilter>
            {
                new PagingColumnFilter() { Column = "usergroupid_exclude", Value = id.ToString() },
            };

            var model = new TablePagingViewModel<UserAdminBasicViewModel>
            {
                Results = await this.userService.GetLHUserAdminBasicPageAsync(pagingRequestModel),
                SortColumn = "Id",
                SortDirection = "A",
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = 1,
                HasItems = model.Results != null && model.Results.Items.Any(),
                PageSize = this.websettings.Value.DefaultPageSize,
                TotalItems = model.Results != null ? model.Results.TotalItemCount : 0,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results != null ? model.Results.TotalItemCount : 0,
                DisplayedCount = model.Results != null ? model.Results.Items.Count() : 0,
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = 0,
                CreateRequired = false,
            };

            return this.PartialView("_UsersModal", model);
        }

        /// <summary>
        /// The SelectUserList.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> SelectUserList(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            requestModel.Sanitize();
            requestModel.PageSize = this.websettings.Value.DefaultPageSize;

            var model = new TablePagingViewModel<UserAdminBasicViewModel>
            {
                Results = await this.userService.GetLHUserAdminBasicPageAsync(requestModel),
                SortColumn = requestModel.SortColumn,
                SortDirection = requestModel.SortDirection,
                Filter = requestModel.Filter,
            };

            model.Paging = new PagingViewModel
            {
                CurrentPage = requestModel.Page,
                HasItems = model.Results.Items.Any(),
                PageSize = this.websettings.Value.DefaultPageSize,
                TotalItems = model.Results.TotalItemCount,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = model.Filter.Count(),
                CreateRequired = false,
            };

            return this.PartialView("_UsersModal", model);
        }

        /// <summary>
        /// The UserGroupCatalogues.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> UserGroupCatalogues(int id)
        {
            var vm = await this.userGroupService.GetUserGroupAdminRoleDetailByIdAsync(id);
            var catalogues = vm.Where(c => c.ScopeType == Nhs.Models.Enums.ScopeTypeEnum.Catalogue).ToList();

            return this.PartialView("_UserGroupCatalogues", catalogues);
        }

        private void ClearUserCachedPermissions(string userIdList)
        {
            if (!string.IsNullOrWhiteSpace(userIdList))
            {
                foreach (var userId in userIdList.Split(","))
                {
                    _ = Task.Run(async () => { await this.userService.ClearUserCachedPermissions(int.Parse(userId)); });
                }
            }
        }
    }
}