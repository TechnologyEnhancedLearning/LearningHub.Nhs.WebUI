// <copyright file="ProviderController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provider operations.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ApiControllerBase
    {
        /// <summary>
        /// The Provider service.
        /// </summary>
        private readonly IProviderService providerService;

        /// <summary>
        /// The User Provider service.
        /// </summary>
        private readonly IUserProviderService userProviderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderController"/> class.
        /// </summary>
        /// <param name="userService">
        /// The user service.
        /// </param>
        /// <param name="providerService">
        /// The Provider service.
        /// </param>
        /// <param name="userProviderService">
        /// The Uesr Provider service.
        /// </param>
        /// <param name="logger">The logger.</param>
        public ProviderController(
            IUserService userService,
            IProviderService providerService,
            IUserProviderService userProviderService,
            ILogger<ProviderController> logger)
            : base(userService, logger)
        {
            this.providerService = providerService;
            this.userProviderService = userProviderService;
        }

        /// <summary>
        /// Get Providers.
        /// </summary>
        /// <returns>List of Provider.</returns>
        [HttpGet("all")]
        public async Task<ActionResult> GetAllProviders()
        {
            return this.Ok(await this.providerService.GetAllAsync());
        }

        /// <summary>
        /// Get specific Provider by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            return this.Ok(await this.providerService.GetByIdAsync(id));
        }

        /// <summary>
        /// Get providers by user Id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetProvidersByUserId/{userId}")]
        public async Task<ActionResult> GetProvidersByUserIdAsync(int userId)
        {
            return this.Ok(await this.providerService.GetByUserIdAsync(userId));
        }

        /// <summary>
        /// Get providers by resource version Id.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersion id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetProvidersByResource/{resourceVersionId}")]
        public async Task<ActionResult> GetProvidersByResourceVersionIdAsync(int resourceVersionId)
        {
            return this.Ok(await this.providerService.GetByResourceVersionIdAsync(resourceVersionId));
        }

        /// <summary>
        /// Update user providers.
        /// </summary>
        /// <param name="userProviderUpdateViewModel">The user provider update model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("UpdateUserProvider")]
        public async Task<IActionResult> UpdateUserProviderAsync(UserProviderUpdateViewModel userProviderUpdateViewModel)
        {
            try
            {
                var vr = await this.userProviderService.UpdateUserProviderAsync(userProviderUpdateViewModel);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }
    }
}
