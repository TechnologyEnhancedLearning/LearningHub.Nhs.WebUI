namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;

    /// <summary>
    /// The DashboardService.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IMapper mapper;
        private IResourceVersionRepository resourceVersionRepository;
        private ICatalogueNodeVersionRepository catalogueNodeVersionRepository;
        private IRatingService ratingService;
        private IProviderService providerService;
        /// <summary>
        /// The moodleApiService.
        /// </summary>
        private readonly IMoodleApiService moodleApiService;

        /// <summary>
        /// The resourceActivityRepository.
        /// </summary>
        private readonly IResourceActivityRepository resourceActivityRepository;

        /// <summary>
        /// The resource repository.
        /// </summary>
        private readonly IResourceRepository resourceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardService"/> class.
        /// </summary>
        /// <param name="mapper">mapper.</param>
        /// <param name="resourceVersionRepository">resourceVersionRepository.</param>
        /// <param name="catalogueNodeVersionRepository">catalogueNodeVersionRepository.</param>
        /// <param name="ratingService">ratingService.</param>
        /// <param name="providerService">providerService.</param>
        /// <param name="moodleApiService">moodleApiService.</param>
        /// <param name="resourceActivityRepository">resourceActivityRepository.</param>
        /// <param name="resourceRepository">resourceRepository.</param>
        public DashboardService(IMapper mapper, IResourceVersionRepository resourceVersionRepository, ICatalogueNodeVersionRepository catalogueNodeVersionRepository, IRatingService ratingService, IProviderService providerService, IMoodleApiService moodleApiService, IResourceActivityRepository resourceActivityRepository, IResourceRepository resourceRepository)
        {
            this.mapper = mapper;
            this.resourceVersionRepository = resourceVersionRepository;
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
            this.ratingService = ratingService;
            this.providerService = providerService;
            this.moodleApiService = moodleApiService;
            this.resourceActivityRepository = resourceActivityRepository;
            this.resourceRepository = resourceRepository;
        }

        /// <summary>
        /// GetMyAccessLearnings.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardMyLearningResponseViewModel> GetMyAccessLearnings(string dashboardType, int pageNumber, int userId)
        {
            var (resourceCount, resources) = dashboardType.ToLower() != "my-catalogues" ? resourceVersionRepository.GetResources(dashboardType, pageNumber, userId) : (resourceCount: 0, resources: new List<DashboardResourceDto>());

            var cataloguesResponse = dashboardType.ToLower() == "my-catalogues" ? catalogueNodeVersionRepository.GetCatalogues(dashboardType, pageNumber, userId) : (TotalCount: 0, Catalogues: new List<DashboardCatalogueDto>());

            var catalogueList = cataloguesResponse.Catalogues.Any() ? mapper.Map<List<DashboardCatalogueViewModel>>(cataloguesResponse.Catalogues) : new List<DashboardCatalogueViewModel>();
            if (catalogueList.Any())
            {
                foreach (var catalogue in catalogueList)
                {
                    catalogue.Providers = await providerService.GetByCatalogueVersionIdAsync(catalogue.NodeVersionId);
                }
            }

            var resourceList = resources.Any() ? mapper.Map<List<DashboardResourceViewModel>>(resources) : new List<DashboardResourceViewModel>();
            if (resourceList.Any())
            {
                foreach (var resource in resourceList)
                {
                    resource.Providers = await providerService.GetByResourceVersionIdAsync(resource.ResourceVersionId);
                }
            }

            var response = new DashboardMyLearningResponseViewModel
            {
                Type = dashboardType,
                Resources = resourceList,
                Catalogues = catalogueList,
                TotalCount = dashboardType.ToLower() == "my-catalogues" ? cataloguesResponse.TotalCount : resourceCount,
                CurrentPage = pageNumber,
            };

            return response;
        }

        /// <summary>
        /// Get in progress courses and Elearning..
        /// </summary>
        /// <param name="dashboardTrayLearningResourceType">The dashboardTrayLearningResource type.</param>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceType">The resourceType.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardMyLearningResponseViewModel> GetMyCoursesAndElearning(string dashboardTrayLearningResourceType, string dashboardType, int pageNumber, int userId, string resourceType)
        {
            MyLearningActivitiesDetailedViewModel myInProgressActivities = new();
            MyLearningCertificatesDetailedViewModel certificates = new();
            List<DashboardResourceDto> resources = new();
            int resourceCount = 0;
            if (dashboardType == "my-in-progress")
            {
                myInProgressActivities = await this.GetMyInprogressLearningAsync(dashboardTrayLearningResourceType, pageNumber, userId);
            }
            else if (dashboardType == "my-certificates")
            {
                certificates = await this.GetUserCertificateDetailsAsync(dashboardTrayLearningResourceType, pageNumber, userId);
            }
            else
            {
                var result = resourceVersionRepository.GetResources(dashboardType, pageNumber, userId);
                resourceCount = result.resourceCount;
                resources = result.resources ?? new List<DashboardResourceDto>();
            }

            var cataloguesResponse = dashboardType.ToLower() == "my-catalogues" ? catalogueNodeVersionRepository.GetCatalogues(dashboardType, pageNumber, userId) : (TotalCount: 0, Catalogues: new List<DashboardCatalogueDto>());

            var catalogueList = cataloguesResponse.Catalogues.Any() ? mapper.Map<List<DashboardCatalogueViewModel>>(cataloguesResponse.Catalogues) : new List<DashboardCatalogueViewModel>();
            if (catalogueList.Any())
            {
                foreach (var catalogue in catalogueList)
                {
                    catalogue.Providers = await providerService.GetByCatalogueVersionIdAsync(catalogue.NodeVersionId);
                }
            }

            var resourceList = resources.Any() ? mapper.Map<List<DashboardResourceViewModel>>(resources) : new List<DashboardResourceViewModel>();
            if (resourceList.Any())
            {
                foreach (var resource in resourceList)
                {
                    resource.Providers = await providerService.GetByResourceVersionIdAsync(resource.ResourceVersionId);
                }
            }

            var response = new DashboardMyLearningResponseViewModel
            {
                Type = dashboardType,
                Resources = resourceList,
                Catalogues = catalogueList,
                Activities = myInProgressActivities.Activities,
                UserCertificates = certificates.Certificates,
                TotalCount = dashboardType?.ToLower() switch
                {
                    "my-catalogues" => cataloguesResponse.TotalCount,
                    "my-in-progress" => myInProgressActivities?.TotalCount ?? 0,
                    "my-certificates" => certificates?.TotalCount ?? 0,
                    _ => resourceCount
                },
                CurrentPage = pageNumber,
            };

            return response;
        }

        /// <summary>
        /// Gets the user in progress my leraning activities..
        /// </summary>
        /// <param name="dashboardTrayLearningResourceType">The dashboardTrayLearningResourceType.</param>
        /// <param name="pageNumber">The pageNumber.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyLearningActivitiesDetailedViewModel> GetMyInprogressLearningAsync(string dashboardTrayLearningResourceType, int pageNumber, int userId)
        {
            List<MyLearningActivitiesViewModel> result = new();
            List<MoodleEnrolledCourseResponseModel> entrolledCourses = new();
            List<MyLearningCombinedActivitiesViewModel> mappedMyLearningActivities = new();
            List<MyLearningCombinedActivitiesViewModel> mappedEnrolledCourses = new();
            if (dashboardTrayLearningResourceType != "courses")
            {
                result = await this.resourceActivityRepository.GetUserInprogressLearningActivities(userId, pageNumber);
            }

            if (dashboardTrayLearningResourceType != "elearning")
            {
                entrolledCourses = await this.moodleApiService.GetInProgressEnrolledCoursesAsync(userId);
            }

            if (result != null)
            {
                mappedMyLearningActivities = result.Select(Activity => new MyLearningCombinedActivitiesViewModel
                {
                    UserId = userId,
                    ResourceId = Activity.ResourceId,
                    ResourceVersionId = Activity.ResourceVersionId,
                    ResourceReferenceId = Activity.ResourceReferenceId,
                    IsCurrentResourceVersion = Activity.IsCurrentResourceVersion,
                    MajorVersion = Activity.MajorVersion,
                    MinorVersion = Activity.MinorVersion,
                    ResourceType = Activity.ResourceType,
                    Title = Activity.Title,
                    CertificateEnabled = Activity.CertificateEnabled,
                    ActivityStatus = Activity.ActivityStatus,
                    ActivityDate = Activity.ActivityDate,
                    ScorePercentage = Activity.ScorePercentage,
                    TotalActivities = 0,
                    CompletedActivities = 0,
                }).ToList();
            }

            if (entrolledCourses != null)
            {
                mappedEnrolledCourses = entrolledCourses.Select(course => new MyLearningCombinedActivitiesViewModel
                {
                    UserId = userId,
                    ResourceId = (int)course.Id,
                    ResourceVersionId = (int)course.Id,
                    IsCurrentResourceVersion = true,
                    ResourceReferenceId = (int)course.Id,
                    MajorVersion = 1,
                    MinorVersion = 0,
                    ResourceType = ResourceTypeEnum.Moodle,
                    Title = course.DisplayName,
                    CertificateEnabled = course.CertificateEnabled,
                    ActivityStatus = (course.Completed == true || course.ProgressPercentage.TrimEnd('%') == "100") ? ActivityStatusEnum.Completed : ActivityStatusEnum.Incomplete,
                    ActivityDate = course.LastAccessDate.HasValue
                            ? DateTimeOffset.FromUnixTimeSeconds(course.LastAccessDate.Value)
                            : DateTimeOffset.MinValue,
                    ScorePercentage = Convert.ToInt32(course.ProgressPercentage.TrimEnd('%')),
                    TotalActivities = course.TotalActivities,
                    CompletedActivities = course.CompletedActivities,
                }).ToList();
            }

            // Combine both result sets
            var combainedUserActivities = mappedMyLearningActivities.Concat(mappedEnrolledCourses).ToList();
            int skip = (pageNumber - 1) * 3;
            var totalCount = combainedUserActivities.Count();
            bool isLastPage = skip + 3 >= 8;
            int pageSize = isLastPage ? 2 : 3;
            var pagedResults = combainedUserActivities.OrderByDescending(activity => activity.ActivityDate).Skip(skip).Take(pageSize).ToList();

            // Count total records.
            MyLearningActivitiesDetailedViewModel viewModel = new MyLearningActivitiesDetailedViewModel()
            {
                TotalCount = totalCount > 8 ? 8 : totalCount,
                Activities = pagedResults,
            };

            return viewModel;
        }

        /// <summary>
        /// Gets the resource certificate details.
        /// </summary>
        /// <param name="dashboardTrayLearningResourceType">The dashboardTrayLearningResourceType.</param>
        /// <param name="pageNumber">The pageNumber.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyLearningCertificatesDetailedViewModel> GetUserCertificateDetailsAsync(string dashboardTrayLearningResourceType, int pageNumber, int userId)
        {
            try
            {
                Task<List<MoodleUserCertificateResponseModel>>? courseCertificatesTask = null;
                Task<List<UserCertificateViewModel>>? resourceCertificatesTask = null;

                if (dashboardTrayLearningResourceType != "elearning")
                {
                    courseCertificatesTask = moodleApiService.GetUserCertificateAsync(userId);

                }
                if (dashboardTrayLearningResourceType != "courses")
                {
                    resourceCertificatesTask = resourceRepository.GetUserCertificateDetails(userId);
                }

                // Await all active tasks in parallel
                if (courseCertificatesTask != null & dashboardTrayLearningResourceType == "all")
                    await Task.WhenAll(courseCertificatesTask, resourceCertificatesTask);
                else if (dashboardTrayLearningResourceType == "elearning")
                    await resourceCertificatesTask;
                else
                    await courseCertificatesTask;
                IEnumerable<UserCertificateViewModel> resourceCertificates = Enumerable.Empty<UserCertificateViewModel>();
                if (resourceCertificatesTask != null)
                {
                    resourceCertificates = resourceCertificatesTask.Result ?? Enumerable.Empty<UserCertificateViewModel>();
                }

                IEnumerable<UserCertificateViewModel> mappedCourseCertificates = Enumerable.Empty<UserCertificateViewModel>();

                if (courseCertificatesTask != null)
                {
                    var courseCertificates = courseCertificatesTask.Result ?? Enumerable.Empty<MoodleUserCertificateResponseModel>();

                    mappedCourseCertificates = courseCertificates.Select(c => new UserCertificateViewModel
                    {
                        Title = string.IsNullOrWhiteSpace(c.ResourceTitle) ? c.ResourceName : c.ResourceTitle,
                        ResourceTypeId = (int)ResourceTypeEnum.Moodle,
                        ResourceReferenceId = 0,
                        MajorVersion = 0,
                        MinorVersion = 0,
                        AwardedDate = c.AwardedDate.HasValue
                            ? DateTimeOffset.FromUnixTimeSeconds(c.AwardedDate.Value)
                            : DateTimeOffset.MinValue,
                        CertificatePreviewUrl = c.PreviewLink,
                        CertificateDownloadUrl = c.DownloadLink
                    });
                }

                var allCertificates = resourceCertificates.Concat(mappedCourseCertificates);

                var allowedTypeIds = new List<int> { 6 };

                allCertificates = allCertificates.Where(c => allowedTypeIds.Contains(c.ResourceTypeId));

                var orderedCertificates = allCertificates.OrderByDescending(c => c.AwardedDate);
                int skip = (pageNumber - 1) * 3;
                var totalCount = orderedCertificates.Count();
                var pagedResults = orderedCertificates
                        .Skip(skip)
                        .Take(3)
                        .ToList();
                return new MyLearningCertificatesDetailedViewModel
                {
                    Certificates = pagedResults,
                    TotalCount = totalCount > 8 ? 8 : totalCount
                };
            }
            catch (Exception ex)
            {
                throw null;
            }
        }
        /// <summary>
        /// GetCatalogues.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardCatalogueResponseViewModel> GetCatalogues(string dashboardType, int pageNumber, int userId)
        {
            var (catalogueCount, catalogues) = catalogueNodeVersionRepository.GetCatalogues(dashboardType, pageNumber, userId);

            var catalogueList = catalogues.Any() ? mapper.Map<List<DashboardCatalogueViewModel>>(catalogues) : new List<DashboardCatalogueViewModel>();
            foreach (var catalogue in catalogueList)
            {
                catalogue.Providers = await providerService.GetByCatalogueVersionIdAsync(catalogue.NodeVersionId);
            }

            var response = new DashboardCatalogueResponseViewModel
            {
                Type = dashboardType,
                Catalogues = catalogueList,
                TotalCount = catalogueCount,
                CurrentPage = pageNumber,
            };
            return response;
        }

        /// <summary>
        /// GetResources.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The number of rows to return.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardResourceResponseViewModel> GetResources(string dashboardType, int pageNumber, int userId)
        {
            var (resourceCount, resources) = resourceVersionRepository.GetResources(dashboardType, pageNumber, userId);

            var resourceList = resources.Any() ? mapper.Map<List<DashboardResourceViewModel>>(resources) : new List<DashboardResourceViewModel>();

            foreach (var resource in resourceList)
            {
                resource.Providers = await providerService.GetByResourceVersionIdAsync(resource.ResourceVersionId);
            }

            var response = new DashboardResourceResponseViewModel
            {
                Type = dashboardType,
                Resources = resourceList,
                TotalCount = resourceCount,
                CurrentPage = pageNumber,
            };

            return response;
        }
    }
}
