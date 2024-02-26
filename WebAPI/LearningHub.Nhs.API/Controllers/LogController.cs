namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Log;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The log controller.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ApiControllerBase
    {
        /// <summary>
        /// The log service.
        /// </summary>
        private ILogService logService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogController"/> class.
        /// </summary>
        /// <param name="elfhUserService">
        /// The elfh user service.
        /// </param>
        /// <param name="logService">
        /// The log service.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public LogController(
            IUserService elfhUserService,
            ILogService logService,
            ILogger<LogController> logger)
            : base(elfhUserService, logger)
        {
            this.logService = logService;
        }

        // GET api/Log/GetById/id

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await this.logService.GetByIdAsync(id);

            return this.Ok(log);
        }

        // GET api/Log/GetPage/page/pageSize

        /// <summary>
        /// The get page of Log records.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetPage/{page}/{pageSize}")]
        public async Task<IActionResult> GetPage(int page, int pageSize)
        {
            PagedResultSet<LogBasicViewModel> pagedResultSet = await this.logService.GetPageAsync(page, pageSize);
            return this.Ok(pagedResultSet);
        }

        // GET api/Log/GetFilteredPage/page/pageSize/sortColumn/sortDirection/filter

        /// <summary>
        /// Get a filtered page of Log records.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{filter}")]
        public async Task<IActionResult> GetFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string filter)
        {
            PagedResultSet<LogBasicViewModel> pagedResultSet = await this.logService.GetPageAsync(page, pageSize, sortColumn, sortDirection, filter);
            return this.Ok(pagedResultSet);
        }
    }
}