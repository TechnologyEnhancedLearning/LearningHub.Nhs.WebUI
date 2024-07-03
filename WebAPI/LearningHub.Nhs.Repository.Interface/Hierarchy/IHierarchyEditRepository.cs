namespace LearningHub.Nhs.Repository.Interface.Hierarchy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;

    /// <summary>
    /// The HierarchyEditRepository interface.
    /// </summary>
    public interface IHierarchyEditRepository : IGenericRepository<HierarchyEdit>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HierarchyEdit> GetByIdAsync(int id);

        /// <summary>
        /// The get by root node path id async.
        /// </summary>
        /// <param name="rootNodePathId">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<HierarchyEdit>> GetByRootNodePathIdAsync(int rootNodePathId);

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="rootNodePathId">The root node path id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The hierarchy edit id.</returns>
        Task<int> Create(int rootNodePathId, int userId);

        /// <summary>
        /// The discard.
        /// </summary>
        /// <param name="hierarchyEditId">The root node id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task Discard(int hierarchyEditId, int userId);

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> CreateFolder(FolderEditViewModel folderEditViewModel, int userId);

        /// <summary>
        /// Updates a folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateFolder(FolderEditViewModel folderEditViewModel, int userId);

        /// <summary>
        /// Updates a nodePathDisplayVersion.
        /// </summary>
        /// <param name="nodePathDisplayVersion">The nodePathDisplayVersion<see cref="NodePathDisplayVersionModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> UpdateNodePathDisplayVersionAsync(NodePathDisplayVersionModel nodePathDisplayVersion, int userId);

        /// <summary>
        /// Updates a resourceReferenceDisplayVersion.
        /// </summary>
        /// <param name="resourceReferenceDisplayVersion">The resourceReferenceDisplayVersion<see cref="ResourceReferenceDisplayVersionModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<int> UpdateResourceReferenceDisplayVersionAsync(ResourceReferenceDisplayVersionModel resourceReferenceDisplayVersion, int userId);

        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteFolder(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// Moves a node up.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task MoveNodeUp(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// Moves a node down.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task MoveNodeDown(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// Moves a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel <see cref="MoveNodeViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task MoveNode(MoveNodeViewModel moveNodeViewModel, int userId);

        /// <summary>
        /// References a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel <see cref="MoveNodeViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task ReferenceNode(MoveNodeViewModel moveNodeViewModel, int userId);

        /// <summary>
        /// References an external node.
        /// </summary>
        /// <param name="referenceExternalNodeViewModel">The referenceExternalNodeViewModel <see cref="ReferenceExternalNodeViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task ReferenceExternalNode(ReferenceExternalNodeViewModel referenceExternalNodeViewModel, int userId);

        /// <summary>
        /// Moves a resource up in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task HierarchyEditMoveResourceUp(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// Moves a resource down in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task HierarchyEditMoveResourceDown(int hierarchyEditDetailId, int userId);

        /// <summary>
        /// Moves a resource in a HierarchyEdit.
        /// </summary>
        /// <param name="moveResourceViewModel">The view model <see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task HierarchyEditMoveResource(HierarchyEditMoveResourceViewModel moveResourceViewModel, int userId);

        /// <summary>
        /// References a resource in a HierarchyEdit.
        /// </summary>
        /// <param name="moveResourceViewModel">The view model <see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task HierarchyEditReferenceResource(HierarchyEditMoveResourceViewModel moveResourceViewModel, int userId);

        /// <summary>
        /// References a resource in a HierarchyEdit.
        /// </summary>
        /// <param name="referenceExternalResourceViewModel">The view model <see cref="ReferenceExternalResourceViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task HierarchyEditReferenceExternalResource(ReferenceExternalResourceViewModel referenceExternalResourceViewModel, int userId);

        /// <summary>
        /// Moves a resource up.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task MoveResourceUp(int nodeId, int resourceId, int userId);

        /// <summary>
        /// Moves a resource down.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task MoveResourceDown(int nodeId, int resourceId, int userId);

        /// <summary>
        /// Moves a resource.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="sourceNodeId">The id of the node to move the resource from.</param>
        /// <param name="destinationNodeId">The id of the node to move the resource to.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>A list of nodeIds affected by the moved resource. The nodes that will need to be refreshed in the UI.</returns>
        Task<List<MoveResourceResultViewModel>> MoveResource(int sourceNodeId, int destinationNodeId, int resourceId, int userId);

        /// <summary>
        /// The publishing.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="userId">The user id.</param>
        void SubmitForPublishing(int hierarchyEditId, int userId);

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="isMajorRevision">The is major revision.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The publication id.</returns>
        int Publish(int hierarchyEditId, bool isMajorRevision, string notes, int userId);

        /// <summary>
        /// Set hierarchy edit to "publishing".
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="userId">The user id.</param>
        void Publishing(int hierarchyEditId, int userId);

        /// <summary>
        /// Set hierarchy edit to "failed to publish".
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="userId">The user id.</param>
        void FailedToPublish(int hierarchyEditId, int userId);
    }
}
