using LearningHub.Nhs.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Shared.Helpers
{
    public static class MoodleHelper
    {
        /// TODO: Remove this method after adding to Moodle resource types to models project.
        /// <summary>
        /// Returns a prettified resource type name, suitable for display in the UI. Includes video/audio duration string.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="durationInMilliseconds">The media duration in milliseconds.</param>
        /// <returns>The resource type name, and duration if applicable.</returns>
        public static string GetPrettifiedResourceTypeNameMoodle(ResourceTypeEnum resourceType, int? durationInMilliseconds = 0)
        {
            switch (resourceType)
            {
                case ResourceTypeEnum.Assessment:
                    return "Assessment";
                case ResourceTypeEnum.Article:
                    return "Article";
                case ResourceTypeEnum.Audio:
                    string durationText = FormattingHelper.GetDurationText(durationInMilliseconds ?? 0);
                    durationText = string.IsNullOrEmpty(durationText) ? string.Empty : " - " + durationText;
                    return "Audio" + durationText;
                case ResourceTypeEnum.Equipment:
                    return "Equipment";
                case ResourceTypeEnum.Image:
                    return "Image";
                case ResourceTypeEnum.Scorm:
                    return "elearning";
                case ResourceTypeEnum.Video:
                    durationText = FormattingHelper.GetDurationText(durationInMilliseconds ?? 0);
                    durationText = string.IsNullOrEmpty(durationText) ? string.Empty : " - " + durationText;
                    return "Video" + durationText;
                case ResourceTypeEnum.WebLink:
                    return "Web link";
                case ResourceTypeEnum.GenericFile:
                    return "File";
                case ResourceTypeEnum.Embedded:
                    return "Embedded";
                case ResourceTypeEnum.Case:
                    return "Case";
                case ResourceTypeEnum.Html:
                    return "HTML";
                case ResourceTypeEnum.Moodle:
                    return "Course";
                default:
                    return "File";
            }
        }

        /// TODO: Remove this method after adding to Moodle resource types to models project.
        /// <summary>
        /// Findwise Moodle resource type dictionary.
        /// </summary>
        public static readonly Dictionary<string, ResourceTypeEnum> FindwiseResourceMoodleTypeDict = new Dictionary<string, ResourceTypeEnum>()
        {
            { "video", ResourceTypeEnum.Video },
            { "article", ResourceTypeEnum.Article },
            { "case", ResourceTypeEnum.Case },
            { "weblink", ResourceTypeEnum.WebLink },
            { "audio", ResourceTypeEnum.Audio },
            { "scorm", ResourceTypeEnum.Scorm },
            { "assessment", ResourceTypeEnum.Assessment },
            { "genericfile", ResourceTypeEnum.GenericFile },
            { "image", ResourceTypeEnum.Image },
            { "html", ResourceTypeEnum.Html },
            { "moodle", ResourceTypeEnum.Moodle },
        };

        /// <summary>
        /// Returns a prettified resource type name, suitable for display in the UI. Excludes video/audio duration string.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <returns>The resource type name, and duration if applicable.</returns>
        public static string GetPrettifiedResourceTypeName(ResourceTypeEnum resourceType)
        {
            switch (resourceType)
            {
                case ResourceTypeEnum.Assessment:
                    return "Assessment";
                case ResourceTypeEnum.Article:
                    return "Article";
                case ResourceTypeEnum.Audio:
                    return "Audio";
                case ResourceTypeEnum.Equipment:
                    return "Equipment";
                case ResourceTypeEnum.Image:
                    return "Image";
                case ResourceTypeEnum.Scorm:
                    return "elearning";
                case ResourceTypeEnum.Video:
                    return "Video";
                case ResourceTypeEnum.WebLink:
                    return "Web link";
                case ResourceTypeEnum.GenericFile:
                    return "File";
                case ResourceTypeEnum.Embedded:
                    return "Embedded";
                case ResourceTypeEnum.Case:
                    return "Case";
                case ResourceTypeEnum.Html:
                    return "HTML";
                default:
                    return "File";
            }
        }
    }
}
