namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Hierarchy;

    /// <summary>
    /// Defines the <see cref="IHierarchyService" />.
    /// </summary>
    public interface IHierarchyService
    {
        /////// <summary>
        /////// Gets the basic details of a node. Currently catalogues or folders.
        /////// </summary>
        /////// <param name="nodeId">The node id.</param>
        /////// <returns>The <see cref="Task"/>.</returns>
        ////Task<NodeViewModel> GetNodeDetails(int nodeId);

        /// <summary>
        /// Gets the basic details of a node path. Currently catalogues or folders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<NodePathViewModel> GetNodePathDetails(int nodePathId);

        /// <summary>
        /// Gets the basic details of all nodes in the NodePath of a particular node.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodePathViewModel>> GetNodePathNodes(int nodeId);

        /// <summary>
        /// Gets the contents of a node path for the catalogue landing page - i.e. published folders and published resources only.
        /// Only returns the items found directly in the specified node path, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node id.</param>
        /// <param name="includeEmptyFolder">Include Empty Folder or not.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentBrowseViewModel>> GetNodeContentsForCatalogueBrowse(int nodePathId, bool includeEmptyFolder);

        /// <summary>
        /// Gets the contents of a node for the My Contributions page - i.e. published folders only, and all resources (i.e. all statuses).
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentEditorViewModel>> GetNodeContentsForCatalogueEditor(int nodePathId);

        /// <summary>
        /// Gets the contents of a node path (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// Set returnPublishedOnly to true if only published resource data is needed.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentAdminViewModel>> GetNodeContentsAdminAsync(int nodePathId, bool readOnly);

        /// <summary>
        /// The get node paths.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>List of NodePathViewModel.</returns>
        Task<List<NodePathViewModel>> GetNodePathsForNodeAsync(int nodeId);

        /// <summary>
        /// Gets the hierarchy edits for the supplied root node path id.
        /// </summary>
        /// <param name="rootNodePathId">The root node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<HierarchyEditViewModel>> GetHierarchyEdits(int rootNodePathId);

        /// <summary>
        /// The CreateHierarchyEditAsync.
        /// </summary>
        /// <param name="rootNodePathId">The rootNodePathId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> CreateHierarchyEditAsync(int rootNodePathId);

        /// <summary>
        /// The DiscardHierarchyEditAsync.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchyEditId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> DiscardHierarchyEditAsync(int hierarchyEditId);

        /// <summary>
        /// The SubmitHierarchyEditForPublishAsync.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> SubmitHierarchyEditForPublishAsync(PublishHierarchyEditViewModel publishViewModel);

        /// <summary>
        /// The CreateFolder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> CreateFolderAsync(FolderEditViewModel folderEditViewModel);

        /// <summary>
        /// The UpdateFolder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> UpdateFolderAsync(FolderEditViewModel folderEditViewModel);

        /// <summary>
        /// The DeleteFolder.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> DeleteFolder(int hierarchyEditDetailId);

        /// <summary>
        /// The GetFolder.
        /// </summary>
        /// <param name="nodeVersionId">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<FolderViewModel> GetFolderAsync(int nodeVersionId);

        /// <summary>
        /// The MoveNodeUp.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> MoveNodeUp(int hierarchyEditDetailId);

        /// <summary>
        /// The MoveNodeDown.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> MoveNodeDown(int hierarchyEditDetailId);

        /// <summary>
        /// The MoveNode.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel<see cref="MoveNodeViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> MoveNodeAsync(MoveNodeViewModel moveNodeViewModel);

        /// <summary>
        /// Moves a resource up in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> HierarchyEditMoveResourceUp(int hierarchyEditDetailId);

        /// <summary>
        /// Moves a resource down in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The hierarchy edit detail id.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> HierarchyEditMoveResourceDown(int hierarchyEditDetailId);

        /// <summary>
        /// Moves a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel<see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> HierarchyEditMoveResource(HierarchyEditMoveResourceViewModel moveResourceViewModel);

        /// <summary>
        /// The MoveResourceUp.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> MoveResourceUp(int nodeId, int resourceId);

        /// <summary>
        /// The MoveResourceDown.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> MoveResourceDown(int nodeId, int resourceId);

        /// <summary>
        /// The MoveResource.
        /// </summary>
        /// <param name="sourceNodeId">The id of the node to move the resource from.</param>
        /// <param name="destinationNodeId">The id of the node to move the resource to.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> MoveResourceAsync(int sourceNodeId, int destinationNodeId, int resourceId);

        /// <summary>
        /// Create a reference to a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel<see cref="MoveNodeViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> ReferenceNodeAsync(MoveNodeViewModel moveNodeViewModel);

        /// <summary>
        /// References a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel<see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <returns>IActionResult.</returns>
        Task<ApiResponse> HierarchyEditReferenceResource(HierarchyEditMoveResourceViewModel moveResourceViewModel);
    }
}
