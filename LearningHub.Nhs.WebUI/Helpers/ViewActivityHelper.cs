namespace LearningHub.Nhs.WebUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.WebUI.Models;

    /// <summary>
    /// Defines the <see cref="ViewActivityHelper" />.
    /// </summary>
    public static class ViewActivityHelper
    {
        /// <summary>
        /// The GetResourceTypeText.
        /// </summary>
        /// <param name="activityDetailedItemViewModel">The activityDetailedItemViewModel.</param>
        /// <returns>The .</returns>
        public static string GetResourceTypeText(this ActivityDetailedItemViewModel activityDetailedItemViewModel)
        {
            string typeText = string.Empty;
            switch (activityDetailedItemViewModel.ResourceType)
            {
                case ResourceTypeEnum.Assessment:
                    typeText = "Assessment";
                    break;
                case ResourceTypeEnum.Article:
                    typeText = "Article";
                    break;
                case ResourceTypeEnum.Audio:
                    typeText = "Audio";
                    break;
                case ResourceTypeEnum.Embedded:
                    typeText = "Embedded";
                    break;
                case ResourceTypeEnum.Equipment:
                    typeText = "Equipment";
                    break;
                case ResourceTypeEnum.GenericFile:
                    typeText = "File";
                    break;
                case ResourceTypeEnum.Image:
                    typeText = "Image";
                    break;
                case ResourceTypeEnum.Scorm:
                    typeText = "elearning";
                    break;
                case ResourceTypeEnum.Video:
                    typeText = "Video";
                    break;
                case ResourceTypeEnum.WebLink:
                    typeText = "Web link";
                    break;
                case ResourceTypeEnum.Case:
                    typeText = "Case";
                    break;
                case ResourceTypeEnum.Html:
                    typeText = "HTML";
                    break;
                default:
                    typeText = string.Empty;
                    break;
            }

            if (activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Video || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Audio)
            {
                var duration = GetDurationText(activityDetailedItemViewModel.ResourceDurationMilliseconds);
                typeText = $"{duration} {typeText.ToLower()}";
            }

            return typeText;
        }

        /// <summary>
        /// GetResourceTypeVerb.
        /// </summary>
        /// <param name="activityDetailedItemViewModel">The activityDetailedItemViewModel.</param>
        /// <returns>The .</returns>
        public static string GetResourceTypeVerb(this ActivityDetailedItemViewModel activityDetailedItemViewModel)
        {
            string result = string.Empty;
            switch (activityDetailedItemViewModel.ResourceType)
            {
                case ResourceTypeEnum.Article:
                    return "Read";
                case ResourceTypeEnum.Audio:
                    return "Played " + GetDurationText(activityDetailedItemViewModel.ActivityDurationSeconds * 1000);
                case ResourceTypeEnum.Embedded:
                    return string.Empty;
                case ResourceTypeEnum.Equipment:
                    return "Used equipment/visited facility";
                case ResourceTypeEnum.GenericFile:
                    return "Downloaded";
                case ResourceTypeEnum.Image:
                    if (activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Downloaded)
                    {
                        return "Downloaded";
                    }
                    else
                    {
                        return "Viewed";
                    }

                case ResourceTypeEnum.Scorm:
                    if (activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Downloaded)
                    {
                        return "Downloaded";
                    }
                    else
                    {
                        return "Accessed";
                    }

                case ResourceTypeEnum.Video:
                    return "Played " + GetDurationText(activityDetailedItemViewModel.ActivityDurationSeconds * 1000);
                case ResourceTypeEnum.WebLink:
                    return "Visited";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// GetActivityStatusDisplayText.
        /// </summary>
        /// <param name="activityDetailedItemViewModel">The activityDetailedItemViewModel.</param>
        /// <returns>The .</returns>
        public static string GetActivityStatusDisplayText(this ActivityDetailedItemViewModel activityDetailedItemViewModel)
        {
            if (activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Launched
                     && (activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Article
                         || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.WebLink
                         || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Image
                         || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Html
                         || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Case))
            {
                return "Completed";
            }
            else if (activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Launched
                && (activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.GenericFile))
            {
                return "Downloaded";
            }
            else if (activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Assessment)
            {
                if (activityDetailedItemViewModel.Complete)
                {
                    return activityDetailedItemViewModel.ScorePercentage >= activityDetailedItemViewModel.AssessmentDetails.PassMark ? "Passed" : "Failed";
                }
                else
                {
                    return activityDetailedItemViewModel.ScorePercentage >= activityDetailedItemViewModel.AssessmentDetails.PassMark ? "Passed" : "Incomplete";
                }
            }
            else
            {
                return GetActivityStatusText(activityDetailedItemViewModel);
            }
        }

        /// <summary>
        /// GetActivityStatus.
        /// </summary>
        /// <param name="activityDetailedItemViewModel">The activityDetailedItemViewModel.</param>
        /// <returns>The <see cref="ActivityStatusEnum"/>.</returns>
        public static ActivityStatusEnum GetActivityStatus(this ActivityDetailedItemViewModel activityDetailedItemViewModel)
        {
            if (activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Launched
                && (activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Article
                    || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.WebLink
                    || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Image
                    || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Html
                    || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Case))
            {
                return ActivityStatusEnum.Completed;
            }
            else if (activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Launched
                && (activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.GenericFile))
            {
                return ActivityStatusEnum.Downloaded;
            }
            else if (activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Assessment)
            {
                if (activityDetailedItemViewModel.Complete)
                {
                    return activityDetailedItemViewModel.ScorePercentage >= activityDetailedItemViewModel.AssessmentDetails.PassMark ? ActivityStatusEnum.Passed : ActivityStatusEnum.Failed;
                }
                else
                {
                    return activityDetailedItemViewModel.ScorePercentage >= activityDetailedItemViewModel.AssessmentDetails.PassMark ? ActivityStatusEnum.Passed : ActivityStatusEnum.InProgress;
                }
            }
            else
            {
                return activityDetailedItemViewModel.ActivityStatus;
            }
        }

        /// <summary>
        /// GetActivityStatusText.
        /// </summary>
        /// <param name="activityDetailedItemViewModel">The activityDetailedItemViewModel.</param>
        /// <returns>The .</returns>
        public static string GetActivityStatusText(this ActivityDetailedItemViewModel activityDetailedItemViewModel)
        {
            switch (activityDetailedItemViewModel.ActivityStatus)
            {
                case ActivityStatusEnum.Launched:
                    return "Launched";
                case ActivityStatusEnum.InProgress:
                    return "Incomplete";
                case ActivityStatusEnum.Completed:
                    return "Completed";
                case ActivityStatusEnum.Passed:
                    return "Passed";
                case ActivityStatusEnum.Failed:
                    return "Failed";
                case ActivityStatusEnum.Downloaded:
                    return "Downloaded";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// CanShowScore.
        /// </summary>
        /// <param name="activityDetailedItemViewModel">The activityDetailedItemViewModel.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool CanShowScore(this ActivityDetailedItemViewModel activityDetailedItemViewModel)
        {
            if ((activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Scorm && (activityDetailedItemViewModel.MasteryScore > 0 || activityDetailedItemViewModel.MasteryScore == null) && ((activityDetailedItemViewModel.ScorePercentage > 0 && activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Passed) || activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Failed)) || ((activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Assessment || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Case) && activityDetailedItemViewModel.Complete))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// CanViewPercentage.
        /// </summary>
        /// <param name="activityDetailedItemViewModel">The activityDetailedItemViewModel.</param>
        /// <returns>The <see cref="bool"/>bool.</returns>
        public static bool CanViewPercentage(this ActivityDetailedItemViewModel activityDetailedItemViewModel)
        {
            if (((activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Video || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Audio) && activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.InProgress) || (activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Assessment && activityDetailedItemViewModel.Complete == false))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// CanViewProgress.
        /// </summary>
        /// <param name="activityDetailedItemViewModel">The activityDetailedItemViewModel.</param>
        /// <returns>The <see cref="bool"/>bool.</returns>
        public static bool CanViewProgress(this ActivityDetailedItemViewModel activityDetailedItemViewModel)
        {
            if ((activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Video || activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Audio) && activityDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.InProgress)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// GetDurationText.
        /// </summary>
        /// <param name="durationInMilliseconds">The durationInMilliseconds<see cref="int"/>.</param>
        /// <returns>The string.</returns>
        public static string GetDurationText(this int durationInMilliseconds)
        {
            string duration = string.Empty;

            if (durationInMilliseconds > 0)
            {
                // Azure media player rounds duration to nearest second. e.g. 8:59.88 becomes 9:00. LH needs to match.
                // Moment.js can't round to the nearest second, so do that first with the raw millisecond value.
                TimeSpan initialTime = TimeSpan.FromMilliseconds(durationInMilliseconds);
                var t = TimeSpan.FromSeconds(Math.Round(initialTime.TotalSeconds, 3));
                if (t.Hours > 0)
                {
                    if (t.Minutes == 0)
                    {
                        duration = $"{t.Hours}hr";
                    }
                    else
                    {
                        duration = $"{t.Hours}hr {t.Minutes}min";
                        duration = duration.Replace("0hr", string.Empty);
                    }
                }
                else
                {
                    if (t.Seconds == 0)
                    {
                        duration = $"{t.Minutes}min";
                    }
                    else
                    {
                        duration = $"{t.Minutes}min {t.Seconds}sec";
                        duration = duration.Replace("0min", string.Empty);
                    }
                }
            }

            return duration;
        }

        /// <summary>
        /// The GetActivityParameters.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref=""/>.</returns>
        public static Dictionary<string, string> GetActivityParameters(object model)
        {
            var routeData = model.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(model)?.ToString());
            routeData.Remove("MostRecentResources");
            routeData.Remove("Activities");
            routeData.Remove("TotalCount");
            routeData.Remove("MyLearningPaging");
            routeData.Remove("Skip");
            routeData.Remove("Take");

            if (routeData.ContainsKey("StartDate") && !string.IsNullOrWhiteSpace(routeData["StartDate"]))
            {
                routeData["StartDate"] = DateTime.Parse(routeData["StartDate"]).ToString("o");
            }

            if (routeData.ContainsKey("EndDate") && !string.IsNullOrWhiteSpace(routeData["EndDate"]))
            {
                routeData["EndDate"] = DateTime.Parse(routeData["EndDate"].ToString()).ToString("o");
            }

            return routeData.Where(entry => !string.IsNullOrWhiteSpace(entry.Value)).ToDictionary(e => e.Key, e => e.Value);
        }

        /// <summary>
        /// CanDownloadCertificate.
        /// </summary>
        /// <param name="activityDetailedItemViewModel">The activityDetailedItemViewModel.</param>
        /// <returns>The <see cref="bool"/>bool.</returns>
        public static bool CanDownloadCertificate(this ActivityDetailedItemViewModel activityDetailedItemViewModel)
        {
            if (activityDetailedItemViewModel.CertificateEnabled)
            {
                if (activityDetailedItemViewModel.ResourceType == ResourceTypeEnum.Scorm)
                {
                    if (GetActivityStatusDisplayText(activityDetailedItemViewModel) == "Completed" || GetActivityStatusDisplayText(activityDetailedItemViewModel) == "Passed")
                    {
                        return true;
                    }
                }
                else
                {
                    if (GetActivityStatusDisplayText(activityDetailedItemViewModel) == "Completed" || GetActivityStatusDisplayText(activityDetailedItemViewModel) == "Passed" || GetActivityStatusDisplayText(activityDetailedItemViewModel) == "Downloaded")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
