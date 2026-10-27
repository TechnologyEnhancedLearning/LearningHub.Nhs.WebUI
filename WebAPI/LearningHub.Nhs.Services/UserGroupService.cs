namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The user group service.
    /// </summary>
    public class UserGroupService : IUserGroupService
    {
        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The catalogue service.
        /// </summary>
        private ICatalogueService catalogueService;

        /// <summary>
        /// The user group repository.
        /// </summary>
        private IUserGroupRepository userGroupRepository;

        /// <summary>
        /// The user user group repository.
        /// </summary>
        private IUserUserGroupRepository userUserGroupRepository;

        /// <summary>
        /// The scope repository.
        /// </summary>
        private IScopeRepository scopeRepository;

        /// <summary>
        /// The role user group repository.
        /// </summary>
        private IRoleUserGroupRepository roleUserGroupRepository;

        /// <summary>
        /// The user group attribute repository.
        /// </summary>
        private IUserGroupAttributeRepository userGroupAttributeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupService"/> class.
        /// </summary>
        /// <param name="catalogueService">The catalogue service.</param>
        /// <param name="userGroupRepository">The user group repository.</param>
        /// <param name="userUserGroupRepository">The user - user group repository.</param>
        /// <param name="scopeRepository">The scope repository.</param>
        /// <param name="roleUserGroupRepository">The role - user group repository.</param>
        /// <param name="userGroupAttributeRepository">The user group attribute repository.</param>
        /// <param name="mapper">The mapper.</param>
        public UserGroupService(
            ICatalogueService catalogueService,
            IUserGroupRepository userGroupRepository,
            IUserUserGroupRepository userUserGroupRepository,
            IScopeRepository scopeRepository,
            IRoleUserGroupRepository roleUserGroupRepository,
            IUserGroupAttributeRepository userGroupAttributeRepository,
            IMapper mapper)
        {
            this.catalogueService = catalogueService;
            this.userGroupRepository = userGroupRepository;
            this.userUserGroupRepository = userUserGroupRepository;
            this.scopeRepository = scopeRepository;
            this.roleUserGroupRepository = roleUserGroupRepository;
            this.userGroupAttributeRepository = userGroupAttributeRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserGroup> GetByIdAsync(int id)
        {
            return await this.userGroupRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<UserGroup> GetByIdAsync(int id, bool includeRoles)
        {
            return await this.userGroupRepository.GetByIdAsync(id, includeRoles);
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userGroup">The user group.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateAsync(int userId, UserGroup userGroup)
        {
            var validationResult = await this.ValidateAsync(userGroup);

            if (validationResult.IsValid)
            {
                validationResult.CreatedId = await this.userGroupRepository.CreateAsync(userId, userGroup);
            }

            return validationResult;
        }

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userGroupId">The user group id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteAsync(int userId, int userGroupId)
        {
            var userGroup = await this.userGroupRepository.GetByIdAsync(userGroupId);

            if (userGroup == null)
            {
                return new LearningHubValidationResult(false, "User group not found.");
            }

            try
            {
                await this.userGroupRepository.DeleteAsync(userId, userGroupId);
                return new LearningHubValidationResult(true);
            }
            catch (Exception ex)
            {
                return new LearningHubValidationResult(false, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// The validate async.
        /// </summary>
        /// <param name="userGroup">The user group.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> ValidateAsync(UserGroup userGroup)
        {
            var userGroupValidator = new UserGroupValidator();
            var validatorResult = await userGroupValidator.ValidateAsync(userGroup);

            var retVal = new LearningHubValidationResult(validatorResult);

            if (retVal.IsValid)
            {
                var ug = await this.userGroupRepository.GetByNameAsync(userGroup.Name);
                if (ug != null)
                {
                    if (userGroup.IsNew()
                        || (!userGroup.IsNew() && userGroup.Id != ug.Id))
                    {
                        var detail = string.Format("Name '{0}' is already in use", userGroup.Name);
                        retVal.Add(new LearningHubValidationResult(false, detail));
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Returns a user group detail view model for the supplied id.
        /// </summary>
        /// <param name="id">The user group id.</param>
        /// <returns>The <see cref="UserGroupAdminDetailViewModel"/>.</returns>
        public async Task<UserGroupAdminDetailViewModel> GetUserGroupAdminDetailByIdAsync(int id)
        {
            var userGroup = await this.userGroupRepository.GetByIdAsync(id, true);

            var model = this.mapper.Map<UserGroupAdminDetailViewModel>(userGroup);

            return model;
        }

        /// <summary>
        /// Returns a list of role user group detail view models for the supplied user group id.
        /// </summary>
        /// <param name="userGroupId">The user group id.</param>
        /// <returns>The list of <see cref="RoleUserGroupViewModel"/>.</returns>
        public async Task<List<RoleUserGroupViewModel>> GetUserGroupRoleDetailByUserGroupId(int userGroupId)
        {
            var vm = await this.roleUserGroupRepository.GetRoleUserGroupViewModelsByUserGroupId(userGroupId);

            return vm;
        }

        /// <summary>
        /// Returns a list of role user group detail view models for the supplied user id.
        /// </summary>
        /// <param name="userId">The user group id.</param>
        /// <returns>The list of <see cref="RoleUserGroupViewModel"/>.</returns>
        public async Task<List<RoleUserGroupViewModel>> GetRoleUserGroupDetailByUserId(int userId)
        {
            var vm = await this.roleUserGroupRepository.GetRoleUserGroupViewModelsByUserId(userId);

            return vm;
        }

        /// <summary>
        /// Create a user group.
        /// </summary>
        /// <param name="userGroupAdminDetailViewModel">The user group admin detail view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateUserGroupAsync(UserGroupAdminDetailViewModel userGroupAdminDetailViewModel, int currentUserId)
        {
            var ug = await this.userGroupRepository.GetByNameAsync(userGroupAdminDetailViewModel.Name);
            if (ug != null)
            {
                return new LearningHubValidationResult(false, "User group name already exists");
            }

            var userGroup = new UserGroup();
            userGroup.Name = userGroupAdminDetailViewModel.Name;
            userGroup.Description = userGroupAdminDetailViewModel.Description;

            var userGroupValidator = new UserGroupValidator();
            var validationResult = await userGroupValidator.ValidateAsync(userGroup);
            var retVal = new LearningHubValidationResult(validationResult);

            if (retVal.IsValid)
            {
                retVal.CreatedId = await this.userGroupRepository.CreateAsync(currentUserId, userGroup);
            }

            return retVal;
        }

        /// <summary>
        /// Updates a user group.
        /// </summary>
        /// <param name="userGroupAdminDetailViewModel">The user group admin detail view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateUserGroupAsync(UserGroupAdminDetailViewModel userGroupAdminDetailViewModel, int currentUserId)
        {
            var userGroupValidator = new UserGroupValidator();

            var userGroup = await this.userGroupRepository.GetByIdAsync(userGroupAdminDetailViewModel.Id);

            userGroup.Name = userGroupAdminDetailViewModel.Name;
            userGroup.Description = userGroupAdminDetailViewModel.Description;

            var validationResult = await userGroupValidator.ValidateAsync(userGroup);
            var retVal = new LearningHubValidationResult(validationResult);

            if (retVal.IsValid)
            {
                await this.userGroupRepository.UpdateAsync(currentUserId, userGroup);
            }

            return retVal;
        }

        /// <summary>
        /// Adds a user group attribute.
        /// </summary>
        /// <param name="userGroupAttribute">The user group attribute view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> AddUserGroupAttribute(UserGroupAttributeViewModel userGroupAttribute, int currentUserId)
        {
            if (!userGroupAttribute.IsNew())
            {
                return new LearningHubValidationResult(false, $"Error - supplied user group attribute is not new.");
            }

            var existing = await this.userGroupAttributeRepository.GetByUserGroupIdAttributeId(userGroupAttribute.UserGroupId, userGroupAttribute.AttributeId);
            if (existing != null)
            {
                return new LearningHubValidationResult(false, $"Error - user group attribute already exists for the supplied identifiers.");
            }

            try
            {
                var entity = this.mapper.Map<UserGroupAttribute>(userGroupAttribute);
                int id = await this.userGroupAttributeRepository.CreateAsync(currentUserId, entity);

                var retVal = new LearningHubValidationResult(true);
                retVal.CreatedId = id;
                return retVal;
            }
            catch (Exception ex)
            {
                return new LearningHubValidationResult(false, $"Error adding user group attribute: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a user group attribute.
        /// </summary>
        /// <param name="userGroupAttribute">The user group attribute view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteUserGroupAttributeAsync(UserGroupAttributeViewModel userGroupAttribute, int currentUserId)
        {
            var entity = await this.userGroupAttributeRepository.GetByIdAsync(userGroupAttribute.UserGroupAttributeId);

            if (entity == null)
            {
                return new LearningHubValidationResult(false, "UserGroupAttribute not found.");
            }

            entity.Deleted = true;
            await this.userGroupAttributeRepository.UpdateAsync(currentUserId, entity);

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Adds a list of users to a user group.
        /// </summary>
        /// <param name="userUserGroups">The user user group view model list.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> AddUserUserGroups(List<UserUserGroupViewModel> userUserGroups, int currentUserId)
        {
            try
            {
                foreach (var userUserGroup in userUserGroups)
                {
                    var userUserGroupslst = await this.userUserGroupRepository.GetByUserIdandUserGroupIdAsync(userUserGroup.UserId, userUserGroup.UserGroupId);
                    if (userUserGroupslst == null)
                    {
                      var entity = this.mapper.Map<UserUserGroup>(userUserGroup);
                      await this.userUserGroupRepository.CreateAsync(currentUserId, entity);
                    }
                }
            }
            catch (Exception ex)
            {
                return new LearningHubValidationResult(false, $"Error adding users to user group: {ex.Message}");
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Adds a list of role user groups.
        /// Creates a new Scope entry if necessary.
        /// </summary>
        /// <param name="roleUserGroups">The role user group view model list.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> AddRoleUserGroups(List<RoleUserGroupUpdateViewModel> roleUserGroups, int currentUserId)
        {
            var newRoleUserGroups = new List<RoleUserGroup>();
            try
            {
                foreach (var roleUserGroup in roleUserGroups)
                {
                    // Cater for Catalogue Scope Type.
                    // Create new Scope if necessary.
                    // Reture validation failure if any supplied RoleUserGroup already exists.
                    if (roleUserGroup.ScopeType == ScopeTypeEnum.Catalogue)
                    {
                        int scopeId;
                        var scope = await this.scopeRepository.GetByCatalogueNodeIdAsync(roleUserGroup.CatalogueNodeId);
                        if (scope == null)
                        {
                            scope = new Scope() { ScopeType = ScopeTypeEnum.Catalogue, CatalogueNodeId = roleUserGroup.CatalogueNodeId };
                            scopeId = await this.scopeRepository.CreateAsync(currentUserId, scope);
                        }
                        else
                        {
                            scopeId = scope.Id;

                            var rug = await this.roleUserGroupRepository.GetByRoleIdUserGroupIdScopeIdAsync(roleUserGroup.RoleId, roleUserGroup.UserGroupId, scopeId);
                            if (rug != null)
                            {
                                return new LearningHubValidationResult(false, $"RoleUserGroup with Catalogue ScopeType already exists: RoleId={roleUserGroup.RoleId}, UserGroupId={roleUserGroup.UserGroupId}, ScopeId={scopeId}");
                            }
                        }

                        newRoleUserGroups.Add(new RoleUserGroup()
                        {
                            RoleId = roleUserGroup.RoleId,
                            UserGroupId = roleUserGroup.UserGroupId,
                            ScopeId = scopeId,
                        });
                    }
                }

                foreach (var roleUserGroup in newRoleUserGroups)
                {
                    await this.roleUserGroupRepository.CreateAsync(currentUserId, roleUserGroup);
                }
            }
            catch (Exception ex)
            {
                return new LearningHubValidationResult(false, $"Error adding role user group: {ex.Message}");
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Removes user from a user group.
        /// </summary>
        /// <param name="userUserGroupViewModel">The user user group view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteUserUserGroupAsync(UserUserGroupViewModel userUserGroupViewModel, int currentUserId)
        {
            var userUserGroup = await this.userUserGroupRepository.GetByIdAsync(userUserGroupViewModel.Id);

            if (userUserGroup == null)
            {
                return new LearningHubValidationResult(false, "User user group not found.");
            }

            userUserGroup.Deleted = true;
            await this.userUserGroupRepository.UpdateAsync(currentUserId, userUserGroup);

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Removes a role - user group.
        /// </summary>
        /// <param name="roleUserGroupViewModel">The role user group view model.</param>
        /// <param name="currentUserId">The current user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteRoleUserGroupAsync(RoleUserGroupUpdateViewModel roleUserGroupViewModel, int currentUserId)
        {
            var roleUserGroup = await this.roleUserGroupRepository.GetByIdAsync(roleUserGroupViewModel.Id);

            if (roleUserGroup == null)
            {
                return new LearningHubValidationResult(false, "Role - UserGroup association not found.");
            }

            // Do not allow delete of "default restricted access user group" in a Restricted Catalogue
            if (roleUserGroup.RoleId == (int)RoleEnum.Reader
                &&
                roleUserGroup.UserGroup.UserGroupAttribute.Any(uga => uga.AttributeId == (int)AttributeEnum.RestrictedAccess)
                &&
                roleUserGroup.Scope.ScopeType == ScopeTypeEnum.Catalogue)
            {
                var catalogue = this.catalogueService.GetBasicCatalogue(roleUserGroup.Scope.CatalogueNodeId.Value);
                if (catalogue.RestrictedAccess)
                {
                    return new LearningHubValidationResult(false, "Cannot delete the default Restricted Access User Group in a Restricted Catalogue");
                }
            }

            // Ensure that at least one Local Admin group exists for a Restricted Catalogue
            if (roleUserGroup.RoleId == (int)RoleEnum.LocalAdmin
                &&
                roleUserGroup.Scope.ScopeType == ScopeTypeEnum.Catalogue)
            {
                var catalogue = this.catalogueService.GetBasicCatalogue(roleUserGroup.Scope.CatalogueNodeId.Value);
                if (catalogue.RestrictedAccess && !catalogue.Hidden)
                {
                    var roleUserGroups = await this.roleUserGroupRepository.GetByRoleIdCatalogueId(roleUserGroup.RoleId, catalogue.NodeId);
                    if (!roleUserGroups.Any(rug => rug.Id != roleUserGroupViewModel.Id))
                    {
                        return new LearningHubValidationResult(false, "A Restricted Catalogue that is visible must have at least one Local Admin User Group.");
                    }
                }
            }

            roleUserGroup.Deleted = true;
            await this.roleUserGroupRepository.UpdateAsync(currentUserId, roleUserGroup);

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// Returns a list of "role - user group" info - filtered, sorted and paged as required.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The presetFilter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PagedResultSet<RoleUserGroupViewModel>> GetRoleUserGroupAdminFilteredPage(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "")
        {
            var presetFilterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(presetFilter);
            var filterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(filter);

            PagedResultSet<RoleUserGroupViewModel> result = new PagedResultSet<RoleUserGroupViewModel>();

            var items = this.roleUserGroupRepository.GetAll();

            items = this.PresetFilterRoleUserGroupItems(items, presetFilterCriteria);
            items = this.FilterRoleUserGroupItems(items, filterCriteria);

            result.TotalItemCount = items.Count();

            items = this.OrderRoleUserGroupItems(items, sortColumn, sortDirection);
            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            result.Items = await this.mapper.ProjectTo<RoleUserGroupViewModel>(items).ToListAsync();

            return result;
        }

        /// <summary>
        /// Returns a list of "user - user group" info - filtered, sorted and paged as required.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The presetFilter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PagedResultSet<UserUserGroupViewModel>> GetUserUserGroupAdminFilteredPage(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "")
        {
            var presetFilterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(presetFilter);
            var filterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(filter);

            PagedResultSet<UserUserGroupViewModel> result = new PagedResultSet<UserUserGroupViewModel>();

            var items = this.userUserGroupRepository.GetAll();

            items = this.PresetFilterUserUserGroupItems(items, presetFilterCriteria);
            items = this.FilterUserUserGroupItems(items, filterCriteria);

            result.TotalItemCount = items.Count();

            items = this.OrderUserUserGroupItems(items, sortColumn, sortDirection);
            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            result.Items = await this.mapper.ProjectTo<UserUserGroupViewModel>(items).ToListAsync();

            return result;
        }

        /// <summary>
        /// Returns a list of basic user group info - filtered, sorted and paged as required.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The preset filter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PagedResultSet<UserGroupAdminBasicViewModel>> GetUserGroupAdminBasicPageAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "")
        {
            var presetFilterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(presetFilter);
            var filterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(filter);

            PagedResultSet<UserGroupAdminBasicViewModel> result = new PagedResultSet<UserGroupAdminBasicViewModel>();

            var items = this.userGroupRepository.GetAll();

            items = this.PresetFilterUserGroupItems(items, presetFilterCriteria);
            items = this.FilterUserGroupItems(items, filterCriteria);

            result.TotalItemCount = items.Count();

            items = this.OrderUserGroupItems(items, sortColumn, sortDirection);

            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            result.Items = await this.mapper.ProjectTo<UserGroupAdminBasicViewModel>(items).ToListAsync();

            return result;
        }

        /// <summary>
        /// Apply preset filter to the items for user user group search.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="presetFilterCriteria">The filter criteria.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<UserUserGroup> PresetFilterUserUserGroupItems(IQueryable<UserUserGroup> items, List<PagingColumnFilter> presetFilterCriteria)
        {
            if (presetFilterCriteria == null || presetFilterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in presetFilterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "userid":
                        items = items.Where(x => x.UserId == int.Parse(filter.Value));
                        break;
                    case "usergroupid":
                        items = items.Where(x => x.UserGroupId == int.Parse(filter.Value));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Filter the items for user user group search.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<UserUserGroup> FilterUserUserGroupItems(IQueryable<UserUserGroup> items, List<PagingColumnFilter> filterCriteria)
        {
            if (filterCriteria == null || filterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in filterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "userid":
                        int enteredId = 0;
                        int.TryParse(filter.Value, out enteredId);
                        items = items.Where(x => x.UserId == enteredId);
                        break;
                    case "usergroupid":
                        items = items.Where(x => x.UserGroupId == int.Parse(filter.Value));
                        break;
                    case "usergroupname":
                        items = items.Where(x => x.UserGroup.Name.Contains(filter.Value));
                        break;
                    case "username":
                        items = items.Where(x => x.User.UserName.Contains(filter.Value));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Filter the items for role user group search.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<RoleUserGroup> FilterRoleUserGroupItems(IQueryable<RoleUserGroup> items, List<PagingColumnFilter> filterCriteria)
        {
            if (filterCriteria == null || filterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in filterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "id":
                        int enteredId = 0;
                        int.TryParse(filter.Value, out enteredId);
                        items = items.Where(x => x.UserGroupId == enteredId);
                        break;
                    case "name":
                        items = items.Where(x => x.UserGroup.Name.Contains(filter.Value));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Order the items for user - user group search.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<UserUserGroup> OrderUserUserGroupItems(IQueryable<UserUserGroup> items, string sortColumn, string sortDirection)
        {
            switch (sortColumn.ToLower())
            {
                case "username":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.User.UserName);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.User.UserName);
                    }

                    break;
                default:
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.UserId);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.UserId);
                    }

                    break;
            }

            return items;
        }

        /// <summary>
        /// Order the items for role - user group.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<RoleUserGroup> OrderRoleUserGroupItems(IQueryable<RoleUserGroup> items, string sortColumn, string sortDirection)
        {
            switch (sortColumn.ToLower())
            {
                case "usergroupname":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.UserGroup.Name);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.UserGroup.Name);
                    }

                    break;
                default:
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.UserGroupId);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.UserGroupId);
                    }

                    break;
            }

            return items;
        }

        /// <summary>
        /// Preset filter the items for user group search.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="presetFilterCriteria">The filter criteria.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<UserGroup> PresetFilterUserGroupItems(IQueryable<UserGroup> items, List<PagingColumnFilter> presetFilterCriteria)
        {
            if (presetFilterCriteria == null || presetFilterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in presetFilterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "userid":
                        int enteredId = 0;
                        int.TryParse(filter.Value, out enteredId);
                        items = items.Where(x => x.Id == enteredId);
                        break;
                    case "userid_exclude":
                        items = items.Where(x => !x.UserUserGroup.Any(uug => uug.UserId == int.Parse(filter.Value)));
                        break;
                    case "cataloguenodeid":
                        var roleFilter = presetFilterCriteria.Where(f => f.Column.ToLower() == "roleid").FirstOrDefault();
                        int catalogueNodeId = 0;
                        int.TryParse(filter.Value, out catalogueNodeId);
                        if (roleFilter == null)
                        {
                            items = items.Where(x => x.RoleUserGroup.Any(rug => rug.Scope.CatalogueNodeId == catalogueNodeId));
                        }
                        else
                        {
                            int roleId = 0;
                            int.TryParse(roleFilter.Value, out roleId);
                            items = items.Where(x => x.RoleUserGroup.Any(rug => rug.RoleId == roleId && rug.Scope.CatalogueNodeId == catalogueNodeId));
                        }

                        break;
                    case "cataloguenodeid_exclude":
                        var excludeRoleFilter = presetFilterCriteria.Where(f => f.Column.ToLower() == "roleid_exclude").FirstOrDefault();
                        int excludeCatalogueNodeId = 0;
                        int.TryParse(filter.Value, out excludeCatalogueNodeId);
                        if (excludeRoleFilter == null)
                        {
                            items = items.Where(x => x.RoleUserGroup.Any(rug => rug.Scope.CatalogueNodeId == excludeCatalogueNodeId));
                        }
                        else
                        {
                            int roleId = 0;
                            int.TryParse(excludeRoleFilter.Value, out roleId);
                            items = items.Where(x => !x.RoleUserGroup.Any(rug => (rug.RoleId == roleId && rug.Scope.CatalogueNodeId == excludeCatalogueNodeId)
                                                                                || (rug.RoleId == roleId && rug.Scope.ScopeType == ScopeTypeEnum.Catalogue && !rug.Scope.CatalogueNodeId.HasValue)));
                        }

                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Preset filter the items for role user group.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="presetFilterCriteria">The filter criteria.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<RoleUserGroup> PresetFilterRoleUserGroupItems(IQueryable<RoleUserGroup> items, List<PagingColumnFilter> presetFilterCriteria)
        {
            if (presetFilterCriteria == null || presetFilterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in presetFilterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "roleid":
                        int roleId = 0;
                        int.TryParse(filter.Value, out roleId);
                        items = items.Where(x => x.RoleId == roleId);
                        break;
                    case "cataloguenodeid":
                        int catalogueNodeId = 0;
                        int.TryParse(filter.Value, out catalogueNodeId);
                        items = items.Where(x => x.Scope.CatalogueNodeId == catalogueNodeId);

                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Filter the items for user group search.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<UserGroup> FilterUserGroupItems(IQueryable<UserGroup> items, List<PagingColumnFilter> filterCriteria)
        {
            if (filterCriteria == null || filterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in filterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "id":
                        int enteredId = 0;
                        int.TryParse(filter.Value, out enteredId);
                        items = items.Where(x => x.Id == enteredId);
                        break;
                    case "usergroupname":
                    case "name":
                        items = items.Where(x => x.Name.Contains(filter.Value));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Order the items for user group search.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<UserGroup> OrderUserGroupItems(IQueryable<UserGroup> items, string sortColumn, string sortDirection)
        {
            switch (sortColumn.ToLower())
            {
                case "name":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.Name);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.Name);
                    }

                    break;
                default:
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.Id);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.Id);
                    }

                    break;
            }

            return items;
        }
    }
}
