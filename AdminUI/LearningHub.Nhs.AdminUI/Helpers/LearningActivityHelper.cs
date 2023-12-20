// <copyright file="LearningActivityHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.WebUI.Models;

    /// <summary>
    /// Defines the <see cref="LearningActivityHelper" />.
    /// </summary>
    public static class LearningActivityHelper
    {
        /// <summary>
        /// The GetResourceTypeText.
        /// </summary>
        /// <param name="myLearningDetailedItemViewModel">The myLearningDetailedItemViewModel.</param>
        /// <returns>The .</returns>
        public static string GetResourceTypeText(this MyLearningDetailedItemViewModel myLearningDetailedItemViewModel)
        {
            string typeText = string.Empty;
            switch (myLearningDetailedItemViewModel.ResourceType)
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
                default:
                    typeText = string.Empty;
                    break;
            }

            if (myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Video || myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Audio)
            {
                var duration = GetDurationText(myLearningDetailedItemViewModel.ResourceDurationMilliseconds);
                typeText = $"{duration} {typeText.ToLower()}";
            }

            return typeText;
        }

        /// <summary>
        /// GetResourceTypeVerb.
        /// </summary>
        /// <param name="myLearningDetailedItemViewModel">The myLearningDetailedItemViewModel.</param>
        /// <returns>The .</returns>
        public static string GetResourceTypeVerb(this MyLearningDetailedItemViewModel myLearningDetailedItemViewModel)
        {
            string result = string.Empty;
            switch (myLearningDetailedItemViewModel.ResourceType)
            {
                case ResourceTypeEnum.Article:
                    return "Read";
                case ResourceTypeEnum.Audio:
                    return "Played " + GetDurationText(myLearningDetailedItemViewModel.ActivityDurationSeconds * 1000);
                case ResourceTypeEnum.Embedded:
                    return string.Empty;
                case ResourceTypeEnum.Equipment:
                    return "Used equipment/visited facility";
                case ResourceTypeEnum.GenericFile:
                    return "Downloaded";
                case ResourceTypeEnum.Image:
                    if (myLearningDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Downloaded)
                    {
                        return "Downloaded";
                    }
                    else
                    {
                        return "Viewed";
                    }

                case ResourceTypeEnum.Scorm:
                    if (myLearningDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Downloaded)
                    {
                        return "Downloaded";
                    }
                    else
                    {
                        return "Accessed";
                    }

                case ResourceTypeEnum.Video:
                    return "Played " + GetDurationText(myLearningDetailedItemViewModel.ActivityDurationSeconds * 1000);
                case ResourceTypeEnum.WebLink:
                    return "Visited";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// GetActivityStatusDisplayText.
        /// </summary>
        /// <param name="myLearningDetailedItemViewModel">The myLearningDetailedItemViewModel.</param>
        /// <returns>The .</returns>
        public static string GetActivityStatusDisplayText(this MyLearningDetailedItemViewModel myLearningDetailedItemViewModel)
        {
            if (myLearningDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Launched
                     && (myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Article
                         || myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.WebLink
                         || myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Image
                         || myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Case))
            {
                return "Completed";
            }
            else if (myLearningDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Launched
                && (myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.GenericFile))
            {
                return "Downloaded";
            }
            else if (myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Assessment)
            {
                if (myLearningDetailedItemViewModel.Complete)
                {
                    return myLearningDetailedItemViewModel.ScorePercentage >= myLearningDetailedItemViewModel.AssessmentDetails.PassMark ? "Passed" : "Failed";
                }
                else
                {
                    return myLearningDetailedItemViewModel.ScorePercentage >= myLearningDetailedItemViewModel.AssessmentDetails.PassMark ? "Passed" : "Incomplete";
                }
            }
            else
            {
                return GetActivityStatusText(myLearningDetailedItemViewModel);
            }
        }

        /// <summary>
        /// GetActivityStatus.
        /// </summary>
        /// <param name="myLearningDetailedItemViewModel">The myLearningDetailedItemViewModel.</param>
        /// <returns>The <see cref="ActivityStatusEnum"/>.</returns>
        public static ActivityStatusEnum GetActivityStatus(this MyLearningDetailedItemViewModel myLearningDetailedItemViewModel)
        {
            if (myLearningDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Launched
                && (myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Article
                    || myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.WebLink
                    || myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Image
                    || myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Case))
            {
                return ActivityStatusEnum.Completed;
            }
            else if (myLearningDetailedItemViewModel.ActivityStatus == ActivityStatusEnum.Launched
                && (myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.GenericFile))
            {
                return ActivityStatusEnum.Downloaded;
            }
            else if (myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Assessment)
            {
                if (myLearningDetailedItemViewModel.Complete)
                {
                    return myLearningDetailedItemViewModel.ScorePercentage >= myLearningDetailedItemViewModel.AssessmentDetails.PassMark ? ActivityStatusEnum.Passed : ActivityStatusEnum.Failed;
                }
                else
                {
                    return myLearningDetailedItemViewModel.ScorePercentage >= myLearningDetailedItemViewModel.AssessmentDetails.PassMark ? ActivityStatusEnum.Passed : ActivityStatusEnum.InProgress;
                }
            }
            else
            {
                return myLearningDetailedItemViewModel.ActivityStatus;
            }
        }

        /// <summary>
        /// GetActivityStatusText.
        /// </summary>
        /// <param name="myLearningDetailedItemViewModel">The myLearningDetailedItemViewModel.</param>
        /// <returns>The .</returns>
        public static string GetActivityStatusText(this MyLearningDetailedItemViewModel myLearningDetailedItemViewModel)
        {
            switch (myLearningDetailedItemViewModel.ActivityStatus)
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
        /// CanDownloadCertificate.
        /// </summary>
        /// <param name="myLearningDetailedItemViewModel">The myLearningDetailedItemViewModel.</param>
        /// <returns>The <see cref="bool"/>bool.</returns>
        public static bool CanDownloadCertificate(this MyLearningDetailedItemViewModel myLearningDetailedItemViewModel)
        {
            if (myLearningDetailedItemViewModel.CertificateEnabled)
            {
                if (myLearningDetailedItemViewModel.ResourceType == ResourceTypeEnum.Scorm)
                {
                    if (GetActivityStatusDisplayText(myLearningDetailedItemViewModel) == "Completed" || GetActivityStatusDisplayText(myLearningDetailedItemViewModel) == "Passed")
                    {
                        return true;
                    }
                }
                else
                {
                    if (GetActivityStatusDisplayText(myLearningDetailedItemViewModel) == "Completed" || GetActivityStatusDisplayText(myLearningDetailedItemViewModel) == "Passed" || GetActivityStatusDisplayText(myLearningDetailedItemViewModel) == "Downloaded")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
