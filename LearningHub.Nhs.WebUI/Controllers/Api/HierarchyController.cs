namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// HierarchyController.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HierarchyController : BaseApiController
    {
        private readonly IHierarchyService hierarchyService;
        private readonly ICatalogueService catalogueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyController"/> class.
        /// </summary>
        /// <param name="hierarchyService">hierarchyService.</param>
        /// <param name="catalogueService">catalogueService.</param>
        public HierarchyController(IHierarchyService hierarchyService, ICatalogueService catalogueService)
            : base(null)
        {
            this.hierarchyService = hierarchyService;
            this.catalogueService = catalogueService;
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
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetActiveNodePathToNode/{nodeId}")]
        public async Task<IActionResult> GetActiveNodePathToNode(int nodeId)
        {
            var npl = await this.hierarchyService.GetNodePathsForNodeAsync(nodeId);
            return this.Ok(npl.First(np => np.IsActive));
        }

        /// <summary>
        /// Gets the contents of a node for the catalogue landing page - i.e. published folders and published resources only.
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="includeEmptyFolder">Include Empty Folder or not.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetNodeContentsForCatalogueBrowse/{nodeId}/{includeEmptyFolder}")]
        public async Task<ActionResult> GetNodeContentsForCatalogueBrowse(int nodeId, bool includeEmptyFolder)
        {
            List<NodeContentBrowseViewModel> viewModels = await this.hierarchyService.GetNodeContentsForCatalogueBrowse(nodeId, includeEmptyFolder);
            return this.Ok(viewModels);
        }

        /// <summary>
        /// Gets the contents of a node for the My Contributions page - i.e. published folders only, and all resources (i.e. all statuses).
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetNodeContentsForCatalogueEditor/{nodePathId}")]
        public async Task<IActionResult> GetNodeContentsForCatalogueEditor(int nodePathId)
        {
            List<NodeContentEditorViewModel> viewModels = await this.hierarchyService.GetNodeContentsForCatalogueEditor(nodePathId);
            return this.Ok(viewModels);
        }

        /// <summary>
        /// Check Catalogue has external reference.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("CheckCatalogueHasExternalReference/{nodeId}")]
        public async Task<bool> CheckCatalogueHasExternalReference(int nodeId)
        {
            var val = await this.hierarchyService.CheckCatalogueHasExternalReference(nodeId);
            return val;
        }

        /// <summary>
        /// Gets the contents of a node path (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// Set readOnly to true if read only data is needed.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetNodeContentsAdmin/{nodePathId}/{readOnly}")]
        public async Task<ActionResult> GetNodeContentsAdmin(int nodePathId, bool readOnly)
        {
            List<NodeContentAdminViewModel> viewModels = await this.hierarchyService.GetNodeContentsAdminAsync(nodePathId, readOnly);
            return this.Ok(viewModels);
        }

        /// <summary>
        /// Gets the latest hierarchy edit for the supplied root node path id.
        /// ** Note that WebAPI method returns a list of all HierarchyEdits
        /// For IT1, there may only be a single edit in 'Draft'.
        /// ** Future iterations may be required to handle multiple edits within a hierarchy branch.
        /// Interim IT1/IT2 controller implementation returns a list of HierarchyEdits.
        /// The first element is the hierarchy edit in draft (if one exists).
        /// The second element is the last published hierarchy edit (if one exists).
        /// </summary>
        /// <param name="rootNodePathId">The root node path id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetHierarchyEdit/{rootNodePathId}")]
        public async Task<ActionResult> GetHierarchyEditInDraft(int rootNodePathId)
        {
            var viewModels = await this.hierarchyService.GetHierarchyEdits(rootNodePathId);

            var hierarchyEditInDraft = viewModels == null ? null : viewModels.OrderByDescending(he => he.Id).FirstOrDefault();
            if (hierarchyEditInDraft != null)
            {
                if (!(hierarchyEditInDraft.HierarchyEditStatus == Nhs.Models.Enums.HierarchyEditStatusEnum.Draft
                    || hierarchyEditInDraft.HierarchyEditStatus == Nhs.Models.Enums.HierarchyEditStatusEnum.Publishing
                    || hierarchyEditInDraft.HierarchyEditStatus == Nhs.Models.Enums.HierarchyEditStatusEnum.SubmittedToPublishingQueue))
                {
                    hierarchyEditInDraft = null;
                }
            }

            var hierarchyEditLastPublished = viewModels == null ? null : viewModels.Where(he => he.HierarchyEditStatus == Nhs.Models.Enums.HierarchyEditStatusEnum.Published).OrderByDescending(he => he.Id).FirstOrDefault();

            var retVal = new List<HierarchyEditViewModel>();
            retVal.Add(hierarchyEditInDraft);
            retVal.Add(hierarchyEditLastPublished);

            return this.Ok(retVal);
        }

        /// <summary>
        /// The CreateHierarchyEdit.
        /// </summary>
        /// <param name="rootNodePathId">The rootNodePathId<see cref="int"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [Route("CreateHierarchyEdit/{rootNodePathId}")]
        public async Task<IActionResult> CreateHierarchyEditAsync(int rootNodePathId)
        {
            var apiResponse = await this.hierarchyService.CreateHierarchyEditAsync(rootNodePathId);
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

        /// <summary>
        /// The remove ReferenceNode.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchyEditDetailId<see cref="hierarchyEditDetailId"/>.</param>
        /// <returns>IActionResult.</returns>
        [Route("RemoveReferenceNode/{hierarchyEditDetailId}")]
        public async Task<IActionResult> RemoveReferenceNode(int hierarchyEditDetailId)
        {
            var apiResponse = await this.hierarchyService.RemoveReferenceNodeAsync(hierarchyEditDetailId);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="nodePathId">The nodePathId of the catalogue being edited<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet("GetReferencableCatalogues/{nodePathId}")]
        public async Task<IActionResult> GetRelatedCatalogues(int nodePathId)
        {
            var catalogue = await this.catalogueService.GetReferencableCataloguesAsync(nodePathId);
            return this.Ok(catalogue);
        }

        /// <summary>
        /// The ReferenceNode.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel<see cref="MoveNodeViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("ReferenceNode")]
        public async Task<IActionResult> ReferenceNode(MoveNodeViewModel moveNodeViewModel)
        {
            var apiResponse = await this.hierarchyService.ReferenceNodeAsync(moveNodeViewModel);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// References a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel<see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("HierarchyEditReferenceResource")]
        public async Task<IActionResult> HierarchyEditReferenceResource(HierarchyEditMoveResourceViewModel moveResourceViewModel)
        {
            var apiResponse = await this.hierarchyService.HierarchyEditReferenceResource(moveResourceViewModel);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// The ReferenceExternalNode.
        /// </summary>
        /// <param name="referenceExternalNodeViewModel">The moveNodeViewModel<see cref="MoveNodeViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("ReferenceExternalNode")]
        public async Task<IActionResult> ReferenceExternalNode(ReferenceExternalNodeViewModel referenceExternalNodeViewModel)
        {
            var apiResponse = await this.hierarchyService.ReferenceExternalNodeAsync(referenceExternalNodeViewModel);
            return this.Ok(apiResponse.ValidationResult);
        }

        /// <summary>
        /// References an external resource in a hierarchy edit.
        /// </summary>
        /// <param name="referenceExternalResourceViewModel">The referenceExternalResourceViewModel<see cref="ReferenceExternalResourceViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("HierarchyEditReferenceExternalResource")]
        public async Task<IActionResult> HierarchyEditReferenceExternalResource(ReferenceExternalResourceViewModel referenceExternalResourceViewModel)
        {
            var apiResponse = await this.hierarchyService.HierarchyEditReferenceExternalResource(referenceExternalResourceViewModel);
            return this.Ok(apiResponse.ValidationResult);
        }
    }
}
