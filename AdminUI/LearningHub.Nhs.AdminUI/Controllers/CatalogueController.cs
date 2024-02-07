// <copyright file="CatalogueController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Drawing;
    using System.Linq;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Extensions;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Common.Enums;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="CatalogueController" />.
    /// </summary>
    [Route("[controller]")]
    public class CatalogueController : BaseController
    {
        /// <summary>
        /// Defines the CatalogueImageDirectory.
        /// </summary>
        private const string CatalogueImageDirectory = "CatalogueImageDirectory";

        /// <summary>
        /// Defines the RequestKey.
        /// </summary>
        private const string RequestKey = "CatalogueAdminsRequest";

        /// <summary>
        /// Defines the ViewModelKey.
        /// </summary>
        private const string ViewModelKey = "CatalogueAdminsViewModel";

        /// <summary>
        /// Defines the catalogueService.
        /// </summary>
        private readonly ICatalogueService catalogueService;

        /// <summary>
        /// Defines the userGroupService.
        /// </summary>
        private readonly IUserGroupService userGroupService;

        /// <summary>
        /// Defines the _fileService.
        /// </summary>
        private readonly IFileService fileService;

        /// <summary>
        /// Defines the providerService.
        /// </summary>
        private readonly IProviderService providerService;

        /// <summary>
        /// Defines the _settings.
        /// </summary>
        private readonly WebSettings settings;

        /// <summary>
        /// Defines the websettings.
        /// </summary>
        private readonly IOptions<WebSettings> websettings;

        /// <summary>
        /// Defines the logger.
        /// </summary>
        private ILogger<LogController> logger;

        /// <summary>
        /// Defines the _adminsAddPageSize.
        /// </summary>
        private int adminsAddPageSize = 6;

        /// <summary>
        /// Defines the _pageSize.
        /// </summary>
        private int pageSize = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="catalogueService">The catalogueService<see cref="ICatalogueService"/>.</param>
        /// <param name="userGroupService">The userService<see cref="IUserGroupService"/>.</param>
        /// <param name="fileService">The fileService<see cref="IFileService"/>.</param>
        /// <param name="providerService">The providerService<see cref="IProviderService"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{LogController}"/>.</param>
        /// <param name="options">The options<see cref="IOptions{WebSettings}"/>.</param>
        /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        public CatalogueController(
            IWebHostEnvironment hostingEnvironment,
            ICatalogueService catalogueService,
            IUserGroupService userGroupService,
            IFileService fileService,
            IProviderService providerService,
            ILogger<LogController> logger,
            IOptions<WebSettings> options,
            IOptions<WebSettings> websettings)
        : base(hostingEnvironment)
        {
            this.catalogueService = catalogueService;
            this.userGroupService = userGroupService;
            this.fileService = fileService;
            this.providerService = providerService;
            this.logger = logger;
            this.websettings = websettings;
            this.settings = options.Value;
        }

        /// <summary>
        /// The Add.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add()
        {
            var vm = new CatalogueViewModel();
            vm.Providers = await this.providerService.GetProviders();
            return this.View("Edit", vm);
        }

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="vm">The vm<see cref="CatalogueViewModel"/>.</param>
        /// <param name="badgeImage">The badgeImage<see cref="IFormFile"/>.</param>
        /// <param name="cardImage">The cardImage<see cref="IFormFile"/>.</param>
        /// <param name="bannerImage">The bannerImage<see cref="IFormFile"/>.</param>
        /// <param name="certificateImage">The certificateImage<see cref="IFormFile"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit(CatalogueViewModel vm, IFormFile badgeImage, IFormFile cardImage, IFormFile bannerImage, IFormFile certificateImage)
        {
            await this.UploadFile(badgeImage, name => vm.BadgeUrl = name);
            await this.UploadFile(cardImage, name => vm.CardImageUrl = name);
            await this.UploadFile(bannerImage, name => vm.BannerUrl = name);
            await this.UploadFile(certificateImage, name => vm.CertificateUrl = name);
            if (certificateImage != null)
            {
                this.ValidateUploadImage(new FileUploadValidationModel { FileSize = 50, Height = new int[] { 240, 160 }, Width = new int[] { 240 }, Extension = new string[] { ".png", ".jpg", ".gif", ".svg" }, Name = "CertificateUrl", Image = certificateImage });
            }

            this.ValidateCatalogueVm(vm);
            if (!this.ModelState.IsValid)
            {
                this.DeleteInvalidCertificateImage(vm, certificateImage);
                return this.View("Edit", vm);
            }

            try
            {
                var vr = await this.catalogueService.UpdateCatalogueAsync(vm);

                if (!vr.Success)
                {
                    this.ViewBag.ErrorMessage = $"Update failed: {((vr.ValidationResult == null) ? "no details available" : string.Join(",", vr.ValidationResult.Details))}";
                }
            }
            catch (Exception ex)
            {
                this.ViewBag.ErrorMessage = $"Error: {ex.Message}";
            }

            vm = await this.catalogueService.GetCatalogueAsync(vm.CatalogueNodeVersionId);
            return this.View("Edit", vm);
        }

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await this.catalogueService.GetCatalogueAsync(id);
            if (vm == null)
            {
                return this.RedirectToAction("Error");
            }

            return this.View(vm);
        }

        /// <summary>
        /// The Edit CatalogueOwner.
        /// </summary>
        /// <param name="catalogueOnerViewModel">The vm<see cref="CatalogueOwnerViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("EditCatalogueOwner")]
        public async Task<IActionResult> EditCatalogueOwner(CatalogueOwnerViewModel catalogueOnerViewModel)
        {
            this.ValidateCatalogueOwnerVm(catalogueOnerViewModel);
            if (!this.ModelState.IsValid)
            {
                var vmCat = await this.catalogueService.GetCatalogueAsync(catalogueOnerViewModel.CatalogueNodeVersionId);
                return this.View("CatalogueOwner", vmCat);
            }

            try
            {
                var vr = await this.catalogueService.UpdateCatalogueOwnerAsync(catalogueOnerViewModel);

                if (!vr.Success)
                {
                    this.ViewBag.ErrorMessage = $"Update failed: {((vr.ValidationResult == null) ? "no details available" : string.Join(",", vr.ValidationResult.Details))}";
                }
            }
            catch (Exception ex)
            {
                this.ViewBag.ErrorMessage = $"Error: {ex.Message}";
            }

            var vm = await this.catalogueService.GetCatalogueAsync(catalogueOnerViewModel.CatalogueNodeVersionId);
            return this.View("CatalogueOwner", vm);
        }

        /// <summary>
        /// The CatalogueOwner.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("CatalogueOwner/{id}")]
        public async Task<IActionResult> CatalogueOwner(int id)
        {
            var vm = await this.catalogueService.GetCatalogueAsync(id);
            if (vm == null)
            {
                return this.RedirectToAction("Error");
            }

            this.ViewData["CatalogueName"] = vm.Name;
            this.ViewData["id"] = id;

            return this.View(vm);
        }

        /// <summary>
        /// The UserGroups.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("UserGroups/{id}")]
        public async Task<IActionResult> UserGroups(int id)
        {
            var vm = await this.catalogueService.GetCatalogueAsync(id);
            if (vm == null)
            {
                return this.RedirectToAction("Error");
            }

            this.ViewData["id"] = id;
            this.ViewData["CatalogueName"] = vm.Name;
            return this.View(vm);
        }

        /// <summary>
        /// The Error.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("Error")]
        public async Task<IActionResult> Error()
        {
            this.ViewData["SupportUrl"] = this.settings.SupportForm;
            return this.View();
        }

        /// <summary>
        /// The Hide.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("Hide/{nodeId}")]
        public async Task<IActionResult> Hide(int nodeId)
        {
            await this.catalogueService.HideCatalogueAsync(nodeId);
            return this.Ok();
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="searchTerm">The searchTerm<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            var catalogues = await this.catalogueService.GetCataloguesAsync(searchTerm);

            // Filter out community catalogue
            catalogues = catalogues.Where(x => x.NodeId != 1).ToList();
            this.ViewData["searchTerm"] = searchTerm;
            return this.View(catalogues);
        }

        /// <summary>
        /// The PublishNew.
        /// </summary>
        /// <param name="vm">The vm<see cref="CatalogueViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("PublishNew")]
        public async Task<IActionResult> PublishNew(CatalogueViewModel vm)
        {
            vm.Status = Nhs.Models.Enums.VersionStatusEnum.Published;
            vm.Hidden = true;
            return await this.CreateCatalogue(vm);
        }

        /// <summary>
        /// The Folders.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("ContentStructure/{id}")]
        public async Task<IActionResult> ContentStructure(int id)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(id);
            this.ViewData["CatalogueName"] = catalogue.Name;
            this.ViewData["id"] = id;
            return this.View();
        }

        /// <summary>
        /// The Resources.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("Resources/{id}")]
        public async Task<IActionResult> Resources(int id)
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.pageSize;
            pagingRequestModel.SortColumn = "Id";
            pagingRequestModel.SortDirection = "D";
            pagingRequestModel.Filter = new List<PagingColumnFilter>();
            var vm = await this.catalogueService.GetResourcesAsync(id, pagingRequestModel);
            var model = new TablePagingViewModel<ResourceAdminSearchResultViewModel>
            {
                Results = vm.Resources,
                SortColumn = "Id",
                SortDirection = "D",
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = 1,
                HasItems = model.Results != null && model.Results.Items.Any(),
                PageSize = this.pageSize,
                TotalItems = model.Results != null ? model.Results.TotalItemCount : 0,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results != null ? model.Results.TotalItemCount : 0,
                DisplayedCount = model.Results != null ? model.Results.Items.Count() : 0,
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = 0,
                CreateRequired = false,
                NoResultsMessage = "There are no resources in this catalogue.",
            };

            var catalogue = await this.catalogueService.GetCatalogueAsync(id);

            this.ViewData["CatalogueName"] = catalogue.Name;
            this.ViewData["id"] = id;
            return this.View(model);
        }

        /// <summary>
        /// The Resources.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("Resources/{id}")]
        public async Task<IActionResult> Resources(int id, string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);
            requestModel.PageSize = this.pageSize;
            var vm = await this.catalogueService.GetResourcesAsync(id, requestModel);
            requestModel.PageSize = this.pageSize;

            var model = new TablePagingViewModel<ResourceAdminSearchResultViewModel>
            {
                Results = vm.Resources,
                SortColumn = requestModel.SortColumn,
                SortDirection = requestModel.SortDirection,
                Filter = requestModel.Filter,
            };

            model.Paging = new PagingViewModel
            {
                CurrentPage = requestModel.Page,
                HasItems = model.Results.Items.Any(),
                PageSize = this.pageSize,
                TotalItems = model.Results.TotalItemCount,
            };

            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = model.Filter.Count(),
                CreateRequired = false,
                NoResultsMessage = "There are no resources in this catalogue.",
            };

            this.ViewData["id"] = id;
            this.ViewData["CatalogueName"] = vm.CatalogueName;
            return this.View(model);
        }

        /// <summary>
        /// The Show.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("Show/{nodeId}")]
        public async Task<IActionResult> Show(int nodeId)
        {
            var vr = await this.catalogueService.ShowCatalogueAsync(nodeId);
            if (vr.ValidationResult.IsValid)
            {
                return this.Json(new
                {
                    success = true,
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.ValidationResult.Details.Count > 0 ? $"Cannot Show Catalogue:<br/>{string.Join("<br/>", vr.ValidationResult.Details)}" : $"Error showing catalogue.",
                });
            }
        }

        /// <summary>
        /// The UserGroupList.
        /// </summary>
        /// <param name="roleId">The id<see cref="int"/>.</param>
        /// <param name="catalogueNodeId">The catalogueNodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("RoleUserGroupList/{roleId}/{catalogueNodeId}")]
        public async Task<IActionResult> RoleUserGroupList(int roleId, int catalogueNodeId)
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.settings.DefaultPageSize;
            pagingRequestModel.SortColumn = "UserGroupId";
            pagingRequestModel.SortDirection = "A";
            pagingRequestModel.PresetFilter = new List<PagingColumnFilter>
            {
                new PagingColumnFilter() { Column = "roleid", Value = roleId.ToString() },
                new PagingColumnFilter() { Column = "cataloguenodeid", Value = catalogueNodeId.ToString() },
            };

            var model = new TablePagingViewModel<RoleUserGroupViewModel>
            {
                Results = await this.userGroupService.GetRoleUserGroupPageAsync(pagingRequestModel),
                SortColumn = "UserGroupId",
                SortDirection = "A",
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = 1,
                HasItems = model.Results != null && model.Results.Items.Any(),
                PageSize = this.settings.DefaultPageSize,
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
            return this.PartialView("_RoleUserGroups", model);
        }

        /// <summary>
        /// The UserGroupList.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("RoleUserGroupList")]
        public async Task<IActionResult> RoleUserGroupList(string pagingRequestModel)
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
            requestModel.PageSize = this.settings.DefaultPageSize;

            var model = new TablePagingViewModel<RoleUserGroupViewModel>
            {
                Results = await this.userGroupService.GetRoleUserGroupPageAsync(requestModel),
                SortColumn = requestModel.SortColumn,
                SortDirection = requestModel.SortDirection,
                Filter = requestModel.Filter,
            };

            model.Paging = new PagingViewModel
            {
                CurrentPage = requestModel.Page,
                HasItems = model.Results.Items.Any(),
                PageSize = this.settings.DefaultPageSize,
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

            return this.PartialView("_RoleUserGroups", model);
        }

        /// <summary>
        /// The SelectUserGroupList.
        /// </summary>
        /// <param name="roleId">The id<see cref="int"/>.</param>
        /// <param name="catalogueNodeId">The catalogueNodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("SelectUserGroupList/{roleId}/{catalogueNodeId}")]
        public async Task<IActionResult> SelectUserGroupList(int roleId, int catalogueNodeId)
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.settings.DefaultPageSize;
            pagingRequestModel.SortColumn = "Id";
            pagingRequestModel.SortDirection = "A";
            pagingRequestModel.Filter = new List<PagingColumnFilter>();

            pagingRequestModel.PresetFilter = new List<PagingColumnFilter>
            {
                new PagingColumnFilter() { Column = "roleid_exclude", Value = roleId.ToString() },
                new PagingColumnFilter() { Column = "cataloguenodeid_exclude", Value = catalogueNodeId.ToString() },
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
                PageSize = this.settings.DefaultPageSize,
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
        /// The SelectUserList.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("SelectUserGroupList")]
        public async Task<IActionResult> SelectUserGroupList(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            requestModel.Sanitize();
            requestModel.PageSize = this.settings.DefaultPageSize;

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
                PageSize = this.settings.DefaultPageSize,
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
        /// The AddUserGroupsToCatalogue.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId<see cref="int"/>.</param>
        /// <param name="roleId">The id<see cref="int"/>.</param>
        /// <param name="userGroupIdList">The userGroupIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("AddUserGroupsToCatalogue")]
        public async Task<IActionResult> AddUserGroupsToCatalogue(int catalogueNodeId, int roleId, string userGroupIdList)
        {
            var vr = await this.userGroupService.AddUserGroupsToCatalogue(catalogueNodeId, roleId, userGroupIdList);
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
                    details = vr.Details.Count > 0 ? vr.Details[0] : $"Error adding user groups to catalogue",
                });
            }
        }

        /// <summary>
        /// The CreateCatalogue.
        /// </summary>
        /// <param name="vm">The vm<see cref="CatalogueViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        private async Task<IActionResult> CreateCatalogue(
            CatalogueViewModel vm)
        {
            var files = this.Request.Form.Files;
            var bannerImage = files.SingleOrDefault(x => x.Name == "BannerImage");
            var cardImage = files.SingleOrDefault(x => x.Name == "CardImage");
            var badgeImage = files.SingleOrDefault(x => x.Name == "BadgeImage");
            var certificateImage = files.SingleOrDefault(x => x.Name == "CertificateImage");
            await this.UploadFile(badgeImage, name => vm.BadgeUrl = name);
            await this.UploadFile(cardImage, name => vm.CardImageUrl = name);
            await this.UploadFile(bannerImage, name => vm.BannerUrl = name);
            await this.UploadFile(certificateImage, name => vm.CertificateUrl = name);
            if (certificateImage != null)
            {
                this.ValidateUploadImage(new FileUploadValidationModel { FileSize = 50, Height = new int[] { 240, 160 }, Width = new int[] { 240 }, Extension = new string[] { ".png", ".jpg", ".gif", ".svg" }, Name = "CertificateUrl", Image = certificateImage });
            }

            this.ValidateCatalogueVm(vm);

            vm.Providers = await this.providerService.GetProviders();
            if (!this.ModelState.IsValid)
            {
                this.DeleteInvalidCertificateImage(vm, certificateImage);
                return this.View("Edit", vm);
            }

            try
            {
                var apiResponse = await this.catalogueService.CreateCatalogueAsync(vm);

                if (apiResponse.Success)
                {
                    if (apiResponse.ValidationResult.IsValid)
                    {
                        vm = await this.catalogueService.GetCatalogueAsync(apiResponse.ValidationResult.CreatedId.Value);
                        vm.Providers = await this.providerService.GetProviders();
                        return this.View("Edit", vm);
                    }
                    else
                    {
                        if (apiResponse.ValidationResult.Details.Contains("NameUnique"))
                        {
                            this.ModelState.AddModelError("Name", "A catalogue with this name already exists");
                        }

                        if (apiResponse.ValidationResult.Details.Contains("UrlUnique"))
                        {
                            this.ModelState.AddModelError("Url", "A catalogue with this url already exists");
                        }

                        return this.View("Edit", vm);
                    }
                }
                else
                {
                    this.ViewBag.ErrorMessage = $"Update failed: {((apiResponse.ValidationResult == null) ? "no details available" : string.Join(",", apiResponse.ValidationResult.Details))}";
                    return this.View("Edit", vm);
                }
            }
            catch (Exception ex)
            {
                this.ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return this.View("Edit", vm);
            }
        }

        /// <summary>
        /// The InitializeDefaultTable.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="results">The results<see cref="PagedResultSet{T}"/>.</param>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <param name="filter">The filter<see cref="List{PagingColumnFilter}"/>.</param>
        /// <param name="currentPage">The currentPage<see cref="int"/>.</param>
        /// <param name="sortColumn">The sortColumn<see cref="string"/>.</param>
        /// <param name="sortDirection">The sortDirection<see cref="string"/>.</param>
        /// <returns>The <see cref="TablePagingViewModel{T}"/>.</returns>
        private TablePagingViewModel<T> InitializeDefaultTable<T>(
            PagedResultSet<T> results,
            int pageSize,
            List<PagingColumnFilter> filter = null,
            int currentPage = 1,
            string sortColumn = "Id",
            string sortDirection = "D")
            where T : class, new()
        {
            if (filter == null)
            {
                filter = new List<PagingColumnFilter>();
            }

            var model = new TablePagingViewModel<T>
            {
                Results = results,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                Filter = filter,
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = currentPage,
                HasItems = model.Results != null && model.Results.Items.Any(),
                PageSize = pageSize,
                TotalItems = model.Results != null ? model.Results.TotalItemCount : 0,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results != null ? model.Results.TotalItemCount : 0,
                DisplayedCount = model.Results != null ? model.Results.Items.Count() : 0,
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = filter.Count,
                CreateRequired = false,
            };
            return model;
        }

        /// <summary>
        /// The InitializePagingRequest.
        /// </summary>
        /// <param name="pageSize">The pageSize<see cref="int"/>.</param>
        /// <returns>The <see cref="PagingRequestModel"/>.</returns>
        private PagingRequestModel InitializePagingRequest(int pageSize)
        {
            return new PagingRequestModel
            {
                Page = 1,
                PageSize = pageSize,
                SortColumn = "Id",
                SortDirection = "D",
                Filter = new List<PagingColumnFilter>(),
            };
        }

        /// <summary>
        /// The UploadFile.
        /// </summary>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        /// <param name="setImageName">The setImageName<see cref="Action{string}"/>.</param>
        private async Task UploadFile(IFormFile file, Action<string> setImageName)
        {
            if (file == null)
            {
                return;
            }

            var fileName = Guid.NewGuid().ToString();
            await this.fileService.ProcessFile(
                file.OpenReadStream(),
                fileName,
                CatalogueImageDirectory,
                file.FileName);
            setImageName(fileName);
        }

        /// <summary>
        /// The UploadFile.
        /// </summary>
        /// <param name="file">The file<see cref="IFormFile"/>.</param>
        private void ValidateUploadImage(FileUploadValidationModel file)
        {
            double fileSize = file.Image.Length / 1024;
            if (fileSize < 0 || fileSize > file.FileSize)
            {
                this.ModelState.AddModelError(file.Name, $"The file size of {fileSize}kB is not supported.");
            }

            string extn = System.IO.Path.GetExtension(file.Image.FileName);
            if (!(file.Extension.Any() && file.Extension.Contains(extn.ToLower())))
            {
                this.ModelState.AddModelError(file.Name, $"The file format of {extn.ToLower()} is not supported.");
            }

            int width = 0;
            int height = 0;
            try
            {
                using (var image = Image.FromStream(file.Image.OpenReadStream()))
                {
                    width = image.Width;
                    height = image.Height;
                }

                if (!(file.Width.Contains(width) && file.Height.Contains(height)))
                {
                    this.ModelState.AddModelError(file.Name, $"The file size of {width} x {height} is not supported.");
                }
            }
            catch (Exception)
            {
                if (!(file.Extension.Any() && file.Extension.Contains(extn.ToLower())))
                {
                    this.ModelState.AddModelError(file.Name, $"The file is not a valid image");
                }
            }
        }

        /// <summary>
        /// The ValidateCatalogueVm.
        /// </summary>
        /// <param name="vm">The vm<see cref="CatalogueViewModel"/>.</param>
        private void ValidateCatalogueVm(CatalogueViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Name))
            {
                this.ModelState.AddModelError("Name", "The name is required.");
            }
            else if (vm.Name.Length > 255)
            {
                this.ModelState.AddModelError("Name", "The name ha a maximum length of 255 characters.");
            }

            if (string.IsNullOrWhiteSpace(vm.Description))
            {
                this.ModelState.AddModelError("Description", "The description is required.");
            }

            if (string.IsNullOrWhiteSpace(vm.Url))
            {
                this.ModelState.AddModelError("Url", "The url is required.");
            }
            else if (vm.Url.Length > 1000)
            {
                this.ModelState.AddModelError("Url", "The url has a maximum length of 1000 characters.");
            }

            if (vm.Keywords == null || !vm.Keywords.Any() || vm.Keywords.All(x => x == null))
            {
                this.ModelState.AddModelError("Keywords", "At least one keyword must be entered.");
            }
            else if (vm.Keywords != null && vm.Keywords.Count > 5)
            {
                this.ModelState.AddModelError("Keywords", "A maximum of five keywords can be entered.");
            }

            var urlValidRegex = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9\\-]{0,}$");

            if (!urlValidRegex.IsMatch(vm.Url))
            {
                this.ModelState.AddModelError("Url", "The url may only contain letters, numbers or hyphens.");
            }
        }

        /// <summary>
        /// The DeleteInvalidCertificateImage.
        /// </summary>
        /// <param name="vm">The vm<see cref="CatalogueViewModel"/>.</param>
        /// <param name="file">The vm<see cref="IFormFile"/>.</param>
        private void DeleteInvalidCertificateImage(CatalogueViewModel vm, IFormFile file)
        {
            bool certError = this.ModelState.Keys.Any(e => e == "CertificateUrl" && this.ModelState[e].Errors.Count > 0);
            if (certError)
            {
                vm.CertificateUrl = string.Empty;
                file = null;
            }
        }

        /// <summary>
        /// The ValidateCatalogueOwnerVm.
        /// </summary>
        /// <param name="vm">The vm<see cref="CatalogueOwnerViewModel"/>.</param>
        private void ValidateCatalogueOwnerVm(CatalogueOwnerViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.OwnerName))
            {
                this.ModelState.AddModelError("OwnerName", "The owner name is required.");
            }
            else if (vm.OwnerName.Length > 250)
            {
                this.ModelState.AddModelError("OwnerName", "The owner name has a maximum length of 250 characters.");
            }

            if (string.IsNullOrWhiteSpace(vm.OwnerEmailAddress))
            {
                this.ModelState.AddModelError("OwnerEmailAddress", "The owner name is required.");
            }
            else if (vm.OwnerEmailAddress.Length > 250)
            {
                this.ModelState.AddModelError("OwnerEmailAddress", "The owner email address has a maximum length of 250 characters.");
            }
            else
            {
                Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                Match match = regex.Match(vm.OwnerEmailAddress);
                if (!match.Success)
                {
                    this.ModelState.AddModelError("OwnerEmailAddress", "The owner email address is not valid.");
                }
            }

            if (string.IsNullOrWhiteSpace(vm.Notes))
            {
                this.ModelState.AddModelError("Notes", "The notes are required.");
            }
        }
    }
}
