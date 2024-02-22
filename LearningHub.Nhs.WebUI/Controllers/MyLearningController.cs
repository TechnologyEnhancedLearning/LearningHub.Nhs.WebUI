// <copyright file="MyLearningController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Extensions;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using LearningHub.Nhs.WebUI.Models.Learning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The MyLearningController.
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(LoginWizardFilter))]
    public class MyLearningController : BaseController
    {
        private const int MyLearningPageSize = 10;
        private readonly IMyLearningService myLearningService;
        private readonly IResourceService resourceService;
        private readonly IHierarchyService hierarchyService;
        private readonly IUserService userService;
        private readonly IPDFReportService pdfReportService;
        private readonly IFileService fileService;
        private readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="httpClientFactory">The httpClientFactory.</param>
        /// <param name="myLearningService">myLearning service.</param>
        /// <param name="resourceService">resource Service.</param>
        /// <param name="hierarchyService">hierarchy Service.</param>
        /// <param name="userService">user Service.</param>
        /// <param name="pdfReportService">PDF Report Service.</param>
        /// <param name="fileService">fileService.</param>
        public MyLearningController(IWebHostEnvironment hostingEnvironment, ILogger<ResourceController> logger, IOptions<Settings> settings, IHttpClientFactory httpClientFactory, IMyLearningService myLearningService, IResourceService resourceService, IHierarchyService hierarchyService, IUserService userService, IPDFReportService pdfReportService, IFileService fileService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.myLearningService = myLearningService;
            this.resourceService = resourceService;
            this.userService = userService;
            this.pdfReportService = pdfReportService;
            this.hierarchyService = hierarchyService;
            this.fileService = fileService;
            this.filePath = "CatalogueImageDirectory";
        }

        /// <summary>
        /// RenderRazorViewToString.
        /// </summary>
        /// <param name="controller">controller.</param>
        /// <param name="viewName">viewName.</param>
        /// <param name="model">model.</param>
        /// <returns>Html as string.</returns>
        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine =
                    controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as
                        ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions());
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Index.
        /// </summary>
        /// <param name="learningRequest">learningRequest.</param>
        /// <param name="myLearningDashboard">The my learning dashboard type.</param>
        /// <returns>IActionResult.</returns>
        [Route("MyLearning")]
        [Route("MyLearning/activity")]
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Index(MyLearningViewModel learningRequest = null, string myLearningDashboard = null)
        {
            var myLearningRequestModel = new MyLearningRequestModel
            {
                SearchText = learningRequest.SearchText?.Trim(),
                Skip = learningRequest.CurrentPageIndex * MyLearningPageSize,
                Take = MyLearningPageSize,
                TimePeriod = !string.IsNullOrWhiteSpace(learningRequest.TimePeriod) ? learningRequest.TimePeriod : "allDates",
                StartDate = learningRequest.StartDate,
                EndDate = learningRequest.EndDate,
                Weblink = learningRequest.Weblink,
                File = learningRequest.File,
                Video = learningRequest.Video,
                Article = learningRequest.Article,
                Case = learningRequest.Case,
                Image = learningRequest.Image,
                Audio = learningRequest.Audio,
                Elearning = learningRequest.Elearning,
                Html = learningRequest.Html,
                Assessment = learningRequest.Assessment,
                Complete = learningRequest.Complete,
                Incomplete = learningRequest.Incomplete,
                Passed = learningRequest.Passed,
                Failed = learningRequest.Failed,
                Downloaded = learningRequest.Downloaded,
                CertificateEnabled = learningRequest.CertificateEnabled,
            };

            if (myLearningDashboard != null)
            {
                if (myLearningDashboard == "my-in-progress")
                {
                    myLearningRequestModel.Incomplete = true;
                    myLearningRequestModel.Failed = true;
                }
                else if (myLearningDashboard == "my-recent-completed")
                {
                    myLearningRequestModel.Complete = true;
                    myLearningRequestModel.Passed = true;
                    myLearningRequestModel.Downloaded = true;
                }
                else if (myLearningDashboard == "my-certificates")
                {
                    myLearningRequestModel.CertificateEnabled = true;
                    myLearningRequestModel.Complete = true;
                    myLearningRequestModel.Passed = true;
                    myLearningRequestModel.Downloaded = true;
                }
            }

            switch (learningRequest.MyLearningFormActionType)
            {
                case MyLearningFormActionTypeEnum.NextPageChange:
                    learningRequest.CurrentPageIndex += 1;
                    myLearningRequestModel.Skip = learningRequest.CurrentPageIndex * MyLearningPageSize;
                    break;

                case MyLearningFormActionTypeEnum.PreviousPageChange:
                    learningRequest.CurrentPageIndex -= 1;
                    myLearningRequestModel.Skip = learningRequest.CurrentPageIndex * MyLearningPageSize;
                    break;
                case MyLearningFormActionTypeEnum.BasicSearch:

                    myLearningRequestModel = new MyLearningRequestModel
                    {
                        SearchText = learningRequest.SearchText?.Trim(),
                        TimePeriod = !string.IsNullOrWhiteSpace(learningRequest.TimePeriod) ? learningRequest.TimePeriod : "allDates",
                        Skip = learningRequest.CurrentPageIndex * MyLearningPageSize,
                        Take = MyLearningPageSize,
                    };
                    break;

                case MyLearningFormActionTypeEnum.ApplyWeekFilter:
                    myLearningRequestModel = new MyLearningRequestModel
                    {
                        SearchText = learningRequest.SearchText?.Trim(),
                        TimePeriod = "thisWeek",
                        Skip = learningRequest.CurrentPageIndex * MyLearningPageSize,
                        Take = MyLearningPageSize,
                    };
                    break;
                case MyLearningFormActionTypeEnum.ApplyMonthFilter:
                    myLearningRequestModel = new MyLearningRequestModel
                    {
                        SearchText = learningRequest.SearchText?.Trim(),
                        TimePeriod = "thisMonth",
                        Skip = learningRequest.CurrentPageIndex * MyLearningPageSize,
                        Take = MyLearningPageSize,
                    };
                    break;

                case MyLearningFormActionTypeEnum.ApplyTwelveMonthFilter:
                    myLearningRequestModel = new MyLearningRequestModel
                    {
                        SearchText = learningRequest.SearchText?.Trim(),
                        TimePeriod = "last12Months",
                        Skip = learningRequest.CurrentPageIndex * MyLearningPageSize,
                        Take = MyLearningPageSize,
                    };
                    break;

                case MyLearningFormActionTypeEnum.ApplyMajorFilters:
                    if (learningRequest.TimePeriod == "dateRange")
                    {
                        if (!this.ModelState.IsValid)
                        {
                            break;
                        }

                        myLearningRequestModel.TimePeriod = learningRequest.TimePeriod;
                        myLearningRequestModel.StartDate = learningRequest.GetStartDate().HasValue ? learningRequest.GetStartDate().Value : null;
                        myLearningRequestModel.EndDate = learningRequest.GetEndDate().HasValue ? learningRequest.GetEndDate().Value : null;
                    }

                    break;

                case MyLearningFormActionTypeEnum.ClearAllFilters:

                    myLearningRequestModel = new MyLearningRequestModel
                    {
                        SearchText = learningRequest.SearchText?.Trim(),
                        Skip = learningRequest.CurrentPageIndex * MyLearningPageSize,
                        TimePeriod = "allDates",
                        Take = MyLearningPageSize,
                    };
                    break;
            }

            var result = await this.myLearningService.GetActivityDetailed(myLearningRequestModel);
            var response = new MyLearningViewModel(myLearningRequestModel);
            if (learningRequest.TimePeriod == "dateRange")
            {
                response.StartDay = learningRequest.StartDay;
                response.StartMonth = learningRequest.StartMonth;
                response.StartYear = learningRequest.StartYear;
                response.EndDay = learningRequest.EndDay;
                response.EndMonth = learningRequest.EndMonth;
                response.EndYear = learningRequest.EndYear;
            }

            if (result != null)
            {
                response.TotalCount = result.TotalCount;
                response.Activities = result.Activities.Select(entry => new ActivityDetailedItemViewModel(entry)).ToList();
                if (response.Activities.Any())
                {
                    foreach (var activity in response.Activities)
                    {
                        if (!response.MostRecentResources.Contains(activity.ResourceId))
                        {
                            activity.IsMostRecent = true;
                            response.MostRecentResources.Add(activity.ResourceId);
                        }
                    }
                }
            }

            response.MyLearningPaging = new MyLearningPagingModel() { CurrentPage = learningRequest.CurrentPageIndex, PageSize = MyLearningPageSize, TotalItems = response.TotalCount, HasItems = response.TotalCount > 0 };
            this.ViewBag.MyLearningHelpUrl = this.Settings.SupportUrls.MyLearningHelpUrl;
            return this.View(response);
        }

        /// <summary>
        /// Function to export activity report to pdf.
        /// </summary>
        /// <param name="myLearningRequestModel">myLearningRequestModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("/MyLearning/ExportToPDF")]
        [HttpPost]
        public async Task<IActionResult> ExportToPDF(MyLearningRequestModel myLearningRequestModel)
        {
            var filter = myLearningRequestModel;
            filter.Skip = 0;
            filter.Take = 999;
            var userDetails = await this.userService.GetCurrentUserBasicDetailsAsync();
            var response = new MyLearningViewModel();
            var result = await this.myLearningService.GetActivityDetailed(filter);
            if (result != null)
            {
                response.TotalCount = result.TotalCount;
                response.Activities = result.Activities.Select(entry => new ActivityDetailedItemViewModel(entry)).ToList();
                if (response.Activities.Any())
                {
                    foreach (var activity in response.Activities)
                    {
                        if (!response.MostRecentResources.Contains(activity.ResourceId))
                        {
                            activity.IsMostRecent = true;
                            response.MostRecentResources.Add(activity.ResourceId);
                        }
                    }
                }
            }

            Tuple<UserBasicViewModel, MyLearningViewModel> modelData = Tuple.Create(userDetails, response);
            var renderedViewHTML = RenderRazorViewToString(this, "ExportToPDF", modelData);
            ReportStatusModel reportStatusModel = new ReportStatusModel();
            var pdfReportResponse = await this.pdfReportService.PdfReport(renderedViewHTML, userDetails.Id);
            if (pdfReportResponse != null)
            {
                do
                {
                    reportStatusModel = await this.pdfReportService.PdfReportStatus(pdfReportResponse);
                }
                while (reportStatusModel.Id == 1);

                var pdfReportFile = await this.pdfReportService.GetPdfReportFile(pdfReportResponse);
                if (pdfReportFile != null)
                {
                    var fileName = "ActivityReport.pdf";
                    return this.File(pdfReportFile, FileHelper.GetContentTypeFromFileName(fileName), fileName);
                }
            }

            return this.View(new Tuple<UserBasicViewModel, MyLearningViewModel>(userDetails, response));
        }

        /// <summary>
        /// Gets the played segment data for the progress modal in My Learning screen.
        /// </summary>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="resourceReferenceId">The resourceVersionId.</param>
        /// <param name="version">The majorVersion.</param>
        /// <param name="maxTime">The max time.</param>
        /// <param name="returnUrl">Return url.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("my-learning/activity/{resourceId}/view-progress")]
        public async Task<IActionResult> ViewProgress(int resourceId, int resourceReferenceId, int version, long maxTime, string returnUrl = "/")
        {
            this.ViewBag.ReturnUrl = returnUrl;
            var playedSegments = await this.myLearningService.GetPlayedSegments(resourceId, version);

            var allSegments = new List<ResourcePlayedSegment>();
            var currentTime = 0;
            var mediaLengthInSeconds = maxTime / 1000M;

            foreach (var segment in playedSegments.OrderBy(p => p.SegmentStartTime))
            {
                if (segment.SegmentStartTime > currentTime)
                {
                    allSegments.Add(new ResourcePlayedSegment
                    {
                        SegmentStartTime = currentTime,
                        SegmentEndTime = segment.SegmentStartTime,
                        Played = false,
                    });
                }

                allSegments.Add(new ResourcePlayedSegment
                {
                    SegmentStartTime = segment.SegmentStartTime,
                    SegmentEndTime = segment.SegmentEndTime,
                    Played = true,
                });

                currentTime = segment.SegmentEndTime;
            }

            if (currentTime < mediaLengthInSeconds)
            {
                allSegments.Add(new ResourcePlayedSegment
                {
                    SegmentStartTime = currentTime,
                    SegmentEndTime = (int)mediaLengthInSeconds,
                    Played = false,
                });
            }

            allSegments.ForEach(s => s.Percentage = Math.Round((s.SegmentEndTime - s.SegmentStartTime) / mediaLengthInSeconds * 100, 2));

            var vm = new ActivityViewProgress
            {
                ResourceReferenceId = resourceReferenceId,
                Segments = allSegments,
                MediaLength = ResourcePlayedSegment.GetDurationHhmmss((int)mediaLengthInSeconds),
            };

            return this.View(vm);
        }

        /// <summary>
        /// Gets the certificate details of an activity.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <param name="minorVersion">The minorVersion.</param>
        /// <param name="userId">The user Id.</param>
        /// <param name="downloadCert">The downloadCert flag.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("mylearning/certificate/{resourceReferenceId}")]
        [Route("mylearning/certificate/{resourceReferenceId}/{userId}")]
        [Route("mylearning/certificate/{*path}")]
        public async Task<IActionResult> GetCertificateDetails(int resourceReferenceId, int? majorVersion = 0, int? minorVersion = 0, int? userId = 0, bool downloadCert = false)
        {
            CertificateDetails certificateDetails = null;
            string base64Image = string.Empty;
            var activity = await this.myLearningService.GetResourceCertificateDetails(resourceReferenceId, majorVersion, minorVersion, userId);
            if (activity.Item1 > 0 && activity.Item2 != null && ViewActivityHelper.CanDownloadCertificate(new ActivityDetailedItemViewModel(activity.Item2)))
            {
                var resource = await this.resourceService.GetItemByIdAsync(resourceReferenceId);
                var nodePathNodes = await this.hierarchyService.GetNodePathNodes(resource.NodePathId);
                var currentUser = await this.userService.GetUserByUserIdAsync((userId == 0) ? this.CurrentUserId : (int)userId);
                var userEmployment = await this.userService.GetUserEmploymentByIdAsync(currentUser.PrimaryUserEmploymentId ?? 0);
                if (activity.Item2.CertificateUrl != null && downloadCert)
                {
                    var file = await this.fileService.DownloadFileAsync(this.filePath, activity.Item2.CertificateUrl);
                    if (file != null)
                    {
                        byte[] imageArray = new BinaryReader(file.Content).ReadBytes((int)file.ContentLength);
                        base64Image = Convert.ToBase64String(imageArray);
                    }
                }

                certificateDetails = new CertificateDetails { AccessCount = activity.Item1, ProfessionalRegistrationNumber = userEmployment?.MedicalCouncilNo, NodeViewModels = nodePathNodes, UserViewModel = currentUser, ResourceItemViewModel = resource, ActivityDetailedItemViewModel = new ActivityDetailedItemViewModel(activity.Item2), DownloadCertificate = downloadCert, CertificateBase64Image = base64Image };
            }

            return this.View("LearningCertificate", certificateDetails);
        }

        /// <summary>
        /// Gets the certificate details of an activity.
        /// </summary>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <param name="minorVersion">The minorVersion.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("mylearning/downloadcertificate")]
        public async Task<IActionResult> DownloadCertificate(int resourceReferenceId, int? majorVersion = 0, int? minorVersion = 0, int? userId = 0)
        {
            CertificateDetails certificateDetails = null;
            string base64Image = string.Empty;
            var activity = await this.myLearningService.GetResourceCertificateDetails(resourceReferenceId, majorVersion, minorVersion, userId);
            if (activity.Item1 > 0 && activity.Item2 != null && ViewActivityHelper.CanDownloadCertificate(new ActivityDetailedItemViewModel(activity.Item2)))
            {
                var resource = await this.resourceService.GetItemByIdAsync(resourceReferenceId);
                var nodePathNodes = await this.hierarchyService.GetNodePathNodes(resource.NodePathId);
                var currentUser = await this.userService.GetUserByUserIdAsync((userId == 0) ? this.CurrentUserId : (int)userId);
                var userEmployment = await this.userService.GetUserEmploymentByIdAsync(currentUser.PrimaryUserEmploymentId ?? 0);
                if (activity.Item2.CertificateUrl != null)
                {
                    var file = await this.fileService.DownloadFileAsync(this.filePath, activity.Item2.CertificateUrl);
                    if (file != null)
                    {
                        byte[] imageArray = new BinaryReader(file.Content).ReadBytes((int)file.ContentLength);
                        base64Image = Convert.ToBase64String(imageArray);
                    }
                }

                certificateDetails = new CertificateDetails { AccessCount = activity.Item1, ProfessionalRegistrationNumber = userEmployment?.MedicalCouncilNo, NodeViewModels = nodePathNodes, UserViewModel = currentUser, ResourceItemViewModel = resource, ActivityDetailedItemViewModel = new ActivityDetailedItemViewModel(activity.Item2), DownloadCertificate = true, CertificateBase64Image = base64Image };
                var renderedViewHTML = new List<string>();
                certificateDetails.PageNo++;
                renderedViewHTML.Add(RenderRazorViewToString(this, "LearningCertificate", certificateDetails));
                certificateDetails.PageNo++;
                renderedViewHTML.Add(RenderRazorViewToString(this, "LearningCertificate", certificateDetails));

                ReportStatusModel reportStatusModel = new ReportStatusModel();
                var pdfReportResponse = await this.pdfReportService.PdfReport(renderedViewHTML, currentUser.Id);
                if (pdfReportResponse != null)
                {
                    do
                    {
                        reportStatusModel = await this.pdfReportService.PdfReportStatus(pdfReportResponse);
                    }
                    while (reportStatusModel.Id == 1);

                    var pdfReportFile = await this.pdfReportService.GetPdfReportFile(pdfReportResponse);
                    if (pdfReportFile != null)
                    {
                        string fileName = this.GenerateCertificateName(certificateDetails.ActivityDetailedItemViewModel.Title);
                        return this.File(pdfReportFile, FileHelper.GetContentTypeFromFileName(fileName), fileName);
                    }
                }
            }

            return this.View("LearningCertificate", certificateDetails);
        }

        /// <summary>
        /// Gets the Certificate name.
        /// </summary>
        /// <param name="resourceTitile">The resourceTitile.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string GenerateCertificateName(string resourceTitile)
        {
            if (!string.IsNullOrEmpty(resourceTitile))
            {
                if (resourceTitile.Length <= 71)
                {
                    string filename = "LH_Certificate_" + resourceTitile + ".pdf";
                    return filename;
                }
                else if (resourceTitile.Length > 71)
                {
                    string filename = "LH_Certificate_" + resourceTitile.Truncate(67, true) + "_ " + ".pdf";
                    return filename;
                }
            }

            return "LearningCertificate.pdf";
        }
    }
}