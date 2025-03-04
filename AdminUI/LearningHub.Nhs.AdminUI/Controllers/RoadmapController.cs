namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.AdminUI.Models;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Paging;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="RoadmapController" />.
    /// </summary>
    public class RoadmapController : BaseController
    {
        /// <summary>
        /// Defines the RoadmapImageDirectory.
        /// </summary>
        public const string RoadmapImageDirectory = "RoadmapImage";

        /// <summary>
        /// Defines the _config.
        /// </summary>
        private readonly WebSettings config;

        /// <summary>
        /// Defines the _fileService.
        /// </summary>
        private readonly IFileService fileService;

        /// <summary>
        /// Defines the _roadmapService.
        /// </summary>
        private readonly IRoadmapService roadmapService;

        /// <summary>
        /// Defines the websettings.
        /// </summary>
        private readonly IOptions<WebSettings> websettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoadmapController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="roadmapService">The roadmapService<see cref="IRoadmapService"/>.</param>
        /// <param name="config">The config<see cref="IOptions{WebSettings}"/>.</param>
        /// <param name="fileService">The fileService<see cref="IFileService"/>.</param>
        /// /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        public RoadmapController(
            IWebHostEnvironment hostingEnvironment,
            IRoadmapService roadmapService,
            IOptions<WebSettings> config,
            IFileService fileService,
            IOptions<WebSettings> websettings)
        : base(hostingEnvironment)
        {
            this.roadmapService = roadmapService;
            this.config = config.Value;
            this.fileService = fileService;
            this.websettings = websettings;
        }

        /// <summary>
        /// The AddUpdate.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public IActionResult AddUpdate()
        {
            return this.View("Update", new UpdateViewModel());
        }

        /// <summary>
        /// The AddUpdate.
        /// </summary>
        /// <param name="update">The update<see cref="UpdateViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> AddUpdate(UpdateViewModel update)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("Update", update);
            }

            await this.UploadFile(update);
            var roadmap = new Roadmap
            {
                Description = update.Description,
                Title = update.Title,
                RoadmapDate = update.RoadmapDate,
                ImageName = update.ImageName,
                Published = update.Published,
                RoadmapTypeId = update.RoadmapTypeId,
            };
            var roadmapId = await this.roadmapService.AddRoadmap(roadmap);
            return this.RedirectToAction("Details", new { id = roadmapId });
        }

        /// <summary>
        /// The DeleteUpdate.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteUpdate(int id)
        {
            await this.roadmapService.DeleteRoadmapAsync(id);
            return this.RedirectToAction("Updates");
        }

        /// <summary>
        /// The EditUpdate.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> EditUpdate(int id)
        {
            var roadmap = await this.roadmapService.GetRoadmap(id);
            var vm = new UpdateViewModel
            {
                Id = roadmap.Id,
                Description = roadmap.Description,
                ImageName = roadmap.ImageName,
                Published = roadmap.Published,
                RoadmapDate = roadmap.RoadmapDate,
                Title = roadmap.Title,
            };
            return this.View("Update", vm);
        }

        /// <summary>
        /// The EditUpdate.
        /// </summary>
        /// <param name="update">The update<see cref="UpdateViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> EditUpdate(UpdateViewModel update)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("Update", update);
            }

            await this.UploadFile(update);
            var roadmap = new Roadmap
            {
                Description = update.Description,
                Title = update.Title,
                ImageName = update.ImageName,
                Published = update.Published,
                RoadmapTypeId = update.RoadmapTypeId,
                RoadmapDate = update.RoadmapDate,
                Id = update.Id,
            };
            await this.roadmapService.UpdateRoadmap(roadmap);
            return this.RedirectToAction("Details", new { roadmap.Id });
        }

        /// <summary>
        /// The Updates.
        /// </summary>
        /// <param name="searchTerm">The searchTerm<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> Updates(string searchTerm = null)
        {
            var updates = await this.roadmapService.GetUpdates();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                updates = updates.Where(x => x.Title.ToLower().Contains(lowerSearchTerm)).ToList();
                this.ViewData["SearchTerm"] = searchTerm;
            }

            var model = new TablePagingViewModel<Roadmap>
            {
                Results = new PagedResultSet<Roadmap> { Items = updates, TotalItemCount = updates.Count },
                SortColumn = "Id",
                SortDirection = "A",
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = 1,
                HasItems = model.Results.Items.Any(),
                PageSize = this.config.DefaultPageSize,
                TotalItems = model.Results.TotalItemCount,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = 0,
                CreateRequired = false,
            };
            return this.View(model);
        }

        /// <summary>
        /// The Details.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var roadmap = await this.roadmapService.GetIdAsync(id);
            return this.View(roadmap);
        }

        /// <summary>
        /// The UploadFile.
        /// </summary>
        /// <param name="vm">The vm<see cref="UpdateViewModel"/>.</param>
        private async Task UploadFile(UpdateViewModel vm)
        {
            var files = this.Request.Form.Files;
            if (files.Any())
            {
                // ToDo: Delete old image if the roadmap already has one.
                var file = files.Single();
                var fileName = Guid.NewGuid().ToString();
                await this.fileService.ProcessFile(file.OpenReadStream(), fileName, RoadmapImageDirectory);
                vm.ImageName = fileName;
            }
        }
    }
}
