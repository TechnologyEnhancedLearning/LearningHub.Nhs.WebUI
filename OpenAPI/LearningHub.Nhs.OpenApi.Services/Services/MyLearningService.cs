namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.PerformanceData;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Azure.Core;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Repositories.Helpers;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The rating service.
    /// </summary>
    public class MyLearningService : IMyLearningService
    {
        /// <summary>
        /// The resourceActivityRepository.
        /// </summary>
        private readonly IResourceActivityRepository resourceActivityRepository;

        /// <summary>
        /// The catalogueNodeVersionRepository.
        /// </summary>
        private readonly ICatalogueNodeVersionRepository catalogueNodeVersionRepository;

        /// <summary>
        /// The mediaResourcePlayedSegmentRepository.
        /// </summary>
        private readonly IMediaResourcePlayedSegmentRepository mediaResourcePlayedSegmentRepository;

        /// <summary>
        /// The assessment resource activity repository.
        /// </summary>
        private readonly IAssessmentResourceActivityRepository assessmentResourceActivityRepository;

        /// <summary>
        /// The assessment resource activity interaction repository.
        /// </summary>
        private readonly IAssessmentResourceActivityInteractionRepository assessmentResourceActivityInteractionRepository;

        /// <summary>
        /// The scorm resource activity interaction repository.
        /// </summary>
        private readonly IScormActivityRepository scormActivityRepository;

        /// <summary>
        /// The media resource activity interaction repository.
        /// </summary>
        private readonly IMediaResourceActivityRepository mediaResourceActivity;

        /// <summary>
        /// The resource repository.
        /// </summary>
        private readonly IResourceRepository resourceRepository;

        /// <summary>
        /// The moodleApiService.
        /// </summary>
        private readonly IMoodleApiService moodleApiService;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly LearningHubConfig settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningService"/> class.
        /// </summary>
        /// <param name="resourceActivityRepository">The resource activity repository.</param>
        /// <param name="catalogueNodeVersionRepository">The catalogue node repository.</param>
        /// <param name="mediaResourcePlayedSegmentRepository">The mediaResourcePlayedSegmentRepository.</param>
        /// <param name="assessmentResourceActivityRepository">The assessmentResourceActivityRepository.</param>
        /// <param name="assessmentResourceActivityInteractionRepository">The assessmentResourceActivityInteractionRepository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="scormActivityRepository">The scormActivityRepository.</param>
        /// <param name="mediaResourceActivity">The mediaResourceActivity.</param>
        /// <param name="resourceRepository">The resourceActivity</param>
        /// <param name="moodleApiService">The moodleApiService.</param>
        public MyLearningService(
            IResourceActivityRepository resourceActivityRepository,
            IMediaResourcePlayedSegmentRepository mediaResourcePlayedSegmentRepository,
            IAssessmentResourceActivityRepository assessmentResourceActivityRepository,
            IAssessmentResourceActivityInteractionRepository assessmentResourceActivityInteractionRepository,
            ICatalogueNodeVersionRepository catalogueNodeVersionRepository,
            IMapper mapper,
            IOptions<LearningHubConfig> settings,
            IScormActivityRepository scormActivityRepository,
            IMediaResourceActivityRepository mediaResourceActivity,
            IResourceRepository resourceRepository,
            IMoodleApiService moodleApiService)
        {
            this.resourceActivityRepository = resourceActivityRepository;
            this.mediaResourcePlayedSegmentRepository = mediaResourcePlayedSegmentRepository;
            this.assessmentResourceActivityRepository = assessmentResourceActivityRepository;
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
            this.assessmentResourceActivityInteractionRepository = assessmentResourceActivityInteractionRepository;
            this.mapper = mapper;
            this.settings = settings.Value;
            this.scormActivityRepository = scormActivityRepository;
            this.mediaResourceActivity = mediaResourceActivity;
            this.resourceRepository = resourceRepository;
            this.moodleApiService = moodleApiService;
        }

        /// <summary>
        /// Gets the activity records for the detailed activity tab of My Learning screen.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyLearningDetailedViewModel> GetActivityDetailed(int userId, MyLearningRequestModel requestModel)
        {
            var activityQuery = resourceActivityRepository.GetByUserIdFromSP(userId, requestModel, settings.DetailedMediaActivityRecordingStartDate).Result.OrderByDescending(r => r.ActivityStart).DistinctBy(l => l.Id);

            // Count total records.
            MyLearningDetailedViewModel viewModel = new MyLearningDetailedViewModel()
            {
                TotalCount = resourceActivityRepository.GetTotalCount(userId, requestModel, settings.DetailedMediaActivityRecordingStartDate),
            };

            // Return only the requested batch.
            var activityEntities = activityQuery.ToList();

            viewModel.Activities = await this.PopulateMyLearningDetailedItemViewModels(activityEntities, userId);

            return viewModel;
        }

        /// <summary>
        /// Gets the user recent my leraning activities..
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyLearningActivitiesDetailedViewModel> GetUserRecentMyLearningActivitiesAsync(int userId, MyLearningRequestModel requestModel)
        {
            try
            {
                var result = await resourceActivityRepository.GetUserRecentMyLearningActivities(userId, requestModel);

                var entrolledCourses = await this.moodleApiService.GetRecentEnrolledCoursesAsync(userId, requestModel, 6);
                List<MyLearningCombinedActivitiesViewModel> mappedMyLearningActivities = new();
                List<MyLearningCombinedActivitiesViewModel> mappedEnrolledCourses = new();
                List<MyLearningCombinedActivitiesViewModel> combainedUserActivities = new();

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
                 combainedUserActivities = mappedMyLearningActivities.Concat(mappedEnrolledCourses).ToList();


                var pagedResults = combainedUserActivities.OrderByDescending(activity => activity.ActivityDate).Skip(requestModel.Skip).Take(requestModel.Take).ToList();

                // Count total records.
                MyLearningActivitiesDetailedViewModel viewModel = new MyLearningActivitiesDetailedViewModel()
                {
                    TotalCount = combainedUserActivities.Count(),
                    Activities = pagedResults,
                };

                return viewModel;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the user learning history activities.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyLearningActivitiesDetailedViewModel> GetUserLearningHistoryAsync(int userId, MyLearningRequestModel requestModel)
        {
            try
            {
                (string strActivityStatus, bool activityStatusEnumFlag) = resourceActivityRepository.GetActivityStatusFilter(requestModel);
                (string strResourceTypes, bool resourceTypeFlag) = resourceActivityRepository.ApplyResourceTypesfilters(requestModel);
                var result = new List<MyLearningActivitiesViewModel>();

                if (
        (!activityStatusEnumFlag && !resourceTypeFlag && !requestModel.Courses) ||
        (activityStatusEnumFlag && resourceTypeFlag && !requestModel.Courses) ||
        (activityStatusEnumFlag && !resourceTypeFlag && !requestModel.Courses) ||
        (!activityStatusEnumFlag && resourceTypeFlag && !requestModel.Courses) ||
        (!activityStatusEnumFlag && resourceTypeFlag && requestModel.Courses) ||
        (activityStatusEnumFlag && resourceTypeFlag && requestModel.Courses))
                {
                    if (requestModel.SearchText != null)
                    {
                        result = await resourceActivityRepository.GetUserLearningHistoryBasedonSearchText(userId, requestModel);
                    }
                    else
                    {
                        result = await resourceActivityRepository.GetUserLearningHistory(userId, requestModel);
                    }
                }

                List<MyLearningCombinedActivitiesViewModel> mappedMyLearningActivities = new();
                List<MyLearningCombinedActivitiesViewModel> mappedEnrolledCourses = new();
                List<MyLearningCombinedActivitiesViewModel> combainedUserActivities = new();

                if (result != null)
                {
                    mappedMyLearningActivities = result.Select(activity => new MyLearningCombinedActivitiesViewModel
                    {
                        UserId = userId,
                        ResourceId = activity.ResourceId,
                        ResourceVersionId = activity.ResourceVersionId,
                        ResourceReferenceId = activity.ResourceReferenceId,
                        IsCurrentResourceVersion = activity.IsCurrentResourceVersion,
                        MajorVersion = activity.MajorVersion,
                        MinorVersion = activity.MinorVersion,
                        ResourceType = activity.ResourceType,
                        Title = activity.Title,
                        CertificateEnabled = activity.CertificateEnabled,
                        ActivityStatus = activity.ActivityStatus,
                        ActivityDate = activity.ActivityDate,
                        ScorePercentage = activity.ScorePercentage,
                        TotalActivities = 0,
                        CompletedActivities = 0,
                    }).ToList();
                }

                List<MoodleEnrolledCourseResponseModel> entrolledCourses = new();

                if (
                    (!activityStatusEnumFlag && !resourceTypeFlag && !requestModel.Courses) ||
                    (!activityStatusEnumFlag && !resourceTypeFlag && requestModel.Courses) ||
                    (!activityStatusEnumFlag && resourceTypeFlag && requestModel.Courses) ||
                    (activityStatusEnumFlag && resourceTypeFlag && requestModel.Courses) ||
                    (activityStatusEnumFlag && !resourceTypeFlag && requestModel.Courses) ||
                    (activityStatusEnumFlag && !resourceTypeFlag && !requestModel.Courses))
                {
                    entrolledCourses = await this.moodleApiService.GetEnrolledCoursesHistoryAsync(userId, requestModel);
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
                            ScorePercentage = int.TryParse(course.ProgressPercentage.TrimEnd('%'), out var score) ? score : 0,
                            TotalActivities = course.TotalActivities,
                            CompletedActivities = course.CompletedActivities,
                        }).ToList();
                    }
                }

                // Combine both result sets
                combainedUserActivities = mappedMyLearningActivities.Concat(mappedEnrolledCourses).ToList();

                var pagedResults = combainedUserActivities.OrderByDescending(activity => activity.ActivityDate).Skip(requestModel.Skip).Take(requestModel.Take).ToList();

                // Count total records.
                MyLearningActivitiesDetailedViewModel viewModel = new MyLearningActivitiesDetailedViewModel()
                {
                    TotalCount = combainedUserActivities.Count(),
                    Activities = pagedResults,
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the played segment data for the progress modal in My Learning screen.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<PlayedSegmentViewModel>> GetPlayedSegments(int userId, int resourceId, int majorVersion)
        {
            var playedSegments = await mediaResourcePlayedSegmentRepository.GetPlayedSegmentsAsync(userId, resourceId, majorVersion);
            var viewModel = mapper.Map<List<PlayedSegmentViewModel>>(playedSegments);

            return viewModel;
        }

        /// <summary>
        /// Gets the resource certificate details of a resource reference.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="majorVersion">The majorVersion.</param>
        /// <param name="minorVersion">The minorVersion.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Tuple<int, MyLearningDetailedItemViewModel>> GetResourceCertificateDetails(int userId, int resourceReferenceId, int majorVersion, int minorVersion)
        {
            MyLearningDetailedItemViewModel myLearningDetailedItemViewModel = null;
            var activityQuery = resourceActivityRepository.GetByUserId(userId);
            if (majorVersion > 0)
            {
                activityQuery = activityQuery.Where(x => x.MajorVersion == majorVersion && x.MinorVersion == minorVersion);
            }
            else
            {
                // filter and use the latest version only
                activityQuery = activityQuery.Where(rv => rv.ResourceVersionId == rv.Resource.CurrentResourceVersionId);
            }

            activityQuery = activityQuery.Where(x => x.Resource.ResourceReference.FirstOrDefault() != null && x.Resource.ResourceReference.Any(rr => rr.OriginalResourceReferenceId == resourceReferenceId));
            int totalNumberOfAccess = activityQuery.Count();
            var activityEntities = await activityQuery.OrderByDescending(x => x.Score).ThenByDescending(x => x.ActivityStart).ToListAsync();
            activityEntities.RemoveAll(x => x.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm && (x.ActivityStatusId == (int)ActivityStatusEnum.Downloaded || x.ActivityStatusId == (int)ActivityStatusEnum.Incomplete || x.ActivityStatusId == (int)ActivityStatusEnum.InProgress));
            if (activityEntities.Any() && activityEntities.FirstOrDefault()?.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment)
            {
                totalNumberOfAccess = activityQuery.SelectMany(x => x.AssessmentResourceActivity)
                                                   .OrderByDescending(a => a.CreateDate)
                                                   .Count();

                var assessmentType = activityEntities.First().ResourceVersion.AssessmentResourceVersion.AssessmentType;

                if (assessmentType == AssessmentTypeEnum.Formal)
                {
                    activityEntities = activityEntities.Where(x => x.AssessmentResourceActivity.FirstOrDefault() != null && 
                                                                   x.AssessmentResourceActivity.First().Score.HasValue && 
                                                                   (int)Math.Round(x.AssessmentResourceActivity.First().Score.Value,
                                                                       MidpointRounding.AwayFromZero) >= x.ResourceVersion.AssessmentResourceVersion.PassMark)
                                                                    .ToList();
                }
                else if (assessmentType == AssessmentTypeEnum.Informal)
                {
                    activityEntities = activityEntities.Where(x =>x.ActivityStatusId == (int)ActivityStatusEnum.Completed).ToList();
                }
            }
            else if (activityEntities.Any() && (activityEntities.FirstOrDefault()?.Resource.ResourceTypeEnum == ResourceTypeEnum.Video || activityEntities.FirstOrDefault()?.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio))
            {
                totalNumberOfAccess = activityQuery.SelectMany(x => x.MediaResourceActivity).OrderByDescending(a => a.CreateDate).ToList().Count();
            }

            if (activityEntities.Any())
            {
                ResourceActivity filteredActivity = null;
                if (activityEntities.FirstOrDefault()?.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio || activityEntities.FirstOrDefault()?.Resource.ResourceTypeEnum == ResourceTypeEnum.Video)
                {
                    filteredActivity = activityEntities.Where(x => x.MediaResourceActivity != null && x.MediaResourceActivity.Any(x => x.PercentComplete == 100)).OrderByDescending(x => x.ActivityStart).FirstOrDefault();
                }
                else
                {
                    filteredActivity = activityEntities.Where(x => x.ActivityStatusId == (int)ActivityStatusEnum.Completed || x.ActivityStatusId == (int)ActivityStatusEnum.Incomplete || x.ActivityStatusId == (int)ActivityStatusEnum.Passed || x.ActivityStatusId == (int)ActivityStatusEnum.Downloaded).OrderByDescending(x => x.ActivityStart).FirstOrDefault();
                }

                if (filteredActivity != null)
                {
                    var result = await PopulateMyLearningDetailedItemViewModels(new List<ResourceActivity> { filteredActivity }, userId);
                    myLearningDetailedItemViewModel = result.FirstOrDefault();
                    if (filteredActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm)
                    {
                        var sa = scormActivityRepository.GetAll().OrderByDescending(x => x.Id).Where(x => x.ResourceActivity.ResourceVersionId == filteredActivity.ResourceVersionId && x.CreateUserId == filteredActivity.UserId);
                        if (sa.Any())
                        {
                            myLearningDetailedItemViewModel.ActivityDurationSeconds = 0;
                            foreach (var item in sa.Where(x => x.CmiCoreLessonStatus.HasValue))
                            {
                                myLearningDetailedItemViewModel.ActivityDurationSeconds = myLearningDetailedItemViewModel.ActivityDurationSeconds + item.DurationSeconds;
                            }
                        }
                    }
                    else if (filteredActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Video || filteredActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio)
                    {
                        var ma = mediaResourceActivity.GetAll().OrderByDescending(x => x.Id).Where(x => x.ResourceActivity.ResourceVersionId == filteredActivity.ResourceVersionId && x.CreateUserId == filteredActivity.UserId);
                        if (ma.Any())
                        {
                            myLearningDetailedItemViewModel.ActivityDurationSeconds = 0;
                            foreach (var item in ma)
                            {
                                if (item.SecondsPlayed.HasValue)
                                {
                                    myLearningDetailedItemViewModel.ActivityDurationSeconds = myLearningDetailedItemViewModel.ActivityDurationSeconds + (int)item.SecondsPlayed;
                                }
                            }
                        }
                    }

                    return new Tuple<int, MyLearningDetailedItemViewModel>(totalNumberOfAccess, result.FirstOrDefault());
                }

                return new Tuple<int, MyLearningDetailedItemViewModel>(0, myLearningDetailedItemViewModel);
            }

            return new Tuple<int, MyLearningDetailedItemViewModel>(0, myLearningDetailedItemViewModel);
        }

        /// <inheritdoc/>
        public async Task<List<MyLearningDetailedItemViewModel>> PopulateMyLearningDetailedItemViewModels(List<ResourceActivity> resourceActivities, int userId)
        {
            var viewModels = new List<MyLearningDetailedItemViewModel>();

            var activityNodes = (from entity in resourceActivities
                                 let data = entity.NodePath.CatalogueNodeId
                                 select data).Distinct().ToArray();

            var activityCatalogues = catalogueNodeVersionRepository.GetAll()
                .Include(x => x.NodeVersion).Where(c => activityNodes.Contains(c.NodeVersion.NodeId));

            foreach (ResourceActivity resourceActivity in resourceActivities)
            {
                var viewModel = new MyLearningDetailedItemViewModel()
                {
                    Title = resourceActivity.ResourceVersion.Title,
                    ResourceId = resourceActivity.ResourceId,
                    MajorVersion = resourceActivity.MajorVersion,
                    MinorVersion = resourceActivity.MinorVersion,
                    Version = resourceActivity.MajorVersion + "." + resourceActivity.MinorVersion,
                    ResourceType = resourceActivity.Resource.ResourceTypeEnum,
                    ActivityDate = resourceActivity.ActivityStart ?? resourceActivity.CreateDate,
                    ActivityStatus = (ActivityStatusEnum)resourceActivity.ActivityStatusId,
                    IsCurrentResourceVersion = resourceActivity.ResourceVersionId == resourceActivity.Resource.CurrentResourceVersionId,
                    VersionStatusId = (int?)resourceActivity.ResourceVersion.VersionStatusEnum,
                };

                var latestActivityCheck = await resourceActivityRepository.GetAllTheActivitiesFromSP(resourceActivity.CreateUserId, resourceActivity.ResourceVersionId);
                var allAttempts = latestActivityCheck;
                latestActivityCheck.RemoveAll(x => x.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm && (x.ActivityStatusId == (int)ActivityStatusEnum.Downloaded || x.ActivityStatusId == (int)ActivityStatusEnum.Incomplete || x.ActivityStatusId == (int)ActivityStatusEnum.InProgress));
                if (latestActivityCheck.Any() && latestActivityCheck.FirstOrDefault()?.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment)
                {

                    latestActivityCheck = latestActivityCheck.Where(x => x.AssessmentResourceActivity.FirstOrDefault() != null && (x.ResourceVersion.AssessmentResourceVersion.AssessmentType == AssessmentTypeEnum.Formal && x.AssessmentResourceActivity.First().Score.HasValue && (int)Math.Round(x.AssessmentResourceActivity.First().Score.Value, MidpointRounding.AwayFromZero) >= x.ResourceVersion.AssessmentResourceVersion.PassMark) ||(x.ResourceVersion.AssessmentResourceVersion.AssessmentType == AssessmentTypeEnum.Informal && x.ActivityStatusId == (int)ActivityStatusEnum.Completed)).ToList();
                }

                ResourceActivity expectedActivity = null;
                if (resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio || resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Video)
                {
                    expectedActivity = latestActivityCheck.Where(x => x.LaunchResourceActivityId == resourceActivity.Id && resourceActivity.MediaResourceActivity != null && resourceActivity.MediaResourceActivity.Any(y => y.PercentComplete == 100)).OrderByDescending(y => y.ActivityStart).FirstOrDefault();
                }
                else
                {
                    expectedActivity = latestActivityCheck.Where(x => x.Id == resourceActivity.Id && (x.ActivityStatusId == (int)ActivityStatusEnum.Completed || x.ActivityStatusId == (int)ActivityStatusEnum.Incomplete || x.ActivityStatusId == (int)ActivityStatusEnum.Passed || x.ActivityStatusId == (int)ActivityStatusEnum.Downloaded)).OrderByDescending(x => x.ActivityStart).FirstOrDefault();
                }

                if (latestActivityCheck.Any() && expectedActivity != null)
                {
                    bool isExpectedActivityIdMatched = false;
                    if (resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio || resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Video)
                    {
                        isExpectedActivityIdMatched = latestActivityCheck.OrderByDescending(x => x.ActivityStart).FirstOrDefault().Id == expectedActivity.LaunchResourceActivityId;
                    }
                    else
                    {
                        isExpectedActivityIdMatched = latestActivityCheck.OrderByDescending(x => x.ActivityStart).FirstOrDefault().Id == expectedActivity.Id;
                    }

                    viewModel.CertificateEnabled = isExpectedActivityIdMatched && resourceActivity.ResourceVersion.CertificateEnabled.GetValueOrDefault(false);
                }
                else
                {
                    viewModel.CertificateEnabled = false;
                }

                var catalogueNodeVersion = await activityCatalogues
                .SingleOrDefaultAsync(x => x.NodeVersion.NodeId == resourceActivity.NodePath.CatalogueNodeId);
                if (catalogueNodeVersion != null)
                {
                    viewModel.CertificateUrl = catalogueNodeVersion.CertificateUrl;
                }

                var resourceReference = resourceActivity.Resource.ResourceReference.FirstOrDefault(x => x.NodePathId == resourceActivity.NodePathId);

                if (resourceReference != null)
                {
                    viewModel.ResourceReferenceId = resourceReference.OriginalResourceReferenceId;
                }

                if (resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm)
                {
                    viewModel.ScorePercentage = resourceActivity.Score.HasValue ? (int)resourceActivity.Score : 0;
                    viewModel.MasteryScore = resourceActivity.ResourceVersion.ScormResourceVersion.ScormResourceVersionManifest.MasteryScore;
                    var sa = resourceActivity.ScormActivity.FirstOrDefault();
                    if (sa != null && sa.CmiCoreLessonStatus.HasValue)
                    {
                        viewModel.ActivityStatus = (ActivityStatusEnum)sa.CmiCoreLessonStatus;
                        viewModel.ActivityDurationSeconds = sa.DurationSeconds;
                        viewModel.ScorePercentage = sa.CmiCoreScoreRaw.HasValue ? (int)sa.CmiCoreScoreRaw : 0;
                    }
                }

                if (resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Video || resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio)
                {
                    if (resourceActivity.MediaResourceActivity != null && resourceActivity.MediaResourceActivity.Any())
                    {
                        viewModel.ActivityStatus = resourceActivity.MediaResourceActivity.First().PercentComplete == 100 ? ActivityStatusEnum.Completed : ActivityStatusEnum.InProgress;
                        viewModel.CompletionPercentage = Convert.ToInt32(resourceActivity.MediaResourceActivity.First().PercentComplete.GetValueOrDefault());
                        viewModel.ActivityDurationSeconds = resourceActivity.MediaResourceActivity.First().SecondsPlayed.GetValueOrDefault();
                    }
                    else if (viewModel.ActivityDate >= settings.DetailedMediaActivityRecordingStartDate)
                    {
                        // If media activity is from after the date when detailed activity recording began it should have an MRA. If it doesn't something has gone wrong.
                        viewModel.ActivityStatus = ActivityStatusEnum.InProgress;
                    }

                    if (resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Video)
                    {
                        viewModel.ResourceDurationMilliseconds = resourceActivity.ResourceVersion.VideoResourceVersion.DurationInMilliseconds.GetValueOrDefault();
                    }
                    else if (resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio)
                    {
                        viewModel.ResourceDurationMilliseconds = resourceActivity.ResourceVersion.AudioResourceVersion.DurationInMilliseconds.GetValueOrDefault();
                    }
                }

                if (resourceActivity.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment)
                {
                    var activity = resourceActivity.AssessmentResourceActivity.Count() > 0 ? resourceActivity.AssessmentResourceActivity.First() : null;
                    if (activity == null)
                    {
                        viewModel.Complete = false;
                        viewModel.CompletionPercentage = 0;
                        viewModel.AssessmentDetails = new MyLearningAssessmentDetails
                        {
                            PassMark = resourceActivity.ResourceVersion.AssessmentResourceVersion.PassMark,
                        };
                    }
                    else
                    {
                        viewModel.AssessmentResourceActivityId = activity.Id;
                        viewModel.Complete = activity.Score != null;
                        viewModel.ScorePercentage = activity.Score.HasValue ? (int)Math.Round(activity.Score.Value, MidpointRounding.AwayFromZero) : 0;

                        var currentAttempt = allAttempts.FindIndex(a => a.Id == resourceActivity.Id) + 1;

                        var assessmemntActivityQuestion = await resourceActivityRepository.GetAssessmentActivityCompletionPercentage(resourceActivity.CreateUserId, resourceActivity.ResourceVersionId, resourceActivity.Id);
                        viewModel.CompletionPercentage = (int)assessmemntActivityQuestion.CompletionPercentage;
                        if (viewModel.CompletionPercentage == 100)
                        {
                            if (resourceActivity.ResourceVersion.AssessmentResourceVersion.AssessmentType == AssessmentTypeEnum.Informal)
                            {
                                viewModel.ActivityStatus = ActivityStatusEnum.Completed;
                            }
                            else
                            {
                                viewModel.ActivityStatus = viewModel.ScorePercentage >= resourceActivity.ResourceVersion.AssessmentResourceVersion.PassMark
                                    ? ActivityStatusEnum.Passed
                                    : ActivityStatusEnum.Failed;
                            }
                        }
                        else
                        {
                            viewModel.ActivityStatus = ActivityStatusEnum.InProgress;
                        }

                        viewModel.AssessmentDetails = new MyLearningAssessmentDetails
                        {
                            ExtraAttemptReason = activity.Reason,
                            CurrentAttempt = currentAttempt,
                            MaximumAttempts = resourceActivity.ResourceVersion.AssessmentResourceVersion.MaximumAttempts,
                            PassMark = resourceActivity.ResourceVersion.AssessmentResourceVersion.PassMark,
                        };
                    }
                }

                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        /// <summary>
        /// Gets the resource certificate details.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requestModel">The request model</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyLearningCertificatesDetailedViewModel> GetUserCertificateDetails(int userId, MyLearningRequestModel requestModel)
        {
            Task<List<MoodleUserCertificateResponseModel>>? courseCertificatesTask = null;
            var filteredResource = GetFilteredResourceType(requestModel);

            if (filteredResource.Count() == 0 || (filteredResource.Any() && requestModel.Courses))
            {
                courseCertificatesTask = !string.IsNullOrWhiteSpace(requestModel.SearchText) ?
                    moodleApiService.GetUserCertificateAsync(userId, requestModel.SearchText) : moodleApiService.GetUserCertificateAsync(userId);

            }

            var resourceCertificatesTask = !string.IsNullOrWhiteSpace(requestModel.SearchText) ?
                 resourceRepository.GetUserCertificateDetails(userId, requestModel.SearchText) : resourceRepository.GetUserCertificateDetails(userId);


            // Await all active tasks in parallel
            if (courseCertificatesTask != null)
                await Task.WhenAll(courseCertificatesTask, resourceCertificatesTask);
            else
                await resourceCertificatesTask;

            var resourceCertificates = resourceCertificatesTask.Result ?? Enumerable.Empty<UserCertificateViewModel>();

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
                    CertificateDownloadUrl = c.DownloadLink,
                    ResourceVersionId = 0
                });
            }

            var allCertificates = resourceCertificates.Concat(mappedCourseCertificates);

            if (filteredResource != null && filteredResource.Any())
            {
                var allowedTypeIds = filteredResource
                    .Select(entry => Enum.TryParse<ResourceTypeEnum>(entry, true, out var parsed) ? (int?)parsed : null)
                    .Where(id => id.HasValue).Select(id => id.Value).ToHashSet();

                allCertificates = allCertificates.Where(c => allowedTypeIds.Contains(c.ResourceTypeId));
            }

            var orderedCertificates = allCertificates.OrderByDescending(c => c.AwardedDate);

            var totalCount = orderedCertificates.Count();
            var pagedResults = orderedCertificates
                .Skip(requestModel.Skip)
                .Take(requestModel.Take)
                .ToList();

            return new MyLearningCertificatesDetailedViewModel
            {
                Certificates = pagedResults,
                TotalCount = totalCount
            };
        }




        private IQueryable<ResourceActivity> ApplyFilters(IQueryable<ResourceActivity> query, MyLearningRequestModel requestModel)
        {
            // Text filter - Title, Keywords or Description.
            if (!string.IsNullOrEmpty(requestModel.SearchText))
            {
                query = query.Where(x => x.ResourceVersion.Title.Contains(requestModel.SearchText) ||
                                         x.ResourceVersion.Description.Contains(requestModel.SearchText) ||
                                         x.ResourceVersion.ResourceVersionKeyword.Any(y => y.Keyword.Contains(requestModel.SearchText)));
            }

            // Resource Type filter.
            if (requestModel.Article || requestModel.Audio || requestModel.Elearning || requestModel.Html || requestModel.File || requestModel.Image || requestModel.Video || requestModel.Weblink || requestModel.Assessment || requestModel.Case)
            {
                query = query.Where(x =>
                    requestModel.Article && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Article ||
                    requestModel.Audio && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio ||
                    requestModel.Elearning && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm ||
                    requestModel.Html && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Html ||
                    requestModel.File && x.Resource.ResourceTypeEnum == ResourceTypeEnum.GenericFile ||
                    requestModel.Image && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Image ||
                    requestModel.Video && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Video ||
                    requestModel.Weblink && x.Resource.ResourceTypeEnum == ResourceTypeEnum.WebLink ||
                    requestModel.Assessment && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment ||
                    requestModel.Case && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Case);
            }

            // Status Filter.
            // All media activity prior to start of detailed media activity recording count as complete.
            // All media activity after start of detailed media activity recording have to be 100% complete.
            // Scorm treated as "in progress" when there is a launch event but no subsequent associated complete / passed / failed.
            // Weblink, Article, Image treated as "complete" when there is a launch event.
            if (requestModel.Complete || requestModel.Incomplete || requestModel.Passed || requestModel.Failed || requestModel.Downloaded)
            {
                query = query.Where(x =>
                requestModel.Complete && ((x.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio || x.Resource.ResourceTypeEnum == ResourceTypeEnum.Video) &&
                                            (x.ActivityStart.Value < settings.DetailedMediaActivityRecordingStartDate ||
                                            x.MediaResourceActivity != null && x.MediaResourceActivity.Any() && x.MediaResourceActivity.First().PercentComplete == 100)
                                            ||
                                            (x.Resource.ResourceTypeEnum == ResourceTypeEnum.WebLink || x.Resource.ResourceTypeEnum == ResourceTypeEnum.Article || x.Resource.ResourceTypeEnum == ResourceTypeEnum.Image || x.Resource.ResourceTypeEnum == ResourceTypeEnum.Case) && x.ActivityStatusId == (int)ActivityStatusEnum.Launched
                                            ||
                                            x.Resource.ResourceTypeEnum != ResourceTypeEnum.Audio && x.Resource.ResourceTypeEnum != ResourceTypeEnum.Video && x.ActivityStatusId == (int)ActivityStatusEnum.Completed) ||
                requestModel.Incomplete && ((x.Resource.ResourceTypeEnum == ResourceTypeEnum.Audio || x.Resource.ResourceTypeEnum == ResourceTypeEnum.Video) &&
                                         x.ActivityStart.Value >= settings.DetailedMediaActivityRecordingStartDate &&
                                         (x.MediaResourceActivity == null || !x.MediaResourceActivity.Any() || x.MediaResourceActivity.First().PercentComplete < 100)
                                         ||
                                         //// Note - too slow to do this: (x.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm && x.ScormActivity.FirstOrDefault() != null && x.ScormActivity.First().CmiCoreLessonStatus == (int)ActivityStatusEnum.InProgress)
                                         //// - only returning "launched" for non-media when there is no subsequent event referencing the launch.
                                         //// (x.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm && x.ActivityStatusId == (int)ActivityStatusEnum.Launched)
                                         x.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm && x.ScormActivity.Any() && x.ScormActivity.First().CmiCoreLessonStatus == (int)ActivityStatusEnum.InProgress
                                         ||
                                         x.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment && x.AssessmentResourceActivity.Any() && !x.AssessmentResourceActivity.First().Score.HasValue) ||
                requestModel.Passed && x.ActivityStatusId == (int)ActivityStatusEnum.Passed ||
                requestModel.Passed && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment && x.AssessmentResourceActivity.Any() && x.AssessmentResourceActivity.First().Score >= x.ResourceVersion.AssessmentResourceVersion.PassMark ||
                requestModel.Failed && x.ActivityStatusId == (int)ActivityStatusEnum.Failed ||
                requestModel.Failed && x.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment && x.AssessmentResourceActivity.Any() && x.AssessmentResourceActivity.First().Score < x.ResourceVersion.AssessmentResourceVersion.PassMark ||
                requestModel.Downloaded && x.ActivityStatusId == (int)ActivityStatusEnum.Downloaded);
            }

            // Date Filter.
            if (!string.IsNullOrEmpty(requestModel.TimePeriod))
            {
                var now = DateTime.Now.Date;

                if (requestModel.TimePeriod == "dateRange")
                {
                    if (requestModel.StartDate.HasValue || requestModel.EndDate.HasValue)
                    {
                        if (requestModel.StartDate.HasValue)
                        {
                            query = query.Where(x => x.ActivityStart >= requestModel.StartDate);
                        }

                        if (requestModel.EndDate.HasValue)
                        {
                            query = query.Where(x => x.ActivityStart < requestModel.EndDate.Value.AddDays(1));
                        }
                    }
                    else
                    {
                        throw new ArgumentException("If RequestModel.TimePeriod is set to 'dateRange', the RequestModel.StartDate and/or EndDate must also be specified.");
                    }
                }
                else if (requestModel.TimePeriod == "thisWeek")
                {
                    // Definition of this week is anything from the Monday prior, or today if today is Monday.
                    var firstDateOfWeek = now.FirstDateInWeek(DayOfWeek.Monday);
                    query = query.Where(x => x.ActivityStart >= firstDateOfWeek);
                }
                else if (requestModel.TimePeriod == "thisMonth")
                {
                    // Definition of this month is anything from the 1st of the current month.
                    query = query.Where(x => x.ActivityStart >= new DateTime(now.Year, now.Month, 1));
                }
                else if (requestModel.TimePeriod == "last12Months")
                {
                    // Definition of the last 12 months is anything from the same date 12 months ago. e.g. if today is 7th Oct 2020, then return anything from 7th Oct 2019.
                    query = query.Where(x => x.ActivityStart > now.AddMonths(-12));
                }
            }

            if (requestModel.CertificateEnabled)
            {
                query = query.Where(x => x.ResourceVersion.CertificateEnabled.Equals(true));
            }

            return query;
        }

        private static List<string> GetFilteredResourceType(MyLearningRequestModel model)
        {
            var selectors = new Dictionary<string, Func<MyLearningRequestModel, bool>>
            {
                { nameof(model.Weblink),    m => m.Weblink },
                { nameof(model.File),       m => m.File },
                { nameof(model.Video),      m => m.Video },
                { nameof(model.Article),    m => m.Article },
                { nameof(model.Case),       m => m.Case },
                { nameof(model.Image),      m => m.Image },
                { nameof(model.Audio),      m => m.Audio },
                { nameof(model.Elearning),  m => m.Elearning },
                { nameof(model.Html),       m => m.Html },
                { nameof(model.Assessment), m => m.Assessment },
                { nameof(model.Courses),    m => m.Courses }
            };

            var normalisationMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(model.Courses), "Moodle" }
            };

            return selectors
                .Where(kvp => kvp.Value(model))
                .Select(kvp => normalisationMap.TryGetValue(kvp.Key, out var mapped) ? mapped : kvp.Key).ToList();
        }

    }
}