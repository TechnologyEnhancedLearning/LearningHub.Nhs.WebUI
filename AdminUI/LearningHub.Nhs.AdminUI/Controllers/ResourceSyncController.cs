// <copyright file="ResourceSyncController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The ResourceSyncController.
    /// </summary>
    public class ResourceSyncController : BaseController
    {
        /// <summary>
        /// Defines the resourceSyncService.
        /// </summary>
        private readonly IResourceSyncService resourceSyncService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSyncController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="resourceSyncService">The resourceSyncService<see cref="IResourceSyncService"/>.</param>
        public ResourceSyncController(
            IWebHostEnvironment hostingEnvironment,
            IResourceSyncService resourceSyncService)
        : base(hostingEnvironment)
        {
            this.resourceSyncService = resourceSyncService;
        }

        /// <summary>
        /// The AddToSyncList.
        /// </summary>
        /// <param name="vm">The vm<see cref="ResourceSyncViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> AddToSyncList(ResourceSyncViewModel vm)
        {
            await this.resourceSyncService.AddToSyncListAsync(vm.ResourceIds);
            return this.Ok();
        }

        /// <summary>
        /// The RemoveFromSyncList.
        /// </summary>
        /// <param name="vm">The vm<see cref="ResourceSyncViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveFromSyncList(ResourceSyncViewModel vm)
        {
            await this.resourceSyncService.RemoveFromSyncListAsync(vm.ResourceIds);
            return this.Ok();
        }

        /// <summary>
        /// The Resources.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Resources()
        {
            var resources = await this.resourceSyncService.GetResourceSyncs();
            return this.View(resources);
        }

        /// <summary>
        /// The SyncSingle.
        /// </summary>
        /// <param name="resourceSyncSingleViewModel">The resourceSyncSingleViewModel<see cref="ResourceSyncSingleViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> SyncSingle(ResourceSyncSingleViewModel resourceSyncSingleViewModel)
        {
            var apiResponse = await this.resourceSyncService.SyncSingle(resourceSyncSingleViewModel.ResourceId);
            var vr = apiResponse.ValidationResult;
            if (vr.IsValid)
            {
                return this.Json(new
                {
                    success = true,
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.Details.Count > 0 ? vr.Details[0] : string.Empty,
                });
            }
        }

        /// <summary>
        /// The SyncWithFindwise.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> SyncWithFindwise()
        {
            var apiResponse = await this.resourceSyncService.SyncWithFindwise();
            var vr = apiResponse.ValidationResult;
            if (vr.IsValid)
            {
                return this.Json(new
                {
                    success = true,
                });
            }
            else
            {
                return this.Json(new
                {
                    success = false,
                    details = vr.Details.Count > 0 ? vr.Details[0] : string.Empty,
                });
            }
        }
    }
}
