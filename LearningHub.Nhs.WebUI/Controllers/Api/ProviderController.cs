// <copyright file="ProviderController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the <see cref="ProviderController" />.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : BaseApiController
    {
        private IProviderService providerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderController"/> class.
        /// </summary>
        /// <param name="providerService">Provider service.</param>
        /// <param name="logger">Logger.</param>
        public ProviderController(IProviderService providerService, ILogger<ContributeController> logger)
            : base(logger)
        {
            this.providerService = providerService;
        }

        /// <summary>
        /// The GetProviders.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetProviders")]
        public async Task<ActionResult> GetProvidersAsync()
        {
            var providers = await this.providerService.GetProviders();
            return this.Ok(providers);
        }

        /// <summary>
        /// The GetProvidersForUserAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetProvidersForUser")]
        public async Task<ActionResult> GetProvidersForUserAsync()
        {
            var providers = await this.providerService.GetProvidersForUserAsync(this.CurrentUserId);
            return this.Ok(providers);
        }
    }
}
