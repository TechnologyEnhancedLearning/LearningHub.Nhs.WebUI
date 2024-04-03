namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Extensions;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Entities.External;
    using LearningHub.Nhs.Models.Paging;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ExternalSystemController" />.
    /// </summary>
    public class ExternalSystemController : BaseController
    {
        /// <summary>
        /// Defines the _config.
        /// </summary>
        private readonly WebSettings config;

        /// <summary>
        /// Defines the websettings.
        /// </summary>
        private readonly IOptions<WebSettings> websettings;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private ILogger<HomeController> logger;

        /// <summary>
        /// Defines the _externalsystemService.
        /// </summary>
        private IExternalSystemService externalSystemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalSystemController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="config">The config<see cref="IOptions{WebSettings}"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{HomeController}"/>.</param>
        /// <param name="externalSystemService">The notificationService<see cref="IExternalSystemService"/>.</param>
        /// /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        public ExternalSystemController(
            IWebHostEnvironment hostingEnvironment,
            IOptions<WebSettings> config,
            ILogger<HomeController> logger,
            IExternalSystemService externalSystemService,
            IOptions<WebSettings> websettings)
        : base(hostingEnvironment)
        {
            this.logger = logger;
            this.config = config.Value;
            this.externalSystemService = externalSystemService;
            this.websettings = websettings;
        }

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="source">The source<see cref="string"/>.</param>
        /// <param name="copy">The copy<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id, string source = "", bool copy = false)
        {
            this.ViewBag.source = source;
            this.ViewBag.copy = false;
            var externalsystem = await this.externalSystemService.GetIdAsync(id);
            return this.View("CreateEdit", externalsystem);
        }

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="externalSystem">The notification<see cref="ExternalSystem"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(ExternalSystem externalSystem)
        {
            int externalSystemId = externalSystem.Id;
            this.ViewData["Reffer"] = this.Request.Headers["Referer"].ToString();
            this.ViewData["externalSystem"] = externalSystem;
            if (externalSystem.Id == 0)
            {
                var isCreatIdReturnedOrError = await this.externalSystemService.Create(externalSystem);
                bool creatIdReturned = int.TryParse(isCreatIdReturnedOrError, out externalSystemId);
                if (creatIdReturned == false)
                {
                    this.ViewBag.UpdateSaveError = isCreatIdReturnedOrError;
                    return this.View("CreateEdit", this.ViewData["externalSystem"]);
                }
            }
            else
            {
                this.ViewBag.UpdateSaveError = await this.externalSystemService.Edit(externalSystem);
                if (this.ViewBag.UpdateSaveError != null)
                {
                    return await this.RedirecToCreatEdit(externalSystem);
                }
            }

            return this.RedirectToAction("Details", new { id = externalSystemId });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            this.ViewBag.source = "list";
            var externalsystem = new ExternalSystem();
            externalsystem.CreateDate = DateTime.Now.Date;
            externalsystem.AmendDate = DateTime.Now.Date;
            return this.View("CreateEdit", externalsystem);
        }

        /// <summary>
        /// The Details.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var externalSystem = await this.externalSystemService.GetIdAsync(id);
            return this.View(externalSystem);
        }

        /// <summary>
        /// Delete a notification.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await this.externalSystemService.DeleteAsync(id);
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> Index()
        {
            return await this.GetExternalSystem(
                new PagingRequestModel
                {
                    Page = 1,
                    PageSize = this.config.DefaultPageSize,
                    SortColumn = "Name",
                    SortDirection = "D",
                });
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(string pagingRequestModel)
        {
            var requestModel = JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);
            return await this.GetExternalSystem(requestModel);
        }

        private async Task<IActionResult> GetExternalSystem(PagingRequestModel requestModel)
        {
            requestModel.PageSize = this.config.DefaultPageSize;
            requestModel.Sanitize();
            var model = new TablePagingViewModel<ExternalSystem>
            {
                Results = await this.externalSystemService.GetExternalSystems(requestModel),
                SortColumn = requestModel.SortColumn,
                SortDirection = requestModel.SortDirection,
                Filter = requestModel.Filter,
            };

            model.Paging = new PagingViewModel
            {
                CurrentPage = requestModel.Page,
                HasItems = model.Results.Items.Any(),
                PageSize = this.config.DefaultPageSize,
                TotalItems = model.Results.TotalItemCount,
            };

            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = model.Filter != null ? model.Filter.Count() : 0,
                CreateRequired = true,
            };

            return this.View(model);
        }

        private async Task<IActionResult> RedirecToCreatEdit(ExternalSystem externalSystem)
        {
            var externalsystem = this.externalSystemService.GetIdAsync(externalSystem.Id);

            this.ViewBag.source = "list";
            this.ViewBag.copy = false;
            var eSystem = await this.externalSystemService.GetIdAsync(externalSystem.Id);
            return this.View("CreateEdit", eSystem);
        }
    }
}