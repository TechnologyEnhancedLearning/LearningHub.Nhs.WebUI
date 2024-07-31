namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The HierarchyService interface.
    /// </summary>
    public interface IHierarchyService
    {
        /// <summary>
        /// Gets the basic details of a node. Currently catalogues or folders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The node details.</returns>
        Task<NodeViewModel> GetNodeDetails(int nodeId);

        /// <summary>
        /// Gets the basic details of all Nodes in a particular NodePath.
        /// </summary>
        /// <param name="nodePathId">The NodePath id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodePathNodeViewModel>> GetNodePathNodes(int nodePathId);

        /// <summary>
        /// The get catalogue locations for resource reference.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<CatalogueLocationsViewModel> GetCatalogueLocationsForResourceReference(int resourceReferenceId);

        /// <summary>
        /// Gets the contents of a node for the catalogue landing page - i.e. published folders and published resources only.
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="includeEmptyFolder">Include Empty Folder or not.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentBrowseViewModel>> GetNodeContentsForCatalogueBrowse(int nodeId, bool includeEmptyFolder);

        /// <summary>
        /// Gets the contents of a node for the My Contributions page - i.e. published folders only, and all resources (i.e. all statuses).
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentEditorViewModel>> GetNodeContentsForCatalogueEditor(int nodeId);

        /// <summary>
        /// Gets the contents of a node (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentAdminViewModel>> GetNodeContentsAdminAsync(int nodeId, bool readOnly);

        /// <summary>
        /// Gets hierarchy edit detail for the supplied root node id.
        /// </summary>
        /// <param name="rootNodeId">The root node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<HierarchyEditViewModel>> GetHierarchyEdits(int rootNodeId);

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="rootNodeId">The root node id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The hierarchy edit id.</returns>
        Task<int> CreateHierarchyEdit(int rootNodeId, int userId);

        /// <summary>
        /// The discard.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> DiscardHierarchyEdit(int hierarchyEditId, int userId);

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        Task<LearningHubValidationResult> CreateFolder(FolderEditViewModel folderEditViewModel, int userId);

        /// <summary>
        /// Updates a folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        Task<LearningHubValidationResult> UpdateFolder(FolderEditViewModel folderEditViewModel, int userId);

        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> DeleteFolder(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// The GetFolder.
        /// </summary>
        /// <param name="nodeVersionId">The node version id.</param>
        /// <returns>The folder view model.</returns>
        Task<FolderViewModel> GetFolderAsync(int nodeVersionId);

        /// <summary>
        /// Moves a node up.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> MoveNodeUp(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// Moves a node down.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> MoveNodeDown(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// Moves a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel <see cref="MoveNodeViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        Task<LearningHubValidationResult> MoveNode(MoveNodeViewModel moveNodeViewModel, int userId);

        /// <summary>
        /// Moves a resource up in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> HierarchyEditMoveResourceUp(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// Moves a resource down in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> HierarchyEditMoveResourceDown(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// Moves a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel <see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        Task<LearningHubValidationResult> HierarchyEditMoveResource(HierarchyEditMoveResourceViewModel moveResourceViewModel, int userId);

        /// <summary>
        /// Moves a resource up.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> MoveResourceUp(int nodeId, int resourceId, int userId);

        /// <summary>
        /// Moves a resource down.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> MoveResourceDown(int nodeId, int resourceId, int userId);

        /// <summary>
        /// Moves a resource into a different folder.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="sourceNodeId">The id of the node to move the resource from.</param>
        /// <param name="destinationNodeId">The id of the node to move the resource to.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        Task<LearningHubValidationResult> MoveResource(int sourceNodeId, int destinationNodeId, int resourceId, int userId);

        /// <summary>
        /// Submit hierarchy edit for publishing.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> SubmitHierarchyEditForPublish(PublishHierarchyEditViewModel publishViewModel);

        /// <summary>
        /// The publish hierarchy edit.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> PublishHierarchyEditAsync(PublishHierarchyEditViewModel publishViewModel);

        /// <summary>
        /// Set hierarchy edit to publishing.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        LearningHubValidationResult SetHierarchyEditPublishing(PublishHierarchyEditViewModel publishViewModel);

        /// <summary>
        /// Set hierarchy edit to publishing failed.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        LearningHubValidationResult SetHierarchyEditFailedToPublish(PublishHierarchyEditViewModel publishViewModel);

        /// <summary>
        /// The get node resource lookups.
        /// IT1 - use as quick lookup for whether a node has published resources.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{NodeResourceLookupViewModel}"/>.</returns>
        Task<List<NodeResourceLookupViewModel>> GetNodeResourceLookupAsync(int nodeId);

        /// <summary>
        /// The get node paths for node.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{NodePathViewModel}"/>.</returns>
        Task<List<NodePathViewModel>> GetNodePathsForNodeAsync(int nodeId);
    }
}
