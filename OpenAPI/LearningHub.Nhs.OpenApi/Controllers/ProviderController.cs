namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Provider operations.
    /// </summary>
    [Route("Provider")]
    [ApiController]
    public class ProviderController : OpenApiControllerBase
    {
        /// <summary>
        /// The Provider service.
        /// </summary>
        private readonly IProviderService providerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderController"/> class.
        /// </summary>
        /// <param name="providerService">
        /// The Provider service.
        /// <param name="logger">The logger.</param>
        public ProviderController(IProviderService providerService)
        {
            this.providerService = providerService;
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
            return this.Ok(await providerService.GetByIdAsync(id));
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
            return this.Ok(await providerService.GetByUserIdAsync(userId));
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
            return this.Ok(await providerService.GetByResourceVersionIdAsync(resourceVersionId));
        }

    }
}
