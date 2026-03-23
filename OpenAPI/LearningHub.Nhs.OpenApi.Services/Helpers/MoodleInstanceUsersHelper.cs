namespace LearningHub.Nhs.OpenApi.Services.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Blocks;

    /// <summary>
    /// A class containing helper functions for building moodle user instances.
    /// </summary>
    public class MoodleInstanceUsersHelper
    {
        /// <summary>
        /// BuildMoodleUserInstances.
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public static MoodleInstanceUserIdsViewModel BuildUserIdsByInstance(
         IEnumerable<MoodleUserMapping> mappings)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var m in mappings)
            {
                if (m?.User is null) continue;
                if (m.User.Id <= 0) continue;

                var key = string.IsNullOrWhiteSpace(m.Instance)
                    ? $"instance_{m.User.Id}"
                    : m.Instance!;

                if (!dict.ContainsKey(key))
                {
                    dict[key] = m.User.Id;
                }
            }

            return new MoodleInstanceUserIdsViewModel
            {
                MoodleInstanceUserIds = dict
            };
        }
    }
}
