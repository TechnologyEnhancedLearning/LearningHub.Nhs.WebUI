using LearningHub.Nhs.AdminUI.Configuration;
using LearningHub.Nhs.AdminUI.Extensions;
using LearningHub.Nhs.AdminUI.Interfaces;
using LearningHub.Nhs.Models.Paging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using LearningHub.Nhs.Models.GovNotifyMessaging;

namespace LearningHub.Nhs.AdminUI.Controllers
{
    /// <summary>
    /// Defines the <see cref="GovNotifyDashboardController" />.
    /// </summary>
    public class GovNotifyDashboardController : BaseController
    {
        /// <summary>
        /// Defines the websettings.
        /// </summary>
        private readonly IOptions<WebSettings> websettings;

        /// <summary>
        /// Defines the govNotifyDashboardService.
        /// </summary>
        private IGovNotifyDashboardService govNotifyDashboardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GovNotifyDashboardController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="govNotifyDashboardService">The govNotifyDashboardService<see cref="IGovNotifyDashboardService"/>.</param>
        /// <param name="websettings">The websettings<see cref="IOptions{WebSettings}"/>.</param>
        public GovNotifyDashboardController(IWebHostEnvironment hostingEnvironment,
            IGovNotifyDashboardService govNotifyDashboardService,
            IOptions<WebSettings> websettings)
            : base(hostingEnvironment)
        {
            this.websettings = websettings;
            this.govNotifyDashboardService = govNotifyDashboardService;
        }

        /// <summary>
        /// Initial call to get paginated result.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> Index()
        {
            return await this.GetMessageRequests(
                new PagingRequestModel
                {
                    Page = 1,
                    PageSize = this.websettings.Value.DefaultPageSize,
                    SortColumn = "CreatedAt",
                    SortDirection = "D",
                });
        }

        /// <summary>
        /// Get paginated result based on the filters.
        /// </summary>
        /// <param name="pagingRequestModel"></param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(string pagingRequestModel)
        {
            var requestModel = JsonConvert.DeserializeObject<PagingRequestModel>(pagingRequestModel);
            return await this.GetMessageRequests(requestModel);
        }

        /// <summary>
        /// Get message request details by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var notification = await this.govNotifyDashboardService.GetMessageRequestById(id);
            return this.View(notification);
        }

        private async Task<IActionResult> GetMessageRequests(PagingRequestModel requestModel)
        {
            requestModel.Sanitize();
            requestModel.PageSize = this.websettings.Value.DefaultPageSize;

            var model = new TablePagingViewModel<MessageRequestViewModel>
            {
                Results = await this.govNotifyDashboardService.GetPagedAsync(requestModel),
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
                FilterCount = model.Filter != null ? model.Filter.Count() : 0,
                CreateRequired = false,
            };

            return this.View(model);
        }
    }
}
