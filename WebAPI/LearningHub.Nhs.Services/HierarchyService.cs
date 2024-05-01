namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Constants;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The hierarchy service.
    /// </summary>
    public class HierarchyService : IHierarchyService
    {
        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// The queue communicator service.
        /// </summary>
        private readonly IQueueCommunicatorService queueCommunicatorService;

        /// <summary>
        /// The node repository.
        /// </summary>
        private readonly INodeRepository nodeRepository;

        /// <summary>
        /// The node resource repository.
        /// </summary>
        private readonly INodeResourceRepository nodeResourceRepository;

        /// <summary>
        /// The node resource lookup repository.
        /// </summary>
        private readonly INodeResourceLookupRepository nodeResourceLookupRepository;

        /// <summary>
        /// The node path repository.
        /// </summary>
        private readonly INodePathRepository nodePathRepository;

        /// <summary>
        /// The resource reference repository.
        /// </summary>
        private readonly IResourceReferenceRepository resourceReferenceRepository;

        /// <summary>
        /// The hierarchy edit repository.
        /// </summary>
        private readonly IHierarchyEditRepository hierarchyEditRepository;

        /// <summary>
        /// The hierarchy edit detail repository.
        /// </summary>
        private readonly IHierarchyEditDetailRepository hierarchyEditDetailRepository;

        /// <summary>
        /// The folder node version repository.
        /// </summary>
        private readonly IFolderNodeVersionRepository folderNodeVersionRepository;

        /// <summary>
        /// The publication repository.
        /// </summary>
        private readonly IPublicationRepository publicationRepository;

        /// <summary>
        /// The publication log repository.
        /// </summary>
        private readonly IPublicationLogRepository publicationLogRepository;

        /// <summary>
        /// The rating service.
        /// </summary>
        private readonly IRatingService ratingService;

        /// <summary>
        /// The caching service.
        /// </summary>
        private readonly ICachingService cachingService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<HierarchyService> logger;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The internalSystemService.
        /// </summary>
        private readonly IInternalSystemService internalSystemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyService"/> class.
        /// </summary>
        /// <param name="userService">
        /// The user service.
        /// </param>
        /// <param name="queueCommunicatorService">
        /// The queue communicator service.
        /// </param>
        /// <param name="nodeRepository">
        /// The node repository.
        /// </param>
        /// <param name="nodeResourceRepository">
        /// The node resource repository.
        /// </param>
        /// <param name="nodeResourceLookupRepository">
        /// The node resource lookup repository.
        /// </param>
        /// <param name="nodePathRepository">
        /// The node path repository.
        /// </param>
        /// <param name="resourceReferenceRepository">
        /// The resource reference repository.
        /// </param>
        /// <param name="hierarchyEditRepository">
        /// The hierarchy edit repository.
        /// </param>
        /// <param name="hierarchyEditDetailRepository">
        /// The hierarchy edit detail repository.
        /// </param>
        /// <param name="folderNodeVersionRepository">
        /// The folder node version repository.
        /// </param>
        /// <param name="publicationRepository">
        /// The publication repository.
        /// </param>
        /// <param name="publicationLogRepository">
        /// The publication log repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="ratingService">
        /// The rating service.
        /// </param>
        /// <param name="cachingService">
        /// The caching service.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        /// <param name="internalSystemService">The internalSystemService.</param>
        public HierarchyService(
            IUserService userService,
            IQueueCommunicatorService queueCommunicatorService,
            INodeRepository nodeRepository,
            INodeResourceRepository nodeResourceRepository,
            INodeResourceLookupRepository nodeResourceLookupRepository,
            INodePathRepository nodePathRepository,
            IResourceReferenceRepository resourceReferenceRepository,
            IHierarchyEditRepository hierarchyEditRepository,
            IHierarchyEditDetailRepository hierarchyEditDetailRepository,
            IFolderNodeVersionRepository folderNodeVersionRepository,
            IPublicationRepository publicationRepository,
            IPublicationLogRepository publicationLogRepository,
            IRatingService ratingService,
            ICachingService cachingService,
            ILogger<HierarchyService> logger,
            IOptions<Settings> settings,
            IMapper mapper,
            IInternalSystemService internalSystemService)
        {
            this.userService = userService;
            this.queueCommunicatorService = queueCommunicatorService;
            this.nodeRepository = nodeRepository;
            this.nodeResourceRepository = nodeResourceRepository;
            this.nodeResourceLookupRepository = nodeResourceLookupRepository;
            this.nodePathRepository = nodePathRepository;
            this.resourceReferenceRepository = resourceReferenceRepository;
            this.hierarchyEditRepository = hierarchyEditRepository;
            this.hierarchyEditDetailRepository = hierarchyEditDetailRepository;
            this.folderNodeVersionRepository = folderNodeVersionRepository;
            this.publicationRepository = publicationRepository;
            this.publicationLogRepository = publicationLogRepository;
            this.ratingService = ratingService;
            this.cachingService = cachingService;
            this.logger = logger;
            this.settings = settings.Value;
            this.mapper = mapper;
            this.internalSystemService = internalSystemService;
        }

        /// <summary>
        /// Gets the basic details of a node. Currently catalogues or folders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The node details.</returns>
        public NodeViewModel GetNodeDetails(int nodeId)
        {
            var retVal = this.nodeRepository.GetNodeDetails(nodeId);

            return retVal;
        }

        /// <summary>
        /// Gets the basic details of all Nodes in a particular NodePath.
        /// </summary>
        /// <param name="nodePathId">The NodePath id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodePathNodeViewModel>> GetNodePathNodes(int nodePathId)
        {
            var retVal = await this.nodePathRepository.GetNodePathNodes(nodePathId);

            return retVal;
        }

        /// <summary>
        /// The get catalogue locations for resource reference.
        /// </summary>
        /// <param name="resourceReferenceId">The resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<CatalogueLocationsViewModel> GetCatalogueLocationsForResourceReference(int resourceReferenceId)
        {
            var vm = new CatalogueLocationsViewModel();

            var rr = await this.resourceReferenceRepository.GetByIdAsync(resourceReferenceId, false);

            if (rr != null)
            {
                vm.CatalogueLocations = this.nodeResourceRepository.GetCatalogueLocationsForResource(rr.ResourceId);
            }

            return vm;
        }

        /// <summary>
        /// Gets the contents of a node for the catalogue landing page - i.e. published folders and published resources only.
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="includeEmptyFolder">Include Empty Folder or not.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeContentBrowseViewModel>> GetNodeContentsForCatalogueBrowse(int nodeId, bool includeEmptyFolder)
        {
            // Attempt to retrieve from cache. If not found then add to cache.
            // Note: include basic resource rating information when serving from the cache.
            var retVal = new List<NodeContentBrowseViewModel>();

            string cacheKey = $"{CacheKeys.PublishedNodeContents}:{nodeId}";
            var nodeContents = await this.cachingService.GetAsync<List<NodeContentBrowseViewModel>>(cacheKey);
            if (includeEmptyFolder)
            {
                nodeContents.ResponseEnum = CacheReadResponseEnum.NotFound;
            }

            if (nodeContents.ResponseEnum == CacheReadResponseEnum.Found)
            {
                retVal = nodeContents.Item;
            }
            else
            {
                retVal = await this.nodeRepository.GetNodeContentsForCatalogueBrowse(nodeId, includeEmptyFolder);

                // Ensure that any Node only exists once in the returned data set.
                var duplicateNodes = retVal.Where(n => n.NodeId.HasValue).GroupBy(x => x.NodeId).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicateNodes.Count > 0)
                {
                    throw new Exception($"Corrupt data. Duplicate Nodes returned in NodeContent for NodeId={nodeId}");
                }

                if (!includeEmptyFolder)
                {
                    await this.cachingService.SetAsync(cacheKey, retVal);
                }
            }

            var list = retVal.Where(ncm => ncm.ResourceVersionId.HasValue);

            if (list.Count() > 0)
            {
                var tasks = list.Select(async ncm =>
                {
                    string cacheKey = $"{CacheKeys.RatingSummaryBasic}:{ncm.ResourceVersionId.Value}";
                    var retVal = await this.cachingService.GetAsync<RatingSummaryBasicViewModel>(cacheKey);
                    return retVal.ResponseEnum == CacheReadResponseEnum.Found ? retVal.Item : null;
                });

                var ratings = await Task.WhenAll(tasks);

                foreach (var ncm in list.Where(ncm => ncm.ResourceVersionId.HasValue))
                {
                    var rating = ratings.Where(r => r != null && r.EntityVersionId == ncm.ResourceVersionId.Value).FirstOrDefault();
                    if (rating == null)
                    {
                        rating = await this.ratingService.GetRatingSummaryBasic(ncm.ResourceVersionId.Value);
                    }

                    ncm.RatingSummaryBasicViewModel = rating;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the contents of a node for the My Contributions page - i.e. published folders only, and all resources (i.e. all statuses).
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeContentEditorViewModel>> GetNodeContentsForCatalogueEditor(int nodeId)
        {
            // Not cached, retrieve directly from the database.
            var vm = await this.nodeRepository.GetNodeContentsForCatalogueEditor(nodeId);

            // Ensure that any Node only exists once in the returned data set.
            var duplicateNodes = vm.Where(n => n.NodeId.HasValue).GroupBy(x => x.NodeId).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
            if (duplicateNodes.Count > 0)
            {
                throw new Exception($"Corrupt data. Duplicate Nodes returned in NodeContent for NodeId={nodeId}");
            }

            return vm;
        }

        /// <summary>
        /// Gets the contents of a node (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeContentAdminViewModel>> GetNodeContentsAdminAsync(int nodeId, bool readOnly)
        {
            // Other parameter combinations served from the database.
            var vm = await this.nodeRepository.GetNodeContentsAdminAsync(nodeId, readOnly);

            // Ensure that any Node only exists once in the returned data set.
            var duplicateNodes = vm.Where(n => n.NodeId.HasValue).GroupBy(x => x.NodeId).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
            if (duplicateNodes.Count > 0)
            {
                throw new Exception($"Error. Duplicate Nodes returned in NodeContent for NodeId={nodeId}");
            }

            return vm;
        }

        /// <summary>
        /// Gets hierarchy edit detail for the supplied root node id.
        /// </summary>
        /// <param name="rootNodeId">The root node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<HierarchyEditViewModel>> GetHierarchyEdits(int rootNodeId)
        {
            var hierarchyEdits = await this.hierarchyEditRepository.GetByRootNodeIdAsync(rootNodeId);

            var vms = this.mapper.Map<List<HierarchyEditViewModel>>(hierarchyEdits);

            foreach (var he in vms)
            {
                // IT1 - temporary means.
                // Refactor to obtain via sotred proc / use include filter on upgrade of EFCore.
                if (he.HierarchyEditStatus == HierarchyEditStatusEnum.Draft)
                {
                    var hed = await this.hierarchyEditDetailRepository.GetByNodeIdAsync(he.Id, rootNodeId);
                    he.RootHierarchyEditDetailId = hed.Id;
                }
            }

            return vms;
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="rootNodeId">The root node id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The hierarchy edit id.</returns>
        public async Task<int> CreateHierarchyEdit(int rootNodeId, int userId)
        {
            return await this.hierarchyEditRepository.Create(rootNodeId, userId);
        }

        /// <summary>
        /// The discard.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> DiscardHierarchyEdit(int hierarchyEditId, int userId)
        {
            await this.hierarchyEditRepository.Discard(hierarchyEditId, userId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        public async Task<LearningHubValidationResult> CreateFolder(FolderEditViewModel folderEditViewModel, int userId)
        {
            int createdId = await this.hierarchyEditRepository.CreateFolder(folderEditViewModel, userId);
            var retVal = new LearningHubValidationResult(true);
            retVal.CreatedId = createdId;
            return retVal;
        }

        /// <summary>
        /// Updates a folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateFolder(FolderEditViewModel folderEditViewModel, int userId)
        {
            await this.hierarchyEditRepository.UpdateFolder(folderEditViewModel, userId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> DeleteFolder(int hierarchyEditDetailId, int userId)
        {
            try
            {
                await this.hierarchyEditRepository.DeleteFolder(hierarchyEditDetailId, userId);
                var retVal = new LearningHubValidationResult(true);
                return retVal;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Active folder branch contains draft resource/s and cannot be deleted")
                {
                    return new LearningHubValidationResult(false, ex.Message);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// The GetFolder.
        /// </summary>
        /// <param name="nodeVersionId">The node version id.</param>
        /// <returns>The folder view model.</returns>
        public async Task<FolderViewModel> GetFolderAsync(int nodeVersionId)
        {
            var folder = await this.folderNodeVersionRepository.GetFolderAsync(nodeVersionId);
            return this.mapper.Map<FolderViewModel>(folder);
        }

        /// <summary>
        /// Moves a node up.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> MoveNodeUp(int hierarchyEditDetailId, int userId)
        {
            await this.hierarchyEditRepository.MoveNodeUp(hierarchyEditDetailId, userId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Moves a node down.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> MoveNodeDown(int hierarchyEditDetailId, int userId)
        {
            await this.hierarchyEditRepository.MoveNodeDown(hierarchyEditDetailId, userId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Moves a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel <see cref="MoveNodeViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        public async Task<LearningHubValidationResult> MoveNode(MoveNodeViewModel moveNodeViewModel, int userId)
        {
            await this.hierarchyEditRepository.MoveNode(moveNodeViewModel, userId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Creates a reference to a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel <see cref="MoveNodeViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        public async Task<LearningHubValidationResult> ReferenceNode(MoveNodeViewModel moveNodeViewModel, int userId)
        {
            await this.hierarchyEditRepository.ReferenceNode(moveNodeViewModel, userId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Moves a resource up in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> HierarchyEditMoveResourceUp(int hierarchyEditDetailId, int userId)
        {
            await this.hierarchyEditRepository.HierarchyEditMoveResourceUp(hierarchyEditDetailId, userId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Moves a resource down in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> HierarchyEditMoveResourceDown(int hierarchyEditDetailId, int userId)
        {
            await this.hierarchyEditRepository.HierarchyEditMoveResourceDown(hierarchyEditDetailId, userId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Moves a resource in a hierarchy edit.
        /// </summary>
        /// <param name="moveResourceViewModel">The moveResourceViewModel <see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        public async Task<LearningHubValidationResult> HierarchyEditMoveResource(HierarchyEditMoveResourceViewModel moveResourceViewModel, int userId)
        {
            await this.hierarchyEditRepository.HierarchyEditMoveResource(moveResourceViewModel, userId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Moves a resource up.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        public async Task<LearningHubValidationResult> MoveResourceUp(int nodeId, int resourceId, int userId)
        {
            await this.hierarchyEditRepository.MoveResourceUp(nodeId, resourceId, userId);
            await this.RefreshCacheForNodeContents(nodeId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Moves a resource down.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        public async Task<LearningHubValidationResult> MoveResourceDown(int nodeId, int resourceId, int userId)
        {
            await this.hierarchyEditRepository.MoveResourceDown(nodeId, resourceId, userId);
            await this.RefreshCacheForNodeContents(nodeId);
            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Moves a resource into a different folder.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="sourceNodeId">The id of the node to move the resource from.</param>
        /// <param name="destinationNodeId">The id of the node to move the resource to.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        public async Task<LearningHubValidationResult> MoveResource(int sourceNodeId, int destinationNodeId, int resourceId, int userId)
        {
            var vm = await this.hierarchyEditRepository.MoveResource(sourceNodeId, destinationNodeId, resourceId, userId);

            foreach (var node in vm)
            {
                await this.RefreshCacheForNodeContents(node.NodeId);
            }

            var retVal = new LearningHubValidationResult(true);
            return retVal;
        }

        /// <summary>
        /// Submit hierarchy edit for publishing.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> SubmitHierarchyEditForPublish(PublishHierarchyEditViewModel publishViewModel)
        {
            if (publishViewModel.UserId == 0)
            {
                return new LearningHubValidationResult(false, "Operation to submit a hierarchy edit for publishing requires a UserId");
            }

            this.hierarchyEditRepository.SubmitForPublishing(publishViewModel.HierarchyEditId, publishViewModel.UserId);

            try
            {
                // send to azure storage queue
                var internalSystem = await this.internalSystemService.GetByIdAsync((int)InternalSystemType.HierarchyEditpublishQueue);
                var hierarchyEditPublishQueue = internalSystem.IsOffline ? $"{this.settings.HierarchyEditPublishQueueName}-temp" : this.settings.HierarchyEditPublishQueueName;
                await this.queueCommunicatorService.SendAsync(hierarchyEditPublishQueue, publishViewModel);
            }
            catch (Exception ex)
            {
                this.hierarchyEditRepository.FailedToPublish(publishViewModel.HierarchyEditId, publishViewModel.UserId);

                string message = $"Failed to submit 'publish' message to 'HierarchyEditPublisher' queue: {ex.Message}";

                return new LearningHubValidationResult(false, message);
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The publish hierarch edit.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<int> PublishHierarchyEditAsync(PublishHierarchyEditViewModel publishViewModel)
        {
            var currentUserId = publishViewModel.UserId;
            var publicationId = this.hierarchyEditRepository.Publish(publishViewModel.HierarchyEditId, publishViewModel.IsMajorRevision, publishViewModel.Notes, currentUserId);

            bool success = true; // From Findwise in IT2

            if (success)
            {
                // Refresh cached NodeContents data that has been affected by the publication.
                // IT1 - publication log only contains "Node" entries for Nodes that have had a change to their "NodeResourceLookup" information.
                // This initial provision serves caching in the WebUI for catalogue browsing.
                var cacheOperationList = await this.publicationRepository.GetCacheOperations(publicationId);

                foreach (var o in cacheOperationList)
                {
                    await this.RefreshCacheForNodeContents(o.NodeId);
                }

                return publicationId;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Set hierarchy edit to publishing.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public LearningHubValidationResult SetHierarchyEditPublishing(PublishHierarchyEditViewModel publishViewModel)
        {
            var currentUserId = publishViewModel.UserId;
            if (currentUserId == 0)
            {
                return new LearningHubValidationResult(false, "Operation to set a hierarchy edit to 'publishing' requires a UserId");
            }

            this.hierarchyEditRepository.Publishing(publishViewModel.HierarchyEditId, currentUserId);

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Set hierarchy edit to publishing failed.
        /// </summary>
        /// <param name="publishViewModel">The publishViewModel<see cref="PublishHierarchyEditViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public LearningHubValidationResult SetHierarchyEditFailedToPublish(PublishHierarchyEditViewModel publishViewModel)
        {
            var currentUserId = publishViewModel.UserId;
            if (currentUserId == 0)
            {
                return new LearningHubValidationResult(false, "Operation to set a hierarchy edit to 'publishing failed' requires a UserId");
            }

            this.hierarchyEditRepository.FailedToPublish(publishViewModel.HierarchyEditId, currentUserId);

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The get node resource lookups.
        /// IT1 - use as quick lookup for whether a node has published resources.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{NodeResourceLookupViewModel}"/>.</returns>
        public async Task<List<NodeResourceLookupViewModel>> GetNodeResourceLookupAsync(int nodeId)
        {
            var nodeResourceLookupList = await this.nodeResourceLookupRepository.GetByNodeIdAsync(nodeId);

            return this.mapper.Map<List<NodeResourceLookupViewModel>>(nodeResourceLookupList);
        }

        /// <summary>
        /// The get node paths for node.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{NodePathViewModel}"/>.</returns>
        public async Task<List<NodePathViewModel>> GetNodePathsForNodeAsync(int nodeId)
        {
            var nodePathList = await this.nodePathRepository.GetNodePathsForNodeId(nodeId);

            return this.mapper.Map<List<NodePathViewModel>>(nodePathList);
        }

        private async Task RefreshCacheForNodeContents(int nodeId)
        {
            string cacheKey = $"{CacheKeys.PublishedNodeContents}:{nodeId}";

            // Add to cache from db (note - use repository).
            var nodeContents = await this.nodeRepository.GetNodeContentsForCatalogueBrowse(nodeId, false);
            await this.cachingService.SetAsync(cacheKey, nodeContents);
        }
    }
}