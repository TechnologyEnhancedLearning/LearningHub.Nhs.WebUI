namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The hierarchy service.
    /// </summary>
    public class ActivityService : IActivityService
    {
        private readonly IAssessmentResourceActivityRepository assessmentResourceActivityRepository;
        private readonly IAssessmentResourceActivityInteractionRepository assessmentResourceActivityInteractionRepository;
        private readonly IUserService userService;
        private readonly IResourceVersionRepository resourceVersionRepository;
        private readonly IMediaResourceActivityRepository mediaResourceActivityRepository;
        private readonly IMediaResourceActivityInteractionRepository mediaResourceActivityInteractionRepository;
        private readonly IResourceActivityRepository resourceActivityRepository;
        private readonly IScormActivityRepository scormActivityRepository;
        private readonly IAssessmentResourceVersionRepository assessmentResourceVersionRepository;
        private readonly IBlockCollectionRepository blockCollectionRepository;
        private readonly ITimezoneOffsetManager timezoneOffsetManager;
        private readonly ILogger<ActivityService> logger;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityService"/> class.
        /// </summary>
        /// <param name="resourceActivityRepository">The resource activity repository.</param>
        /// <param name="assessmentResourceActivityRepository">The assessment resource activity repository.</param>
        /// <param name="assessmentResourceActivityInteractionRepository">The assessment resource activity interaction repository.</param>
        /// <param name="mediaResourceActivityRepository">The media resource activity repository.</param>
        /// <param name="mediaResourceActivityInteractionRepository">The media resource activity interaction repository.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="resourceVersionRepository">The resourceVersionRepository.</param>
        /// <param name="assessmentResourceVersionRepository">The assessmentResourceVersionRepository.</param>
        /// <param name="blockCollectionRepository">The blockCollectionRepository.</param>
        /// <param name="scormActivityRepository">The scorm activity repository.</param>
        /// <param name="timezoneOffsetManager">The timezoneOffsetManager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        public ActivityService(
            IResourceActivityRepository resourceActivityRepository,
            IAssessmentResourceActivityRepository assessmentResourceActivityRepository,
            IAssessmentResourceActivityInteractionRepository assessmentResourceActivityInteractionRepository,
            IMediaResourceActivityRepository mediaResourceActivityRepository,
            IMediaResourceActivityInteractionRepository mediaResourceActivityInteractionRepository,
            IUserService userService,
            IResourceVersionRepository resourceVersionRepository,
            IAssessmentResourceVersionRepository assessmentResourceVersionRepository,
            IBlockCollectionRepository blockCollectionRepository,
            IScormActivityRepository scormActivityRepository,
            ITimezoneOffsetManager timezoneOffsetManager,
            ILogger<ActivityService> logger,
            IMapper mapper)
        {
            this.resourceActivityRepository = resourceActivityRepository;
            this.assessmentResourceActivityRepository = assessmentResourceActivityRepository;
            this.assessmentResourceActivityInteractionRepository = assessmentResourceActivityInteractionRepository;
            this.mediaResourceActivityRepository = mediaResourceActivityRepository;
            this.mediaResourceActivityInteractionRepository = mediaResourceActivityInteractionRepository;
            this.userService = userService;
            this.resourceVersionRepository = resourceVersionRepository;
            this.assessmentResourceVersionRepository = assessmentResourceVersionRepository;
            this.blockCollectionRepository = blockCollectionRepository;
            this.scormActivityRepository = scormActivityRepository;
            this.timezoneOffsetManager = timezoneOffsetManager;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// Create an activity record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createResourceActivityViewModel">The resource Activity View Model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateResourceActivity(
            int userId,
            CreateResourceActivityViewModel createResourceActivityViewModel)
        {
            var result = new LearningHubValidationResult();
            result.IsValid = false;

            var createResourceActivityValidator = new CreateResourceActivityValidator();

            var vr = await createResourceActivityValidator.ValidateAsync(createResourceActivityViewModel);

            if (vr.IsValid)
            {
                int id = 0;
                ResourceTypeEnum resourceType = await resourceVersionRepository.GetResourceType(createResourceActivityViewModel.ResourceVersionId);

                // If its an assessment activity, check there haven't been too many attempts.
                if (resourceType == ResourceTypeEnum.Assessment)
                {
                    var tooManyAttempts = await UserHasUsedAllAttempts(userId, createResourceActivityViewModel.ResourceVersionId);
                    if (tooManyAttempts && createResourceActivityViewModel.ExtraAttemptReason == null)
                    {
                        return new LearningHubValidationResult(true, "TooManyAttempts");
                    }
                }

                // If it's a media activity being marked as complete, analyse the media activity interactions to update this user's played segments.
                if ((resourceType == ResourceTypeEnum.Video || resourceType == ResourceTypeEnum.Audio) && createResourceActivityViewModel.ActivityStatus == ActivityStatusEnum.Completed)
                {
                    id = await this.CompleteMediaActivityAsync(
                        createResourceActivityViewModel.ResourceVersionId,
                        createResourceActivityViewModel.NodePathId,
                        createResourceActivityViewModel.MediaResourceActivityId.Value,
                        userId,
                        createResourceActivityViewModel.LaunchResourceActivityId.Value,
                        timezoneOffsetManager.ConvertToUserTimezone(createResourceActivityViewModel.ActivityEnd.Value));
                }
                else
                {
                    // For anything else, simply create the activity record.
                    id = resourceActivityRepository.CreateActivity(
                        userId,
                        createResourceActivityViewModel.ResourceVersionId,
                        createResourceActivityViewModel.NodePathId,
                        createResourceActivityViewModel.LaunchResourceActivityId,
                        createResourceActivityViewModel.ActivityStatus,
                        createResourceActivityViewModel.ActivityStart.HasValue ? timezoneOffsetManager.ConvertToUserTimezone(createResourceActivityViewModel.ActivityStart.Value) : null,
                        createResourceActivityViewModel.ActivityEnd.HasValue ? timezoneOffsetManager.ConvertToUserTimezone(createResourceActivityViewModel.ActivityEnd.Value) : null);
                }

                if (id > 0)
                {
                    result.CreatedId = id;
                    result.IsValid = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Create media resource activity record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createMediaResourceActivityViewModel">The media resource activity view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateMediaResourceActivity(
            int userId,
            CreateMediaResourceActivityViewModel createMediaResourceActivityViewModel)
        {
            var result = new LearningHubValidationResult();
            result.IsValid = false;

            var createMediaResourceActivityValidator = new CreateMediaResourceActivityValidator();

            var vr = await createMediaResourceActivityValidator.ValidateAsync(createMediaResourceActivityViewModel);

            if (vr.IsValid)
            {
                var mediaResourceActivity = mapper.Map<MediaResourceActivity>(createMediaResourceActivityViewModel);
                mediaResourceActivity.ActivityStart = timezoneOffsetManager.ConvertToUserTimezone(mediaResourceActivity.ActivityStart);
                int id = await mediaResourceActivityRepository.CreateAsync(userId, mediaResourceActivity);

                if (id > 0)
                {
                    result.CreatedId = id;
                    result.IsValid = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Create media resource activity interaction record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createMediaResourceActivityInteractionViewModel">The media resource activity interaction view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateMediaResourceActivityInteraction(
            int userId,
            CreateMediaResourceActivityInteractionViewModel createMediaResourceActivityInteractionViewModel)
        {
            var result = new LearningHubValidationResult();
            result.IsValid = false;

            var createResourceActivityInteractionValidator = new CreateMediaResourceActivityInteractionValidator();

            var vr = await createResourceActivityInteractionValidator.ValidateAsync(createMediaResourceActivityInteractionViewModel);
            if (vr.IsValid)
            {
                var entity = new MediaResourceActivityInteraction();
                entity.MediaResourceActivityId = createMediaResourceActivityInteractionViewModel.MediaResourceActivityId;
                entity.MediaResourceActivityTypeId = Convert.ToInt32(createMediaResourceActivityInteractionViewModel.MediaResourceActivityType);
                entity.DisplayTime = TimeSpan.FromSeconds(createMediaResourceActivityInteractionViewModel.DurationInSeconds);
                entity.ClientDateTime = timezoneOffsetManager.ConvertToUserTimezone(createMediaResourceActivityInteractionViewModel.ClientDateTime);

                var id = await mediaResourceActivityInteractionRepository.CreateAsync(userId, entity);

                if (id > 0)
                {
                    result.CreatedId = id;
                    result.IsValid = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Create assessment resource activity record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createAssessmentResourceActivityViewModel">The assessment resource activity view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateAssessmentResourceActivity(
            int userId,
            CreateAssessmentResourceActivityViewModel createAssessmentResourceActivityViewModel)
        {
            var result = new LearningHubValidationResult();
            result.IsValid = false;

            var createAssessmentResourceActivityValidator = new CreateAssessmentResourceActivityValidator();

            var vr = await createAssessmentResourceActivityValidator.ValidateAsync(createAssessmentResourceActivityViewModel);

            if (vr.IsValid)
            {
                var id = 0;
                var assessmentResourceActivity = mapper.Map<AssessmentResourceActivity>(createAssessmentResourceActivityViewModel);
                var resourceVersionId = (await resourceActivityRepository.GetByIdAsync(assessmentResourceActivity.ResourceActivityId)).ResourceVersionId;
                var resourceType = await resourceVersionRepository.GetResourceType(resourceVersionId);
                var numberAttempts = resourceActivityRepository.GetByUserId(userId)
                              .Where(x => x.ResourceVersionId == resourceVersionId)
                              .SelectMany(x => x.AssessmentResourceActivity)
                              .OrderByDescending(a => a.CreateDate)
                              .ToList().Count();
                var maxAttempts = (await assessmentResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId)).MaximumAttempts;
                if ((!maxAttempts.HasValue || numberAttempts <= maxAttempts ||
                     createAssessmentResourceActivityViewModel.ExtraAttemptReason != null)
                    && resourceType == ResourceTypeEnum.Assessment)
                {
                    id = await assessmentResourceActivityRepository.CreateAsync(userId, assessmentResourceActivity);
                }

                if (id > 0)
                {
                    result.CreatedId = id;
                    result.IsValid = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Create assessment resource activity interaction record for the supplied params.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="createAssessmentResourceActivityInteractionViewModel">The assessment resource activity interaction view model.</param>
        /// <param name="answerInOrder">Whether the user should answer questions in order.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateAssessmentResourceActivityInteraction(
            int userId,
            CreateAssessmentResourceActivityInteractionViewModel createAssessmentResourceActivityInteractionViewModel,
            bool answerInOrder)
        {
            var result = new LearningHubValidationResult();
            result.IsValid = false;

            var createResourceActivityInteractionValidator = new CreateAssessmentResourceActivityInteractionValidator();

            var vr = await createResourceActivityInteractionValidator.ValidateAsync(createAssessmentResourceActivityInteractionViewModel);
            if (vr.IsValid)
            {
                var assessmentResourceActivity = await assessmentResourceActivityRepository.GetByIdAsync(createAssessmentResourceActivityInteractionViewModel.AssessmentResourceActivityId);
                var questions = await this.GetAssessmentQuestionsByResourceActivityId(assessmentResourceActivity.ResourceActivityId);
                var allQuestionsAnswered = assessmentResourceActivity.AssessmentResourceActivityInteractions.Count() + 1 == questions.Count();
                var questionBlock = questions
                    .ElementAt(createAssessmentResourceActivityInteractionViewModel.QuestionNumber)
                    .QuestionBlock;

                // Note: this reuses the validation from elsewhere, which needs a viewmodel for some reason.
                var questionViewModel = mapper.Map<QuestionBlockViewModel>(questionBlock);
                if (!AnswerSubmissionValidationHelper.IsAnswerSubmissionValid(createAssessmentResourceActivityInteractionViewModel.Answers.ToList(), questionViewModel))
                {
                    result.Details = new List<string>(new string[] { "Invalid request. Please check whether the submitted answers are for the correct question." });
                    return result;
                }

                if (!await this.UserCanSubmitAssessmentAnswers(userId, answerInOrder, assessmentResourceActivity, createAssessmentResourceActivityInteractionViewModel, questionBlock))
                {
                    return result;
                }

                var id = await assessmentResourceActivityInteractionRepository.CreateInteraction(userId, new AssessmentResourceActivityInteraction()
                {
                    AssessmentResourceActivityId = assessmentResourceActivity.Id,
                    QuestionBlockId = questionBlock.Id,
                    Answers = questionBlock.QuestionType != QuestionTypeEnum.MatchGame
                        ? this.MapAnswerSelectionsToIds(questionBlock.Answers, createAssessmentResourceActivityInteractionViewModel.Answers)
                        : this.MapMatchAnswerSelectionsToIds(questionBlock.Answers, createAssessmentResourceActivityInteractionViewModel.Answers),
                });

                if (allQuestionsAnswered)
                {
                    await this.CompleteAssessmentResourceActivity(userId, assessmentResourceActivity.Id);
                }

                result.CreatedId = id;
                result.IsValid = true;
            }

            return result;
        }

        /// <summary>
        /// Gets the resource version id of the assessment, linked to the given activity.
        /// </summary>
        /// <param name="assessmentResourceActivityId">The Id for the assessment resource activity.</param>
        /// <returns>Returns the corresponding resource version id.</returns>
        public async Task<int> GetAssessmentResourceIdByActivity(int assessmentResourceActivityId)
        {
            var assessmentResourceActivity =
                await assessmentResourceActivityRepository.GetByIdAsync(assessmentResourceActivityId);

            return assessmentResourceActivity.ResourceActivity.ResourceVersionId;
        }

        /// <summary>
        /// Gets the latest assessment resource activity for the given user and resource version id's.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The assessment resource activity task.</returns>
        public async Task<AssessmentResourceActivity> GetLatestAssessmentResourceActivityByResourceVersionAndUserId(int resourceVersionId, int userId)
        {
            return await assessmentResourceActivityRepository.GetLatestAssessmentResourceActivity(resourceVersionId, userId);
        }

        /// <summary>
        /// Gets the attempts the user has made for the given assessment.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="resourceVersionId">The Id for the assessment resource.</param>
        /// <returns>Returns how many attempts the user has made.</returns>
        public int GetAttempts(int userId, int resourceVersionId)
        {
            return resourceActivityRepository.GetByUserId(userId)
                .Where(x => x.ResourceVersionId == resourceVersionId)
                .SelectMany(x => x.AssessmentResourceActivity)
                .OrderByDescending(a => a.CreateDate)
                .ToList().Count();
        }

        /// <summary>
        /// Gets the answers for all the activities of a given assessment.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="resourceVersionId">The Id for the assessment resource.</param>
        /// <returns>Returns the answers for all the activities of a given assessment.</returns>
        public async Task<Dictionary<int, Dictionary<int, IEnumerable<int>>>> GetAnswersForAllTheAssessmentResourceActivities(int userId, int resourceVersionId)
        {
            var result = new Dictionary<int, Dictionary<int, IEnumerable<int>>>();
            var assessmentResourceActivities = resourceActivityRepository.GetByUserId(userId)
                .Where(x => x.ResourceVersionId == resourceVersionId)
                .SelectMany(x => x.AssessmentResourceActivity)
                .OrderByDescending(a => a.CreateDate)
                .ToList();

            foreach (var activity in assessmentResourceActivities)
            {
                result[activity.Id] = await this.GetUserAssessmentAnswers(activity.Id);
            }

            return result;
        }

        /// <summary>
        /// This method cleans up incomplete media activities. Required if for any reason, the end of the user's activity was not recorded normally. For example - browser crash, power loss, connection loss.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task CleanUpIncompleteActivitiesAsync(int userId)
        {
            try
            {
                var incompleteActivities = await resourceActivityRepository.GetIncompleteMediaActivities(userId);

                foreach (var resourceActivity in incompleteActivities)
                {
                    if (resourceActivity.MediaResourceActivity != null && resourceActivity.MediaResourceActivity.Any())
                    {
                        var mediaResourceActivity = resourceActivity.MediaResourceActivity.FirstOrDefault();

                        if (mediaResourceActivity != null && mediaResourceActivity.MediaResourceActivityInteraction != null && mediaResourceActivity.MediaResourceActivityInteraction.Any())
                        {
                            await this.CompleteMediaActivityAsync(
                                resourceActivity.ResourceVersionId,
                                resourceActivity.NodePathId,
                                mediaResourceActivity.Id,
                                userId,
                                resourceActivity.Id,
                                DateTimeOffset.Now);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// Launch scorm activity.
        /// </summary>
        /// <param name="currentUserId">The user Id.</param>
        /// <param name="launchScormActivityViewModel">The scorm Activity View Model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ScormActivityViewModel> LaunchScormActivity(int currentUserId, LaunchScormActivityViewModel launchScormActivityViewModel)
        {
            var scormActivitySummary = scormActivityRepository.GetScormActivitySummary(currentUserId, launchScormActivityViewModel.ResourceReferenceId);
            var scormActivityId = scormActivityRepository.Create(currentUserId, launchScormActivityViewModel.ResourceReferenceId);

            var resourceVersion = await resourceVersionRepository.GetCurrentResourceForResourceReferenceIdAsync(launchScormActivityViewModel.ResourceReferenceId);
            var activeContent = new ActiveContentViewModel()
            {
                ScormActivityId = scormActivityId,
                ResourceReferenceId = launchScormActivityViewModel.ResourceReferenceId,
                ResourceId = resourceVersion.ResourceId,
                ResourceVersionId = resourceVersion.Id,
            };

            await userService.AddActiveContent(activeContent, currentUserId);

            ScormActivity scormActivity;
            if (scormActivitySummary.IncompleteActivityId.HasValue)
            {
                scormActivity = scormActivityRepository.Clone(scormActivitySummary.IncompleteActivityId.Value, scormActivityId);
                var result = mapper.Map<ScormActivityViewModel>(scormActivity);
                result.ClonedFromScormActivityId = scormActivitySummary.IncompleteActivityId.Value;
                result.TotalTime = scormActivitySummary.TotalTime;
                return result;
            }
            else
            {
                scormActivity = await scormActivityRepository.GetByIdAsync(scormActivityId);
                return mapper.Map<ScormActivityViewModel>(scormActivity);
            }
        }

        /// <summary>
        /// Update scorm activity.
        /// </summary>
        /// <param name="currentUserId">The user Id.</param>
        /// <param name="updateScormActivityViewModel">The update scorm Activity View Model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ScormUpdateResponseViewModel> UpdateScormActivity(int currentUserId, ScormActivityViewModel updateScormActivityViewModel)
        {
            var updateScormActivityValidator = new ScormActivityValidator();

            var vr = await updateScormActivityValidator.ValidateAsync(updateScormActivityViewModel);

            if (vr.IsValid)
            {
                ScormActivity scormActivity = mapper.Map<ScormActivity>(updateScormActivityViewModel);
                await scormActivityRepository.UpdateAsync(currentUserId, scormActivity);

                ScormUpdateResponseViewModel response = mapper.Map<ScormUpdateResponseViewModel>(scormActivity);
                response.IsValid = true;
                return response;
            }
            else
            {
                var response = new ScormUpdateResponseViewModel();
                response.IsValid = false;
                return response;
            }
        }

        /// <summary>
        /// Complete scorm activity.
        /// </summary>
        /// <param name="currentUserId">The user Id.</param>
        /// <param name="completeScormActivityViewModel">The update scorm Activity View Model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CompleteScormActivity(int currentUserId, ScormActivityViewModel completeScormActivityViewModel)
        {
            try
            {
                if (completeScormActivityViewModel.LessonStatusId.HasValue
                    && (completeScormActivityViewModel.LessonStatusId.Value == (int)ActivityStatusEnum.Completed
                        || completeScormActivityViewModel.LessonStatusId == (int)ActivityStatusEnum.Passed
                        || completeScormActivityViewModel.LessonStatusId == (int)ActivityStatusEnum.Failed))
                {
                    // Handle activity "complete" event - create new ResourceActivity record & perform any re-calc status updates.
                    scormActivityRepository.Complete(currentUserId, completeScormActivityViewModel.InstanceId);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                // Release associated active content
                _ = await userService.ReleaseActiveContent(new ActiveContentReleaseViewModel() { UserId = currentUserId, ScormActivityId = completeScormActivityViewModel.InstanceId });
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The resolve scorm activity.
        /// Resolves any completed active content that does not have associated completion events.
        /// Required when LMS has not received an LMSFinish event yet has received an LMSCommit with
        /// a "Completed" status - scenario may arise due to lost connection etc.
        /// </summary>
        /// <param name="userId">The user id.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ResolveScormActivity(int userId)
        {
            var activeContent = await userService.GetActiveContentAsync(userId);

            foreach (var acvm in activeContent)
            {
                // Handle "activity complete" event - create new ResourceActivity record & perform any re-calc status updates.
                scormActivityRepository.Resolve(acvm.ScormActivityId);
            }

            // Release all associated active content
            _ = await userService.ReleaseActiveContent(new ActiveContentReleaseViewModel() { UserId = userId, ReleaseAll = true });
        }

        /// <summary>
        /// Gets the user Id for a given assessment activity.
        /// </summary>
        /// <param name="assessmentResourceActivityId"> The assessment resource activity Id.</param>
        /// <returns> The corresponding user Id.</returns>
        public int GetUserIdForActivity(int assessmentResourceActivityId)
        {
            return assessmentResourceActivityRepository.GetByIdAsync(assessmentResourceActivityId).Result.CreateUserId;
        }

        /// <summary>
        /// Check user scorm activity data suspend data need to be cleared.
        /// </summary>
        /// <param name="lastScormActivityId">last scorm activity id.</param>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <returns>boolean.</returns>
        public Task<bool> CheckUserScormActivitySuspendDataToBeCleared(int lastScormActivityId, int resourceVersionId)
        {
            return scormActivityRepository.CheckUserScormActivitySuspendDataToBeCleared(lastScormActivityId, resourceVersionId);
        }

        /// <summary>
        /// Carries out the operations required to record the end of a media resource activity. Different scenarios require different data updates, all covered in this method.
        /// - Closing a browser tab.
        /// - Logging in after leaving a browser tab open. (tidy up process)
        /// - Watching more of a video in a browser tab that was left open.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="mediaResourceActivityId">The mediaResourceActivityId.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="launchResourceActivityId">The launchResourceActivityId.</param>
        /// <param name="activityEnd">The activityEnd.</param>
        /// <returns>The id of the ResourceActivity record created.</returns>
        private async Task<int> CompleteMediaActivityAsync(int resourceVersionId, int nodePathId, int mediaResourceActivityId, int userId, int launchResourceActivityId, DateTimeOffset activityEnd)
        {
            int id;

            // 1. Check if the launch activity has already been ended. This could happen if the user logs in on a different device/browser whilst leaving the original page still open. The login process will have automatically ended the activity.
            var endActivity = await resourceActivityRepository.GetAll()
                .Where(x => x.LaunchResourceActivityId == launchResourceActivityId).FirstOrDefaultAsync();

            if (endActivity != null)
            {
                // If one is found, mark it as deleted and create a new one with an updated end date. It's a "fact" table, and we're not allowed to update the existing rows.
                endActivity.Deleted = true;
                await resourceActivityRepository.UpdateAsync(userId, endActivity);
            }

            // 2. Check if the last interaction is a "playing" one. If so, we need to artificially create a pause event using the same timestamp in order for the analysis SP to function correctly.
            // This scenario happens when the activity is being ended by the login tidy up process.
            var lastInteraction = mediaResourceActivityInteractionRepository.GetAll().Where(x => x.MediaResourceActivityId == mediaResourceActivityId).OrderBy(x => x.ClientDateTime).LastOrDefault();

            if (lastInteraction != null && lastInteraction.MediaResourceActivityTypeId == (int)MediaResourceActivityTypeEnum.Playing)
            {
                var pauseInteraction = new MediaResourceActivityInteraction
                {
                    ClientDateTime = lastInteraction.ClientDateTime,
                    DisplayTime = lastInteraction.DisplayTime,
                    MediaResourceActivityId = lastInteraction.MediaResourceActivityId,
                    MediaResourceActivityTypeId = (int)MediaResourceActivityTypeEnum.Pause,
                };
                await mediaResourceActivityInteractionRepository.CreateAsync(userId, pauseInteraction);
            }

            // 3. Create new end activity record. All scenarios. Start with ActivityStatus of Incomplete. Update to Completed in step 6 if all of media played.
            id = resourceActivityRepository.CreateActivity(userId, resourceVersionId, nodePathId, launchResourceActivityId, ActivityStatusEnum.Incomplete, null, activityEnd);

            // 4. Analyse the activity to update the user's played segments for this resource activity. All scenarios.
            await mediaResourceActivityInteractionRepository.CalculatePlayedMediaSegments(userId, resourceVersionId, mediaResourceActivityId);

            // 5. Re-analyse any completed activities that started after this one because their cumulative percentage complete will now be wrong.
            // This scenario happens if user watches a video in multiple tabs or devices and they close an earlier activity after a later one.
            // This also updates the ActivityStatusId on the end ResourceActivity record if the percentage complete is 100%.
            var launchActivity = await resourceActivityRepository.GetByIdAsync(launchResourceActivityId);
            var laterActivities = resourceActivityRepository.GetAll()
                .Include(x => x.MediaResourceActivity)
                .Where(x => x.MediaResourceActivity != null &&
                            x.UserId == userId &&
                            x.ResourceVersionId == resourceVersionId &&
                            x.ActivityStart > launchActivity.ActivityStart &&
                            !x.LaunchResourceActivityId.HasValue).ToList();

            foreach (var ra in laterActivities)
            {
                if (ra.MediaResourceActivity.FirstOrDefault() != null)
                {
                    await mediaResourceActivityInteractionRepository.CalculatePlayedMediaSegments(userId, resourceVersionId, ra.MediaResourceActivity.First().Id);
                }
            }

            return id;
        }

        /// <summary>
        /// Completes an assessment resource activity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="assessmentResourceActivityId">The assessment resource activity id.</param>
        /// <returns>The task that resolves with the validation result.</returns>
        private async Task<LearningHubValidationResult> CompleteAssessmentResourceActivity(int userId, int assessmentResourceActivityId)
        {
            var result = new LearningHubValidationResult { IsValid = false };

            var assessmentResourceActivity = await assessmentResourceActivityRepository.GetByIdAsync(assessmentResourceActivityId);
            var resourceActivity = assessmentResourceActivity.ResourceActivity;

            if (await this.ResourceActivityIsCompleted(resourceActivity.Id))
            {
                return result;
            }

            var assessmentResourceVersion = await assessmentResourceVersionRepository
               .GetByResourceVersionIdAsync(assessmentResourceActivity.ResourceActivity.ResourceVersionId);
            var assessmentResource = await this.CalculateAssessmentResourceActivityScore(userId, assessmentResourceActivity);

            var activityStatus = ActivityStatusEnum.Completed;
            if (assessmentResourceVersion != null && (int)assessmentResourceVersion.AssessmentType == 2)
            {
                if (assessmentResource.Score >= assessmentResourceVersion.PassMark)
                {
                    activityStatus = ActivityStatusEnum.Passed;
                }
                else
                {
                    activityStatus = ActivityStatusEnum.Failed;
                }
            }

            resourceActivityRepository.CreateActivity(
                resourceActivity.CreateUserId,
                resourceActivity.ResourceVersionId,
                resourceActivity.NodePathId,
                resourceActivity.Id,
                activityStatus,
                null,
                timezoneOffsetManager.ConvertToUserTimezone(DateTimeOffset.UtcNow));

            result.IsValid = true;
            return result;
        }

        /// <summary>
        /// Calculates and updates the score for an assessment resource activity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="assessmentResourceActivity">The assessment resource activity.</param>
        /// <returns>The task.</returns>
        private async Task<AssessmentResourceActivity> CalculateAssessmentResourceActivityScore(int userId, AssessmentResourceActivity assessmentResourceActivity)
        {
            var questions = await GetAssessmentQuestionsByResourceActivityId(assessmentResourceActivity.ResourceActivityId);
            var answers = (await assessmentResourceActivityInteractionRepository.
                    GetInteractionsForAssessmentResourceActivity(assessmentResourceActivity.Id))
                    .OrderBy(a => a.QuestionBlockId)
                    .Select(this.GetQuestionAnswers);

            var (userScore, maxScore) = ScoreCalculationHelper.CalculateScore(
                questions.Select(q => mapper.Map<BlockViewModel>(q)).ToList(),
                answers);

            if (userScore == null)
            {
                throw new ValidationException("Invalid number of answers submitted.");
            }

            assessmentResourceActivity.Score = Math.Round(Convert.ToDecimal(userScore) / maxScore * 100, 3);
            await assessmentResourceActivityRepository.UpdateAsync(userId, assessmentResourceActivity);
            return assessmentResourceActivity;
        }

        /// <summary>
        /// Gets the block collection for an assessment, given its resource activity id.
        /// </summary>
        /// <param name="resourceActivityId">The resource activity id.</param>
        /// <returns>The block collection task.</returns>
        private async Task<IEnumerable<Block>> GetAssessmentQuestionsByResourceActivityId(int resourceActivityId)
        {
            var resourceActivity = await resourceActivityRepository.GetByIdAsync(resourceActivityId);
            var assessmentResourceVersion = await assessmentResourceVersionRepository.GetByResourceVersionIdAsync(resourceActivity.ResourceVersionId);

            var questionBlocks = await blockCollectionRepository.GetQuestionBlocks(assessmentResourceVersion.AssessmentContentId.Value);

            return questionBlocks;
        }

        /// <summary>
        /// Determines whether a user can submit a given set of answers to an assessment activity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="answerInOrder">The answerInOrder property of the activity.</param>
        /// <param name="assessmentResourceActivity">The activity.</param>
        /// <param name="createAssessmentResourceActivityInteractionViewModel">The interaction creation view model containing the selected answers.</param>
        /// <param name="questionBlock">The targeted question.</param>
        /// <returns>The task that evaluates to true if the user is allowed to create this interaction.</returns>
        private async Task<bool> UserCanSubmitAssessmentAnswers(int userId, bool answerInOrder, AssessmentResourceActivity assessmentResourceActivity, CreateAssessmentResourceActivityInteractionViewModel createAssessmentResourceActivityInteractionViewModel, QuestionBlock questionBlock)
        {
            return await UserAnswerSelectionIsValid(userId, answerInOrder, assessmentResourceActivity, createAssessmentResourceActivityInteractionViewModel) &&
                !await UserHasAlreadyAnsweredQuestion(userId, assessmentResourceActivity.Id, questionBlock.Id) &&
                !AssessmentResourceActivityIsFinished(assessmentResourceActivity);
        }

        /// <summary>
        /// Maps a list of selected answer orders to answer IDs.
        /// </summary>
        /// <param name="availableAnswers">The collection of all available answers.</param>
        /// <param name="selectedIndices">The selected answers, given by their orders.</param>
        /// <returns>The mapped list.</returns>
        private ICollection<AssessmentResourceActivityInteractionAnswer> MapAnswerSelectionsToIds(ICollection<QuestionAnswer> availableAnswers, ICollection<int> selectedIndices)
        {
            // Sort the answers of the target question to make sure they match the front-end order
            var orderedAnswers = availableAnswers.OrderBy(answer => answer.Order).ToList();
            return selectedIndices.Distinct().Select(answerNumber => new AssessmentResourceActivityInteractionAnswer()
            {
                QuestionAnswerId = orderedAnswers[answerNumber].Id,
            }).ToList();
        }

        private ICollection<AssessmentResourceActivityInteractionAnswer> MapMatchAnswerSelectionsToIds(
            ICollection<QuestionAnswer> availableAnswers,
            ICollection<int> answerStatus)
        {
            var orderedAnswers = availableAnswers.OrderBy(answer => answer.Order).ToList();
            return answerStatus.Select((q, i) => q != -1
                ? new AssessmentResourceActivityInteractionAnswer()
                {
                    QuestionAnswerId = orderedAnswers[i].Id,
                    MatchedQuestionAnswerId = orderedAnswers[q].Id,
                }
                : null).Where(a => a != null).ToList();
        }

        /// <summary>
        /// Determines whether or not a user has already answered a question in the given assessment resource activity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="assessmentResourceActivityId">The id of the assessment resource activity.</param>
        /// <param name="questionBlockId">The id of the question block.</param>
        /// <returns>The task that resolves to true if the user has already answered the question.</returns>
        private async Task<bool> UserHasAlreadyAnsweredQuestion(int userId, int assessmentResourceActivityId, int questionBlockId)
        {
            return await assessmentResourceActivityInteractionRepository
                    .GetInteractionForQuestion(userId, assessmentResourceActivityId, questionBlockId) != null;
        }

        /// <summary>
        /// Determines whether or not a user has used up all attempts for a resource version.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionId">The id of the resource version.</param>
        /// <returns>The task that resolves to true if the user has used up all attempts for a resource version.</returns>
        private async Task<bool> UserHasUsedAllAttempts(int userId, int resourceVersionId)
        {
            var numberAttempts = resourceActivityRepository.GetByUserId(userId)
               .Where(x => x.ResourceVersionId == resourceVersionId)
               .SelectMany(x => x.AssessmentResourceActivity)
               .OrderByDescending(a => a.CreateDate)
               .ToList().Count();
            var assessmentResourceActivity = await assessmentResourceVersionRepository.GetByResourceVersionIdAsync(resourceVersionId);
            var maxAttempts = assessmentResourceActivity.MaximumAttempts;
            return numberAttempts >= maxAttempts;
        }

        /// <summary>
        /// Determines whether an assessment resource activity is finished.
        /// </summary>
        /// <param name="activity">The assessment resource activity.</param>
        /// <returns>The boolean.</returns>
        private bool AssessmentResourceActivityIsFinished(AssessmentResourceActivity activity)
        {
            return activity.Score != null;
        }

        /// <summary>
        /// Determines whether a user's answer selection is valid.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="answerInOrder">The answerInOrder setting of the activity.</param>
        /// <param name="assessmentResourceActivity">The assessment resource activity.</param>
        /// <param name="createAssessmentResourceActivityInteractionViewModel">The interaction creation view model.</param>
        /// <returns>The task that resolves to true if the selection is valid.</returns>
        private async Task<bool> UserAnswerSelectionIsValid(int userId, bool answerInOrder, AssessmentResourceActivity assessmentResourceActivity, CreateAssessmentResourceActivityInteractionViewModel createAssessmentResourceActivityInteractionViewModel)
        {
            return !answerInOrder || await UserHasAnsweredQuestionsUpTo(userId, assessmentResourceActivity.Id, createAssessmentResourceActivityInteractionViewModel.QuestionNumber);
        }

        /// <summary>
        /// Validates whether the user has answered all previous questions.
        /// </summary>
        /// <param name="userId">The Id of the user.</param>
        /// <param name="assessmentResourceActivityId">The Id for the assessment resource activity.</param>
        /// <param name="questionNumber">The number of the question the user is trying to submit the answers to.</param>
        /// <returns>Returns true if the user has answered all previous questions.</returns>
        private async Task<bool> UserHasAnsweredQuestionsUpTo(int userId, int assessmentResourceActivityId, int questionNumber)
        {
            var assessmentResourceActivity = await assessmentResourceActivityRepository.GetByIdAsync(assessmentResourceActivityId);

            if (assessmentResourceActivity == null)
            {
                return false;
            }

            var assessmentResourceActivityInteractions = assessmentResourceActivityInteractionRepository.GetAll()
                .Where(i => i.AssessmentResourceActivityId == assessmentResourceActivityId).ToList();
            return assessmentResourceActivityInteractions.Count >= questionNumber && userId == assessmentResourceActivity.CreateUserId;
        }

        /// <summary>
        /// Determines whether a resource activity is completed.
        /// </summary>
        /// <param name="resourceActivityId">The resource activity id.</param>
        /// <returns>True if the resource activity has been completed.</returns>
        private async Task<bool> ResourceActivityIsCompleted(int resourceActivityId)
        {
            return await resourceActivityRepository
                .GetAll()
                .Where(ra => ra.LaunchResourceActivityId == resourceActivityId && ra.ActivityStatusId == (int)ActivityStatusEnum.Completed)
                .FirstOrDefaultAsync() != null;
        }

        private async Task<Dictionary<int, IEnumerable<int>>> GetUserAssessmentAnswers(int assessmentResourceActivityId)
        {
            var assessmentInteractions = await assessmentResourceActivityInteractionRepository.GetInteractionsForAssessmentResourceActivity(
                assessmentResourceActivityId);
            var assessmentInteractionsDictionary = new Dictionary<int, IEnumerable<int>>();
            foreach (var assessmentInteraction in assessmentInteractions)
            {
                var answers = this.GetQuestionAnswers(assessmentInteraction);

                assessmentInteractionsDictionary.Add(
                    assessmentInteraction.QuestionBlock.Block.Order, answers);
            }

            return assessmentInteractionsDictionary;
        }

        private IEnumerable<int> GetQuestionAnswers(
            AssessmentResourceActivityInteraction assessmentResourceActivityInteraction)
        {
            switch (assessmentResourceActivityInteraction.QuestionBlock.QuestionType)
            {
                case QuestionTypeEnum.MatchGame:
                    return GetMatchQuestionAnswers(assessmentResourceActivityInteraction);
                default:
                    return assessmentResourceActivityInteraction.Answers.Select(y => y.QuestionAnswer.Order);
            }
        }

        private IEnumerable<int> GetMatchQuestionAnswers(
            AssessmentResourceActivityInteraction assessmentResourceActivityInteraction)
        {
            var matchAnswersCount =
                assessmentResourceActivityInteraction.QuestionBlock.Answers.Count;
            return Enumerable.Range(0, matchAnswersCount).Select(i =>
                assessmentResourceActivityInteraction.Answers.FirstOrDefault(a => a.QuestionAnswer.Order == i)?.MatchedQuestionAnswer.Order ?? -1);
        }
    }
}