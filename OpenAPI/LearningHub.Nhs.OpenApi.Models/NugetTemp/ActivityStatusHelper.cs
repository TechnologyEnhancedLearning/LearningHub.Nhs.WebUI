namespace LearningHub.Nhs.OpenApi.Models.NugetTemp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource.AzureMediaAsset;

    /// <summary>
    /// The Activity Status Helper.
    /// </summary>
    public static class ActivityStatusHelper
    {
        /// <summary>
        /// Gets the activity status description based on the activity status ID.
        /// As it is in the database.
        /// </summary>
        /// <param name="activityStatus">The activity status enum.</param>
        /// <param name="resourceType">The resourceType enum.</param>
        /// <returns>The activity status description.</returns>
        public static string GetActivityStatusDescription(ActivityStatusEnum? activityStatus, ResourceTypeEnum? resourceType = null)
        {
            switch (activityStatus)
            {
                // case ActivityStatusEnum.Launched: return "Launched"; // Not in use in db
                // case ActivityStatusEnum.InProgress: return "In Progress"; // Not in use in db
                // case ActivityStatusEnum.Viewed: return "Viewed"; Viewed not in database
                case ActivityStatusEnum.Failed: return "Failed";
                case ActivityStatusEnum.Passed: return "Passed";
                case ActivityStatusEnum.Downloaded: return "Downloaded";
               // case ActivityStatusEnum.Incomplete: return "In progress"; // In complete goes to In progress -- qqqq looks like models not uptodate
                case ActivityStatusEnum.Completed:
                    if (resourceType == ResourceTypeEnum.Article || resourceType == ResourceTypeEnum.Image || resourceType == ResourceTypeEnum.Case /*|| resourceType == ResourceTypeEnum.Html*/)
                    {
                        return "Viewed";
                    }
                    else if (resourceType == ResourceTypeEnum.WebLink)
                    {
                        return "Launched";
                    }
                    else if (resourceType == ResourceTypeEnum.GenericFile)
                    {
                        return "Downloaded";
                    }
                    else
                    {
                        return "Completed";
                    }

                case null: return string.Empty;
                default: throw new ArgumentOutOfRangeException(nameof(activityStatus), activityStatus, "is not valid in Enum");
            }
        }

        /// <summary>
        /// Gets a list of GetMajorVersionIdActivityStatusDescriptionLSPerResource
        /// </summary>
        /// <param name="resource">.</param>
        /// <param name="resourceActivities">List resourceActivities.</param>
        /// <returns>A list of <see cref="MajorVersionIdActivityStatusDescription"/>.</returns>
        public static IEnumerable<MajorVersionIdActivityStatusDescription>? GetMajorVersionIdActivityStatusDescriptionLSPerResource(Resource resource, IEnumerable<ResourceActivity> resourceActivities)
        {
            return resourceActivities
                .Where(ra => ra.ResourceId == resource.Id)
                .Select(x => new MajorVersionIdActivityStatusDescription(
                    x.MajorVersion,
                    ActivityStatusHelper.GetActivityStatusDescription((ActivityStatusEnum)x.ActivityStatusId, (ResourceTypeEnum?)resource.ResourceTypeEnum)));
        }
    }
}