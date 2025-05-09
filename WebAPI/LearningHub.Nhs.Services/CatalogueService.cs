﻿namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Email;
    using LearningHub.Nhs.Models.Email.Models;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.Interface.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The Catalogue Service.
    /// </summary>
    public class CatalogueService : ICatalogueService
    {
        private readonly IMapper mapper;
        private readonly INodeRepository nodeRepository;
        private readonly ICatalogueNodeVersionRepository catalogueNodeVersionRepository;
        private readonly INodeResourceRepository nodeResourceRepository;
        private readonly IUserRepository userRepository;
        private readonly IRoleUserGroupRepository roleUserGroupRepository;
        private readonly IResourceVersionRepository resourceVersionRepository;
        private readonly IFindwiseApiFacade findwiseApiFacade;
        private readonly IUserUserGroupRepository userUserGroupRepository;
        private readonly IEmailSenderService emailSenderService;
        private readonly Settings settings;
        private readonly IProviderService providerService;

        /// <summary>
        /// Defines the nodeActivityRepository.
        /// </summary>
        private readonly INodeActivityRepository nodeActivityRepository;
        private readonly IBookmarkRepository bookmarkRepository;

        /// <summary>
        /// The userService.
        /// </summary>
        ////private readonly elfhHub.Nhs.Services.Interface.IUserService userService;
        private readonly ICatalogueAccessRequestRepository catalogueAccessRequestRepository;
        private readonly IUserProfileRepository userProfileRepository;
        private readonly INotificationSenderService notificationSenderService;
        private readonly ITimezoneOffsetManager timezoneOffsetManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueService"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="nodeRepository">The nodeRepository.</param>
        /// <param name="catalogueNodeVersionRepository">The catalogueNodeVersionRepository.</param>
        /// <param name="nodeResourceRepository">The nodeResourceRepository.</param>
        /// <param name="userRepository">The userRepository.</param>
        /// <param name="roleUserGroupRepository">The roleUserGroupRepository.</param>
        /// <param name="resourceVersionRepository">The resourceVersionRepository.</param>
        /// <param name="findwiseApiFacade">The findwiseApiFacade.</param>
        /// <param name="nodeActivityRepository">The nodeActivityRepository.</param>
        /// <param name="bookmarkRepository">The bookmarkRepository.</param>
        /// <param name="userUserGroupRepository">The user user group repository.</param>
        /// <param name="emailSenderService">The email sender service.</param>
        /// <param name="catalogueAccessRequestRepository">The catalogueAccessRequestRepository.</param>
        /// <param name="userProfileRepository">The userDetailsRepository.</param>
        /// <param name="notificationSenderService">The notificationSenderService.</param>
        /// <param name="providerService">The Provider Service.</param>
        /// <param name="options">The options.</param>
        /// <param name="timezoneOffsetManager">The timezoneOffsetManager.</param>
        public CatalogueService(
            IMapper mapper,
            INodeRepository nodeRepository,
            ICatalogueNodeVersionRepository catalogueNodeVersionRepository,
            INodeResourceRepository nodeResourceRepository,
            IUserRepository userRepository,
            IRoleUserGroupRepository roleUserGroupRepository,
            IResourceVersionRepository resourceVersionRepository,
            IFindwiseApiFacade findwiseApiFacade,
            IUserUserGroupRepository userUserGroupRepository,
            IEmailSenderService emailSenderService,
            ////elfhHub.Nhs.Services.Interface.IUserService userService,
            ICatalogueAccessRequestRepository catalogueAccessRequestRepository,
            IUserProfileRepository userProfileRepository,
            INotificationSenderService notificationSenderService,
            IProviderService providerService,
            IOptions<Settings> options,
            INodeActivityRepository nodeActivityRepository,
            IBookmarkRepository bookmarkRepository,
            ITimezoneOffsetManager timezoneOffsetManager)
        {
            this.mapper = mapper;
            this.nodeRepository = nodeRepository;
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
            this.nodeResourceRepository = nodeResourceRepository;
            this.resourceVersionRepository = resourceVersionRepository;
            this.findwiseApiFacade = findwiseApiFacade;
            this.roleUserGroupRepository = roleUserGroupRepository;
            this.userUserGroupRepository = userUserGroupRepository;
            this.nodeActivityRepository = nodeActivityRepository;
            this.bookmarkRepository = bookmarkRepository;
            ////this.userService = userService;
            this.catalogueAccessRequestRepository = catalogueAccessRequestRepository;
            this.userProfileRepository = userProfileRepository;
            this.notificationSenderService = notificationSenderService;
            this.providerService = providerService;
            this.emailSenderService = emailSenderService;
            this.userRepository = userRepository;
            this.settings = options.Value;
            this.timezoneOffsetManager = timezoneOffsetManager;
        }

        /// <summary>
        /// The Get Basic Catalogue.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNode.</param>
        /// <returns>The catalogues.</returns>
        public CatalogueBasicViewModel GetBasicCatalogue(int catalogueNodeId)
        {
            var catalogue = this.catalogueNodeVersionRepository.GetBasicCatalogue(catalogueNodeId);
            return this.mapper.Map<CatalogueBasicViewModel>(catalogue);
        }

        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The catalogue view model.</returns>
        public async Task<CatalogueViewModel> GetCatalogueAsync(int id)
        {
            var catalogue = await this.catalogueNodeVersionRepository.GetAll()
                .Include(x => x.NodeVersion).ThenInclude(x => x.Node)
                .Include(x => x.Keywords)
                .Include(x => x.CatalogueNodeVersionProvider).Where(x => !x.Deleted)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (catalogue == null)
            {
                return null;
            }

            var vm = this.mapper.Map<CatalogueViewModel>(catalogue);

            // Used by the admin screen to inform the admin user if they need to add a user group.
            vm.HasUserGroup = this.GetRoleUserGroupsForCatalogue(vm.NodeId).Any();
            vm.Providers = await this.providerService.GetAllAsync();
            return vm;
        }

        /// <summary>
        /// Get Catlogue by reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The catalogue view model.</returns>
        public async Task<CatalogueViewModel> GetCatalogueAsync(string reference)
        {
            var catalogue = await this.catalogueNodeVersionRepository.GetCatalogueAsync(reference);

            if (catalogue == null)
            {
                return null;
            }

            return this.mapper.Map<CatalogueViewModel>(catalogue);
        }

        /// <summary>
        /// Get Catlogue by reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogue view model.</returns>
        public async Task<CatalogueViewModel> GetCatalogueAsync(string reference, int userId)
        {
            var catalogue = await this.catalogueNodeVersionRepository.GetCatalogueAsync(reference);

            if (catalogue == null)
            {
                return null;
            }

            await this.RecordNodeActivity(userId, catalogue);

            var catalogueVM = this.mapper.Map<CatalogueViewModel>(catalogue);
            var bookmark = this.bookmarkRepository.GetAll().Where(b => b.NodeId == catalogue.NodeId && b.UserId == userId).FirstOrDefault();
            catalogueVM.BookmarkId = bookmark?.Id;
            catalogueVM.IsBookmarked = !bookmark?.Deleted ?? false;
            catalogueVM.Providers = await this.providerService.GetByCatalogueVersionIdAsync(catalogueVM.Id);
            return catalogueVM;
        }

        /// <summary>
        /// The get catalogues by node id async.
        /// </summary>
        /// <param name="nodeIds">The nodeIds.</param>
        /// <returns>The catalogues.</returns>
        public List<CatalogueViewModel> GetCataloguesByNodeId(IEnumerable<int> nodeIds)
        {
            var nodes = nodeIds.ToArray();
            var catalogues = this.catalogueNodeVersionRepository.GetAll()
                .Include(x => x.NodeVersion)
                .ThenInclude(x => x.Node)
                .Where(x => nodes.Contains(x.NodeVersion.NodeId));
            var outputCatalogues = new List<CatalogueViewModel>();
            foreach (var catalogue in catalogues)
            {
                outputCatalogues.Add(new CatalogueViewModel
                {
                    NodeId = catalogue.NodeVersion.NodeId,
                    BannerUrl = catalogue.BannerUrl,
                    BadgeUrl = catalogue.BadgeUrl,
                    CardImageUrl = catalogue.CardImageUrl,
                    Url = catalogue.Url,
                    Name = catalogue.Name,
                    NodePathId = catalogue.NodeVersion.Node.NodePaths?.FirstOrDefault()?.Id ?? 0,
                    RestrictedAccess = catalogue.RestrictedAccess,
                });
            }

            return outputCatalogues;
        }

        /// <summary>
        /// The CreateCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogue">The catalogue.</param>
        /// <returns>The catalogue id.</returns>
        public async Task<LearningHubValidationResult> CreateCatalogueAsync(int userId, CatalogueViewModel catalogue)
        {
            var validationResult = this.ValidateAddAsync(catalogue);

            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            var catalogueNodeVersionId = await this.catalogueNodeVersionRepository.CreateCatalogueAsync(userId, catalogue);

            // Catalogue is in database, push to findwise
            var cnv = this.catalogueNodeVersionRepository.GetAll()
                .Include(x => x.NodeVersion).ThenInclude(x => x.Node)
                .Include(x => x.Keywords)
                .Include(x => x.CatalogueNodeVersionProvider)
                .SingleOrDefault(x => x.Id == catalogueNodeVersionId);

            if (cnv != null)
            {
                var searchModel = this.mapper.Map<SearchCatalogueRequestModel>(cnv);
                if (searchModel.Description.Length > this.settings.Findwise.DescriptionLengthLimit)
                {
                    searchModel.Description = searchModel.Description.Substring(0, this.settings.Findwise.DescriptionLengthLimit - 4) + "</p>";
                }

                await this.findwiseApiFacade.AddOrReplaceAsync(new List<SearchCatalogueRequestModel> { searchModel });
            }

            return new LearningHubValidationResult(true)
            {
                CreatedId = catalogueNodeVersionId,
            };
        }

        /// <summary>
        /// The GetCatalogues.
        /// </summary>
        /// <param name="searchTerm">The searchTerm.</param>
        /// <returns>The catalogues.</returns>
        public List<CatalogueViewModel> GetCatalogues(string searchTerm)
        {
            IQueryable<CatalogueNodeVersion> catalogueVersions = this.catalogueNodeVersionRepository.GetAll()
                 .Include(x => x.Keywords)
                  .Include(x => x.CatalogueNodeVersionProvider).Where(x => !x.Deleted)
                 .Include(x => x.NodeVersion)
                 .ThenInclude(x => x.Node);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                catalogueVersions = catalogueVersions.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            return this.mapper.ProjectTo<CatalogueViewModel>(catalogueVersions).ToList();
        }

        /// <summary>
        /// The GetCatalogues.
        /// </summary>
        /// <param name="catalogueIds">The catalogue ids.</param>
        /// <returns>The catalogues.</returns>
        public async Task<List<CatalogueViewModel>> GetCatalogues(List<int> catalogueIds)
        {
            var catalogueViewModels = new List<CatalogueViewModel>();

            if (catalogueIds.Count > 0)
            {
                var catalogueNodeVersions = await this.catalogueNodeVersionRepository.GetCatalogues(catalogueIds);
                catalogueViewModels = this.mapper.Map<List<CatalogueViewModel>>(catalogueNodeVersions);
            }

            return catalogueViewModels;
        }

        /// <summary>
        /// The Get Basic Catalogues for user.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogues.</returns>
        public List<CatalogueBasicViewModel> GetCataloguesForUser(int userId)
        {
            var assignedCatalogues = this.catalogueNodeVersionRepository.GetPublishedCataloguesForUserAsync(userId);
            return this.mapper.Map<List<CatalogueBasicViewModel>>(assignedCatalogues);
        }

        /// <summary>
        /// The AccessDetailsAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="reference">The reference.</param>
        /// <returns>The catalogue access details.</returns>
        public async Task<CatalogueAccessDetailsViewModel> AccessDetailsAsync(int userId, string reference)
        {
            var catalogue = await this.GetCatalogueAsync(reference);
            var canManage = await this.IsUserLocalAdminAsync(userId, catalogue.NodeId);
            var accessRequest = this.catalogueAccessRequestRepository.GetByUserIdAndCatalogueId(catalogue.NodeId, userId);
            var details = new CatalogueAccessDetailsViewModel
            {
                CanManage = canManage,
            };
            if (accessRequest == null)
            {
                return details;
            }

            if (accessRequest.CompletedDate != null)
            {
                if (accessRequest.Status == (int)CatalogueAccessRequestStatus.Accepted)
                {
                    details.AcceptedDate = accessRequest.CompletedDate;
                }
                else
                {
                    details.RejectedDate = accessRequest.CompletedDate;
                }
            }
            else
            {
                details.RequestedDate = accessRequest.CreateDate;
            }

            return details;
        }

        /// <summary>
        /// The GetLatestCatalogueAccessRequest.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The CatalogueAccessRequestViewModel.</returns>
        public CatalogueAccessRequestViewModel GetLatestCatalogueAccessRequest(int catalogueNodeId, int userId)
        {
            var accessRequest = this.catalogueAccessRequestRepository.GetByUserIdAndCatalogueId(catalogueNodeId, userId);
            return this.mapper.Map<CatalogueAccessRequestViewModel>(accessRequest);
        }

        /// <summary>
        /// The GetCatalogueAccessRequests.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The CatalogueAccessRequestViewModel list.</returns>
        public List<CatalogueAccessRequestViewModel> GetCatalogueAccessRequests(int catalogueNodeId, int userId)
        {
            var accessRequests = this.catalogueAccessRequestRepository.GetAllByUserIdAndCatalogueId(catalogueNodeId, userId);

            return this.mapper.ProjectTo<CatalogueAccessRequestViewModel>(accessRequests).ToList();
        }

        /// <summary>
        /// The GetRolesForCatalogue.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="includeUser">The includeUser.</param>
        /// <returns>The roleUserGroups.</returns>
        public List<RoleUserGroup> GetRoleUserGroupsForCatalogue(int catalogueNodeId, bool includeUser = false)
        {
            IQueryable<RoleUserGroup> query = this.roleUserGroupRepository.GetAll()
                .Include(x => x.Scope);
            if (includeUser)
            {
                query = query.Include(x => x.UserGroup).ThenInclude(x => x.UserUserGroup);
            }

            return query.Where(x => x.Scope.CatalogueNodeId == catalogueNodeId).ToList();
        }

        /// <summary>
        /// The GetRolesForCatalogueSearch.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="userId">The current user.</param>
        /// <returns>The roleUserGroups.</returns>
        public async Task<List<RoleUserGroup>> GetRoleUserGroupsForCatalogueSearch(int catalogueNodeId, int userId)
        {
            return await this.roleUserGroupRepository.GetAllforSearch(catalogueNodeId, userId);
        }

        /// <summary>
        /// The RequestAccessAsync.
        /// </summary>
        /// <param name="currentUserId">The currentUserId.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="vm">The view model.</param>
        /// <param name="accessType">The accessType.</param>
        /// <returns>The bool.</returns>
        public async Task<bool> RequestAccessAsync(int currentUserId, string reference, CatalogueAccessRequestViewModel vm, string accessType)
        {
            await this.catalogueAccessRequestRepository.CreateCatalogueAccessRequestAsync(
                currentUserId,
                reference,
                vm.Message,
                vm.RoleId,
                this.settings.LearningHubUrl + "Catalogue/Manage/" + reference,
                accessType);

            return true;
        }

        /// <summary>
        /// The InviteUserAsync.
        /// </summary>
        /// <param name="currentUserId">The currentUserId.</param>
        /// <param name="vm">The view model.</param>
        /// <returns>The bool.</returns>
        public async Task<bool> InviteUserAsync(int currentUserId, RestrictedCatalogueInviteUserViewModel vm)
        {
            var catalogue = this.catalogueNodeVersionRepository.GetBasicCatalogue(vm.CatalogueNodeId);
            var user = await this.userProfileRepository.GetByIdAsync(currentUserId);
            var invitedUser = await this.userProfileRepository.GetByEmailAddressAsync(vm.EmailAddress);
            var greeting = "Hi there";
            if (invitedUser != null)
            {
                greeting = $"Dear {invitedUser.FirstName}";
            }

            var emailModel = new SendEmailModel<CatalogueAccessInviteEmailModel>(
                new CatalogueAccessInviteEmailModel
                {
                    CatalogueName = catalogue.Name,
                    CatalogueUrl = $"{this.settings.LearningHubUrl}Catalogue/{catalogue.Url}",
                    AdminFullName = $"{user.FirstName} {user.LastName}",
                    CreateAccountUrl = $"{this.settings.LearningHubUrl}Registration/create-an-account",
                    Greeting = greeting,
                });
            emailModel.EmailAddress = vm.EmailAddress;

            await this.emailSenderService.SendAccessRequestInviteEmail(currentUserId, emailModel);
            return true;
        }

        /// <summary>
        /// The UpdateCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogue">The catalogue.</param>
        /// <returns>The catalogue view model.</returns>
        public async Task<LearningHubValidationResult> UpdateCatalogueAsync(int userId, CatalogueViewModel catalogue)
        {
            var validationResult = this.ValidateEditAsync(catalogue);

            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            await this.catalogueNodeVersionRepository.UpdateCatalogueAsync(userId, catalogue);

            // Update catalogue in findwise
            var cnv = this.catalogueNodeVersionRepository.GetAll()
                .Include(x => x.NodeVersion).ThenInclude(x => x.Node)
                .Include(x => x.Keywords)
                .Include(x => x.CatalogueNodeVersionProvider)
                .SingleOrDefault(x => x.Id == catalogue.CatalogueNodeVersionId);

            if (cnv != null)
            {
                var searchModel = this.mapper.Map<SearchCatalogueRequestModel>(cnv);
                if (searchModel.Description.Length > this.settings.Findwise.DescriptionLengthLimit)
                {
                    searchModel.Description = searchModel.Description.Substring(0, this.settings.Findwise.DescriptionLengthLimit - 4) + "</p>";
                }

                await this.findwiseApiFacade.AddOrReplaceAsync(new List<SearchCatalogueRequestModel> { searchModel });
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The UpdateCatalogueOwnerAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueOwner">The catalogue owner.</param>
        /// <returns>The catalogue view model.</returns>
        public async Task<LearningHubValidationResult> UpdateCatalogueOwnerAsync(int userId, CatalogueOwnerViewModel catalogueOwner)
        {
            await this.catalogueNodeVersionRepository.UpdateCatalogueOwnerAsync(userId, catalogueOwner);

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The ValidateForRestrictedAccess.
        /// </summary>
        /// <param name="catalogueNodeId">The nodeId.</param>
        /// <returns>The task.</returns>
        public async Task<LearningHubValidationResult> ValidateForRestrictedAccess(int catalogueNodeId)
        {
            var retVal = new LearningHubValidationResult(true);
            var catalogue = this.GetBasicCatalogue(catalogueNodeId);
            if (catalogue.RestrictedAccess)
            {
                var localAdminRoleUserGroups = await this.roleUserGroupRepository.GetByRoleIdCatalogueIdWithUsers((int)RoleEnum.LocalAdmin, catalogueNodeId);

                if (localAdminRoleUserGroups.Count == 0)
                {
                    retVal.IsValid = false;
                    retVal.Details.Add("A Restricted Catalogue must have at least one 'Local Admin' User Group which has users.");
                    return retVal;
                }

                var readerRoleUserGroups = await this.roleUserGroupRepository.GetByRoleIdCatalogueId((int)RoleEnum.Reader, catalogueNodeId);
                var defaultReaderRoleUserGroup = readerRoleUserGroups.Where(rug => rug.UserGroup.UserGroupAttribute.Any(uga => uga.AttributeId == (int)AttributeEnum.RestrictedAccess)).FirstOrDefault();
                if (defaultReaderRoleUserGroup == null)
                {
                    retVal.IsValid = false;
                    retVal.Details.Add("A Restricted Catalouge must have a default 'Restricted Access' User Group.");
                }

                if (!retVal.IsValid)
                {
                    return retVal;
                }
            }

            return retVal;
        }

        /// <summary>
        /// The ShowCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="nodeId">The nodeId.</param>
        /// <returns>The task.</returns>
        public async Task<LearningHubValidationResult> ShowCatalogueAsync(int userId, int nodeId)
        {
            var node = await this.nodeRepository.GetByIdAsync(nodeId);
            if (node == null)
            {
                throw new Exception($"Cannot show node '{nodeId}' because it does not exist");
            }

            var vr = await this.ValidateForRestrictedAccess(nodeId);
            if (!vr.IsValid)
            {
                return vr;
            }

            await this.catalogueNodeVersionRepository.ShowCatalogue(userId, nodeId);

            var cnv = this.catalogueNodeVersionRepository.GetAll()
                    .Include(x => x.NodeVersion).ThenInclude(x => x.Node)
                    .Include(x => x.Keywords)
                    .Include(x => x.CatalogueNodeVersionProvider)
                    .SingleOrDefault(x => x.NodeVersion.NodeId == nodeId);

            // update findwise
            if (cnv != null)
            {
                var searchModel = this.mapper.Map<SearchCatalogueRequestModel>(cnv);
                if (searchModel.Description.Length > this.settings.Findwise.DescriptionLengthLimit)
                {
                    searchModel.Description = searchModel.Description.Substring(0, this.settings.Findwise.DescriptionLengthLimit - 4) + "</p>";
                }

                await this.findwiseApiFacade.AddOrReplaceAsync(new List<SearchCatalogueRequestModel> { searchModel });
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The HideCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="nodeId">The nodeId.</param>
        /// <returns>The task.</returns>
        public async Task HideCatalogueAsync(int userId, int nodeId)
        {
            var node = await this.nodeRepository.GetByIdAsync(nodeId);
            if (node == null)
            {
                throw new Exception($"Cannot hide node '{nodeId}' because it does not exist");
            }

            node.Hidden = true;
            await this.nodeRepository.UpdateAsync(userId, node);

            // update findwise
            var cnv = this.catalogueNodeVersionRepository.GetAll()
                    .Include(x => x.NodeVersion).ThenInclude(x => x.Node)
                    .Include(x => x.Keywords)
                    .Include(x => x.CatalogueNodeVersionProvider)
                    .SingleOrDefault(x => x.NodeVersion.NodeId == nodeId);
            if (cnv != null)
            {
                var searchModel = this.mapper.Map<SearchCatalogueRequestModel>(cnv);
                if (searchModel.Description.Length > this.settings.Findwise.DescriptionLengthLimit)
                {
                    searchModel.Description = searchModel.Description.Substring(0, this.settings.Findwise.DescriptionLengthLimit - 4) + "</p>";
                }

                await this.findwiseApiFacade.AddOrReplaceAsync(new List<SearchCatalogueRequestModel> { searchModel });
            }
        }

        /// <summary>
        /// The GetResources.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="id">The catalogueId.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The pageSize.</param>
        /// <param name="sortColumn">The sortColumn.</param>
        /// <param name="sortDirection">The sortDirection.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The catalogueResourcesViewModel.</returns>
        public async Task<CatalogueResourcesViewModel> GetResourcesAsync(int userId, int id, int page, int pageSize, string sortColumn = "", string sortDirection = "", string filter = "")
        {
            var catalogueNodeVersion = await this.catalogueNodeVersionRepository.GetAll()
                .Include(x => x.NodeVersion)
                .ThenInclude(x => x.Node)
                .SingleOrDefaultAsync(x => x.Id == id);
            var nodeId = catalogueNodeVersion.NodeVersion.NodeId;
            var nodeResources = this.nodeResourceRepository.GetAll()
                .Include(x => x.Resource)
                .Include(x => x.Node)
                .ThenInclude(x => x.NodePathNodes)
                .ThenInclude(x => x.NodePath)
                .Where(x => x.Node.NodePathNodes.Where(n => n.NodePath.CatalogueNodeId == nodeId).Count() > 0);

            var resourceVersions = this.resourceVersionRepository.GetAllAdminSearch(userId);

            var catalogueResourceVersions = resourceVersions.Join(
                nodeResources,
                rv => rv.ResourceId,
                nr => nr.ResourceId,
                (rv, nr) => rv);

            var filterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(filter);

            PagedResultSet<ResourceAdminSearchResultViewModel> result = new PagedResultSet<ResourceAdminSearchResultViewModel>();

            var items = catalogueResourceVersions;

            items = this.FilterItems(items, filterCriteria);

            result.TotalItemCount = items.Count();

            items = this.OrderItems(items, sortColumn, sortDirection);

            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            result.Items = this.mapper.ProjectTo<ResourceAdminSearchResultViewModel>(items).ToList();

            return new CatalogueResourcesViewModel
            {
                CatalogueName = catalogueNodeVersion.Name,
                Resources = result,
            };
        }

        /// <summary>
        /// The GetResources.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<CatalogueResourceResponseViewModel> GetResourcesAsync(CatalogueResourceRequestViewModel requestViewModel)
        {
            return await this.nodeResourceRepository.GetResourcesAsync(requestViewModel.NodeId, requestViewModel.CatalogueOrder, requestViewModel.Offset);
        }

        /// <summary>
        /// Returns true if the catalogue is editable by the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="catalogueId">The catalogue id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> CanUserEditCatalogueAsync(int userId, int catalogueId)
        {
            var u = await this.userRepository.GetByIdIncludeRolesAsync(userId);

            if (u != null)
            {
                var ug = u.UserUserGroup.Where(uug => uug.UserGroup.RoleUserGroup.Where(rug => rug.Scope != null && rug.Scope.CatalogueNodeId == catalogueId && (rug.RoleId == (int)RoleEnum.Editor)).ToList().Count > 0).ToList();
                return ug.Count > 0;
            }

            return false;
        }

        /// <summary>
        /// The IsUserLocalAdminAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueId">The catalogueId.</param>
        /// <returns>The task.</returns>
        public async Task<bool> IsUserLocalAdminAsync(int userId, int catalogueId)
        {
            var u = await this.userRepository.GetByIdIncludeRolesAsync(userId);

            return u.UserUserGroup.Any(uug => uug.UserGroup.RoleUserGroup
                .Any(rug => rug.Scope != null && rug.Scope.CatalogueNodeId == catalogueId && (rug.RoleId == (int)RoleEnum.LocalAdmin)));
        }

        /// <summary>
        /// Gets the restricted catalogues access requests for the supplied request view model.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel.</param>
        /// <returns>The access requests.</returns>
        public List<RestrictedCatalogueAccessRequestViewModel> GetRestrictedCatalogueAccessRequests(RestrictedCatalogueAccessRequestsRequestViewModel requestViewModel)
        {
            var vm = this.catalogueNodeVersionRepository.GetRestrictedCatalogueAccessRequests(requestViewModel);

            return vm;
        }

        /// <summary>
        /// The GetRestrictedCatalogueUsers.
        /// </summary>
        /// <param name="requestViewModel">The requestViewModel.</param>
        /// <returns>The users.</returns>
        public RestrictedCatalogueUsersViewModel GetRestrictedCatalogueUsers(RestrictedCatalogueUsersRequestViewModel requestViewModel)
        {
            var vm = this.catalogueNodeVersionRepository.GetRestrictedCatalogueUsers(requestViewModel);

            return vm;
        }

        /// <summary>
        /// Get Restricted Catalogue Summary for the supplied catalogue node id.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>A RestrictedCatalogueSummaryViewModel.</returns>
        public RestrictedCatalogueSummaryViewModel GetRestrictedCatalogueSummary(int catalogueNodeId)
        {
            var vm = this.catalogueNodeVersionRepository.GetRestrictedCatalogueSummary(catalogueNodeId);

            return vm;
        }

        /// <summary>
        /// The RejectAccessAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <param name="responseMessage">The responseMessage.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> RejectAccessAsync(int userId, int accessRequestId, string responseMessage)
        {
            var catalogueAccessRequest = this.catalogueAccessRequestRepository.GetAll().SingleOrDefault(x => x.Id == accessRequestId);
            var catalogue = this.catalogueNodeVersionRepository.GetBasicCatalogue(catalogueAccessRequest.CatalogueNodeId);
            var canEdit = await this.IsUserLocalAdminAsync(userId, catalogue.NodeVersion.NodeId);
            if (!canEdit)
            {
                throw new Exception($"User '{userId}' does not have access to manage catalogue '{catalogue.Url}'");
            }

            var car = this.catalogueAccessRequestRepository.GetAll().Include(x => x.UserProfile).SingleOrDefault(x => x.Id == accessRequestId);
            car.ResponseMessage = responseMessage;
            car.CompletedDate = this.timezoneOffsetManager.ConvertToUserTimezone(DateTimeOffset.UtcNow);
            car.Status = (int)CatalogueAccessRequestStatus.Rejected;
            await this.catalogueAccessRequestRepository.UpdateAsync(userId, car);

            // access request may have previously been accepted, need to check for user user group
            var rug = new RoleUserGroup();
            if (car.RoleId == 8)
            {
                rug = this.roleUserGroupRepository.GetAll()
                   .SingleOrDefault(x => x.RoleId == (int)RoleEnum.Previewer && x.Scope.CatalogueNodeId == catalogue.NodeVersion.NodeId);
            }
            else
            {
                rug = this.roleUserGroupRepository.GetAll()
                       .SingleOrDefault(x => x.RoleId == (int)RoleEnum.Reader && x.Scope.CatalogueNodeId == catalogue.NodeVersion.NodeId);
            }

            if (rug != null)
            {
                var uug = this.userUserGroupRepository.GetAll().SingleOrDefault(x => x.UserGroupId == rug.UserGroupId && x.UserId == car.UserId);
                if (uug != null)
                {
                    uug.Deleted = true;
                    await this.userUserGroupRepository.UpdateAsync(userId, uug);
                }
            }

            await this.notificationSenderService.SendCatalogueAccessRequestRejectedNotification(
                userId,
                catalogue.Name,
                $"{this.settings.LearningHubUrl}Catalogue/{catalogue.Url}",
                responseMessage,
                car.UserId);
            await this.emailSenderService.SendRequestAccessFailureEmail(userId, new SendEmailModel<CatalogueAccessRequestFailureEmailModel>(new CatalogueAccessRequestFailureEmailModel
            {
                UserFirstName = car.UserProfile.FirstName,
                CatalogueName = catalogue.Name,
                RejectionReason = responseMessage,
                CatalogueUrl = this.settings.LearningHubUrl + "Catalogue/" + catalogue.Url,
            })
            { EmailAddress = car.EmailAddress });
            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The AcceptAccessRequest.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> AcceptAccessAsync(int userId, int accessRequestId)
        {
            var catalogueAccessRequest = this.catalogueAccessRequestRepository.GetAll().Include(x => x.UserProfile).SingleOrDefault(x => x.Id == accessRequestId);
            var catalogue = this.catalogueNodeVersionRepository.GetBasicCatalogue(catalogueAccessRequest.CatalogueNodeId);
            var canEdit = await this.IsUserLocalAdminAsync(userId, catalogue.NodeVersion.NodeId);
            if (!canEdit)
            {
                throw new Exception($"User '{userId}' does not have access to manage catalogue '{catalogue.Url}'");
            }

            RoleUserGroup rug = new RoleUserGroup();
            if (catalogueAccessRequest.RoleId == 8)
            {
                RoleUserGroup previewerRoleuserGroup = new RoleUserGroup();
                var previewerRoleuserGroups = await this.roleUserGroupRepository.GetByRoleIdCatalogueId((int)RoleEnum.Previewer, catalogue.NodeVersion.NodeId);

                previewerRoleuserGroup = previewerRoleuserGroups.Where(r => r.RoleId == (int)RoleEnum.Previewer
                                   && r.UserGroup.UserGroupAttribute.Any(uga => uga.AttributeId == (int)AttributeEnum.PreviewerAccess)
                                   && r.Scope.ScopeType == ScopeTypeEnum.Catalogue).FirstOrDefault();

                if (previewerRoleuserGroup == null)
                {
                    var details = new List<string>();
                    details.Add($"No role user group for previewer in catalogue {catalogue.Name}");
                    return new LearningHubValidationResult(false) { Details = details };
                    throw new Exception($"No role user group for previewer in catalogue with node id {catalogue.NodeVersion.NodeId}");
                }

                var previewUserGroupId = previewerRoleuserGroup.UserGroupId;

                var previewExistingUug = this.userUserGroupRepository.GetAll().SingleOrDefault(x => x.UserId == catalogueAccessRequest.UserId && x.UserGroupId == previewUserGroupId && !x.Deleted);

                catalogueAccessRequest.Status = (int)CatalogueAccessRequestStatus.Accepted;
                catalogueAccessRequest.CompletedDate = this.timezoneOffsetManager.ConvertToUserTimezone(DateTimeOffset.UtcNow);
                await this.catalogueAccessRequestRepository.UpdateAsync(userId, catalogueAccessRequest);
                if (previewExistingUug == null)
                {
                    var previewuugId = await this.userUserGroupRepository.CreateAsync(userId, new LearningHub.Nhs.Models.Entities.UserUserGroup
                    {
                        UserGroupId = previewUserGroupId,
                        UserId = catalogueAccessRequest.UserId,
                    });
                }
            }

            var rugs = await this.roleUserGroupRepository.GetByRoleIdCatalogueId((int)RoleEnum.Reader, catalogue.NodeVersion.NodeId);

            rug = rugs.Where(r => r.RoleId == (int)RoleEnum.Reader
                                    && r.UserGroup.UserGroupAttribute.Any(uga => uga.AttributeId == (int)AttributeEnum.RestrictedAccess)
                                    && r.Scope.ScopeType == ScopeTypeEnum.Catalogue).FirstOrDefault();

            if (rug == null)
            {
                throw new Exception($"No role user group for readers in catalogue with node id {catalogue.NodeVersion.NodeId}");
            }

            var userGroupId = rug.UserGroupId;

            var existingUug = this.userUserGroupRepository.GetAll().SingleOrDefault(x => x.UserId == catalogueAccessRequest.UserId && x.UserGroupId == userGroupId && !x.Deleted);

            catalogueAccessRequest.Status = (int)CatalogueAccessRequestStatus.Accepted;
            catalogueAccessRequest.CompletedDate = this.timezoneOffsetManager.ConvertToUserTimezone(DateTimeOffset.UtcNow);
            await this.catalogueAccessRequestRepository.UpdateAsync(userId, catalogueAccessRequest);
            int uugId = 0;
            if (existingUug == null)
            {
                uugId = await this.userUserGroupRepository.CreateAsync(userId, new LearningHub.Nhs.Models.Entities.UserUserGroup
                {
                    UserGroupId = userGroupId,
                    UserId = catalogueAccessRequest.UserId,
                });
            }

            await this.notificationSenderService.SendCatalogueAccessRequestAcceptedNotification(
                userId,
                catalogue.Name,
                $"{this.settings.LearningHubUrl}Catalogue/{catalogue.Url}",
                catalogueAccessRequest.UserId);
            await this.emailSenderService.SendRequestAccessSuccessEmail(userId, new SendEmailModel<CatalogueAccessRequestSuccessEmailModel>(new CatalogueAccessRequestSuccessEmailModel
            {
                UserFirstName = catalogueAccessRequest.UserProfile.FirstName,
                CatalogueName = catalogue.Name,
                CatalogueUrl = $"{this.settings.LearningHubUrl}Catalogue/{catalogue.Url}",
            })
            { EmailAddress = catalogueAccessRequest.EmailAddress });
            return new LearningHubValidationResult(true) { CreatedId = uugId };
        }

        /// <summary>
        /// AccessRequest.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueAccessRequestId">The catalogueAccessRequestId.</param>
        /// <returns>The catalogue access request.</returns>
        public async Task<CatalogueAccessRequestViewModel> AccessRequestAsync(int userId, int catalogueAccessRequestId)
        {
            var catalogueAccessRequest = this.catalogueAccessRequestRepository.GetAll().Include(x => x.UserProfile).SingleOrDefault(x => x.Id == catalogueAccessRequestId);
            string lastResponseMessage = null;
            if (catalogueAccessRequest != null)
            {
                if (catalogueAccessRequest.Status == (int)CatalogueAccessRequestStatus.Pending)
                {
                    // Check for a previous access request which failed
                    // Will have the same userId and catalogueNodeId, not the same accessRequestId, a rejected status and a completed date.
                    var prevFailedRequest = this.catalogueAccessRequestRepository.GetAll()
                        .Where(x => x.UserId == catalogueAccessRequest.UserId && x.CatalogueNodeId == catalogueAccessRequest.CatalogueNodeId)
                        .Where(x => x.Id != catalogueAccessRequest.Id)
                        .Where(x => x.Status == (int)CatalogueAccessRequestStatus.Rejected && x.CompletedDate.HasValue)
                        .OrderByDescending(x => x.CompletedDate.Value)
                        .FirstOrDefault();
                    lastResponseMessage = prevFailedRequest?.ResponseMessage;
                }

                var catalogue = this.catalogueNodeVersionRepository.GetBasicCatalogue(catalogueAccessRequest.CatalogueNodeId);
                var canEdit = await this.IsUserLocalAdminAsync(userId, catalogue.NodeVersion.NodeId);
                if (!canEdit)
                {
                    throw new Exception($"User '{userId}' does not have access to manage catalogue '{catalogue.Url}'");
                }
            }

            var vm = this.mapper.Map<CatalogueAccessRequestViewModel>(catalogueAccessRequest);
            vm.LastResponseMessage = lastResponseMessage;
            return vm;
        }

        /// <summary>
        /// GetAllCataloguesAsync.
        /// </summary>
        /// <param name="filterChar">The filterChar.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<AllCatalogueResponseViewModel> GetAllCataloguesAsync(string filterChar, int userId)
        {
            var catalogueAlphaCount = this.catalogueNodeVersionRepository.GetAllCataloguesAlphaCount(userId);
            var filterCharMod = filterChar.Trim() == "0-9" ? "[0-9]" : filterChar;
            var count = catalogueAlphaCount.FirstOrDefault(ca => ca.Alphabet == filterChar.ToUpper()).Count;
            string prevChar = null, nextChar = null, curChar = null;
            var filterCharIndex = catalogueAlphaCount.FindIndex(ca => ca.Alphabet == filterChar.ToUpper());

            // check count and assign prev and next letter
            if (count != 0)
            {
                for (int i = 0; i < catalogueAlphaCount.Count; i++)
                {
                    if (i == filterCharIndex && i == 0)
                    {
                        prevChar = null;
                    }

                    if (i == filterCharIndex && i == catalogueAlphaCount.Count - 1)
                    {
                        nextChar = null;
                    }

                    if (catalogueAlphaCount[i].Count > 0 && i < filterCharIndex)
                    {
                        curChar = catalogueAlphaCount[i].Alphabet;
                        prevChar = curChar;
                    }

                    if (catalogueAlphaCount[i].Count > 0 && i > filterCharIndex)
                    {
                        curChar = catalogueAlphaCount[i].Alphabet;
                        nextChar = curChar;
                        break;
                    }
                }
            }

            var catalogues = await this.catalogueNodeVersionRepository.GetAllCataloguesAsync(filterCharMod, userId);
            foreach (var catalogue in catalogues)
            {
                catalogue.Providers = await this.providerService.GetByCatalogueVersionIdAsync(catalogue.NodeVersionId);
            }

            var response = new AllCatalogueResponseViewModel
            {
                CataloguesCount = catalogueAlphaCount,
                Catalogues = catalogues,
                FilterChar = filterChar.ToUpper(),
                PrevChar = prevChar,
                NextChar = nextChar,
            };
            return response;
        }

        /// <summary>
        /// The RecordNodeActivity.
        /// </summary>
        /// <param name="userId">The userId<see cref="int"/>.</param>
        /// <param name="catalogue">The catalogue<see cref="CatalogueViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task RecordNodeActivity(int userId, CatalogueViewModel catalogue)
        {
            var nodeActivity = new NodeActivity
            {
                NodeId = catalogue.NodeId,
                UserId = userId,
                CatalogueNodeVersionId = catalogue.CatalogueNodeVersionId,
                ActivityStatusId = (int)ActivityStatusEnum.Launched,
            };
            await this.nodeActivityRepository.CreateAsync(userId, nodeActivity);
        }

        /// <summary>
        /// The ValidateEditAsync.
        /// </summary>
        /// <param name="model">The model<see cref="CatalogueViewModel"/>.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        private LearningHubValidationResult ValidateEditAsync(CatalogueViewModel model)
        {
            var details = new List<string>();
            var nameNotUnique = this.catalogueNodeVersionRepository.GetAll().Any(x => x.Name == model.Name && model.CatalogueNodeVersionId != x.Id);
            var existingCatalogue = this.catalogueNodeVersionRepository.GetAll().SingleOrDefault(x => x.Id == model.CatalogueNodeVersionId);

            if (existingCatalogue == null)
            {
                return new LearningHubValidationResult(false);
            }

            if (nameNotUnique)
            {
                details.Add("NameUnique");
            }

            return new LearningHubValidationResult
            {
                Details = details,
                IsValid = !details.Any(),
            };
        }

        /// <summary>
        /// The ValidateAddAsync.
        /// </summary>
        /// <param name="model">The model<see cref="CatalogueViewModel"/>.</param>
        /// <returns>The <see cref="LearningHubValidationResult"/>.</returns>
        private LearningHubValidationResult ValidateAddAsync(CatalogueViewModel model)
        {
            var nameNotUnique = this.catalogueNodeVersionRepository.GetAll().Any(x => x.Name == model.Name);
            var urlNotUnique = this.catalogueNodeVersionRepository.GetAll().Any(x => x.Url == model.Url);
            var details = new List<string>();
            if (nameNotUnique)
            {
                details.Add("NameUnique");
            }

            if (urlNotUnique)
            {
                details.Add("UrlUnique");
            }

            return new LearningHubValidationResult
            {
                Details = details,
                IsValid = !details.Any(),
            };
        }

        /// <summary>
        /// Filter the items for resource version search.
        /// </summary>
        /// <param name="items">The items<see cref="IQueryable{ResourceVersion}"/>.</param>
        /// <param name="filterCriteria">The filterCriteria<see cref="List{PagingColumnFilter}"/>.</param>
        /// <returns>The <see cref="IQueryable{ResourceVersion}"/>.</returns>
        private IQueryable<ResourceVersion> FilterItems(IQueryable<ResourceVersion> items, List<PagingColumnFilter> filterCriteria)
        {
            if (filterCriteria == null || filterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in filterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "resourceversionid":
                        items = items.Where(x => x.Id == int.Parse(filter.Value));
                        break;
                    case "resourcereference":
                        items = items.Where(x => x.Resource.ResourceReference.Any(resRef => resRef.OriginalResourceReferenceId == int.Parse(filter.Value)));
                        break;
                    case "title":
                        items = items.Where(x => x.Title.Contains(filter.Value));
                        break;
                    case "createuser":
                        items = items.Where(x => x.CreateUser.UserName.Contains(filter.Value));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Order the items for resource version search.
        /// </summary>
        /// <param name="items">The items<see cref="IQueryable{ResourceVersion}"/>.</param>
        /// <param name="sortColumn">The sortColumn<see cref="string"/>.</param>
        /// <param name="sortDirection">The sortDirection<see cref="string"/>.</param>
        /// <returns>The <see cref="IQueryable{ResourceVersion}"/>.</returns>
        private IQueryable<ResourceVersion> OrderItems(IQueryable<ResourceVersion> items, string sortColumn, string sortDirection)
        {
            switch (sortColumn.ToLower())
            {
                case "createuser":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.CreateUser.UserName);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.CreateUser.UserName);
                    }

                    break;
                case "type":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.Resource.ResourceTypeEnum);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.Resource.ResourceTypeEnum);
                    }

                    break;
                case "status":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.VersionStatusEnum);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.VersionStatusEnum);
                    }

                    break;
                case "title":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.Title);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.Title);
                    }

                    break;
                default:
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.Resource.Id);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.Resource.Id);
                    }

                    break;
            }

            return items;
        }
    }
}
