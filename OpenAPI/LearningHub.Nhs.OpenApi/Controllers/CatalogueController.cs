namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Catalogue controller.
    /// </summary>
    [Route("Catalogue")]
    [Authorize]
    public class CatalogueController : OpenApiControllerBase
    {
        private readonly ICatalogueService catalogueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueController"/> class.
        /// </summary>
        /// <param name="catalogueService">The catalogue service.</param>
        public CatalogueController(ICatalogueService catalogueService)
        {
            this.catalogueService = catalogueService;
        }

        /// <summary>
        /// Get all catalogues.
        /// </summary>
        /// <returns>BulkCatalogueViewModel.</returns>
        [HttpGet]
        public async Task<Nhs.OpenApi.Models.ViewModels.BulkCatalogueViewModel> GetAllCatalogues()
        {
            return await this.catalogueService.GetAllCatalogues();
        }

        /// <summary>
        /// Gets AllCatalogues.
        /// </summary>
        /// <param name="filterChar">The filterChar.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("allcatalogues/{filterChar}")]
        public async Task<IActionResult> GetAllCataloguesAsync(string filterChar = null)
        {
            var response = await this.catalogueService.GetAllCataloguesAsync(filterChar, this.CurrentUserId.GetValueOrDefault());
            return this.Ok(response);
        }

        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="searchTerm">The searchTerm.</param>
        /// <returns>The catalogues.</returns>
        [HttpGet]
        [Route("Catalogues")]
        public IActionResult GetCatalogues([FromQuery] string? searchTerm)
        {
            var catalogues = this.catalogueService.GetCatalogues(searchTerm);
            return this.Ok(catalogues);
        }

        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="id">The catalogue node version id.</param>
        /// <returns>The catalogue.</returns>
        [HttpGet]
        [Route("Catalogues/{id}")]
        public async Task<IActionResult> GetCatalogue(int id)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(id);
            return this.Ok(catalogue);
        }

        /// <summary>
        /// The GetCatalogueResources.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The pageSize.</param>
        /// <param name="sortColumn">The sortColumn.</param>
        /// <param name="sortDirection">The sortDirection.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The catalogue resources.</returns>
        [HttpGet]
        [Route("Resources/{id}/{page}/{pageSize}/{sortColumn}/{sortDirection}/{filter}")]
        public async Task<IActionResult> GetCatalogueResources(int id, int page, int pageSize, string sortColumn, string sortDirection, string filter)
        {
            var catalogueResourceViewModel = await this.catalogueService.GetResourcesAsync(this.CurrentUserId.GetValueOrDefault(), id, page, pageSize, sortColumn, sortDirection, filter);
            return this.Ok(catalogueResourceViewModel);
        }

        /// <summary>
        /// GetResources.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <returns>The actionResult.</returns>
        [HttpPost]
        [Route("resources")]
        public async Task<IActionResult> GetResources(CatalogueResourceRequestViewModel requestViewModel)
        {
            var response = await this.catalogueService.GetResourcesAsync(requestViewModel);
            return this.Ok(response);
        }

        /// <summary>
        /// Get Catalogue by reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The catalogue.</returns>
        [HttpGet]
        [Route("catalogue/{reference}")]
        public async Task<IActionResult> GetCatalogueByReference(string reference)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(reference, this.CurrentUserId.GetValueOrDefault());
            return this.Ok(catalogue);
        }

        /// <summary>
        /// Get Catalogue by reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The catalogue.</returns>
        [HttpGet]
        [Route("catalogue-recorded/{reference}")]
        public async Task<IActionResult> GetCatalogueByReferenceRecorded(string reference)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(reference, this.CurrentUserId.GetValueOrDefault());
            return this.Ok(catalogue);
        }

        /// <summary>
        /// The published catalogues for current user.
        /// </summary>
        /// <returns>Published catalogues for current user.</returns>
        [HttpGet]
        [Route("GetForCurrentUser")]
        public IActionResult GetCataloguesForCurrentUser()
        {
            var catalogues = this.catalogueService.GetCataloguesForUser(this.CurrentUserId.GetValueOrDefault());
            return this.Ok(catalogues);
        }

        /// <summary>
        /// The CreateCatalogue.
        /// </summary>
        /// <param name="viewModel">The viewModel.</param>
        /// <returns>The actionResult.</returns>
        [HttpPost]
        [Route("Catalogues")]
        public async Task<IActionResult> CreateCatalogue([FromBody] CatalogueViewModel viewModel)
        {
            try
            {
                var vr = await this.catalogueService.CreateCatalogueAsync(this.CurrentUserId.GetValueOrDefault(), viewModel);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Returns true if the catalogue is editable by the current user.
        /// </summary>
        /// <param name="catalogueId">The catalogue id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("CanCurrentUserEditCatalogue/{catalogueId}")]
        public async Task<IActionResult> CanCurrentUserEditCatalogueAsync(int catalogueId)
        {
            return this.Ok(await this.catalogueService.CanUserEditCatalogueAsync(this.CurrentUserId.GetValueOrDefault(), catalogueId));
        }

        /// <summary>
        /// The InviteUser.
        /// </summary>
        /// <param name="vm">The view model.</param>
        /// <returns>The ActionResult.</returns>
        [HttpPost]
        [Route("InviteUser")]
        [Authorize]
        public async Task<IActionResult> InviteUser(RestrictedCatalogueInviteUserViewModel vm)
        {
            return this.Ok(await this.catalogueService.InviteUserAsync(this.CurrentUserId.GetValueOrDefault(), vm));
        }

        /// <summary>
        /// The RequestAccess.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="catalogueName">The catalogueName.</param>
        /// <param name="vm">The view model.</param>
        /// <param name="accessType">The accessType.</param>
        /// <returns>The ActionResult.</returns>
        [HttpPost]
        [Route("RequestAccess/{reference}/{catalogueName}/{accessType}")]
        [Authorize]
        public async Task<IActionResult> RequestAccess(string reference,string catalogueName, CatalogueAccessRequestViewModel vm, string accessType)
        {
            return this.Ok(await this.catalogueService.RequestAccessAsync(this.CurrentUserId.GetValueOrDefault(), reference, catalogueName, vm, accessType));
        }

        /// <summary>
        /// Gets the restricted catalogues summary for the supplied catalogue node id.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetRestrictedCatalogueSummary/{catalogueNodeId}")]
        public IActionResult GetRestrictedCatalogueSummary(int catalogueNodeId)
        {
            var activityModel = this.catalogueService.GetRestrictedCatalogueSummary(catalogueNodeId);
            return this.Ok(activityModel);
        }

        /// <summary>
        /// The AccessRequest.
        /// </summary>
        /// <param name="accessRequestId">The access request id.</param>
        /// <returns>The access request.</returns>
        [HttpGet("AccessRequest/{accessRequestId}")]
        [Authorize]
        public async Task<IActionResult> AccessRequest(int accessRequestId)
        {
            return this.Ok(await this.catalogueService.AccessRequestAsync(this.CurrentUserId.GetValueOrDefault(), accessRequestId));
        }

        /// <summary>
        /// The UpdateCatalogue.
        /// </summary>
        /// <param name="viewModel">The catalogue.</param>
        /// <returns>The updated catalogue.</returns>
        [HttpPut]
        [Route("Catalogues")]
        public async Task<IActionResult> UpdateCatalogue([FromBody] CatalogueViewModel viewModel)
        {
            try
            {
                var vr = await this.catalogueService.UpdateCatalogueAsync(this.CurrentUserId.GetValueOrDefault(), viewModel);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }


        /// <summary>
        /// The ShowCatalogue.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns>The actionResult.</returns>
        [HttpPost]
        [Route("ShowCatalogue")]
        public async Task<IActionResult> ShowCatalogue(CatalogueBasicViewModel vm)
        {
            try
            {
                var vr = await this.catalogueService.ShowCatalogueAsync(this.CurrentUserId.GetValueOrDefault(), vm.NodeId);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }


        /// <summary>
        /// The HideCatalogue.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns>The actionResult.</returns>
        [HttpPost]
        [Route("HideCatalogue")]
        public async Task<IActionResult> HideCatalogue(CatalogueBasicViewModel vm)
        {
            await this.catalogueService.HideCatalogueAsync(this.CurrentUserId.GetValueOrDefault(), vm.NodeId);
            return this.Ok();
        }


        /// <summary>
        /// The AccessDetails.
        /// </summary>
        /// <param name="reference">The catalogue reference.</param>
        /// <returns>The catalogue access details.</returns>
        [HttpGet]
        [Route("AccessDetails/{reference}")]
        public async Task<IActionResult> AccessDetailsAsync(string reference)
        {
            return this.Ok(await this.catalogueService.AccessDetailsAsync(this.CurrentUserId.GetValueOrDefault(), reference));
        }

        /// <summary>
        /// The GetCatalogueAccessRequests.
        /// </summary>
        /// <param name="catalogueNodeId">The request model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetCatalogueAccessRequests/{catalogueNodeId}")]
        public IActionResult GetCatalogueAccessRequests(int catalogueNodeId)
        {
            return this.Ok(this.catalogueService.GetCatalogueAccessRequests(catalogueNodeId, this.CurrentUserId.GetValueOrDefault()));
        }

        /// <summary>
        /// The GetLatestCatalogueAccessRequest.
        /// </summary>
        /// <param name="catalogueNodeId">The request model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetLatestCatalogueAccessRequest/{catalogueNodeId}")]
        public IActionResult GetLatestCatalogueAccessRequestAsync(int catalogueNodeId)
        {
            return this.Ok(this.catalogueService.GetLatestCatalogueAccessRequest(catalogueNodeId, this.CurrentUserId.GetValueOrDefault()));
        }

        /// <summary>
        /// Gets the restricted catalogues access requests for the supplied request view model.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("GetRestrictedCatalogueAccessRequests")]
        public IActionResult GetRestrictedCatalogueAccessRequests([FromBody] RestrictedCatalogueAccessRequestsRequestViewModel requestModel)
        {
            var activityModel = this.catalogueService.GetRestrictedCatalogueAccessRequests(requestModel);
            return this.Ok(activityModel);
        }

        /// <summary>
        /// Gets the restricted catalogues users for the supplied request view model.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("GetRestrictedCatalogueUsers")]
        public IActionResult GetRestrictedCatalogueUsers([FromBody] RestrictedCatalogueUsersRequestViewModel requestModel)
        {
            var activityModel = this.catalogueService.GetRestrictedCatalogueUsers(requestModel);
            return this.Ok(activityModel);
        }

        /// <summary>
        /// The RejectAccessRequest.
        /// </summary>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <param name="vm">The vm.</param>
        /// <returns>The action result.</returns>
        [HttpPost("RejectAccessRequest/{accessRequestId}")]
        [Authorize]
        public async Task<IActionResult> RejectAccessRequest(int accessRequestId, CatalogueAccessRejectionViewModel vm)
        {
            return this.Ok(await this.catalogueService.RejectAccessAsync(this.CurrentUserId.GetValueOrDefault(), accessRequestId, vm.RejectionReason));
        }


        /// <summary>
        /// The AcceptAccessRequest.
        /// </summary>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <returns>The action result.</returns>
        [HttpPost("AcceptAccessRequest/{accessRequestId}")]
        [Authorize]
        public async Task<IActionResult> AcceptAccessRequest(int accessRequestId)
        {
            return this.Ok(await this.catalogueService.AcceptAccessAsync(this.CurrentUserId.GetValueOrDefault(), accessRequestId));
        }

    }
}
