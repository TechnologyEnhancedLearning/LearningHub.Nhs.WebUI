namespace LearningHub.Nhs.WebUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Hangfire;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Controllers;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using LearningHub.Nhs.WebUI.Models.Resource;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement;

    /// <summary>
    /// Defines the <see cref="StorageController" />.
    /// </summary>
    public class StorageController : BaseController
    {
        private readonly IAzureMediaService azureMediaService;
        private readonly IResourceService resourceService;
        private readonly IRatingService ratingService;
        private readonly IUserService userService;
        private readonly IUserGroupService userGroupService;
        private readonly IActivityService activityService;
        private readonly ICatalogueService catalogueService;
        private readonly IHierarchyService hierarchyService;
        private readonly IMyLearningService myLearningService;
        private readonly IFileService fileService;
        private readonly ICacheService cacheService;
        private readonly IFeatureManager featureManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">Hosting environment.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="azureMediaService">Azure media services.</param>
        /// <param name="resourceService">Resource services.</param>
        /// <param name="ratingService">rating services.</param>
        /// <param name="userService">user services.</param>
        /// <param name="userGroupService">The UserGroupService.</param>
        /// <param name="activityService">The ActivityService.</param>
        /// <param name="catalogueService">The catalogueService.</param>
        /// <param name="myLearningService">The myLearningService.</param>
        /// <param name="hierarchyService">The hierarchyService.</param>
        /// <param name="fileService">The fileService.</param>
        /// <param name="cacheService">The cacheService.</param>
        /// <param name="featureManager"> The Feature flag manager.</param>
        public StorageController(
            IWebHostEnvironment hostingEnvironment,
            ILogger<StorageController> logger,
            IOptions<Settings> settings,
            IHttpClientFactory httpClientFactory,
            IAzureMediaService azureMediaService,
            IResourceService resourceService,
            IRatingService ratingService,
            IUserService userService,
            IUserGroupService userGroupService,
            IActivityService activityService,
            ICatalogueService catalogueService,
            IMyLearningService myLearningService,
            IHierarchyService hierarchyService,
            IFileService fileService,
            ICacheService cacheService,
            IFeatureManager featureManager)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.azureMediaService = azureMediaService;
            this.resourceService = resourceService;
            this.ratingService = ratingService;
            this.userService = userService;
            this.userGroupService = userGroupService;
            this.activityService = activityService;
            this.catalogueService = catalogueService;
            this.hierarchyService = hierarchyService;
            this.myLearningService = myLearningService;
            this.fileService = fileService;
            this.cacheService = cacheService;
            this.featureManager = featureManager;
        }

        /// <summary>
        /// The ClearStorage.
        /// </summary>
        /// <returns>ClearStorage Task.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task ClearStorage()
        {
            // get all published resource.
            var listResourceVersionsToBePurged = new List<int>();
            var resources = await this.resourceService.GetAllPublishedResourceAsync();
            if (resources != null && resources.Any())
            {
                foreach (var resource in resources)
                {
                    try
                    {
                        var vm = await this.resourceService.GetResourceVersionsAsync(resource);
                        if (vm != null & vm.Any())
                        {
                            // filter off current published version
                            var currentVersion = vm.Where(x => x.VersionStatusEnum == VersionStatusEnum.Published && x.ResourceTypeEnum != ResourceTypeEnum.Audio && x.ResourceTypeEnum != ResourceTypeEnum.Video).OrderByDescending(x => x.PublishedDate).FirstOrDefault();
                            if (currentVersion != null)
                            {
                                var previousVersion = vm.Where(x => x.VersionStatusEnum == VersionStatusEnum.Published && x.ResourceVersionId != currentVersion.ResourceVersionId).Select(x => x.ResourceVersionId).ToList();
                                if (previousVersion != null && previousVersion.Any())
                                {
                                    listResourceVersionsToBePurged.AddRange(previousVersion);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            if (listResourceVersionsToBePurged.Any())
            {
                int counter = 10;
                foreach (var entry in listResourceVersionsToBePurged)
                {
                    try
                    {
                        var associatedFile = await this.resourceService.GetObsoleteResourceFile(entry, true);
                        if (associatedFile != null && associatedFile.Any())
                        {
                            BackgroundJob.Schedule(() => this.fileService.PurgeResourceFile(null, associatedFile), TimeSpan.FromSeconds(counter));
                            counter = counter + 10;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
