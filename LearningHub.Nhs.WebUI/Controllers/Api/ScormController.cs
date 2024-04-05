namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource.Activity;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.LearningSessions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="ScormController" />.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ScormController : BaseApiController
    {
        private readonly IUserService userService;
        private readonly IActivityService activityService;
        private readonly IResourceService resourceService;
        private readonly IOptions<Settings> settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormController"/> class.
        /// </summary>
        /// <param name="userService">User service.</param>
        /// <param name="activityService">Activity service.</param>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        public ScormController(
            IUserService userService,
            IActivityService activityService,
            IResourceService resourceService,
            ILogger<ScormController> logger,
            IOptions<Settings> settings)
            : base(logger)
        {
            this.userService = userService;
            this.activityService = activityService;
            this.resourceService = resourceService;
            this.settings = settings;
        }

        /// <summary>
        /// The LMSInitialise.
        /// Returns a new SCO object if possible, otherwise raises an Exception.
        /// </summary>
        /// <param name="resourceReferenceId">resourceReferenceId.</param>
        /// <returns>bool.</returns>
        [HttpPost("LMSInitialise/{resourceReferenceId}")]
        public async Task<ActionResult> LMSInitialise(int resourceReferenceId)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }

            var sco = await this.GetSco(resourceReferenceId);

            if (sco != null)
            {
                this.Logger.LogDebug("LMSInitialise: " + sco.InstanceId.ToString());
            }

            return this.Ok(sco);
        }

        /// <summary>
        /// The LMSCommitObj.
        /// Called using AJAX.
        /// </summary>
        /// <param name="scoObject">sco.</param>
        /// <returns>bool.</returns>
        [HttpPost]
        [Route("LMSCommitObj")]
        public async Task<bool> LMSCommitObj([FromBody] SCO scoObject)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return false;
            }

            this.Logger.LogDebug("LMSCommitObj: " + scoObject.InstanceId.ToString());

            return await this.Commit(scoObject);
        }

        /// <summary>
        /// The LMSCommit.
        /// Called using SendBeacon.
        /// </summary>
        /// <param name="formData">formData.</param>
        /// <returns>bool.</returns>
        [HttpPost]
        [Route("LMSCommit")]
        public async Task<bool> LMSCommit(IFormCollection formData)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return false;
            }

            string sco = formData["sco"];
            SCO scoObject = this.PopulateSCO(sco);

            this.Logger.LogDebug("LMSCommit: " + scoObject.InstanceId.ToString());

            return await this.Commit(scoObject);
        }

        /// <summary>
        /// The LMSFinishObj.
        /// </summary>
        /// <param name="sco">sco.</param>
        /// <returns>bool.</returns>
        [HttpPost]
        [Route("LMSFinishObj")]
        public async Task<bool> LMSFinishObj([FromBody] SCO sco)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return false;
            }

            this.Logger.LogDebug("LMSFinishObj: " + sco.InstanceId.ToString());

            return await this.Finish(sco);
        }

        /// <summary>
        /// The LMSFinish.
        /// </summary>
        /// <param name="formData">formData.</param>
        /// <returns>bool.</returns>
        [HttpPost]
        [Route("LMSFinish")]
        public async Task<bool> LMSFinish(IFormCollection formData)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return false;
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));
            string sco = formData["sco"];
            SCO scoObject = this.PopulateSCO(sco);

            this.Logger.LogDebug("LMSFinish: " + scoObject.InstanceId.ToString());

            return await this.Finish(scoObject);
        }

        private async Task<bool> Finish(SCO scoObject)
        {
            try
            {
                // Check that SCO content is active
                var activeContentList = this.userService.GetActiveContentAsync().Result;
                var activeContent = activeContentList.FirstOrDefault(ac => ac.ScormActivityId == scoObject.InstanceId);

                if (activeContent == null)
                {
                    throw new Exception($"User does not have ActiveContent for ScormActivityId={scoObject.InstanceId}");
                }

                // Note: Commented out code below is implementation of Scorm 1.2 spec for core.cmi.lesson_status "Additional Behaviour Requirements"
                // 29.06.21 Decision to not include "Additional Behaviour Requirements" due to deficiency in Scorm 1.2 spec;
                // - On initial launch of session it is possible to identify that a SCO has not updated lesson_status because the returned lesson_status
                //   will be "Not attempted". However on subsequent session launches for the same session it will not be possible to identify that the
                //   SCO has not updated lesson_status because it will have been initialised with "incomplete".  The "Additional Behaviour Requirements" can
                //   therefore only be reliably invoked for the first Session activity.
                // Note: "Additional Behaviour Requirements" have never been implemented in e-LfH due to SCO being initialised with blank value for cmi.core.credit.
                ////if (scoObject.LessonStatus == ScormLessionStatus.NotAttempted)
                ////{
                ////    var rv = await this.resourceService.GetResourceVersionExtendedAsync(activeContent.ResourceVersionId);
                ////    if (scoObject.Credit == "credit" // Currently always set to "credit" on LMSInitialise.
                ////        && rv.ScormDetails.ScormManifest.MasteryScore.HasValue
                ////        && rv.ScormDetails.ScormManifest.MasteryScore > 0)
                ////    {
                ////        if (scoObject.ScoreRaw.HasValue && scoObject.ScoreRaw >= rv.ScormDetails.ScormManifest.MasteryScore)
                ////        {
                ////            scoObject.LessonStatusId = ScormLessionStatus.ActivityStatusId(ScormLessionStatus.Passed);
                ////        }
                ////        else
                ////        {
                ////            scoObject.LessonStatusId = ScormLessionStatus.ActivityStatusId(ScormLessionStatus.Failed);
                ////        }
                ////    }
                ////    else
                ////    {
                ////        scoObject.LessonStatusId = ScormLessionStatus.ActivityStatusId(ScormLessionStatus.Completed);
                ////    }
                ////}
                ////else
                ////{
                ////    scoObject.LessonStatusId = ScormLessionStatus.ActivityStatusId(scoObject.LessonStatus);
                ////}

                // Require that Lesson Status be provided by the SCO, otherwise all activity retains "incomplete" status.
                if (scoObject.LessonStatus == ScormLessionStatus.NotAttempted)
                {
                    scoObject.LessonStatusId = ScormLessionStatus.ActivityStatusId(ScormLessionStatus.Incomplete);
                }

                // Commit
                var retVal = await this.Commit(scoObject);

                // Create Activity Complete event. (TODO process event using service bus queue - perform any longer running async status re-calc).
                await this.activityService.CompleteScormActivity(scoObject);

                return retVal;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Finish Error: InstanceId={scoObject.InstanceId}, Exception: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> Commit(SCO scoObject)
        {
            try
            {
                var activeContent = this.userService.GetActiveContentAsync().Result;
                if (!activeContent.Any(ac => ac.ScormActivityId == scoObject.InstanceId))
                {
                    throw new Exception($"User does not have ActiveContent for ScormActivityId={scoObject.InstanceId}");
                }

                string validationResult = string.Empty;
                if (!this.ValidateSCO(scoObject, false, ref validationResult))
                {
                    throw new Exception(validationResult);
                }

                // Set DurationSeconds and LessonStatusId
                int activitySeconds = this.TimeToSeconds(scoObject.SessionTime);
                if (this.settings.Value.ScormActivityDurationLimitHours > 0)
                {
                    int activitySecondsLimit = (this.settings.Value.ScormActivityDurationLimitHours * 3600) - 1;

                    if (activitySeconds > activitySecondsLimit)
                    {
                        activitySeconds = activitySecondsLimit;
                    }
                }

                scoObject.DurationSeconds = activitySeconds;

                // Lesson status is always provided by the SCO.
                scoObject.LessonStatusId = ScormLessionStatus.ActivityStatusId(scoObject.LessonStatus);

                // Persist update.
                await this.activityService.UpdateScormActivityAsync(scoObject);
                if (scoObject.LessonStatusId == 3 || scoObject.LessonStatusId == 5)
                {
                    await this.activityService.CompleteScormActivity(scoObject);
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Commit Error: InstanceId={scoObject.InstanceId}, Exception: {ex.Message}");
                throw;
            }
        }

        private async Task<SCO> GetSco(int resourceReferenceId)
        {
            var currentUser = await this.userService.GetUserByUserIdAsync(this.CurrentUserId);
            if (currentUser == null)
            {
                throw new Exception("GetSco - unable to retrieve Current User.");
            }

            // Launch Scorm Activity (create Launch event and new ScormActivity record).
            var launchScormActivityViewModel = new LaunchScormActivityViewModel
            {
                ResourceReferenceId = resourceReferenceId,
            };

            var scormActivity = await this.activityService.LaunchScormActivityAsync(launchScormActivityViewModel);

            if (scormActivity == null)
            {
                throw new Exception("GetSco - unable to create new ScormActivity record.");
            }

            // Initialise SCO object from ScormActivity record
            SCO sco = new SCO
            {
                ScoreChildren = ScormContansts.ScoreChildren,
                Entry = ScormContansts.Entry,
                Version = ScormContansts.Version,
                Children = ScormContansts.Children,
                LessonMode = ScormContansts.LessonMode,
                StudentDataChildren = ScormContansts.StudentDataChildren,
                StudentPreferenceChildren = ScormContansts.StudentPreferenceChildren,
                StudentPreferenceAudio = ScormContansts.StudentPreferenceAudio,
                StudentPreferenceLanguage = ScormContansts.StudentPreferenceLanguage,
                StudentPreferenceSpeed = ScormContansts.StudentPreferenceSpeed,
                StudentPreferenceText = ScormContansts.StudentPreferenceText,
                InteractionsChildren = ScormContansts.InteractionsChildren,
                ObjectivesChildren = ScormContansts.ObjectivesChildren,
                ObjectivesScoreChildren = ScormContansts.ObjectivesScoreChildren,
                LessonStatus = scormActivity.ClonedFromScormActivityId.HasValue ? ScormLessionStatus.Incomplete : ScormLessionStatus.NotAttempted,
                InstanceId = scormActivity.InstanceId,
                ScoreMin = scormActivity.ScoreMin,
                ScoreMax = scormActivity.ScoreMax,
                TotalTime = string.IsNullOrEmpty(scormActivity.TotalTime) ? string.Empty : scormActivity.TotalTime,
                SessionTime = string.Empty,
                StudentId = currentUser.Id.ToString(),
                StudentName = currentUser.LastName + ", " + currentUser.FirstName,
                Exit = scormActivity.Exit,
                ScormActivityInteraction = scormActivity.ScormActivityInteraction,
                ScormActivityObjective = scormActivity.ScormActivityObjective,
            };

            // Set fields for which it is the responsibility of the LMS to specify.
            var activeContentList = await this.userService.GetActiveContentAsync();
            var activeContent = activeContentList.FirstOrDefault();

            if (activeContent == null)
            {
                throw new Exception($"User did not acquire ActiveContent for ResourceReferenceId={resourceReferenceId}");
            }

            var rv = await this.resourceService.GetResourceVersionExtendedAsync(activeContent.ResourceVersionId);

            // 1. StudentDataMasteryScore, StudentDataMaxTimeAllowed, StudentDataTimeLimitAction, Credit
            sco.Credit = "credit";
            sco.CommentsFromLms = ScormContansts.CommentsFromLMS;
            sco.LaunchData = rv.ScormDetails.ScormManifest.LaunchData ?? ScormContansts.LaunchData;
            sco.StudentDataMasteryScore = rv.ScormDetails.ScormManifest.MasteryScore ?? ScormContansts.StudentDataMasteryScore;
            sco.StudentDataMaxTimeAllowed = rv.ScormDetails.ScormManifest.MaxTimeAllowed ?? ScormContansts.StudentDataMaxTimeAllowed;
            sco.StudentDataTimeLimitAction = rv.ScormDetails.ScormManifest.TimeLimitAction ?? ScormContansts.StudentDataTimeLimitAction;

            // 2. LessonLocation, ScoreRaw, SuspendData, Entry
            // If previous Scorm Activity was "incomplete" then offer pick-up from previous lesson location.
            // If previous Scorm Activity was completed / passed / failed then re-start from the beginning with status "incomplete".
            // If no previous Scorm Activity then start from the beginning with status "not attempted".
            if (scormActivity.ClonedFromScormActivityId.HasValue)
            {
                if (scormActivity.LessonStatusId.HasValue && scormActivity.LessonStatusId == ScormLessionStatus.ActivityStatusId("incomplete"))
                {
                    // check scorm suspend data to be cleared
                    if (await this.activityService.CheckSuspendDataToBeCleared(scormActivity.ClonedFromScormActivityId.Value, activeContent.ResourceVersionId))
                    {
                        sco.LessonLocation = string.Empty;
                        sco.ScoreRaw = null;
                        sco.SuspendData = string.Empty;
                    }
                    else
                    {
                        sco.LessonLocation = string.IsNullOrEmpty(scormActivity.LessonLocation) ? string.Empty : scormActivity.LessonLocation;
                        sco.ScoreRaw = scormActivity.ScoreRaw;
                        sco.SuspendData = string.IsNullOrEmpty(scormActivity.SuspendData) ? string.Empty : scormActivity.SuspendData;
                    }
                }
                else
                {
                    sco.LessonLocation = string.Empty;
                    sco.ScoreRaw = null;
                    sco.SuspendData = string.Empty;
                }

                if (scormActivity.Exit == ScormExitValue.Suspend)
                {
                    sco.Entry = sco.Entry = ScormEntryValue.Resume; // cmi.core.entry to "resume" if the SCO set cmi.core.exit to "suspend".
                }
                else
                {
                    sco.Entry = string.Empty;
                }
            }
            else
            {
                sco.LessonLocation = string.Empty;
                sco.ScoreRaw = null;
                sco.SuspendData = string.Empty;
                sco.Entry = ScormEntryValue.AbInitio;
            }

            return sco;
        }

        private SCO PopulateSCO(string sco)
        {
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(sco);

            SCO scoObject = new SCO
            {
                InstanceId = (int)jObject["instanceId"],
                Version = (string)jObject["version"],                     // Read Only
                Children = (string)jObject["children"],                   // Read Only
                StudentId = (string)jObject["studentId"],                 // Read Only
                StudentName = (string)jObject["studentName"],             // Read Only
                LessonLocation = (string)jObject["lessonLocation"],
                LessonStatus = (string)jObject["lessonStatus"],
                Credit = (string)jObject["credit"],
                Exit = (string)jObject["exit"],
                Entry = (string)jObject["entry"],
                SessionTime = (string)jObject["sessionTime"],
                TotalTime = (string)jObject["totalTime"],
                SuspendData = (string)jObject["suspendData"],
                LaunchData = (string)jObject["launchData"],
                ScoreChildren = (string)jObject["scoreChildren"],
            };

            decimal scoreValue = 0;
            string scoreRaw = (string)jObject["scoreRaw"];
            if (scoreRaw == string.Empty)
            {
                scoObject.ScoreRaw = null;
            }
            else if (decimal.TryParse(scoreRaw, out scoreValue))
            {
                scoObject.ScoreRaw = scoreValue;
            }

            string scoreMin = (string)jObject["scoreMin"];
            if (scoreMin == string.Empty)
            {
                scoObject.ScoreMin = null;
            }
            else if (decimal.TryParse(scoreMin, out scoreValue))
            {
                scoObject.ScoreMin = scoreValue;
            }

            string scoreMax = (string)jObject["scoreMax"];
            if (scoreMax == string.Empty)
            {
                scoObject.ScoreMax = null;
            }
            else if (decimal.TryParse(scoreMax, out scoreValue))
            {
                scoObject.ScoreMax = scoreValue;
            }

            scoObject.InteractionsChildren = (string)jObject["interactionsChildren"];  // Read Only
            scoObject.ObjectivesChildren = (string)jObject["objectivesChildren"];      // Read Only

            foreach (var item in jObject["interactions"])
            {
                Newtonsoft.Json.Linq.JObject jObject_Interaction = Newtonsoft.Json.Linq.JObject.Parse(item.ToString());
                ScormActivityInteractionViewModel interaction = new ScormActivityInteractionViewModel();
                interaction.ScormActivityId = scoObject.InstanceId;
                interaction.ScormActivityInteractionId = (int)jObject_Interaction["scormActivityInteractionId"];
                interaction.InteractionId = (string)jObject_Interaction["id"];
                interaction.SequenceNumber = (int)jObject_Interaction["sequenceNumber"];
                interaction.Type = (string)jObject_Interaction["type"];
                interaction.Result = (string)jObject_Interaction["result"];
                decimal weighting = 0;
                decimal.TryParse((string)jObject["weighting"], out weighting);
                interaction.Weighting = weighting;
                interaction.Latency = (string)jObject_Interaction["latency"];
                interaction.StudentResponse = (string)jObject_Interaction["studentResponse"];

                var objectives = jObject_Interaction["objectives"];
                var index = 0;
                foreach (var objective in objectives)
                {
                    Newtonsoft.Json.Linq.JObject jObject_Objective = Newtonsoft.Json.Linq.JObject.Parse(objective.ToString());
                    interaction.ScormActivityInteractionObjective.Add(new ScormActivityInteractionObjectiveViewModel { Index = index, ObjectiveId = (string)jObject_Objective["id"] });
                    index++;
                }

                var correctResponses = jObject_Interaction["correctResponses"];
                index = 0;
                foreach (var correctResponse in correctResponses)
                {
                    Newtonsoft.Json.Linq.JObject jObject_CorrectResponse = Newtonsoft.Json.Linq.JObject.Parse(correctResponse.ToString());
                    interaction.ScormActivityInteractionCorrectResponse.Add(new ScormActivityInteractionCorrectResponseViewModel { Index = index, Pattern = (string)jObject_CorrectResponse["pattern"] });
                    index++;
                }

                scoObject.ScormActivityInteraction.Add(interaction);
            }

            foreach (var item in jObject["objectives"])
            {
                Newtonsoft.Json.Linq.JObject jObject_Objective = Newtonsoft.Json.Linq.JObject.Parse(item.ToString());
                ScormActivityObjectiveViewModel objective = new ScormActivityObjectiveViewModel();
                objective.ScormActivityId = scoObject.InstanceId;
                objective.ScormActivityObjectiveId = (int)jObject_Objective["scormActivityObjectiveId"];
                objective.ObjectiveId = (string)jObject_Objective["id"];
                objective.SequenceNumber = (int)jObject_Objective["sequenceNumber"];
                objective.ScoreRaw = (string)jObject_Objective["scoreRaw"];
                objective.ScoreMax = (string)jObject_Objective["scoreMax"];
                objective.ScoreMin = (string)jObject_Objective["scoreMin"];
                objective.Status = (string)jObject_Objective["status"];
                scoObject.ScormActivityObjective.Add(objective);
            }

            return scoObject;
        }

        private bool ValidateSCO(SCO scoObject, bool sessionTimeMandatory, ref string validationMessage)
        {
            try
            {
                bool retVal = true;

                // Validate "SessionTime"
                // Validate "hours" supplied
                // Note - Adapt Sessions supply empty string rather than cmi time on LMS Commit
                // The flag "sessionTimeMandatory" allows LMS Commit to permit a blank/null SessionTime
                // whilst allowing LMS Finish to require a value.
                if (!string.IsNullOrEmpty(scoObject.SessionTime))
                {
                    int hours = int.Parse(scoObject.SessionTime.Substring(0, scoObject.SessionTime.IndexOf(":")));
                    if (hours > 9999) //// SCORM standard permits max 4 chars for the hours field
                    {
                        retVal = false;
                        validationMessage = "Invalid SCO object supplied. SessionTime Hours cannot exceed 9999.";
                    }

                    // Ensure no "-" symbol is present
                    if (scoObject.SessionTime.IndexOf("-") > -1)
                    {
                        retVal = false;
                        validationMessage = "Invalid SCO object supplied. SessionTime cannot contain negative elements.";
                    }
                }

                if (sessionTimeMandatory && string.IsNullOrEmpty(scoObject.SessionTime))
                {
                    retVal = false;
                    validationMessage = "Invalid SCO object supplied. SessionTime is required.";
                }

                return retVal;
            }
            catch (Exception ex)
            {
                this.Logger.LogDebug($"ValidateSCO Exception: {ex.Message}");
                throw new Exception("Error validating supplied SCO object", ex);
            }
        }

        private int TimeToSeconds(string sessionTime)
        {
            if (sessionTime == null || sessionTime == string.Empty)
            {
                return 0;
            }
            else
            {
                int pos1 = sessionTime.IndexOf(':');
                int pos2 = sessionTime.IndexOf(':', pos1 + 1);
                int pos3 = sessionTime.IndexOf('.', pos2 + 1);

                if (pos3 == 0 || pos3 == -1)
                {
                    pos3 = sessionTime.Length;
                }

                string hours = sessionTime.Substring(1, pos1 - 1);
                string minutes = sessionTime.Substring(pos1 + 1, pos2 - pos1 - 1);
                string seconds = sessionTime.Substring(pos2 + 1, pos3 - pos2 - 1);

                int intHours = int.Parse(hours);
                int intMinutes = int.Parse(minutes);
                int intSeconds = int.Parse(seconds);

                int totalSeconds = (intHours * 60 * 60) + (intMinutes * 60) + intSeconds;

                return totalSeconds;
            }
        }
    }
}