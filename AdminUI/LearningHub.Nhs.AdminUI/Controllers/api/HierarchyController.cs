﻿namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Controllers.Api;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.WebUI.Models.Contribute;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// HierarchyController.
    /// </summary>
    [Route("api/hierarchy")]
    [ApiController]
    public class HierarchyController : BaseApiController
    {
        private readonly IHierarchyService hierarchyService;
        private readonly ICatalogueService catalogueService;
        private readonly IConfiguration configuration;
        private WebSettings settings;
        private IFileService fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyController"/> class.
        /// </summary>
        /// <param name="hierarchyService">hierarchyService.</param>
        /// <param name="catalogueService">catalogueService.</param>
        /// <param name="configuration">configuration.</param>
        /// <param name="settings">settings.</param>
        /// <param name="fileService">fileService.</param>
        public HierarchyController(IHierarchyService hierarchyService, ICatalogueService catalogueService, IConfiguration configuration, IOptions<WebSettings> settings, IFileService fileService)
            : base(null)
        {
            this.hierarchyService = hierarchyService;
            this.catalogueService = catalogueService;
            this.configuration = configuration;
            this.fileService = fileService;
            this.settings = settings.Value;
        }

        /// <summary>
        /// The GetSettings.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        [Route("GetSettings")]
        public ActionResult GetSettings()
        {
            UploadSettingsModel settings = new UploadSettingsModel();
            settings.FileUploadSettings = this.settings.FileUploadSettings;
            return this.Ok(settings);
        }

        /// <summary>
        /// The GetCurrentUserId.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("GetCurrentUserId")]
        public IActionResult GetCurrentUserId()
        {
            return this.Ok(this.CurrentUserId);
        }

        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("GetCatalogue/{id}")]
        public async Task<IActionResult> GetCatalogue(int id)
        {
            var catalogue = await this.catalogueService.GetCatalogueAsync(id);
            return this.Ok(catalogue);
        }

        /// <summary>
        /// The GetFolder.
        /// </summary>
        /// <param name="nodeVersionId">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("GetFolder/{nodeVersionId}")]
        public async Task<IActionResult> GetFolder(int nodeVersionId)
        {
            var folder = await this.hierarchyService.GetFolderAsync(nodeVersionId);
            return this.Ok(folder);
        }

        /// <summary>
        /// The get active node path to node.
        /// IT1 - lookup for node path to the supplied node.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        [Route("GetActiveNodePathToNode/{nodeId}")]
        public async Task<IActionResult> GetActiveNodePathToNode(int nodeId)
        {
            var npl = await this.hierarchyService.GetNodePathsForNodeAsync(nodeId);
            return this.Ok(npl.First(np => np.IsActive));
        }

        /// <summary>
        /// Gets the contents of a node (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// Set readOnly to true if read only data is needed.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetNodeContentsAdmin/{nodeId}/{readOnly}")]
        public async Task<ActionResult> GetNodeContentsAdmin(int nodeId, bool readOnly)
        {
            List<NodeContentAdminViewModel> viewModels = await this.hierarchyService.GetNodeContentsAdminAsync(nodeId, readOnly);
            return this.Ok(viewModels);
        }

        /// <summary>
        /// Gets the latest hierarchy edit for the supplied root node id.
        /// ** Note that WebAPI method returns a list of all HierarchyEdits
        /// For IT1, there may only be a single edit in 'Draft'.
        /// ** Future iterations may be required to handle multiple edits within a hierarchy branch.
        /// Interim IT1 controller implementation returns a list of HierarchyEdits.
        /// The first element is the hierarchy edit in draft (if one exists).
        /// The second element is the last published hierarchy edit (if one exists).
        /// </summary>
        /// <param name="rootNodeId">The root node id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetHierarchyEdit/{rootNodeId}")]
        public async Task<ActionResult> GetHierarchyEditInDraft(int rootNodeId)
        {
            var viewModels = await this.hierarchyService.GetHierarchyEdits(rootNodeId);

            var hierarchyEditInDraft = viewModels == null ? null : viewModels.OrderByDescending(he => he.Id).FirstOrDefault();
            var hierarchyEditLastPublished = viewModels == null ? null : viewModels.Where(he => he.HierarchyEditStatus == Nhs.Models.Enums.HierarchyEditStatusEnum.Published).OrderByDescending(he => he.Id).FirstOrDefault();

            var retVal = new List<HierarchyEditViewModel>();
            retVal.Add(hierarchyEditInDraft);
            retVal.Add(hierarchyEditLastPublished);

            return this.Ok(retVal);
        }

        /// <summary>
        /// The CreateHierarchyEdit.
        /// </summary>
        /// <param name="rootNodeId">The rootNodeId<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("CreateHierarchyEdit/{rootNodeId}")]
        public async Task<IActionResult> CreateHierarchyEditAsync(int rootNodeId)
        {
            var apiResponse = await this.hierarchyService.CreateHierarchyEditAsync(rootNodeId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The DiscardHierarchyEdit.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchyEditId<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("DiscardHierarchyEdit/{hierarchyEditId}")]
        public async Task<IActionResult> DiscardHierarchyEdit(int hierarchyEditId)
        {
            var apiResponse = await this.hierarchyService.DiscardHierarchyEditAsync(hierarchyEditId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The PublishHierarchyEdit.
        /// </summary>
        /// <param name="publishViewModel">The hierarchyEditId<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("SubmitHierarchyEditForPublish")]
        public async Task<ActionResult> SubmitHierarchyEditForPublish([FromBody] PublishHierarchyEditViewModel publishViewModel)
        {
            var apiResponse = await this.hierarchyService.SubmitHierarchyEditForPublishAsync(publishViewModel);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The CreateFolder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("CreateFolder")]
        public async Task<IActionResult> CreateFolder(FolderEditViewModel folderEditViewModel)
        {
            var apiResponse = await this.hierarchyService.CreateFolderAsync(folderEditViewModel);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The UpdateFolder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("UpdateFolder")]
        public async Task<IActionResult> UpdateFolder(FolderEditViewModel folderEditViewModel)
        {
            var apiResponse = await this.hierarchyService.UpdateFolderAsync(folderEditViewModel);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The MoveNodeUp.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("MoveNodeUp/{hierarchyEditDetailId}")]
        public async Task<IActionResult> MoveNodeUp(int hierarchyEditDetailId)
        {
            var apiResponse = await this.hierarchyService.MoveNodeUp(hierarchyEditDetailId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The MoveNodeDown.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("MoveNodeDown/{hierarchyEditDetailId}")]
        public async Task<IActionResult> MoveNodeDown(int hierarchyEditDetailId)
        {
            var apiResponse = await this.hierarchyService.MoveNodeDown(hierarchyEditDetailId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The DeleteFolder.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("DeleteFolder/{hierarchyEditDetailId}")]
        public async Task<IActionResult> DeleteFolder(int hierarchyEditDetailId)
        {
            var apiResponse = await this.hierarchyService.DeleteFolder(hierarchyEditDetailId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The MoveNode.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel<see cref="MoveNodeViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("MoveNode")]
        public async Task<IActionResult> MoveNode(MoveNodeViewModel moveNodeViewModel)
        {
            var apiResponse = await this.hierarchyService.MoveNodeAsync(moveNodeViewModel);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// Moves a resource up in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("HierarchyEditMoveResourceUp/{hierarchyEditDetailId}")]
        public async Task<IActionResult> HierarchyEditMoveResourceUp(int hierarchyEditDetailId)
        {
            var apiResponse = await this.hierarchyService.HierarchyEditMoveResourceUp(hierarchyEditDetailId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// Moves a resource down in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("HierarchyEditMoveResourceDown/{hierarchyEditDetailId}")]
        public async Task<IActionResult> HierarchyEditMoveResourceDown(int hierarchyEditDetailId)
        {
            var apiResponse = await this.hierarchyService.HierarchyEditMoveResourceDown(hierarchyEditDetailId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// Moves a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel<see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("HierarchyEditMoveResource")]
        public async Task<IActionResult> HierarchyEditMoveResource(HierarchyEditMoveResourceViewModel moveResourceViewModel)
        {
            var apiResponse = await this.hierarchyService.HierarchyEditMoveResource(moveResourceViewModel);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The MoveResourceUp.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("MoveResourceUp/{nodeId}/{resourceId}")]
        public async Task<IActionResult> MoveResourceUp(int nodeId, int resourceId)
        {
            var apiResponse = await this.hierarchyService.MoveResourceUp(nodeId, resourceId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The MoveResourceDown.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("MoveResourceDown/{nodeId}/{resourceId}")]
        public async Task<IActionResult> MoveResourceDown(int nodeId, int resourceId)
        {
            var apiResponse = await this.hierarchyService.MoveResourceDown(nodeId, resourceId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The MoveResource.
        /// </summary>
        /// <param name="sourceNodeId">The id of the node to move the resource from.</param>
        /// <param name="destinationNodeId">The id of the node to move the resource to.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("MoveResource/{sourceNodeId}/{destinationNodeId}/{resourceId}")]
        public async Task<IActionResult> MoveResource(int sourceNodeId, int destinationNodeId, int resourceId)
        {
            var apiResponse = await this.hierarchyService.MoveResourceAsync(sourceNodeId, destinationNodeId, resourceId);
            return this.Ok(apiResponse.ValidationResult);
        }
    }
}
