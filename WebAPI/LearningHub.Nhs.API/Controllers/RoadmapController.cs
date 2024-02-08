namespace LearningHub.Nhs.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.RoadMap;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The RoadmapController.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoadmapController : ApiControllerBase
    {
        private readonly IRoadmapService roadmapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoadmapController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="roadmapService">The roadmap service.</param>
        /// <param name="logger">The logger.</param>
        public RoadmapController(
            IUserService userService,
            IRoadmapService roadmapService,
            ILogger<RoadmapController> logger)
            : base(userService, logger)
        {
            this.roadmapService = roadmapService;
        }

        /// <summary>
        /// The GetUpdates.
        /// </summary>
        /// <returns>The updates.</returns>
        [HttpGet]
        [Route("Updates")]
        public List<Roadmap> GetUpdates()
        {
            return this.roadmapService.GetUpdates();
        }

        /// <summary>
        /// The GetUpdates.
        /// </summary>
        /// <param name="numberOfResults">numberOfResults.</param>
        /// <returns>The updates.</returns>
        [HttpGet]
        [Route("Updates/{numberOfResults}")]
        [AllowAnonymous]
        public RoadMapResponseViewModel GetUpdates(int numberOfResults = 10)
        {
            return this.roadmapService.GetUpdates(numberOfResults);
        }

        /// <summary>
        /// The GetRoadmap.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The roadmap.</returns>
        [HttpGet]
        [Route("Roadmaps/{id}")]
        public async Task<Roadmap> GetRoadmap(int id)
        {
            return await this.roadmapService.GetRoadmap(id);
        }

        /// <summary>
        /// The AddRoadmap.
        /// </summary>
        /// <param name="roadmap">The roadmap.</param>
        /// <returns>The task.</returns>
        [HttpPost]
        [Route("Roadmaps")]
        public async Task<int> AddRoadmap(Roadmap roadmap)
        {
            return await this.roadmapService.AddRoadmapAsync(this.CurrentUserId, roadmap);
        }

        /// <summary>
        /// The UpdateRoadmap.
        /// </summary>
        /// <param name="roadmap">The roadmap.</param>
        /// <returns>The roadmap id.</returns>
        [HttpPut]
        [Route("Roadmaps")]
        public async Task UpdateRoadmap(Roadmap roadmap)
        {
            await this.roadmapService.UpdateRoadmapAsync(this.CurrentUserId, roadmap);
        }

        /// <summary>
        /// The DeleteRoadmap.
        /// </summary>
        /// <param name="id">The roadmap id.</param>
        /// <returns>The task.</returns>
        [HttpDelete]
        [Route("Roadmaps/{id}")]
        public async Task DeleteRoadmap(int id)
        {
            await this.roadmapService.DeleteRoadmap(this.CurrentUserId, id);
        }

        /// <summary>
        /// The UpdateOrder.
        /// </summary>
        /// <param name="roadmapOrdering">The roadmap ordering.</param>
        /// <returns>The task.</returns>
        [HttpPost]
        public async Task UpdateOrder(RoadmapOrdering roadmapOrdering)
        {
            await this.roadmapService.UpdateOrderingAsync(this.CurrentUserId, roadmapOrdering);
        }
    }
}
