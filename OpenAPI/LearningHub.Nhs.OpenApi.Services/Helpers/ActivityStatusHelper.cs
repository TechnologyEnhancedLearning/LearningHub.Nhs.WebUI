namespace LearningHub.Nhs.OpenApi.Services
{
    using LearningHub.Nhs.Models.Enums;
    using System;
    using System.Security.Cryptography.Xml;

    /// <summary>
    /// Defines the <see cref="ActivityStatusHelper" />.
    /// </summary>
    public static class ActivityStatusHelper
    {
        public static string UserSummaryActvityStatus(ActivityStatusEnum? activityStatus)
        {
            switch (activityStatus)
            {
                case ActivityStatusEnum.Launched: return "Launched";
                case ActivityStatusEnum.InProgress: return "InProgress";
                case ActivityStatusEnum.Completed: return "Completed";
                case ActivityStatusEnum.Failed: return "Failed";
                case ActivityStatusEnum.Passed: return "Passed";
                case ActivityStatusEnum.Downloaded: return "Downloaded";
                case null: return string.Empty;
                default: throw new ArgumentOutOfRangeException(nameof(activityStatus), activityStatus, "is not valid in Enum");
            }
        }
    }
}
