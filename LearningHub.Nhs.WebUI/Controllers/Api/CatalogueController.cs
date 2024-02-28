// <copyright file="CatalogueController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// CatalogueController.
    /// </summary>
    [Authorize]
    [Route("api")]
    [ApiController]
    public class CatalogueController : ControllerBase
    {
        private readonly ICatalogueService catalogueService;
        private readonly IFileService fileService;
        private readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueController"/> class.
        /// </summary>
        /// <param name="catalogueService">catalogueService.</param>
        /// <param name="fileService">fileService.</param>
        public CatalogueController(ICatalogueService catalogueService, IFileService fileService)
        {
            this.fileService = fileService;
            this.catalogueService = catalogueService;
            this.filePath = "CatalogueImageDirectory";
        }

        /// <summary>
        /// Gets Catalogue details.
        /// </summary>
        /// <param name="reference">reference.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("catalogue/{reference}")]
        public async Task<ActionResult> Get(string reference)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(reference);
            return this.Ok(catalogue);
        }

        /// <summary>
        /// Gets Catalogue details, records Node activity.
        /// </summary>
        /// <param name="reference">reference.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("catalogue-recorded/{reference}")]
        public async Task<ActionResult> GetRecorded(string reference)
        {
            var catalogue = await this.catalogueService.GetCatalogueRecordedAsync(reference);
            return this.Ok(catalogue);
        }

        /// <summary>
        /// GetResources.
        /// </summary>
        /// <param name="nodeId">nodeId.</param>
        /// <param name="catalogueOrder">catalogueOrder.</param>
        /// <param name="offset">offset.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("catalogue/resources/{nodeId}/{catalogueOrder}/{offset}")]
        public async Task<IActionResult> GetResources(int nodeId, CatalogueOrder catalogueOrder, int offset)
        {
            var requestViewModel = new CatalogueResourceRequestViewModel { NodeId = nodeId, CatalogueOrder = catalogueOrder, Offset = offset };
            var responseViewModel = await this.catalogueService.GetResourcesAsync(requestViewModel);
            return this.Ok(responseViewModel);
        }

        /// <summary>
        /// Downloads the catalogue media.
        /// </summary>
        /// <param name="fileName">fileName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [AllowAnonymous]
        [Route("catalogue/download-image/{fileName}")]
        public async Task<IActionResult> DownloadImage(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return this.BadRequest("Invalid file path");
            }

            var file = await this.fileService.DownloadFileAsync(this.filePath, fileName);
            if (file != null)
            {
                return this.File(file.Content, file.ContentType, fileName);
            }
            else
            {
                return this.Ok(this.Content("No file found"));
            }
        }

        /// <summary>
        /// The AccessDetails.
        /// </summary>
        /// <param name="reference">The catalogue reference.</param>
        /// <returns>The actionResult.</returns>
        [HttpGet("catalogue/GetAccessDetails/{reference}")]
        public async Task<IActionResult> AccessDetails(string reference)
        {
            return this.Ok(await this.catalogueService.AccessDetailsAsync(reference));
        }

        /// <summary>
        /// The GetLatestCatalogueAccessRequest.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogue node id.</param>
        /// <returns>The actionResult.</returns>
        [HttpGet("catalogue/GetLatestCatalogueAccessRequest/{catalogueNodeId}")]
        public async Task<IActionResult> GetLatestCatalogueAccessRequestAsync(int catalogueNodeId)
        {
            return this.Ok(await this.catalogueService.GetLatestCatalogueAccessRequestAsync(catalogueNodeId));
        }

        /// <summary>
        /// The RequestAccess.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="vm">The vm.</param>
        /// <returns>The actionResult.</returns>
        [HttpPost("catalogue/RequestAccess/{reference}")]
        public async Task<IActionResult> RequestAccess(string reference, CatalogueAccessRequestViewModel vm)
        {
            return this.Ok(await this.catalogueService.RequestAccessAsync(reference, vm, "access"));
        }

        /// <summary>
        /// The InviteUser.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns>The actionResult.</returns>
        [HttpPost("catalogue/InviteUser")]
        public async Task<IActionResult> InviteUser(RestrictedCatalogueInviteUserViewModel vm)
        {
            return this.Ok(await this.catalogueService.InviteUserAsync(vm));
        }

        /// <summary>
        /// Gets the restricted catalogue users.
        /// </summary>
        /// <param name="requestModel">The request model - filter settings.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("catalogue/GetRestrictedCatalogueUsers")]
        public async Task<ActionResult> GetRestrictedCatalogueUsers([FromBody] RestrictedCatalogueUsersRequestViewModel requestModel)
        {
            var activity = await this.catalogueService.GetRestrictedCatalogueUsersAsync(requestModel);

            return this.Ok(activity);
        }

        /// <summary>
        /// Gets Restricted Catalogue Summary.
        /// </summary>
        /// <param name="catalogueNodeId">catalogueNodeId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("catalogue/GetRestrictedCatalogueSummary/{catalogueNodeId}")]
        public async Task<ActionResult> GetRestrictedCatalogueSummary(int catalogueNodeId)
        {
            var catalogue = await this.catalogueService.GetRestrictedCatalogueSummary(catalogueNodeId);
            return this.Ok(catalogue);
        }

        /// <summary>
        /// The AccessRequests.
        /// </summary>
        /// <param name="requestModel">The requestModel.</param>
        /// <returns>The access requests.</returns>
        [HttpPost("catalogue/GetRestrictedCatalogueAccessRequests")]
        public async Task<IActionResult> GetRestrictedCatalogueAccessRequests([FromBody] RestrictedCatalogueAccessRequestsRequestViewModel requestModel)
        {
            return this.Ok(await this.catalogueService.GetRestrictedCatalogueAccessRequestsAsync(requestModel));
        }

        /// <summary>
        /// The AcceptAccessRequest.
        /// </summary>
        /// <param name="accessRequest">The accessRequest.</param>
        /// <returns>The action result.</returns>
        [HttpPost("catalogue/AcceptAccessRequest")]
        public async Task<IActionResult> AcceptAccessRequest([FromBody] CatalogueAccessRequestViewModel accessRequest)
        {
            return this.Ok(await this.catalogueService.AcceptAccessRequestAsync(accessRequest.Id, accessRequest.UserId));
        }

        /// <summary>
        /// The RejectAccessRequest.
        /// </summary>
        /// <param name="accessRequestId">The access request id.</param>
        /// <param name="vm">The vm.</param>
        /// <returns>The action result.</returns>
        [HttpPost("catalogue/RejectAccessRequest/{accessRequestId}")]
        public async Task<IActionResult> RejectAccessRequest(int accessRequestId, CatalogueAccessRejectionViewModel vm)
        {
            return this.Ok(await this.catalogueService.RejectAccessRequestAsync(accessRequestId, vm.RejectionReason));
        }

        /// <summary>
        /// The DismissAccessRequest.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>The validation result.</returns>
        [HttpPost("catalogue/DismissAccessRequest/{catalogueNodeId}")]
        public async Task<IActionResult> DismissAccessRequest(int catalogueNodeId)
        {
            return this.Ok(await this.catalogueService.DismissAccessRequestAsync(catalogueNodeId));
        }

        /// <summary>
        /// The AccessRequest.
        /// </summary>
        /// <param name="accessRequestId">The aaccessRequestId.</param>
        /// <returns>The result.</returns>
        [HttpGet("catalogue/AccessRequest/{accessRequestId}")]
        public async Task<IActionResult> AccessRequest(int accessRequestId)
        {
            return this.Ok(await this.catalogueService.GetCatalogueAccessRequestAsync(accessRequestId));
        }

        /// <summary>
        /// The RemoveUserFromRestrictedAccess.
        /// </summary>
        /// <param name="userUserGroupId">The user - user group id.</param>
        /// <returns>The action result.</returns>
        [HttpPost("catalogue/RemoveUserFromRestrictedAccessUserGroup/{userUserGroupId}")]
        public async Task<IActionResult> RemoveUserFromRestrictedAccessUserGroup(int userUserGroupId)
        {
            return this.Ok(await this.catalogueService.RemoveUserFromRestrictedAccessUserGroup(userUserGroupId));
        }
    }
}
