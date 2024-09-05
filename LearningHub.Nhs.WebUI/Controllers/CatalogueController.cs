namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Catalogue;
    using LearningHub.Nhs.WebUI.Models.Search;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// CatalogueController.
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(LoginWizardFilter))]
    public class CatalogueController : BaseController
    {
        private readonly IDashboardService dashboardService;
        private readonly ISearchService searchService;
        private readonly ICacheService cacheService;
        private LearningHubAuthServiceConfig authConfig;
        private ICatalogueService catalogueService;
        private IUserService userService;
        private IUserGroupService userGroupService;
        private IHierarchyService hierarchyService;
        private Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">httpClientFactory.</param>
        /// <param name="hostingEnvironment">hostingEnvironment.</param>
        /// <param name="logger">logger.</param>
        /// <param name="catalogueService">catalogueService.</param>
        /// <param name="userService">userService.</param>
        /// <param name="settings">settings.</param>
        /// <param name="authConfig">authConfig.</param>
        /// <param name="searchService">searchService.</param>
        /// <param name="cacheService">cacheService.</param>
        /// <param name="dashboardService">Dashboard service.</param>
        /// <param name="hierarchyService">HierarchyService.</param>
        /// <param name="userGroupService">userGroupService.</param>
        public CatalogueController(
           IHttpClientFactory httpClientFactory,
           IWebHostEnvironment hostingEnvironment,
           ILogger<CatalogueController> logger,
           ICatalogueService catalogueService,
           IUserService userService,
           IOptions<Settings> settings,
           LearningHubAuthServiceConfig authConfig,
           ISearchService searchService,
           ICacheService cacheService,
           IDashboardService dashboardService,
           IHierarchyService hierarchyService,
           IUserGroupService userGroupService)
           : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.authConfig = authConfig;
            this.catalogueService = catalogueService;
            this.userService = userService;
            this.searchService = searchService;
            this.cacheService = cacheService;
            this.settings = settings.Value;
            this.dashboardService = dashboardService;
            this.hierarchyService = hierarchyService;
            this.userGroupService = userGroupService;
        }

        /// <summary>
        /// Catalogues.
        /// </summary>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="term">Search term.</param>
        /// <returns>IActionResult.</returns>
        [Route("/catalogues")]
        public async Task<IActionResult> Index(int pageIndex = 1, string term = null)
        {
            if (pageIndex < 1)
            {
                return this.Redirect("/catalogues");
            }

            var itemsOnPage = 9;
            var catalogues = new DashboardCatalogueResponseViewModel();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var termCatalogues = await this.searchService.GetCatalogueSearchResultAsync(
                    new CatalogueSearchRequestModel
                    {
                        SearchText = term,
                        PageIndex = pageIndex - 1,
                        PageSize = itemsOnPage,
                    });

                catalogues.TotalCount = termCatalogues.TotalHits;
                catalogues.Catalogues = termCatalogues.DocumentModel.Select(t => new DashboardCatalogueViewModel
                {
                    Url = t.Url,
                    Name = t.Name,
                    CardImageUrl = t.CardImageUrl,
                    BannerUrl = t.BannerUrl,
                    Description = t.Description,
                    RestrictedAccess = t.RestrictedAccess,
                    HasAccess = t.HasAccess,
                    IsBookmarked = t.IsBookmarked,
                    BookmarkId = t.BookmarkId,
                    NodeId = int.Parse(t.Id),
                    BadgeUrl = t.BadgeUrl,
                    Providers = t.Providers,
                }).ToList();
            }
            else
            {
                catalogues = await this.dashboardService.GetCataloguesAsync("all-catalogues", pageIndex);
            }

            if (System.Math.Ceiling(catalogues.TotalCount / (decimal)itemsOnPage) < pageIndex)
            {
                return this.Redirect("/catalogues");
            }

            this.ViewBag.PageIndex = pageIndex;
            this.ViewBag.ShowPrev = pageIndex != 1;
            this.ViewBag.ShowNext = catalogues.TotalCount > itemsOnPage * pageIndex;
            return this.View("Catalogues", catalogues);
        }

        /// <summary>
        /// Catalogue with authentication.
        /// Forces authentication.
        /// </summary>
        /// <param name="reference">Catalogue URL reference.</param>
        /// <returns>IActionResult.</returns>
        [Route("CatalogueWithAuthentication/{reference}")]
        public IActionResult CatalogueWithAuthentication(string reference)
        {
            this.ViewBag.UserAuthenticated = this.User.Identity.IsAuthenticated;
            return this.Redirect($"/catalogue/{reference}");
        }

        /// <summary>
        /// Index.
        /// </summary>
        /// <param name="reference">Catalogue URL reference.</param>
        /// <param name="tab">The tab name to display.</param>
        /// <param name="nodeId">The nodeId of the current folder. If not supplied, catalogue root contents are displayed.</param>
        /// <param name="search">The SearchRequestViewModel.</param>
        /// <returns>IActionResult.</returns>
        [AllowAnonymous]
        [ServiceFilter(typeof(SsoLoginFilterAttribute))]
        [HttpGet]
        [Route("catalogue/{reference}/{tab?}")]
        public async Task<IActionResult> IndexAsync(string reference, string tab, int? nodeId, SearchRequestViewModel search)
        {
            if (tab == null || (tab == "search" && !this.User.Identity.IsAuthenticated))
            {
                tab = "browse";
            }

            this.ViewBag.Reference = reference;
            this.ViewBag.UserAuthenticated = this.User.Identity.IsAuthenticated;
            this.ViewBag.SupportUrl = this.settings.SupportUrls.SupportForm;
            this.ViewBag.ShowCatalogueFieldsInResources = false;
            this.ViewBag.ActiveTab = tab;

            var catalogue = await this.catalogueService.GetCatalogueAsync(reference);

            if (catalogue == null)
            {
                return this.RedirectToAction("Error", "Home");
            }

            var userGroups = new List<RoleUserGroupViewModel>();
            CatalogueAccessRequestViewModel catalogueAccessRequest = null;
            if (this.ViewBag.UserAuthenticated)
            {
                userGroups = await this.userGroupService.GetRoleUserGroupDetailAsync();
                catalogueAccessRequest = await this.catalogueService.GetLatestCatalogueAccessRequestAsync(catalogue.NodeId);
            }

            var viewModel = new CatalogueIndexViewModel()
            {
                Catalogue = catalogue,
                UserGroups = userGroups.Where(x => x.CatalogueNodeId == catalogue.NodeId).ToList(),
                CatalogueAccessRequest = catalogueAccessRequest,
            };

            if (catalogue.Hidden == true)
            {
                if (!(viewModel.UserGroups.Any(x => x.RoleId == (int)RoleEnum.LocalAdmin || x.RoleId == (int)RoleEnum.Editor || x.RoleId == (int)RoleEnum.Previewer) || this.User.IsInRole("Administrator")))
                {
                    if (viewModel.UserGroups.Count == 0)
                    {
                        return this.View("NoAccess", catalogue);
                    }
                    else
                    {
                        return this.View("NoUserGroupPermission", catalogue);
                    }
                }
            }

            if (tab == "browse")
            {
                if (nodeId.HasValue)
                {
                    // if nodeId has a value it means the user is looking at a subfolder of the catalogue.
                    // Get the folder name and description, plus folder path data needed for the breadcrumbs.
                    viewModel.NodeDetails = await this.hierarchyService.GetNodeDetails(nodeId.Value);
                    viewModel.NodePathNodes = await this.hierarchyService.GetNodePathNodes(viewModel.NodeDetails.NodePathId);
                }
                else
                {
                    // Otherwise user is looking at catalogue root.
                    nodeId = catalogue.NodeId;

                    viewModel.NodePathNodes = new List<NodeViewModel>
                    {
                        new NodeViewModel { Name = catalogue.Name },
                    };
                }

                bool includeEmptyFolder = viewModel.UserGroups.Any(x => x.RoleId == (int)RoleEnum.LocalAdmin || x.RoleId == (int)RoleEnum.Editor || x.RoleId == (int)RoleEnum.Previewer) || this.User.IsInRole("Administrator");
                var nodeContents = await this.hierarchyService.GetNodeContentsForCatalogueBrowse(nodeId.Value, includeEmptyFolder);
                viewModel.NodeContents = nodeContents;
            }
            else if (tab == "search")
            {
                if (viewModel.SearchResults == null)
                {
                    viewModel.SearchResults = new Models.Search.SearchResultViewModel();
                }

                if (search.Term != null)
                {
                    search.CatalogueId = catalogue.NodeId;
                    search.SearchId ??= 0;
                    search.GroupId = !string.IsNullOrWhiteSpace(search.GroupId) && Guid.TryParse(search.GroupId, out Guid groupId) ? groupId.ToString() : Guid.NewGuid().ToString();

                    var searchResult = await this.searchService.PerformSearch(this.User, search);
                    searchResult.CatalogueId = catalogue.NodeId;
                    searchResult.CatalogueUrl = catalogue.Url;
                    if (search.SearchId == 0 && searchResult.ResourceSearchResult != null)
                    {
                        var searchId = await this.searchService.RegisterSearchEventsAsync(
                            search,
                            SearchFormActionTypeEnum.SearchWithinCatalogue,
                            searchResult.ResourceSearchResult.TotalHits);

                        searchResult.ResourceSearchResult.SearchId = searchId;
                    }

                    viewModel.SearchResults = searchResult;
                }
            }

            return this.View(viewModel);
        }

        /// <summary>
        /// Handles sort and filter functionality in Search tab.
        /// Based on SearchController.IndexPost method.
        /// </summary>
        /// <param name="reference">Catalogue URL reference.</param>
        /// <param name="tab">The tab name to display.</param>
        /// <param name="search">Search object.</param>
        /// <param name="resourceCount">The resource result count.</param>
        /// <param name="filters">The search filter.</param>
        /// <param name="sortby">The sort by.</param>
        /// <param name="groupId">The search group id.</param>
        /// <param name="searchId">The search id.</param>
        /// <returns>The actionResult.</returns>
        [HttpPost("catalogue/{reference}/{tab?}")]
        [AllowAnonymous]
        public async Task<IActionResult> IndexPost(string reference, string tab, [FromQuery] SearchRequestViewModel search, int resourceCount, [FromForm] IEnumerable<string> filters, [FromForm] int? sortby, [FromForm] string groupId, [FromForm] int searchId)
        {
            if (search.ResourcePageIndex > 0 && search.Filters?.Any() == true)
            {
                var existingFilters = search.Filters.OrderBy(t => t);
                var newFilters = filters.OrderBy(t => t);
                if (!newFilters.SequenceEqual(existingFilters))
                {
                    search.ResourcePageIndex = null;
                }
            }

            search.Filters = filters;
            search.Sortby = sortby;
            search.GroupId = groupId;
            search.SearchId = searchId;

            await this.searchService.RegisterSearchEventsAsync(search, SearchFormActionTypeEnum.ApplyFilter, resourceCount);

            var routeValues = new RouteValueDictionary(search);
            routeValues.Add("tab", tab);
            routeValues.Add("reference", reference);

            return this.RedirectToAction("index", routeValues);
        }

        /// <summary>
        /// Records analytics events for navigation between pages of results on the search tab.
        /// Based on SearchController.RecordResourceNavigation method.
        /// </summary>
        /// <param name="search">Search object.</param>
        /// <param name="resourceCount">The resource result count.</param>
        /// <param name="reference">The catalogue reference.</param>
        /// <param name="tab">The active tab name.</param>
        /// <returns>The actionResult.</returns>
        [Route("catalogue/record-search-navigation")]
        public async Task<IActionResult> RecordSearchNavigation(SearchRequestViewModel search, int resourceCount, string reference, string tab)
        {
            await this.searchService.RegisterSearchEventsAsync(search, SearchFormActionTypeEnum.ResourceNextPageChange, resourceCount);

            var routeValues = new RouteValueDictionary(search);
            routeValues.Add("tab", tab);
            routeValues.Add("reference", reference);

            return this.RedirectToAction("index", routeValues);
        }

        /// <summary>
        /// Manage.
        /// </summary>
        /// <param name="reference">Catalogue URL reference.</param>
        /// <returns>IActionResult.</returns>
        [Route("catalogue/manage/{reference}")]
        public IActionResult Manage(string reference)
        {
            this.ViewBag.Reference = reference;
            this.ViewBag.UserAuthenticated = this.User.Identity.IsAuthenticated;
            this.ViewBag.SupportUrl = this.settings.SupportUrls.SupportForm + "/2";
            return this.View("Manage");
        }

        /// <summary>
        /// AccessRequests.
        /// </summary>
        /// <param name="reference">Catalogue URL reference.</param>
        /// <returns>IActionResult.</returns>
        [Route("catalogue/manage/{reference}/accessRequests")]
        public IActionResult AccessRequests(string reference)
        {
            this.ViewBag.Reference = reference;
            this.ViewBag.UserAuthenticated = this.User.Identity.IsAuthenticated;
            this.ViewBag.SupportUrl = this.settings.SupportUrls.SupportForm + "/2";
            return this.View("Manage");
        }

        /// <summary>
        /// AccessRequest.
        /// </summary>
        /// <param name="reference">Catalogue URL reference.</param>
        /// <param name="accessRequestId">Access request id.</param>
        /// <returns>IActionResult.</returns>
        [Route("catalogue/manage/{reference}/accessRequest/{accessRequestId}")]
        public IActionResult AccessRequest(string reference, string accessRequestId)
        {
            this.ViewBag.Reference = reference;
            this.ViewBag.UserAuthenticated = this.User.Identity.IsAuthenticated;
            this.ViewBag.SupportUrl = this.settings.SupportUrls.SupportForm + "/2";
            return this.View("Manage");
        }

        /// <summary>
        /// Display screen to enable user to request access to a catalogue.
        /// </summary>
        /// <param name="catalogueNodeVersionId">The catalogueNodeVersionId.</param>
        /// <param name="returnUrl">The returnUrl.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("catalogue/RequestAccess/{catalogueNodeVersionId}")]
        public async Task<IActionResult> RequestAccess(int catalogueNodeVersionId, string returnUrl = null)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(catalogueNodeVersionId);
            var catalogueAccessRequest = await this.catalogueService.GetLatestCatalogueAccessRequestAsync(catalogue.NodeId);
            var currentUser = await this.userService.GetCurrentUserBasicDetailsAsync();

            return this.View("RequestAccess", new CatalogueRequestAccessViewModel
            {
                CatalogueNodeId = catalogue.NodeId,
                CatalogueName = catalogue.Name,
                CatalogueUrl = catalogue.Url,
                CatalogueAccessRequest = catalogueAccessRequest,
                CurrentUser = currentUser,
                ReturnUrl = returnUrl ?? this.Request.Headers["Referer"],
            });
        }

        /// <summary>
        /// Creates a catalogue access request and displays the confirmation screen.
        /// </summary>
        /// <param name="viewModel">The CatalogueRequestAccessViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("catalogue/RequestAccessPost")]
        public async Task<IActionResult> RequestAccess(CatalogueRequestAccessViewModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                var validationResult = await this.catalogueService.RequestAccessAsync(viewModel.CatalogueUrl, new CatalogueAccessRequestViewModel() { Message = viewModel.AccessRequestMessage, RoleId = (int)RoleEnum.Reader }, "access");

                if (validationResult.IsValid)
                {
                    return this.View(
                        "AccessRequested",
                        new CatalogueAccessRequestedViewModel
                        {
                            CatalogueName = viewModel.CatalogueName,
                            CatalogueUrl = viewModel.CatalogueUrl,
                        });
                }
                else
                {
                    return this.Redirect("/Home/Error");
                }
            }
            else
            {
                return this.View("RequestAccess", viewModel);
            }
        }

        /// <summary>
        /// Display screen to enable user to request access to a catalogue.
        /// </summary>
        /// <param name="catalogueNodeVersionId">The catalogueNodeVersionId.</param>
        /// <param name="returnUrl">The returnUrl.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("catalogue/RequestPermission/{catalogueNodeVersionId}")]
        public async Task<IActionResult> RequestPermission(int catalogueNodeVersionId, string returnUrl = null)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(catalogueNodeVersionId);
            var catalogueAccessRequest = await this.catalogueService.GetLatestCatalogueAccessRequestAsync(catalogue.NodeId);
            var currentUser = await this.userService.GetCurrentUserBasicDetailsAsync();

            return this.View("RequestPermission", new CatalogueRequestAccessViewModel
            {
                CatalogueNodeId = catalogue.NodeId,
                CatalogueName = catalogue.Name,
                CatalogueUrl = catalogue.Url,
                CatalogueAccessRequest = catalogueAccessRequest,
                CurrentUser = currentUser,
                ReturnUrl = returnUrl ?? this.Request.Headers["Referer"],
            });
        }

        /// <summary>
        /// Display screen to enable user to request access to a catalogue.
        /// </summary>
        /// <param name="catalogueNodeVersionId">The catalogueNodeVersionId.</param>
        /// <param name="returnUrl">The returnUrl.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("catalogue/RequestPreviewAccess/{catalogueNodeVersionId}")]
        public async Task<IActionResult> RequestPreviewAccess(int catalogueNodeVersionId, string returnUrl = null)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(catalogueNodeVersionId);
            var catalogueAccessRequest = await this.catalogueService.GetLatestCatalogueAccessRequestAsync(catalogue.NodeId);
            var currentUser = await this.userService.GetCurrentUserBasicDetailsAsync();

            return this.View("RequestPreviewAccess", new CatalogueRequestAccessViewModel
            {
                CatalogueNodeId = catalogue.NodeId,
                CatalogueName = catalogue.Name,
                CatalogueUrl = catalogue.Url,
                CatalogueAccessRequest = catalogueAccessRequest,
                CurrentUser = currentUser,
                ReturnUrl = returnUrl ?? this.Request.Headers["Referer"],
            });
        }

        /// <summary>
        /// Creates a catalogue access request and displays the confirmation screen.
        /// </summary>
        /// <param name="viewModel">The CatalogueRequestAccessViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("catalogue/RequestPermissionPost")]
        public async Task<IActionResult> RequestPermission(CatalogueRequestAccessViewModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                var validationResult = await this.catalogueService.RequestAccessAsync(viewModel.CatalogueUrl, new CatalogueAccessRequestViewModel() { Message = viewModel.AccessRequestMessage, RoleId = (int)RoleEnum.Previewer }, "permission");

                if (validationResult.IsValid)
                {
                    return this.View(
                        "PermissionRequested",
                        new CatalogueAccessRequestedViewModel
                        {
                            CatalogueName = viewModel.CatalogueName,
                            CatalogueUrl = viewModel.CatalogueUrl,
                        });
                }
                else
                {
                    return this.Redirect("/Home/Error");
                }
            }
            else
            {
                return this.View("RequestPermission", viewModel);
            }
        }

        /// <summary>
        /// Creates a catalogue access request and displays the confirmation screen.
        /// </summary>
        /// <param name="viewModel">The CatalogueRequestAccessViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("catalogue/RequestPreviewAccessPost")]
        public async Task<IActionResult> RequestPreviewAccess(CatalogueRequestAccessViewModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                var validationResult = await this.catalogueService.RequestAccessAsync(viewModel.CatalogueUrl, new CatalogueAccessRequestViewModel() { Message = viewModel.AccessRequestMessage, RoleId = (int)RoleEnum.Previewer }, "access");

                if (validationResult.IsValid)
                {
                    return this.View(
                        "PreviewAccessRequested",
                        new CatalogueAccessRequestedViewModel
                        {
                            CatalogueName = viewModel.CatalogueName,
                            CatalogueUrl = viewModel.CatalogueUrl,
                        });
                }
                else
                {
                    return this.Redirect("/Home/Error");
                }
            }
            else
            {
                return this.View("RequestPreviewAccess", viewModel);
            }
        }

        /// <summary>
        /// Get all catelogues, filter and pagination based on alphabets.
        /// </summary>
        /// <param name="filterChar">filterChar.</param>
        /// <returns>rk.</returns>
        [Route("/allcatalogue")]
        [Route("/allcatalogue/{filterChar}")]
        public async Task<IActionResult> GetAllCatalogue(string filterChar = "a")
        {
            var pageSize = this.settings.AllCataloguePageSize;
            var catalogues = await this.catalogueService.GetAllCatalogueAsync(filterChar, pageSize);
            return this.View("allcatalogue", catalogues);
        }

        /// <summary>
        /// AllCatalogues Search.
        /// </summary>
        /// <param name="pageIndex">pageIndex.</param>
        /// <param name="term">Search term.</param>
        /// <returns>IActionResult.</returns>
        [Route("/allcataloguesearch")]
        public async Task<IActionResult> GetAllCatalogueSearch(int pageIndex = 1, string term = null)
        {
            var catalogues = new AllCatalogueSearchResponseViewModel();
            var searchString = term?.Trim() ?? string.Empty;
            var allCatalogueSearchPageSize = this.settings.FindwiseSettings.AllCatalogueSearchPageSize;

            if (!string.IsNullOrWhiteSpace(term))
            {
                var termCatalogues = await this.searchService.GetAllCatalogueSearchResultAsync(
                    new AllCatalogueSearchRequestModel
                    {
                        SearchText = searchString,
                        PageIndex = pageIndex - 1,
                        PageSize = allCatalogueSearchPageSize,
                    });

                catalogues.TotalCount = termCatalogues.TotalHits;
                catalogues.Catalogues = termCatalogues.DocumentModel.Select(t => new AllCatalogueViewModel
                {
                    Url = t.Url,
                    Name = t.Name,
                    CardImageUrl = t.CardImageUrl,
                    BannerUrl = t.BannerUrl,
                    Description = t.Description,
                    RestrictedAccess = t.RestrictedAccess,
                    HasAccess = t.HasAccess,
                    IsBookmarked = t.IsBookmarked,
                    BookmarkId = t.BookmarkId,
                    NodeId = int.Parse(t.Id),
                    BadgeUrl = t.BadgeUrl,
                    Providers = t.Providers,
                }).ToList();
            }
            else
            {
                catalogues.TotalCount = 0;
                catalogues.Catalogues = new List<AllCatalogueViewModel>();
            }

            this.ViewBag.PageIndex = pageIndex;
            return this.View("AllCatalogueSearch", catalogues);
        }
    }
}