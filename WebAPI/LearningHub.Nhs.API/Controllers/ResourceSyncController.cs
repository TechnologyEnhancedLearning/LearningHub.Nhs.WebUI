// <copyright file="ResourceSyncController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The ResourceSyncController.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    public class ResourceSyncController : ApiControllerBase
    {
        private readonly IResourceSyncService resourceSyncService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSyncController"/> class.
        /// </summary>
        /// <param name="userService">The userService.</param>
        /// <param name="resourceSyncService">The resourceSyncService.</param>
        /// <param name="logger">The logger.</param>
        public ResourceSyncController(IUserService userService, IResourceSyncService resourceSyncService, ILogger<ResourceSyncController> logger)
            : base(userService, logger)
        {
            this.resourceSyncService = resourceSyncService;
        }

        /// <summary>
        /// The GetResourceSyncs.
        /// </summary>
        /// <returns>The task.</returns>
        [HttpGet("")]
        public IActionResult GetResourceSyncs()
        {
            var syncs = this.resourceSyncService.GetSyncListResourcesForUser(this.CurrentUserId);
            return this.Ok(syncs);
        }

        /// <summary>
        /// The AddToSyncList.
        /// </summary>
        /// <param name="resourceIds">The resourceIds.</param>
        /// <returns>The task.</returns>
        [HttpPost("SyncList")]
        public async Task<IActionResult> AddToSyncList(List<int> resourceIds)
        {
            await this.resourceSyncService.AddToSyncListAsync(this.CurrentUserId, resourceIds);
            return this.Ok();
        }

        /// <summary>
        /// The RemoveFromSyncList.
        /// </summary>
        /// <param name="resourceIds">The resourceIds.</param>
        /// <returns>The task.</returns>
        [HttpPost("SyncListRemove")]
        public async Task<IActionResult> RemoveFromSyncList(List<int> resourceIds)
        {
            await this.resourceSyncService.RemoveFromSyncListAsync(this.CurrentUserId, resourceIds);
            return this.Ok();
        }

        /// <summary>
        /// The SyncWithFindwise.
        /// </summary>
        /// <returns>The action result.</returns>
        [HttpPost("Sync")]
        public async Task<IActionResult> SyncWithFindwise()
        {
            var vr = await this.resourceSyncService.SyncForUserAsync(this.CurrentUserId);
            return this.Ok(new ApiResponse(vr.IsValid, vr));
        }

        /// <summary>
        /// The SyncSingle.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns>The action result.</returns>
        [HttpPost("SyncSingle")]
        public async Task<IActionResult> SyncSingle(ResourceAdminSearchResultViewModel vm)
        {
            var vr = await this.resourceSyncService.SyncSingleAsync(vm.ResourceId);
            return this.Ok(new ApiResponse(vr.IsValid, vr));
        }
    }
}
