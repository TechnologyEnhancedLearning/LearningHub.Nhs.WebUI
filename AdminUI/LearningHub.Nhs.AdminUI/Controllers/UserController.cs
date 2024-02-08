namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Enums;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Extensions;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Common.Enums;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="UserController" />.
    /// </summary>
    public class UserController : BaseController
    {
        /// <summary>
        /// Defines the userService.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Defines the userGroupService.
        /// </summary>
        private readonly IUserGroupService userGroupService;

        /// <summary>
        /// Defines the providerService.
        /// </summary>
        private readonly IProviderService providerService;

        /// <summary>
        /// Defines the websettings.
        /// </summary>
        private readonly IOptions<WebSettings> websettings;

        /// <summary>
        /// Defines the logger.
        /// </summary>
        private ILogger<LogController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="userService">The userService<see cref="IUserService"/>.</param>
        /// <param name="userGroupService">The userGroupService<see cref="IUserGroupService"/>.</param>
        /// <param name="providerService">The providerService<see cref="IProviderService"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{LogController}"/>.</param>
        /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        public UserController(
            IWebHostEnvironment hostingEnvironment,
            IUserService userService,
            IUserGroupService userGroupService,
            IProviderService providerService,
            ILogger<LogController> logger,
            IOptions<WebSettings> websettings)
        : base(hostingEnvironment)
        {
            this.userService = userService;
            this.userGroupService = userGroupService;
            this.providerService = providerService;
            this.logger = logger;
            this.websettings = websettings;
        }

        /// <summary>
        /// The Contact.
        /// </summary>
        /// <param name="contactViewModel">The contactViewModel<see cref="ContactViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel contactViewModel)
        {
            if (this.ModelState.IsValid)
            {
                await this.userService.SendEmailToUserAsync(contactViewModel.EmailAddress, contactViewModel.Subject, contactViewModel.Message, contactViewModel.UserId);
                this.TempData["StatusMessage"] = $"An email has been sent to {contactViewModel.EmailAddress}";
            }

            // TODO: Use PRG
            var userId = contactViewModel.UserId;
            var userDetails = await this.userService.GetUserByUserId(userId);

            // get user details, build vm
            return this.View(new ContactViewModel
            {
                EmailAddress = userDetails.EmailAddress,
                UserId = userId,
            });
        }

        /// <summary>
        /// The Contact.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Contact(int userId)
        {
            var userDetails = await this.userService.GetUserByUserId(userId);

            // get user details, build vm
            return this.View(new ContactViewModel
            {
                EmailAddress = userDetails.EmailAddress,
                UserId = userId,
            });
        }

        /// <summary>
        /// The Contributions.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Contributions(int id)
        {
            var userContributions = await this.userService.GetUserContributionsAsync(id);
            return this.PartialView("_Contributions", userContributions);
        }

        /// <summary>
        /// The Details.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await this.userService.GetUserAdminDetailbyIdAsync(id);
            this.ViewBag.Providers = await this.GetProviders(id);
            return this.View(user);
        }

        /// <summary>
        /// The Details.
        /// </summary>
        /// <param name="user">The user<see cref="UserAdminDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Details(UserAdminDetailViewModel user)
        {
            this.ViewBag.Providers = await this.GetProviders(user.Id);

            if (!this.ModelState.IsValid)
            {
                this.ViewBag.ErrorMessage = "Update failed. Please fix the following error(s):";
                return this.View("Details", user);
            }
            else
            {
                LearningHubValidationResult validationResult = await this.userService.UpdateUser(user);

                if (validationResult.IsValid)
                {
                    if (user.OriginalEmailAddress.ToLower() != user.EmailAddress.Trim().ToLower())
                    {
                        UserHistoryViewModel userHistory = new UserHistoryViewModel()
                        {
                            UserId = user.Id,
                            Detail = "User primary email address updated.",
                            UserHistoryTypeId = (int)UserHistoryType.UserDetails,
                        };
                        await this.userService.StoreUserHistory(userHistory);
                    }

                    user = await this.userService.GetUserAdminDetailbyIdAsync(user.Id);
                    this.ViewBag.SuccessMessage = "Update completed successfully";
                    return this.View("Details", user);
                }
                else
                {
                    this.ViewBag.ErrorMessage = $"Update failed: {string.Join(Environment.NewLine, validationResult.Details)}";
                    return this.View("Details", user);
                }
            }
        }

        /// <summary>
        /// The History.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> History(int id)
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.websettings.Value.DefaultPageSize;
            pagingRequestModel.SortColumn = "UserHistoryId";
            pagingRequestModel.SortDirection = "D";
            pagingRequestModel.PresetFilter = new List<PagingColumnFilter>
            {
                new PagingColumnFilter() { Column = "userid", Value = id.ToString(), },
            };

            var model = new TablePagingViewModel<UserHistoryViewModel>
            {
                Results = await this.userService.GetUserHistoryPageAsync(pagingRequestModel),
                SortColumn = "UserHistoryId",
                SortDirection = "D",
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
            return this.PartialView("_History", model);
        }

        /// <summary>
        /// The History.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> History(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            if (string.IsNullOrEmpty(requestModel.SortColumn))
            {
                requestModel.SortColumn = "History";
            }

            if (string.IsNullOrEmpty(requestModel.SortDirection))
            {
                requestModel.SortDirection = "D";
            }

            requestModel.PageSize = this.websettings.Value.DefaultPageSize;
            requestModel.Sanitize();
            var model = new TablePagingViewModel<UserHistoryViewModel>
            {
                Results = await this.userService.GetUserHistoryPageAsync(requestModel),
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

            return this.PartialView("_History", model);
        }

        /// <summary>
        /// The User learning records.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> UserLearningRecord(int id)
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.websettings.Value.DefaultPageSize;
            pagingRequestModel.SortColumn = "ActivityStart";
            pagingRequestModel.SortDirection = "D";
            pagingRequestModel.PresetFilter = new List<PagingColumnFilter>
            {
                new PagingColumnFilter() { Column = "userid", Value = id.ToString(), },
            };

            var model = new TablePagingViewModel<MyLearningDetailedItemViewModel>
            {
                Results = await this.userService.GetUserLearningRecordsAsync(pagingRequestModel),
                SortColumn = "ActivityStart",
                SortDirection = "D",
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
            Tuple<TablePagingViewModel<MyLearningDetailedItemViewModel>, string> userLearningData = Tuple.Create(model, id.ToString());
            return this.PartialView("_UserLearningRecord", userLearningData);
        }

        /// <summary>
        /// The UserLearningRecord.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> UserLearningRecord(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            if (string.IsNullOrEmpty(requestModel.SortColumn))
            {
                requestModel.SortColumn = "ActivityStart";
            }

            if (string.IsNullOrEmpty(requestModel.SortDirection))
            {
                requestModel.SortDirection = "D";
            }

            requestModel.PageSize = this.websettings.Value.DefaultPageSize;
            requestModel.Sanitize();
            var model = new TablePagingViewModel<MyLearningDetailedItemViewModel>
            {
                Results = await this.userService.GetUserLearningRecordsAsync(requestModel),
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

            Tuple<TablePagingViewModel<MyLearningDetailedItemViewModel>, string> userLearningData = Tuple.Create(model, requestModel.PresetFilter[0].Value);
            return this.PartialView("_UserLearningRecord", userLearningData);
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.websettings.Value.DefaultPageSize;
            pagingRequestModel.SortColumn = "Id";
            pagingRequestModel.SortDirection = "A";
            pagingRequestModel.Filter = new List<PagingColumnFilter>();

            var model = new TablePagingViewModel<elfhHub.Nhs.Models.Common.UserAdminBasicViewModel>
            {
                Results = await this.userService.GetUserAdminBasicPageAsync(pagingRequestModel),
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
            return this.View(model);
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            requestModel.Sanitize();
            requestModel.PageSize = this.websettings.Value.DefaultPageSize;

            var model = new TablePagingViewModel<elfhHub.Nhs.Models.Common.UserAdminBasicViewModel>
            {
                Results = await this.userService.GetUserAdminBasicPageAsync(requestModel),
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

            return this.View(model);
        }

        /// <summary>
        /// The SendAdminPasswordResetEmail.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> SendAdminPasswordResetEmail([FromBody] int userId)
        {
            await this.userService.SendAdminPasswordResetEmail(userId);

            return this.Json(new
            {
                success = true,
                details = $"Successfully sent password reset email",
            });
        }

        /// <summary>
        /// The ClearUserCachedPermissions.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> ClearUserCachedPermissions([FromBody] int userId)
        {
            await this.userService.ClearUserCachedPermissions(userId);

            return this.Json(new
            {
                success = true,
                details = $"Successfully cleared cached permissions for user {userId}",
            });
        }

        /// <summary>
        /// The UserUserGroupList.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> UserUserGroupList(int id)
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.websettings.Value.DefaultPageSize;
            pagingRequestModel.SortColumn = "UserGroupId";
            pagingRequestModel.SortDirection = "A";
            pagingRequestModel.PresetFilter = new List<PagingColumnFilter>
            {
                new PagingColumnFilter() { Column = "userid", Value = id.ToString(), },
            };

            var model = new TablePagingViewModel<UserUserGroupViewModel>
            {
                Results = await this.userGroupService.GetUserUserGroupPageAsync(pagingRequestModel),
                SortColumn = "UserGroupId",
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
            return this.PartialView("_UserUserGroups", model);
        }

        /// <summary>
        /// The UserUserGroupList.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> UserUserGroupList(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            if (string.IsNullOrEmpty(requestModel.SortColumn))
            {
                requestModel.SortColumn = "UserGroupId";
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

            return this.PartialView("_UserUserGroups", model);
        }

        /// <summary>
        /// The SelectUserGroupList.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> SelectUserGroupList(int id)
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.websettings.Value.DefaultPageSize;
            pagingRequestModel.SortColumn = "Id";
            pagingRequestModel.SortDirection = "A";
            pagingRequestModel.Filter = new List<PagingColumnFilter>();
            pagingRequestModel.PresetFilter = new List<PagingColumnFilter>
            {
                new PagingColumnFilter() { Column = "userid_exclude", Value = id.ToString(), },
            };

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

            return this.PartialView("_UserGroupsModal", model);
        }

        /// <summary>
        /// The SelectUserGroupList.
        /// </summary>
        /// <param name="pagingRequestModel">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> SelectUserGroupList(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

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

            return this.PartialView("_UserGroupsModal", model);
        }

        /// <summary>
        /// The AddUserGroupsToUser.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="userGroupIdList">The userGroupIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> AddUserGroupsToUser(int userId, string userGroupIdList)
        {
            var vr = await this.userService.AddUserGroupsToUser(userId, userGroupIdList);
            if (vr.IsValid)
            {
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
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error adding user groups to user",
                });
            }
        }

        /// <summary>
        /// update user providers.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="providerIdList">The providerIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserProviders(int userId, string providerIdList)
        {
            var vr = await this.providerService.UpdateUserProviderAsync(userId, providerIdList);

            if (vr.IsValid)
            {
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
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error updating providers to user",
                });
            }
        }

        /// <summary>
        /// get providers select list item.
        /// </summary>
        /// <returns>list of select list items.</returns>
        private async Task<List<SelectListItem>> GetProviders(int userId)
        {
            var providers = await this.providerService.GetProviders();
            var userproviders = await this.providerService.GetProvidersByUserIdAsync(userId);

            return providers.Select(q => new SelectListItem() { Value = q.Id.ToString(), Text = q.Name, Selected = userproviders.Any(n => n.Id == q.Id) })
                                                                .ToList();
        }
    }
}
