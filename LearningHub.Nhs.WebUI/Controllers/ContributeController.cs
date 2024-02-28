namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Contribute;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="ContributeController" />.
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(LoginWizardFilter))]
    public class ContributeController : BaseController
    {
        private readonly LearningHubAuthServiceConfig authConfig;
        private readonly IContributeService contributeService;
        private readonly IAzureMediaService azureMediaService;
        private readonly IFileService fileService;
        private readonly IResourceService resourceService;
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContributeController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="userService">User service.</param>
        /// <param name="fileService">File service.</param>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="azureMediaService">Azure media service.</param>
        /// <param name="authConfig">Auth config.</param>
        /// <param name="contributeService">Contribure service.</param>
        public ContributeController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment hostingEnvironment,
            ILogger<ContributeController> logger,
            IOptions<Settings> settings,
            IUserService userService,
            IFileService fileService,
            IResourceService resourceService,
            IAzureMediaService azureMediaService,
            LearningHubAuthServiceConfig authConfig,
            IContributeService contributeService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.authConfig = authConfig;

            this.userService = userService;
            this.fileService = fileService;
            this.resourceService = resourceService;
            this.azureMediaService = azureMediaService;
            this.contributeService = contributeService;
        }

        /// <summary>
        /// The Contribute.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("contribute-a-resource")]
        public IActionResult Contribute()
        {
            if (this.User.IsInRole("ReadOnly") || this.User.IsInRole("BasicUser"))
            {
                return this.RedirectToAction("AccessDenied", "Home");
            }

            return this.RedirectToAction("ContributeAResource");
        }

        /// <summary>
        /// The ContributeAResource.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("Contribute/{*path}")]
        [Route("Contribute/contribute-a-resource/{resourceVersionId}")]
        [Route("contribute-new-catalogue-resource/{*catId}")]
        [Route("contribute-new-catalogue-resource/{catId}/{nodeId}")]
        public async Task<IActionResult> ContributeAResource()
        {
            if (this.User.IsInRole("ReadOnly") || this.User.IsInRole("BasicUser"))
            {
                return this.RedirectToAction("AccessDenied", "Home");
            }

            var currentUser = await this.userService.GetUserByUserIdAsync(this.CurrentUserId);
            currentUser.LastName = currentUser.LastName.Replace("'", "\\'");

            if (this.User.Identity.IsAuthenticated)
            {
                this.ViewBag.CurrentUserName = currentUser.FirstName + " " + currentUser.LastName;
                this.ViewBag.ResourceLicenseUrl = this.Settings.ResourceLicenseUrl;
                this.ViewBag.ResourceCertificateUrl = this.Settings.SupportUrls.ResourceCertificateUrl;
                this.ViewBag.SupportUrlExcludedFiles = this.Settings.SupportUrls.ExcludedFiles;

                if (this.Settings.LimitScormToAdmin && !this.User.IsInRole("Administrator"))
                {
                    this.ViewBag.ResourceTypesSupported = "1,2,3";
                }
                else
                {
                    this.ViewBag.ResourceTypesSupported = "1,2,3,6,12";
                }

                return this.View();
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// The CreateVersion.
        /// </summary>
        /// <param name="resourceId">Resource id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("Contribute/create-version/{resourceId}")]
        public async Task<IActionResult> CreateVersion(int resourceId)
        {
            var response = await this.contributeService.CreateNewResourceVersionAsync(resourceId);

            if (response.IsValid)
            {
                var currentUser = await this.userService.GetUserByUserIdAsync(this.CurrentUserId);
                this.ViewBag.CurrentUserName = currentUser.FirstName + " " + currentUser.LastName;
                this.ViewBag.ResourceLicenseUrl = this.Settings.ResourceLicenseUrl;
                this.ViewBag.ResourceCertificateUrl = this.Settings.SupportUrls.ResourceCertificateUrl;
                this.ViewBag.SupportUrlExcludedFiles = this.Settings.SupportUrls.ExcludedFiles;

                int resourceVersionId = response.CreatedId.Value;

                var resourceType = (await this.resourceService.GetResourceVersionAsync(resourceVersionId)).ResourceType;
                if (resourceType == ResourceTypeEnum.Case || resourceType == ResourceTypeEnum.Assessment)
                {
                    return this.RedirectToAction("Edit", "ContributeResource", new { resourceVersionId = resourceVersionId });
                }
                else
                {
                    return new RedirectResult("/Contribute/contribute-a-resource/" + resourceVersionId);
                }
            }
            else
            {
                return this.RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// The MyContributions.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("my-contributions")]
        [Route("my-contributions/{selectedTab}")]
        [Route("my-contributions/{selectedTab}/{catalogueId}")]
        [Route("my-contributions/{selectedTab}/{catalogueId}/{nodeId}")]
        public async Task<IActionResult> MyContributions()
        {
            if ((this.User.IsInRole("ReadOnly") || this.User.IsInRole("BasicUser")) && !await this.resourceService.UserHasPublishedResourcesAsync())
            {
                return this.RedirectToAction("AccessDenied", "Home");
            }

            return this.View();
        }

        /// <summary>
        /// The NoLongerAvailable.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Contribute/no-longer-available")]
        public IActionResult NoLongerAvailable()
        {
            this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;
            return this.View();
        }

        /// <summary>
        /// The UploadArticleFile.
        /// </summary>
        /// <param name="inputForm">Input form.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("/Contribute/UploadArticleFile")]
        public async Task<FileUploadResult> UploadArticleFile(IFormCollection inputForm)
        {
            var file = inputForm.Files[0];
            int.TryParse(inputForm["resourceVersionId"], out int resourceVersionId);
            int.TryParse(inputForm["changeingFileId"], out int existingFileId);
            var currentUserId = this.User.Identity.GetCurrentUserId();
            return await this.contributeService.ProcessArticleFileAsync(resourceVersionId, file, existingFileId, currentUserId);
        }

        /// <summary>
        /// The UploadFile.
        /// </summary>
        /// <param name="inputForm">Input form.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("/Contribute/UploadFile")]
        public async Task<FileUploadResult> UploadFile(IFormCollection inputForm)
        {
            var file = inputForm.Files[0];
            int.TryParse(inputForm["resourceVersionId"], out int resourceVersionId);
            var currentUserId = this.User.Identity.GetCurrentUserId();
            return await this.contributeService.ProcessResourceFileAsync(resourceVersionId, file, currentUserId);
        }

        /// <summary>
        /// The UploadFileChunk.
        /// </summary>
        /// <param name="inputForm">Input form.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("/Contribute/UploadFileChunk")]
        public async Task<FileChunkUploadResult> UploadFileChunk(IFormCollection inputForm)
        {
            int.TryParse(inputForm["fileChunkDetailId"], out int fileChunkDetailId);
            int.TryParse(inputForm["resourceVersionId"], out int resourceVersionId);
            int.TryParse(inputForm["chunkCount"], out int chunkCount);
            int.TryParse(inputForm["chunkIndex"], out int chunkIndex);
            string fileName = inputForm["fileName"];
            int.TryParse(inputForm["fileSize"], out int fileSize);
            var file = inputForm.Files[0];
            var currentUserId = this.User.Identity.GetCurrentUserId();

            return await this.contributeService.UploadFileChunkAsync(fileChunkDetailId, resourceVersionId, chunkCount, chunkIndex, fileName, file, fileSize, currentUserId);
        }

        /// <summary>
        /// The UploadResourceAttachedFile.
        /// </summary>
        /// <param name="inputForm">Input form.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("/Contribute/UploadResourceAttachedFile")]
        public async Task<FileUploadResult> UploadResourceAttachedFile(IFormCollection inputForm)
        {
            var file = inputForm.Files[0];
            int.TryParse(inputForm["resourceVersionId"], out int resourceVersionId);
            int.TryParse(inputForm["resourceType"], out int resourceType);
            int.TryParse(inputForm["attachedFileType"], out int attachedFileType);
            var currentUserId = this.User.Identity.GetCurrentUserId();
            return await this.contributeService.ProcessResourceAttachedFileAsync(resourceVersionId, file, resourceType, attachedFileType, currentUserId);
        }
    }
}
