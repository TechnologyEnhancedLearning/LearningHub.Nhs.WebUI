// <copyright file="LogController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Extensions;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Log;
    using LearningHub.Nhs.Models.Paging;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="LogController" />.
    /// </summary>
    public class LogController : BaseController
    {
        /// <summary>
        /// Defines the _logService.
        /// </summary>
        private readonly ILogService logService;

        /// <summary>
        /// Defines the _websettings.
        /// </summary>
        private readonly IOptions<WebSettings> websettings;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private ILogger<LogController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="logService">The logService<see cref="ILogService"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{LogController}"/>.</param>
        /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        public LogController(
            IWebHostEnvironment hostingEnvironment,
            ILogService logService,
            ILogger<LogController> logger,
            IOptions<WebSettings> websettings)
        : base(hostingEnvironment)
        {
            this.logService = logService;
            this.logger = logger;
            this.websettings = websettings;
        }

        /// <summary>
        /// The Details.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var log = await this.logService.GetIdAsync(id);
            return this.View(log);
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            PagingRequestModel pagingRequestModel = new PagingRequestModel();
            pagingRequestModel.Page = 1;
            pagingRequestModel.PageSize = this.websettings.Value.DefaultPageSize;
            pagingRequestModel.SortColumn = "Id";
            pagingRequestModel.SortDirection = "D";
            pagingRequestModel.Filter = new List<PagingColumnFilter>();

            var model = new TablePagingViewModel<LogBasicViewModel>
            {
                Results = await this.logService.GetPagedAsync(pagingRequestModel),
                SortColumn = "Id",
                SortDirection = "D",
            };
            model.Paging = new PagingViewModel
            {
                CurrentPage = 1,
                HasItems = model.Results != null && model.Results.Items.Any(),
                PageSize = this.websettings.Value.DefaultPageSize,
                TotalItems = model.Results != null ? model.Results.TotalItemCount : 0,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results != null ? model.Results.TotalItemCount : 0,
                DisplayedCount = model.Results != null ? model.Results.Items.Count() : 0,
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = 0,
                CreateRequired = false,
            };
            return this.View(model);
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(string pagingRequestModel)
        {
            var requestModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);

            requestModel.Sanitize();
            requestModel.PageSize = this.websettings.Value.DefaultPageSize;

            var model = new TablePagingViewModel<LogBasicViewModel>
            {
                Results = await this.logService.GetPagedAsync(requestModel),
                SortColumn = requestModel.SortColumn,
                SortDirection = requestModel.SortDirection,
                Filter = requestModel.Filter,
            };

            model.Paging = new PagingViewModel
            {
                CurrentPage = requestModel.Page,
                HasItems = model.Results.Items.Any(),
                PageSize = this.websettings.Value.DefaultPageSize,
                TotalItems = model.Results.TotalItemCount,
            };
            model.ListPageHeader = new ListPageHeaderViewModel
            {
                TotalItemCount = model.Results.TotalItemCount,
                DisplayedCount = model.Results.Items.Count(),
                DefaultPageSize = this.websettings.Value.DefaultPageSize,
                FilterCount = model.Filter.Count(),
                CreateRequired = false,
            };

            return this.View(model);
        }
    }
}
