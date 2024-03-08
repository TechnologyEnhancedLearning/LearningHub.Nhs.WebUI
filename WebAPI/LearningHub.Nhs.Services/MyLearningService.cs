namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Repository.Helpers;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

    /// <summary>
    /// The rating service.
    /// </summary>
    public class MyLearningService : IMyLearningService
    {
        /// <summary>
        /// The resourceActivityRepository.
        /// </summary>
        private readonly IResourceActivityRepository resourceActivityRepository;
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
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly IOptions<Settings> settings;

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
        public MyLearningService(
            IResourceActivityRepository resourceActivityRepository,
            IMediaResourcePlayedSegmentRepository mediaResourcePlayedSegmentRepository,
            IAssessmentResourceActivityRepository assessmentResourceActivityRepository,
            IAssessmentResourceActivityInteractionRepository assessmentResourceActivityInteractionRepository,
            ICatalogueNodeVersionRepository catalogueNodeVersionRepository,
            IMapper mapper,
            IOptions<Settings> settings,
            IScormActivityRepository scormActivityRepository,
            IMediaResourceActivityRepository mediaResourceActivity)
        {
            this.resourceActivityRepository = resourceActivityRepository;
            this.mediaResourcePlayedSegmentRepository = mediaResourcePlayedSegmentRepository;
            this.assessmentResourceActivityRepository = assessmentResourceActivityRepository;
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
            this.assessmentResourceActivityInteractionRepository = assessmentResourceActivityInteractionRepository;
            this.mapper = mapper;
            this.settings = settings;
            this.scormActivityRepository = scormActivityRepository;
            this.mediaResourceActivity = mediaResourceActivity;
        }

        /// <summary>
        /// Gets the activity records for the detailed activity tab of My Learning screen.
        /// </summary>
        /// /// <param name="userId">The user id.</param>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyLearningDetailedViewModel> GetActivityDetailed(int userId, MyLearningRequestModel requestModel)
        {
            var activityQuery = this.resourceActivityRepository.GetByUserIdFromSP(userId, requestModel, this.settings.Value.DetailedMediaActivityRecordingStartDate).Result.OrderByDescending(r => r.ActivityStart).DistinctBy(l => l.Id);

            // Count total records.
            MyLearningDetailedViewModel viewModel = new MyLearningDetailedViewModel()
            {
                TotalCount = this.resourceActivityRepository.GetTotalCount(userId, requestModel, this.settings.Value.DetailedMediaActivityRecordingStartDate),
            };

            // Return only the requested batch.
            var activityEntities = activityQuery.ToList();

            viewModel.Activities = await this.PopulateMyLearningDetailedItemViewModels(activityEntities, userId);

            return viewModel;
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
            var playedSegments = await this.mediaResourcePlayedSegmentRepository.GetPlayedSegmentsAsync(userId, resourceId, majorVersion);
            var viewModel = this.mapper.Map<List<PlayedSegmentViewModel>>(playedSegments);

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
            var activityQuery = this.resourceActivityRepository.GetByUserId(userId);
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
            activityEntities.RemoveAll(x => x.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm && (x.ActivityStatusId == (int)ActivityStatusEnum.Downloaded || x.ActivityStatusId == (int)ActivityStatusEnum.Launched || x.ActivityStatusId == (int)ActivityStatusEnum.InProgress));
            if (activityEntities.Any() && activityEntities.FirstOrDefault()?.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment)
            {
                activityEntities = activityEntities.Where(x => x.AssessmentResourceActivity.FirstOrDefault() != null && x.AssessmentResourceActivity.FirstOrDefault().Score.HasValue && ((int)Math.Round(x.AssessmentResourceActivity.FirstOrDefault().Score.Value, MidpointRounding.AwayFromZero) >= x.ResourceVersion.AssessmentResourceVersion.PassMark)).ToList();
            }

            if (activityEntities.Any())
            {
                var filteredActivities = new List<ResourceActivity> { activityEntities.FirstOrDefault(x => x.ActivityStatusId == ((int)ActivityStatusEnum.Completed) || x.ActivityStatusId == ((int)ActivityStatusEnum.Launched) || x.ActivityStatusId == ((int)ActivityStatusEnum.Passed) || x.ActivityStatusId == ((int)ActivityStatusEnum.Downloaded)) };
                if (filteredActivities.FirstOrDefault() != null)
                {
                    var result = await this.PopulateMyLearningDetailedItemViewModels(filteredActivities, userId);
                    myLearningDetailedItemViewModel = result.FirstOrDefault();
                    if (filteredActivities.FirstOrDefault().Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm)
                    {
                        var sa = this.scormActivityRepository.GetAll().OrderByDescending(x => x.Id).Where(x => x.ResourceActivity.ResourceVersionId == filteredActivities.FirstOrDefault().ResourceVersionId && x.CreateUserId == filteredActivities.FirstOrDefault().UserId);
                        if (sa.Any())
                        {
                            myLearningDetailedItemViewModel.ActivityDurationSeconds = 0;
                            foreach (var item in sa.Where(x => x.CmiCoreLessonStatus.HasValue))
                            {
                                myLearningDetailedItemViewModel.ActivityDurationSeconds = myLearningDetailedItemViewModel.ActivityDurationSeconds + item.DurationSeconds;
                            }
                        }
                    }
                    else if (filteredActivities.FirstOrDefault().Resource.ResourceTypeEnum == ResourceTypeEnum.Video || filteredActivities.FirstOrDefault().Resource.ResourceTypeEnum == ResourceTypeEnum.Audio)
                    {
                        var ma = this.mediaResourceActivity.GetAll().OrderByDescending(x => x.Id).Where(x => x.ResourceActivity.ResourceVersionId == filteredActivities.FirstOrDefault().ResourceVersionId && x.CreateUserId == filteredActivities.FirstOrDefault().UserId);
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
                                 let data = entity.NodePath.NodeId
                                 select data).Distinct().ToArray();

            var activityCatalogues = this.catalogueNodeVersionRepository.GetAll()
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
                    ActivityDate = resourceActivity.ActivityStart.GetValueOrDefault(),
                    ActivityStatus = (ActivityStatusEnum)resourceActivity.ActivityStatusId,
                    IsCurrentResourceVersion = resourceActivity.ResourceVersionId == resourceActivity.Resource.CurrentResourceVersionId,
                    VersionStatusId = (int?)resourceActivity.ResourceVersion.VersionStatusEnum,
                };

                var latestActivityCheck = await this.resourceActivityRepository.GetAllTheActivitiesFromSP(resourceActivity.CreateUserId, resourceActivity.ResourceVersionId);
                latestActivityCheck.RemoveAll(x => x.Resource.ResourceTypeEnum == ResourceTypeEnum.Scorm && (x.ActivityStatusId == (int)ActivityStatusEnum.Downloaded || x.ActivityStatusId == (int)ActivityStatusEnum.Incomplete || x.ActivityStatusId == (int)ActivityStatusEnum.InProgress));
                if (latestActivityCheck.Any() && latestActivityCheck.FirstOrDefault()?.Resource.ResourceTypeEnum == ResourceTypeEnum.Assessment)
                {
                    latestActivityCheck = latestActivityCheck.Where(x => x.AssessmentResourceActivity.FirstOrDefault() != null && x.AssessmentResourceActivity.FirstOrDefault().Score.HasValue && ((int)Math.Round(x.AssessmentResourceActivity.FirstOrDefault().Score.Value, MidpointRounding.AwayFromZero) >= x.ResourceVersion.AssessmentResourceVersion.PassMark)).ToList();
                }

                var expectedActivities = latestActivityCheck.Where(x => x.Id == resourceActivity.Id && (x.ActivityStatusId == ((int)ActivityStatusEnum.Completed) || x.ActivityStatusId == ((int)ActivityStatusEnum.Passed))).OrderByDescending(x => x.ActivityStart).FirstOrDefault();

                if (latestActivityCheck.Any() && expectedActivities != null)
                {
                    if (latestActivityCheck.OrderByDescending(x => x.ActivityStart).FirstOrDefault().Id == expectedActivities.Id)
                    {
                        viewModel.CertificateEnabled = resourceActivity.ResourceVersion.CertificateEnabled.GetValueOrDefault(false);
                    }
                    else
                    {
                        viewModel.CertificateEnabled = false;
                    }
                }
                else
                {
                    viewModel.CertificateEnabled = false;
                }

                var catalogueNodeVersion = await activityCatalogues
                .SingleOrDefaultAsync(x => x.NodeVersion.NodeId == resourceActivity.NodePath.NodeId);
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
                        viewModel.ActivityStatus = (resourceActivity.MediaResourceActivity.First().PercentComplete == 100) ? ActivityStatusEnum.Completed : ActivityStatusEnum.InProgress;
                        viewModel.CompletionPercentage = Convert.ToInt32(resourceActivity.MediaResourceActivity.First().PercentComplete.GetValueOrDefault());
                        viewModel.ActivityDurationSeconds = resourceActivity.MediaResourceActivity.First().SecondsPlayed.GetValueOrDefault();
                    }
                    else if (viewModel.ActivityDate >= this.settings.Value.DetailedMediaActivityRecordingStartDate)
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
                        var totalQuestions = resourceActivity.ResourceVersion.AssessmentResourceVersion.AssessmentContent.Blocks.Where(b => b.BlockType == BlockType.Question).Count();
                        var completedQuestions = activity.AssessmentResourceActivityInteractions.Count();
                        viewModel.CompletionPercentage = totalQuestions == 0 ? 0 : Convert.ToInt32(100 * completedQuestions / Convert.ToDecimal(totalQuestions));
                        viewModel.ScorePercentage = activity.Score.HasValue ? (int)Math.Round(activity.Score.Value, MidpointRounding.AwayFromZero) : 0;

                        var allAttempts = await this.resourceActivityRepository.GetAllTheActivitiesFor(userId, resourceActivity.ResourceVersionId);
                        var currentAttempt = allAttempts.FindIndex(a => a.Id == resourceActivity.Id) + 1;

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
    }
}