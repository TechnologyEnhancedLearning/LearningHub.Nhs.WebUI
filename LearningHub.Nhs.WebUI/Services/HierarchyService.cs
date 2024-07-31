namespace LearningHub.Nhs.WebUI.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The catalogue service.
    /// </summary>
    public class HierarchyService : BaseService<HierarchyService>, IHierarchyService
    {
        /// <summary>
        /// Defines the _facade.
        /// </summary>
        private readonly ILearningHubApiFacade facade;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learning hub http client.</param>
        /// <param name="learningHubApiFacade">The learningHubApiFacade<see cref="ILearningHubApiFacade"/>.</param>
        /// <param name="logger">The logger.</param>
        public HierarchyService(
            ILearningHubHttpClient learningHubHttpClient,
            ILearningHubApiFacade learningHubApiFacade,
            ILogger<HierarchyService> logger)
        : base(learningHubHttpClient, logger)
        {
            this.facade = learningHubApiFacade;
        }

        /////// <summary>
        /////// Gets the basic details of a node. Currently catalogues or folders.
        /////// </summary>
        /////// <param name="nodeId">The node id.</param>
        /////// <returns>The <see cref="Task"/>.</returns>
        ////public async Task<NodeViewModel> GetNodeDetails(int nodeId)
        ////{
        ////    return await this.facade.GetAsync<NodeViewModel>($"Hierarchy/GetNodeDetails/{nodeId}");
        ////}

        /// <inheritdoc />
        public async Task<NodePathViewModel> GetNodePathDetails(int nodePathId)
        {
            return await this.facade.GetAsync<NodePathViewModel>($"Hierarchy/GetNodePathDetails/{nodePathId}");
        }

        /// <summary>
        /// Gets the basic details of all Nodes in a particular NodePath.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodePathViewModel>> GetNodePathNodes(int nodePathId)
        {
            return await this.facade.GetAsync<List<NodePathViewModel>>($"Hierarchy/GetNodePathNodes/{nodePathId}");
        }

        /// <summary>
        /// Gets the contents of a node path for the catalogue landing page - i.e. published folders and published resources only.
        /// Only returns the items found directly in the specified node path, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="includeEmptyFolder">Include Empty Folder or not.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeContentBrowseViewModel>> GetNodeContentsForCatalogueBrowse(int nodePathId, bool includeEmptyFolder)
        {
            return await this.facade.GetAsync<List<NodeContentBrowseViewModel>>($"Hierarchy/GetNodeContentsForCatalogueBrowse/{nodePathId}/{includeEmptyFolder}");
        }

        /// <summary>
        /// Gets the contents of a node for the My Contributions page - i.e. published folders only, and all resources (i.e. all statuses).
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeContentEditorViewModel>> GetNodeContentsForCatalogueEditor(int nodePathId)
        {
            return await this.facade.GetAsync<List<NodeContentEditorViewModel>>($"Hierarchy/GetNodeContentsForCatalogueEditor/{nodePathId}");
        }

        /// <inheritdoc />
        public async Task<List<NodeContentAdminViewModel>> GetNodeContentsAdminAsync(int nodePathId, bool readOnly)
        {
            return await this.facade.GetAsync<List<NodeContentAdminViewModel>>($"Hierarchy/GetNodeContentsAdmin/{nodePathId}/{readOnly}");
        }

        /// <summary>
        /// The get node paths.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>List of NodePathViewModel.</returns>
        public async Task<List<NodePathViewModel>> GetNodePathsForNodeAsync(int nodeId)
        {
            return await this.facade.GetAsync<List<NodePathViewModel>>($"Hierarchy/GetNodePathsForNode/{nodeId}");
        }

        /// <summary>
        /// Gets the hierarchy edits for the supplied root node path id.
        /// </summary>
        /// <param name="rootNodePathId">The root node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<HierarchyEditViewModel>> GetHierarchyEdits(int rootNodePathId)
        {
            return await this.facade.GetAsync<List<HierarchyEditViewModel>>($"Hierarchy/GetHierarchyEdits/{rootNodePathId}");
        }

        /// <summary>
        /// The CreateHierarchyEditAsync.
        /// </summary>
        /// <param name="rootNodePathId">The rootNodePathId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> CreateHierarchyEditAsync(int rootNodePathId)
        {
            return await this.facade.PutAsync($"Hierarchy/CreateHierarchyEdit/{rootNodePathId}");
        }

        /// <summary>
        /// The DiscardHierarchyEditAsync.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchyEditId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> DiscardHierarchyEditAsync(int hierarchyEditId)
        {
            return await this.facade.PutAsync($"Hierarchy/DiscardHierarchyEdit/{hierarchyEditId}");
        }

        /// <summary>
        /// The SubmitHierarchyEditForPublishAsync.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> SubmitHierarchyEditForPublishAsync(PublishHierarchyEditViewModel publishViewModel)
        {
            return await this.facade.PutAsync($"Hierarchy/SubmitHierarchyEditForPublish", publishViewModel);
        }

        /// <summary>
        /// The CreateFolder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> CreateFolderAsync(FolderEditViewModel folderEditViewModel)
        {
            return await this.facade.PostAsync<ApiResponse, FolderEditViewModel>("Hierarchy/CreateFolder", folderEditViewModel);
        }

        /// <summary>
        /// The UpdateFolder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> UpdateFolderAsync(FolderEditViewModel folderEditViewModel)
        {
            return await this.facade.PostAsync<ApiResponse, FolderEditViewModel>("Hierarchy/UpdateFolder", folderEditViewModel);
        }

        /// <summary>
        /// The DeleteFolder.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> DeleteFolder(int hierarchyEditDetailId)
        {
            return await this.facade.PutAsync($"Hierarchy/DeleteFolder/{hierarchyEditDetailId}");
        }

        /// <summary>
        /// The GetFolder.
        /// </summary>
        /// <param name="nodeVersionId">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<FolderViewModel> GetFolderAsync(int nodeVersionId)
        {
            return await this.facade.GetAsync<FolderViewModel>($"Hierarchy/GetFolder/{nodeVersionId}");
        }

        /// <summary>
        /// The MoveNodeUp.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> MoveNodeUp(int hierarchyEditDetailId)
        {
            return await this.facade.PutAsync($"Hierarchy/MoveNodeUp/{hierarchyEditDetailId}");
        }

        /// <summary>
        /// The MoveNodeDown.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> MoveNodeDown(int hierarchyEditDetailId)
        {
            return await this.facade.PutAsync($"Hierarchy/MoveNodeDown/{hierarchyEditDetailId}");
        }

        /// <summary>
        /// The MoveNode.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel<see cref="MoveNodeViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> MoveNodeAsync(MoveNodeViewModel moveNodeViewModel)
        {
            return await this.facade.PostAsync<ApiResponse, MoveNodeViewModel>("Hierarchy/MoveNode", moveNodeViewModel);
        }

        /// <summary>
        /// Moves a resource up in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> HierarchyEditMoveResourceUp(int hierarchyEditDetailId)
        {
            return await this.facade.PutAsync($"Hierarchy/HierarchyEditMoveResourceUp/{hierarchyEditDetailId}");
        }

        /// <summary>
        /// Moves a resource down in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> HierarchyEditMoveResourceDown(int hierarchyEditDetailId)
        {
            return await this.facade.PutAsync($"Hierarchy/HierarchyEditMoveResourceDown/{hierarchyEditDetailId}");
        }

        /// <summary>
        /// Moves a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel<see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> HierarchyEditMoveResource(HierarchyEditMoveResourceViewModel moveResourceViewModel)
        {
            return await this.facade.PostAsync<ApiResponse, HierarchyEditMoveResourceViewModel>("Hierarchy/HierarchyEditMoveResource", moveResourceViewModel);
        }

        /// <summary>
        /// The MoveResourceUp.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> MoveResourceUp(int nodeId, int resourceId)
        {
            return await this.facade.PutAsync($"Hierarchy/MoveResourceUp/{nodeId}/{resourceId}");
        }

        /// <summary>
        /// The MoveResourceDown.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> MoveResourceDown(int nodeId, int resourceId)
        {
            return await this.facade.PutAsync($"Hierarchy/MoveResourceDown/{nodeId}/{resourceId}");
        }

        /// <summary>
        /// The MoveResource.
        /// </summary>
        /// <param name="sourceNodeId">The id of the node to move the resource from.</param>
        /// <param name="destinationNodeId">The id of the node to move the resource to.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> MoveResourceAsync(int sourceNodeId, int destinationNodeId, int resourceId)
        {
            return await this.facade.PutAsync($"Hierarchy/MoveResource/{sourceNodeId}/{destinationNodeId}/{resourceId}");
        }

        /// <summary>
        /// Moves a resource down in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> RemoveReferenceNodeAsync(int hierarchyEditDetailId)
        {
            return await this.facade.PutAsync($"Hierarchy/RemoveReferenceNode/{hierarchyEditDetailId}");
        }

        /// <summary>
        /// Create a reference to a Node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel<see cref="MoveNodeViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> ReferenceNodeAsync(MoveNodeViewModel moveNodeViewModel)
        {
            return await this.facade.PostAsync<ApiResponse, MoveNodeViewModel>("Hierarchy/referenceNode", moveNodeViewModel);
        }

        /// <summary>
        /// References a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel<see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> HierarchyEditReferenceResource(HierarchyEditMoveResourceViewModel moveResourceViewModel)
        {
            return await this.facade.PostAsync<ApiResponse, HierarchyEditMoveResourceViewModel>("Hierarchy/HierarchyEditReferenceResource", moveResourceViewModel);
        }

        /// <summary>
        /// The UpdateFolder.
        /// </summary>
        /// <param name="nodePathDisplayVersionModel">The nodePathDisplayVersionModel<see cref="NodePathDisplayVersionModel"/>.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> UpdateNodePathDisplayVersionAsync(NodePathDisplayVersionModel nodePathDisplayVersionModel)
        {
            return await this.facade.PostAsync<ApiResponse, NodePathDisplayVersionModel>("Hierarchy/UpdateNodePathDisplayVersion", nodePathDisplayVersionModel);
        }

        /// <inheritdoc/>
        public async Task<ApiResponse> UpdateResourceReferenceDisplayVersionAsync(ResourceReferenceDisplayVersionModel resourceReferenceDisplayVersionModel)
        {
            return await this.facade.PostAsync<ApiResponse, ResourceReferenceDisplayVersionModel>("Hierarchy/UpdateResourceReferenceDisplayVersion", resourceReferenceDisplayVersionModel);
        }

        /// <summary>
        /// Create a reference to a Node.
        /// </summary>
        /// <param name="referenceExternalNodeViewModel">The referenceExternalNodeViewModel<see cref="ReferenceExternalNodeViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> ReferenceExternalNodeAsync(ReferenceExternalNodeViewModel referenceExternalNodeViewModel)
        {
            return await this.facade.PostAsync<ApiResponse, ReferenceExternalNodeViewModel>("Hierarchy/referenceExternalNode", referenceExternalNodeViewModel);
        }

        /// <summary>
        /// References an external resource in a hierarchy edit.
        /// </summary>
        /// <param name="referenceExternalResourceViewModel">The referenceExternalResourceViewModel<see cref="ReferenceExternalResourceViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        public async Task<ApiResponse> HierarchyEditReferenceExternalResource(ReferenceExternalResourceViewModel referenceExternalResourceViewModel)
        {
            return await this.facade.PostAsync<ApiResponse, ReferenceExternalResourceViewModel>("Hierarchy/HierarchyEditReferenceExternalResource", referenceExternalResourceViewModel);
        }
    }
}
