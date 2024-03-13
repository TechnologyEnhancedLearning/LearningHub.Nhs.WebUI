namespace LearningHub.Nhs.AdminUI.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Helpers;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Hierarchy;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The catalogue service.
    /// </summary>
    public class HierarchyService : BaseService, IHierarchyService
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
        : base(learningHubHttpClient)
        {
            this.facade = learningHubApiFacade;
        }

        /// <summary>
        /// Gets the contents of a node (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// Set readOnly to true if read only data is needed.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeContentAdminViewModel>> GetNodeContentsAdminAsync(int nodeId, bool readOnly)
        {
            return await this.facade.GetAsync<List<NodeContentAdminViewModel>>($"Hierarchy/GetNodeContentsAdmin/{nodeId}/{readOnly}");
        }

        /// <summary>
        /// The get node paths.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{NodePathViewModel}"/>.</returns>
        public async Task<List<NodePathViewModel>> GetNodePathsForNodeAsync(int nodeId)
        {
            return await this.facade.GetAsync<List<NodePathViewModel>>($"Hierarchy/GetNodePathsForNode/{nodeId}");
        }

        /// <summary>
        /// Gets the hierarchy edits for the supplied root node id.
        /// </summary>
        /// <param name="rootNodeId">The root node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<HierarchyEditViewModel>> GetHierarchyEdits(int rootNodeId)
        {
            return await this.facade.GetAsync<List<HierarchyEditViewModel>>($"Hierarchy/GetHierarchyEdits/{rootNodeId}");
        }

        /// <summary>
        /// The CreateHierarchyEditAsync.
        /// </summary>
        /// <param name="rootNodeId">The rootNodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> CreateHierarchyEditAsync(int rootNodeId)
        {
            return await this.facade.PutAsync($"Hierarchy/CreateHierarchyEdit/{rootNodeId}");
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
    }
}
