namespace LearningHub.Nhs.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The HierarchyController. Provides functionality around the content hierarchy - retrieving node contents for treeviews,
    /// creating/moving/deleting/reordering folders, moving/reordering resources, etc.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("api/[controller]")]
    [ApiController]
    public class HierarchyController : ApiControllerBase
    {
        private readonly IHierarchyService hierarchyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyController"/> class.
        /// </summary>
        /// <param name="userService">The userService.</param>
        /// <param name="hierarchyService">The hierarchyService.</param>
        /// <param name="logger">The logger.</param>
        public HierarchyController(IUserService userService, IHierarchyService hierarchyService, ILogger<HierarchyController> logger)
            : base(userService, logger)
        {
            this.hierarchyService = hierarchyService;
        }

        /////// <summary>
        /////// Gets the basic details of a node. Currently catalogues or folders.
        /////// </summary>
        /////// <param name="nodeId">The node id.</param>
        /////// <returns>The node details.</returns>
        ////[HttpGet]
        ////[Route("GetNodeDetails/{nodeId}")]
        ////public NodeViewModel GetNodeDetails(int nodeId)
        ////{
        ////    var retVal = this.hierarchyService.GetNodeDetails(nodeId);

        ////    return retVal;
        ////}

        /// <summary>
        /// Gets the basic details of a node path. Currently catalogues or folders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <returns>The node details.</returns>
        [HttpGet]
        [Route("GetNodePathDetails/{nodePathId}")]
        public NodePathViewModel GetNodePathDetails(int nodePathId)
        {
            var retVal = this.hierarchyService.GetNodePathDetails(nodePathId);

            return retVal;
        }

        /// <summary>
        /// Gets the basic details of all Nodes in a particular NodePath.
        /// </summary>
        /// <param name="nodePathId">The NodePath id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetNodePathNodes/{nodePathId}")]
        public async Task<List<NodePathViewModel>> GetNodePathNodes(int nodePathId)
        {
            var retVal = await this.hierarchyService.GetNodePathNodes(nodePathId);

            return retVal;
        }

        /// <summary>
        /// Gets the contents of a node path for the catalogue landing page - i.e. published folders and published resources only.
        /// Only returns the items found directly in the specified node path, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="includeEmptyFolder">Include Empty Folder or not.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetNodeContentsForCatalogueBrowse/{nodePathId}/{includeEmptyFolder}")]
        public async Task<IActionResult> GetNodeContentsForCatalogueBrowse(int nodePathId, bool includeEmptyFolder)
        {
            return this.Ok(await this.hierarchyService.GetNodeContentsForCatalogueBrowse(nodePathId, includeEmptyFolder));
        }

        /// <summary>
        /// Gets the contents of a node for the My Contributions page - i.e. published folders only, and all resources (i.e. all statuses).
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetNodeContentsForCatalogueEditor/{nodePathId}")]
        public async Task<IActionResult> GetNodeContentsForCatalogueEditor(int nodePathId)
        {
            return this.Ok(await this.hierarchyService.GetNodeContentsForCatalogueEditor(nodePathId));
        }

        /// <summary>
        /// Gets the contents of a node path (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetNodeContentsAdmin/{nodePathId}/{readOnly}")]
        public async Task<IActionResult> GetNodeContentsAdmin(int nodePathId, bool readOnly)
        {
            return this.Ok(await this.hierarchyService.GetNodeContentsAdminAsync(nodePathId, readOnly));
        }

        /// <summary>
        /// Gets hierarchy edit detail for the supplied root node path id.
        /// </summary>
        /// <param name="rootNodePathId">The root node path id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetHierarchyEdits/{rootNodePathId}")]
        public async Task<IActionResult> GetHierarchyEdits(int rootNodePathId)
        {
            return this.Ok(await this.hierarchyService.GetHierarchyEdits(rootNodePathId));
        }

        /// <summary>
        /// The get node resource lookup.
        /// IT1 - quick lookup for whether a node has published resources.
        /// Services Content Structure Admin.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetNodeResourceLookup/{nodeId}")]
        public async Task<IActionResult> GetNodeResourceLookupAsync(int nodeId)
        {
            return this.Ok(await this.hierarchyService.GetNodeResourceLookupAsync(nodeId));
        }

        /// <summary>
        /// The get node paths for node.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GetNodePathsForNode/{nodeId}")]
        public async Task<IActionResult> GetNodePathsForNodeAsync(int nodeId)
        {
            return this.Ok(await this.hierarchyService.GetNodePathsForNodeAsync(nodeId));
        }

        /// <summary>
        /// Creates hierarchy edit detail for the supplied root node path id.
        /// </summary>
        /// <param name="rootNodePathId">The root node path id.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPut]
        [Route("CreateHierarchyEdit/{rootNodePathId}")]
        public async Task<IActionResult> CreateHierarchyEdit(int rootNodePathId)
        {
            try
            {
                int hierarchyEditId = await this.hierarchyService.CreateHierarchyEdit(rootNodePathId, this.CurrentUserId);
                var vr = new LearningHubValidationResult()
                {
                    IsValid = true,
                    CreatedId = hierarchyEditId,
                };

                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Discards hierarchy edit detail for the supplied root node id.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit view model.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPut]
        [Route("DiscardHierarchyEdit/{hierarchyEditId}")]
        public async Task<IActionResult> DiscardHierarchyEdit(int hierarchyEditId)
        {
            try
            {
                var retVal = await this.hierarchyService.DiscardHierarchyEdit(hierarchyEditId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("CreateFolder")]
        public async Task<IActionResult> CreateFolder(FolderEditViewModel folderEditViewModel)
        {
            try
            {
                var retVal = await this.hierarchyService.CreateFolder(folderEditViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Updates a folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("UpdateFolder")]
        public async Task<IActionResult> UpdateFolder(FolderEditViewModel folderEditViewModel)
        {
            try
            {
                var retVal = await this.hierarchyService.UpdateFolder(folderEditViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Updates a folder reference.
        /// </summary>
        /// <param name="nodePathDisplayVersionModel">The nodePathDisplayVersionModel<see cref="NodePathDisplayVersionModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("UpdateNodePathDisplayVersion")]
        public async Task<IActionResult> UpdateNodePathDisplayVersion(NodePathDisplayVersionModel nodePathDisplayVersionModel)
        {
            try
            {
                var retVal = await this.hierarchyService.UpdateNodePathDisplayVersionAsync(nodePathDisplayVersionModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Updates a resource reference.
        /// </summary>
        /// <param name="resourceReferenceDisplayVersionModel">The resourceReferenceDisplayVersionModel<see cref="ResourceReferenceDisplayVersionModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("UpdateResourceReferenceDisplayVersion")]
        public async Task<IActionResult> UpdateResourceReferenceDisplayVersion(ResourceReferenceDisplayVersionModel resourceReferenceDisplayVersionModel)
        {
            try
            {
                var retVal = await this.hierarchyService.UpdateResourceReferenceDisplayVersionAsync(resourceReferenceDisplayVersionModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// The DeleteFolder.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <returns>The validation result.</returns>
        [HttpPut]
        [Route("DeleteFolder/{hierarchyEditDetailId}")]
        public async Task<IActionResult> DeleteFolder(int hierarchyEditDetailId)
        {
            try
            {
                var retVal = await this.hierarchyService.DeleteFolder(hierarchyEditDetailId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// The GetFolder.
        /// </summary>
        /// <param name="nodeVersionId">The id.</param>
        /// <returns>The catalogue.</returns>
        [HttpGet]
        [Route("GetFolder/{nodeVersionId}")]
        public async Task<IActionResult> GetFolder(int nodeVersionId)
        {
            var folder = await this.hierarchyService.GetFolderAsync(nodeVersionId);
            return this.Ok(folder);
        }

        /// <summary>
        /// The MoveNodeUp.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <returns>The validation result.</returns>
        [HttpPut]
        [Route("MoveNodeUp/{hierarchyEditDetailId}")]
        public async Task<IActionResult> MoveNodeUp(int hierarchyEditDetailId)
        {
            try
            {
                var retVal = await this.hierarchyService.MoveNodeUp(hierarchyEditDetailId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// The MoveNodeDown.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <returns>The validation result.</returns>
        [HttpPut]
        [Route("MoveNodeDown/{hierarchyEditDetailId}")]
        public async Task<IActionResult> MoveNodeDown(int hierarchyEditDetailId)
        {
            try
            {
                var retVal = await this.hierarchyService.MoveNodeDown(hierarchyEditDetailId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Moves a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel<see cref="MoveNodeViewModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("MoveNode")]
        public async Task<IActionResult> MoveNode(MoveNodeViewModel moveNodeViewModel)
        {
            try
            {
                var retVal = await this.hierarchyService.MoveNode(moveNodeViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Create reference to a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel<see cref="MoveNodeViewModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("ReferenceNode")]
        public async Task<IActionResult> ReferenceNode(MoveNodeViewModel moveNodeViewModel)
        {
            try
            {
                var retVal = await this.hierarchyService.ReferenceNode(moveNodeViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Create reference to an external node.
        /// </summary>
        /// <param name="referenceExternalNodeViewModel">The eeferenceExternalNodeViewModel<see cref="ReferenceExternalNodeViewModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("ReferenceExternalNode")]
        public async Task<IActionResult> ReferenceExternalNode(ReferenceExternalNodeViewModel referenceExternalNodeViewModel)
        {
            try
            {
                var retVal = await this.hierarchyService.ReferenceExternalNode(referenceExternalNodeViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Moves a resource up in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <returns>The validation result.</returns>
        [HttpPut]
        [Route("HierarchyEditMoveResourceUp/{hierarchyEditDetailId}")]
        public async Task<IActionResult> HierarchyEditMoveResourceUp(int hierarchyEditDetailId)
        {
            try
            {
                var retVal = await this.hierarchyService.HierarchyEditMoveResourceUp(hierarchyEditDetailId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Moves a resource down in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <returns>The validation result.</returns>
        [HttpPut]
        [Route("HierarchyEditMoveResourceDown/{hierarchyEditDetailId}")]
        public async Task<IActionResult> HierarchyEditMoveResourceDown(int hierarchyEditDetailId)
        {
            try
            {
                var retVal = await this.hierarchyService.HierarchyEditMoveResourceDown(hierarchyEditDetailId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Moves a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel<see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("HierarchyEditMoveResource")]
        public async Task<IActionResult> HierarchyEditMoveResource(HierarchyEditMoveResourceViewModel moveResourceViewModel)
        {
            try
            {
                var retVal = await this.hierarchyService.HierarchyEditMoveResource(moveResourceViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// References a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel<see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("HierarchyEditReferenceResource")]
        public async Task<IActionResult> HierarchyEditReferenceResource(HierarchyEditMoveResourceViewModel moveResourceViewModel)
        {
            try
            {
                var retVal = await this.hierarchyService.HierarchyEditReferenceResource(moveResourceViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// References an external resource in a hierarchy edit.
        /// </summary>
        /// <param name="referenceExternalResourceViewModel">The referenceExternalResourceViewModel<see cref="ReferenceExternalResourceViewModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("HierarchyEditReferenceExternalResource")]
        public async Task<IActionResult> HierarchyEditReferenceExternalResource(ReferenceExternalResourceViewModel referenceExternalResourceViewModel)
        {
            try
            {
                var retVal = await this.hierarchyService.HierarchyEditReferenceExternalResource(referenceExternalResourceViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// The MoveResourceUp.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPut]
        [Route("MoveResourceUp/{nodeId}/{resourceId}")]
        public async Task<IActionResult> MoveResourceUp(int nodeId, int resourceId)
        {
            try
            {
                var retVal = await this.hierarchyService.MoveResourceUp(nodeId, resourceId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// The MoveResourceDown.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPut]
        [Route("MoveResourceDown/{nodeId}/{resourceId}")]
        public async Task<IActionResult> MoveResourceDown(int nodeId, int resourceId)
        {
            try
            {
                var retVal = await this.hierarchyService.MoveResourceDown(nodeId, resourceId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// The MoveResource.
        /// </summary>
        /// <param name="sourceNodeId">The id of the node to move the resource from.</param>
        /// <param name="destinationNodeId">The id of the node to move the resource to.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPut("MoveResource/{sourceNodeId}/{destinationNodeId}/{resourceId}")]
        public async Task<IActionResult> MoveResource(int sourceNodeId, int destinationNodeId, int resourceId)
        {
            try
            {
                var retVal = await this.hierarchyService.MoveResource(sourceNodeId, destinationNodeId, resourceId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Submit HierarchyEdit For Publish.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPut]
        [Route("SubmitHierarchyEditForPublish")]
        public async Task<IActionResult> SubmitHierarchyEditForPublish(PublishHierarchyEditViewModel publishViewModel)
        {
            try
            {
                publishViewModel.UserId = this.CurrentUserId;
                var vr = await this.hierarchyService.SubmitHierarchyEditForPublish(publishViewModel);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Publish HierarchyEdit
        ///   TODO - requires validation.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("PublishHierarchyEdit")]
        public async Task<IActionResult> PublishHierarchyEdit(PublishHierarchyEditViewModel publishViewModel)
        {
            try
            {
                int publicationId = await this.hierarchyService.PublishHierarchyEditAsync(publishViewModel);

                /* TODO - IT2 - confirm submission to Findwise (why here rather than in service method just called?)
                if (publicationId > 0)
                {
                    this.hierarchyService.ConfirmSubmissionToSearch(publicationId, publishViewModel.UserId);
                } */
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }

            return this.Ok(new ApiResponse(true, new LearningHubValidationResult(true)));
        }

        /// <summary>
        /// Set HierarchyEdit to Publishing.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("HierarchyEditPublishing")]
        public IActionResult HierarchyEditPublishing(PublishHierarchyEditViewModel publishViewModel)
        {
            try
            {
                var vr = this.hierarchyService.SetHierarchyEditPublishing(publishViewModel);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Set HierarchyEdit to Publish Failed.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("HierarchyEditFailedToPublish")]
        public IActionResult HierarchyEditFailedToPublish(PublishHierarchyEditViewModel publishViewModel)
        {
            try
            {
                var vr = this.hierarchyService.SetHierarchyEditFailedToPublish(publishViewModel);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// remove a resource reference in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <returns>The validation result.</returns>
        [HttpPut]
        [Route("RemoveReferenceNode/{hierarchyEditDetailId}")]
        public async Task<IActionResult> RemoveReferenceNode(int hierarchyEditDetailId)
        {
            try
            {
                var retVal = await this.hierarchyService.RemoveReferenceNode(hierarchyEditDetailId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Deletes the node reference details.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail identifier.</param>
        /// <returns>The action result.</returns>
        [HttpPut]
        [Route("DeleteNodeReferenceDetails/{hierarchyEditDetailId}")]
        public async Task<IActionResult> DeleteNodeReferenceDetails(int hierarchyEditDetailId)
        {
            try
            {
                var retVal = await this.hierarchyService.DeleteNodeReferenceDetails(hierarchyEditDetailId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Deletes the resource reference details for a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The ID of the hierarchy edit detail.</param>
        /// <returns>An asynchronous task that represents the operation.</returns>
        [HttpPut]
        [Route("DeleteResourceReferenceDetails/{hierarchyEditDetailId}")]
        public async Task<IActionResult> DeleteResourceReferenceDetails(int hierarchyEditDetailId)
        {
            try
            {
                var retVal = await this.hierarchyService.DeleteResourceReferenceDetails(hierarchyEditDetailId, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, retVal));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }
    }
}
