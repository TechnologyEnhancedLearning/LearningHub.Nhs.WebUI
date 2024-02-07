// <copyright file="IHierarchyService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
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
        /// <summary>
        /// Gets the contents of a node (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// Set returnPublishedOnly to true if only published resource data is needed.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentAdminViewModel>> GetNodeContentsAdminAsync(int nodeId, bool readOnly);

        /// <summary>
        /// The get node paths.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{NodePathViewModel}"/>.</returns>
        Task<List<NodePathViewModel>> GetNodePathsForNodeAsync(int nodeId);

        /// <summary>
        /// Gets the hierarchy edits for the supplied root node id.
        /// </summary>
        /// <param name="rootNodeId">The root node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<HierarchyEditViewModel>> GetHierarchyEdits(int rootNodeId);

        /// <summary>
        /// The CreateHierarchyEditAsync.
        /// </summary>
        /// <param name="rootNodeId">The rootNodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ApiResponse> CreateHierarchyEditAsync(int rootNodeId);

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
    }
}
