namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Databricks;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Learning;
    using LearningHub.Nhs.WebUI.Models.Report;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="ReportsController" />.
    /// </summary>
    [ServiceFilter(typeof(LoginWizardFilter))]
    [ServiceFilter(typeof(ReporterPermissionFilter))]
    [Authorize]
    [Route("Reports")]
    public class ReportsController : BaseController
    {
        private const int ReportPageSize = 10;
        private readonly ICacheService cacheService;
        private readonly ICategoryService categoryService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IReportService reportService;
        private readonly IFileService fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportsController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">httpClientFactory.</param>
        /// <param name="cacheService">cacheService.</param>
        /// <param name="multiPageFormService">multiPageFormService.</param>
        /// <param name="reportService">reportService.</param>
        /// <param name="categoryService">categoryService.</param>
        /// <param name="fileService">fileService.</param>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{AccountController}"/>.</param>
        /// <param name="settings">settings.</param>
        public ReportsController(IHttpClientFactory httpClientFactory, IWebHostEnvironment hostingEnvironment, ILogger<AccountController> logger, IOptions<Settings> settings, ICacheService cacheService, IMultiPageFormService multiPageFormService, IReportService reportService, ICategoryService categoryService, IFileService fileService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.cacheService = cacheService;
            this.multiPageFormService = multiPageFormService;
            this.reportService = reportService;
            this.categoryService = categoryService;
            this.fileService = fileService;
        }

        /// <summary>
        /// The Report landing page.
        /// </summary>
        /// <param name="reportHistoryViewModel">reportHistoryViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [ResponseCache(CacheProfileName = "Never")]
        public async Task<IActionResult> Index(ReportHistoryViewModel reportHistoryViewModel = null)
        {
            int page = 1;
            this.TempData.Clear();
            var newReport = new DatabricksRequestModel { Take = ReportPageSize, Skip = 0 };

            await this.multiPageFormService.SetMultiPageFormData(
              newReport,
              MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"),
              this.TempData);

            var historyRequest = new PagingRequestModel
            {
                PageSize = ReportPageSize,
            };

            switch (reportHistoryViewModel.ReportFormActionType)
            {
                case ReportFormActionTypeEnum.NextPageChange:
                    reportHistoryViewModel.CurrentPageIndex += 1;
                    break;

                case ReportFormActionTypeEnum.PreviousPageChange:
                    reportHistoryViewModel.CurrentPageIndex -= 1;
                    break;
                default:
                    reportHistoryViewModel.CurrentPageIndex = 0;
                    break;
            }

            page = page + reportHistoryViewModel.CurrentPageIndex;
            historyRequest.Page = page;

            // get a list of report history and send to view
            var result = await this.reportService.GetReportHistory(historyRequest);
            if (result != null)
            {
                reportHistoryViewModel.TotalCount = result.TotalItemCount;
                reportHistoryViewModel.ReportHistoryModels = result.Items;
            }

            this.ViewData["AllCourses"] = await this.GetCoursesAsync();
            reportHistoryViewModel.ReportPaging = new ReportPagingModel() { CurrentPage = reportHistoryViewModel.CurrentPageIndex, PageSize = ReportPageSize, TotalItems = reportHistoryViewModel.TotalCount, HasItems = reportHistoryViewModel.TotalCount > 0 };
            return this.View(reportHistoryViewModel);
        }

        /// <summary>
        /// CreateReportCourseSelection.
        /// </summary>
        /// <param name="searchText">searchText.</param>
        /// <param name="returnUrl">returnUrl.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("CreateReportCourseSelection")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { "ReportWizardCWF" })]
        public async Task<IActionResult> CreateReportCourseSelection(string searchText = "", string returnUrl = "")
        {
            this.ViewBag.ReturnUrl = returnUrl;
            var reportCreation = await this.multiPageFormService.GetMultiPageFormData<DatabricksRequestModel>(MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);
            var coursevm = new ReportCreationCourseSelection { SearchText = searchText, Courses = reportCreation.Courses != null ? reportCreation.Courses : new List<string>() };
            var getCourses = await this.GetCoursesAsync();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                getCourses = getCourses.Where(x => x.Value.ToLower().Contains(searchText.ToLower())).ToList();
            }

            if (coursevm.Courses.Count == 0 && !string.IsNullOrWhiteSpace(reportCreation.TimePeriod))
            {
                coursevm.Courses = new List<string> { "all" };
            }

            coursevm.BuildCourses(getCourses);
            return this.View(coursevm);
        }

        /// <summary>
        /// CreateReportCourseSelection.
        /// </summary>
        /// <param name="courseSelection">courseSelection.</param>
        /// <param name="returnUrl">returnurl.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("CreateReportCourseSelection")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { "ReportWizardCWF" })]
        public async Task<IActionResult> CreateReportCourseSelection(ReportCreationCourseSelection courseSelection, string returnUrl = "")
        {
            var reportCreation = await this.multiPageFormService.GetMultiPageFormData<DatabricksRequestModel>(MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);

            if (courseSelection.Courses != null)
            {
                if (courseSelection.Courses.Any())
                {
                    reportCreation.Courses = courseSelection.Courses.Contains("all") ? new List<string>() : courseSelection.Courses;
                    await this.multiPageFormService.SetMultiPageFormData(reportCreation, MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return this.Redirect(returnUrl);
                    }

                    return this.RedirectToAction("CreateReportDateSelection");
                }
            }

            this.ModelState.AddModelError("Courses", CommonValidationErrorMessages.CourseRequired);
            reportCreation.Courses = null;
            await this.multiPageFormService.SetMultiPageFormData(reportCreation, MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);
            courseSelection.BuildCourses(await this.GetCoursesAsync());
            courseSelection.Courses = reportCreation.Courses;
            this.ViewBag.ReturnUrl = returnUrl;
            return this.View("CreateReportCourseSelection", courseSelection);
        }

        /// <summary>
        /// CreateReportDateSelection.
        /// </summary>
        /// <param name="returnUrl">returnUrl.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("CreateReportDateSelection")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { "ReportWizardCWF" })]
        public async Task<IActionResult> CreateReportDateSelection(string returnUrl = "")
        {
            this.ViewBag.ReturnUrl = returnUrl;
            {
                var reportCreation = await this.multiPageFormService.GetMultiPageFormData<DatabricksRequestModel>(MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);
                var dateVM = new ReportCreationDateSelection();
                dateVM.TimePeriod = reportCreation.TimePeriod;
                if (reportCreation.StartDate.HasValue && reportCreation.TimePeriod == "Custom")
                {
                    dateVM.StartDay = reportCreation.StartDate.HasValue ? reportCreation.StartDate.GetValueOrDefault().Day : 0;
                    dateVM.StartMonth = reportCreation.StartDate.HasValue ? reportCreation.StartDate.GetValueOrDefault().Month : 0;
                    dateVM.StartYear = reportCreation.StartDate.HasValue ? reportCreation.StartDate.GetValueOrDefault().Year : 0;
                    dateVM.EndDay = reportCreation.EndDate.HasValue ? reportCreation.EndDate.GetValueOrDefault().Day : 0;
                    dateVM.EndMonth = reportCreation.EndDate.HasValue ? reportCreation.EndDate.GetValueOrDefault().Month : 0;
                    dateVM.EndYear = reportCreation.EndDate.HasValue ? reportCreation.EndDate.GetValueOrDefault().Year : 0;
                }

                var minDate = await this.GetMinDate();

                dateVM.DataStart = minDate.DataStart;
                dateVM.HintText = minDate.HintText;

                return this.View(dateVM);
            }
        }

        /// <summary>
        /// CreateReportDateSelection.
        /// </summary>
        /// <param name="reportCreationDate">reportCreationDate.</param>
        /// <param name="returnUrl">returnurl.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("CreateReportSummary")]
        [HttpPost]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { "ReportWizardCWF" })]
        public async Task<IActionResult> CreateReportSummary(ReportCreationDateSelection reportCreationDate, string returnUrl = "")
        {
            // validate date
            var reportCreation = await this.multiPageFormService.GetMultiPageFormData<DatabricksRequestModel>(MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);
            reportCreation.TimePeriod = reportCreationDate.TimePeriod;

            if (reportCreation.TimePeriod == null)
            {
                var minDate = await this.GetMinDate();
                reportCreationDate.DataStart = minDate.DataStart;
                reportCreationDate.HintText = minDate.HintText;
                this.ModelState.AddModelError("TimePeriod", CommonValidationErrorMessages.ReportingPeriodRequired);
                this.ViewBag.ReturnUrl = returnUrl;
                return this.View("CreateReportDateSelection", reportCreationDate);
            }

            if (!this.ModelState.IsValid)
            {
                var minDate = await this.GetMinDate();
                reportCreationDate.DataStart = minDate.DataStart;
                reportCreationDate.HintText = minDate.HintText;
                this.ViewBag.ReturnUrl = returnUrl;
                return this.View("CreateReportDateSelection", reportCreationDate);
            }

            reportCreation.StartDate = reportCreationDate.GetStartDate();
            reportCreation.EndDate = reportCreationDate.GetEndDate();
            await this.multiPageFormService.SetMultiPageFormData(reportCreation, MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);
            return this.RedirectToAction("CourseProgressReport");
        }

        /// <summary>
        /// ViewReport.
        /// </summary>
        /// <param name="reportHistoryId">reportHistoryId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("ViewReport/{reportHistoryId}")]
        [HttpGet]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { "ReportWizardCWF" })]
        public async Task<IActionResult> ViewReport(int reportHistoryId)
        {
            this.TempData.Clear();
            var report = await this.reportService.GetReportHistoryById(reportHistoryId);
            if (report == null)
            {
                return this.RedirectToAction("Index");
            }

            var reportRequest = new DatabricksRequestModel { Take = ReportPageSize, Skip = 0, ReportHistoryId = reportHistoryId };
            var periodCheck = int.TryParse(report.PeriodDays.ToString(), out int numberOfDays);
            if (report.PeriodDays > 0 && periodCheck)
            {
                reportRequest.TimePeriod = report.PeriodDays.ToString();
                reportRequest.StartDate = DateTime.Now.AddDays(-numberOfDays);
                reportRequest.EndDate = DateTime.Now;
            }
            else
            {
                reportRequest.TimePeriod = "Custom";
                reportRequest.StartDate = report.StartDate;
                reportRequest.EndDate = report.EndDate;
            }

            if (report.CourseFilter == "all")
            {
                report.CourseFilter = string.Empty;
            }

            reportRequest.Courses = report.CourseFilter.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToList();

            await this.multiPageFormService.SetMultiPageFormData(reportRequest, MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);
            return this.RedirectToAction("CourseProgressReport");
        }

        /// <summary>
        /// DownloadReport.
        /// </summary>
        /// <param name="reportHistoryId">reportHistoryId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("DownloadReport/{reportHistoryId}")]
        [HttpGet]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { "ReportWizardCWF" })]
        public async Task<IActionResult> DownloadReport(int reportHistoryId)
        {
            var report = await this.reportService.DownloadReport(reportHistoryId);
            if (report == null)
            {
                return this.RedirectToAction("Index");
            }

            var result = await this.fileService.DownloadBlobFileAsync(report.FilePath);
            return this.File(result.Stream, result.ContentType, result.FileName);
        }

        /// <summary>
        /// ViewReport.
        /// </summary>
        /// <param name="reportHistoryId">reportHistoryId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("QueueReportDownload")]
        [HttpPost]
        [ResponseCache(CacheProfileName = "Never")]
        public async Task<IActionResult> QueueReportDownload(int reportHistoryId)
        {
            await this.reportService.QueueReportDownload(reportHistoryId);
            return this.RedirectToAction("CourseProgressReport");
        }

        /// <summary>
        /// CourseCompletionReport.
        /// </summary>
        /// <param name="courseCompletion">courseCompletion.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("CourseProgressReport")]
        [HttpGet]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { "ReportWizardCWF" })]
        public async Task<IActionResult> CourseProgressReport(CourseCompletionViewModel courseCompletion = null)
        {
            int page = 1;

            // validate date
            var reportCreation = await this.multiPageFormService.GetMultiPageFormData<DatabricksRequestModel>(MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);

            switch (courseCompletion.ReportFormActionType)
            {
                case ReportFormActionTypeEnum.NextPageChange:
                    courseCompletion.CurrentPageIndex += 1;
                    page = page + courseCompletion.CurrentPageIndex;
                    reportCreation.Skip = courseCompletion.CurrentPageIndex * ReportPageSize;
                    break;

                case ReportFormActionTypeEnum.PreviousPageChange:
                    courseCompletion.CurrentPageIndex -= 1;
                    page = page + courseCompletion.CurrentPageIndex;
                    reportCreation.Skip = courseCompletion.CurrentPageIndex * ReportPageSize;
                    break;
                default:
                    courseCompletion.CurrentPageIndex = 0;
                    reportCreation.Skip = 0;
                    break;
            }

            DateTimeOffset today = DateTimeOffset.Now.Date;
            DateTimeOffset? startDate = null;
            DateTimeOffset? endDate = null;

            if (int.TryParse(reportCreation.TimePeriod, out int days))
            {
                startDate = today.AddDays(-days);
                endDate = today;
            }
            else if (reportCreation.TimePeriod == "Custom")
            {
                startDate = reportCreation.StartDate;
                endDate = reportCreation.EndDate;
            }

            var result = await this.reportService.GetCourseCompletionReport(new DatabricksRequestModel
            {
                StartDate = startDate,
                EndDate = endDate,
                TimePeriod = reportCreation.TimePeriod,
                Courses = reportCreation.Courses,
                ReportHistoryId = reportCreation.ReportHistoryId,
                Take = reportCreation.Take,
                Skip = page,
            });

            var response = new CourseCompletionViewModel(reportCreation);

            if (result != null)
            {
                response.TotalCount = result.TotalCount;
                response.CourseCompletionRecords = result.CourseCompletionRecords;
                response.ReportHistoryModel = await this.reportService.GetReportHistoryById(result.ReportHistoryId);
                reportCreation.ReportHistoryId = result.ReportHistoryId;
            }

            await this.multiPageFormService.SetMultiPageFormData(reportCreation, MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);

            var allCourses = await this.GetCoursesAsync();

            List<string> matchedCourseNames;

            if (reportCreation.Courses.Count == 0)
            {
                matchedCourseNames = new List<string> { "all courses" };
            }
            else
            {
                matchedCourseNames = allCourses
                    .Where(course => reportCreation.Courses.Contains(course.Key))
                    .Select(course => course.Value)
                    .ToList();
            }

            this.ViewData["matchedCourseNames"] = matchedCourseNames;
            response.ReportPaging = new ReportPagingModel() { CurrentPage = courseCompletion.CurrentPageIndex, PageSize = ReportPageSize, TotalItems = response.TotalCount, HasItems = response.TotalCount > 0 };
            return this.View(response);
        }

        private async Task<List<KeyValuePair<string, string>>> GetCoursesAsync()
        {
            int categoryId = this.Settings.StatMandId;
            var courses = new List<KeyValuePair<string, string>>();
            var subCategories = await this.categoryService.GetCoursesByCategoryIdAsync(categoryId);

            foreach (var subCategory in subCategories.Courses)
            {
                courses.Add(new KeyValuePair<string, string>(subCategory.Id.ToString(), UtilityHelper.ConvertToSentenceCase(subCategory.Displayname)));
            }

            return courses;
        }

        private async Task<ReportCreationDateSelection> GetMinDate()
        {
            var dateVM = new ReportCreationDateSelection();
            var reportCreation = await this.multiPageFormService.GetMultiPageFormData<DatabricksRequestModel>(MultiPageFormDataFeature.AddCustomWebForm("ReportWizardCWF"), this.TempData);

            var result = await this.reportService.GetCourseCompletionReport(new DatabricksRequestModel
            {
                StartDate = null,
                EndDate = null,
                TimePeriod = reportCreation.TimePeriod,
                Courses = reportCreation.Courses,
                ReportHistoryId = 0,
                Take = 1,
                Skip = 1,
            });

            if (result != null)
            {
                var validDate = DateTime.TryParse(result.MinValidDate, out DateTime startDate);
                dateVM.DataStart = validDate ? startDate : null;
                dateVM.HintText = validDate
                    ? $"For example, {startDate.Day} {startDate.Month} {startDate.Year}"
                    : $"For example, {DateTime.Now.Day} {DateTime.Now.Month} {DateTime.Now.Year}";
            }
            else
            {
                dateVM.DataStart = null;
                dateVM.HintText = $"For example, {DateTime.Now.Day} {DateTime.Now.Month} {DateTime.Now.Year}";
            }

            return dateVM;
        }
    }
}
