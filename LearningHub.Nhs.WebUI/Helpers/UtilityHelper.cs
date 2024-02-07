// <copyright file="UtilityHelper.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using HtmlAgilityPack;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Hierarchy;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Defines the <see cref="UtilityHelper" />.
    /// </summary>
    public static class UtilityHelper
    {
        /// <summary>
        /// Findwise resource type dictionary.
        /// </summary>
        public static readonly Dictionary<string, ResourceTypeEnum> FindwiseResourceTypeDict = new Dictionary<string, ResourceTypeEnum>()
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
        };

        /// <summary>
        /// The FormatTwitterDate.
        /// </summary>
        /// <param name="htmlhelper">The htmlhelper<see cref="IHtmlHelper"/>.</param>
        /// <param name="createdAt">The CreatedAt<see cref="DateTime"/>.</param>
        /// <returns>The .</returns>
        public static string FormatTwitterDate(this IHtmlHelper htmlhelper, DateTime createdAt)
        {
            DateTime now = DateTime.Now;
            TimeSpan timespan = now.Subtract(createdAt);

            return timespan.Days > 0 ? string.Format("{0}d", timespan.Days.ToString()) : timespan.Hours > 1 ? string.Format("{0}hr", timespan.Hours.ToString()) : timespan.Minutes > 1 ? string.Format("{0}min", timespan.Minutes.ToString()) : "now";
        }

        /// <summary>
        /// The FormatUnreadNotificationCount.
        /// </summary>
        /// <param name="htmlhelper">The htmlhelper<see cref="IHtmlHelper"/>.</param>
        /// <param name="notificationCount">The NotificationCount<see cref="int"/>.</param>
        /// <returns>The .</returns>
        public static string FormatUnreadNotificationCount(this IHtmlHelper htmlhelper, int notificationCount)
        {
            return notificationCount > 99 ? "99+" : notificationCount.ToString();
        }

        /// <summary>
        /// Removes HTML tags from an input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The output string.</returns>
        public static string StripHtmlFromString(string input)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(input ?? string.Empty);
            var htmlDecoded = HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);
            return htmlDecoded;
        }

        /// <summary>
        /// Returns the resource type icon CSS class name.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <returns>The CSS class name.</returns>
        public static string GetResourceTypeIconClass(ResourceTypeEnum resourceType)
        {
            switch (resourceType)
            {
                case ResourceTypeEnum.Article:
                    return "fa-regular fa-file-alt";
                case ResourceTypeEnum.Audio:
                    return "fa-solid fa-volume-up";
                case ResourceTypeEnum.Equipment:
                    return "fa-solid fa-map-marker-alt";
                case ResourceTypeEnum.GenericFile:
                    return "fa-regular fa-file";
                case ResourceTypeEnum.Image:
                    return "fa-regular fa-image";
                case ResourceTypeEnum.Scorm:
                    return "fa-solid fa-cube";
                case ResourceTypeEnum.Video:
                    return "fa-solid fa-video";
                case ResourceTypeEnum.WebLink:
                    return "fa-solid fa-globe";
                case ResourceTypeEnum.Case:
                    return "fa-solid fa-microscope";
                default:
                    return "fa-regular fa-file";
            }
        }

        /// <summary>
        /// Returns a prettified resource type name, suitable for display in the UI. Includes video/audio duration string.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="durationInMilliseconds">The media duration in milliseconds.</param>
        /// <returns>The resource type name, and duration if applicable.</returns>
        public static string GetPrettifiedResourceTypeName(ResourceTypeEnum resourceType, int? durationInMilliseconds)
        {
            switch (resourceType)
            {
                case ResourceTypeEnum.Assessment:
                    return "Assessment";
                case ResourceTypeEnum.Article:
                    return "Article";
                case ResourceTypeEnum.Audio:
                    return "Audio - " + GetDurationText(durationInMilliseconds.Value);
                case ResourceTypeEnum.Equipment:
                    return "Equipment";
                case ResourceTypeEnum.Image:
                    return "Image";
                case ResourceTypeEnum.Scorm:
                    return "elearning";
                case ResourceTypeEnum.Video:
                    return "Video - " + GetDurationText(durationInMilliseconds.Value);
                case ResourceTypeEnum.WebLink:
                    return "Web link";
                case ResourceTypeEnum.GenericFile:
                    return "File";
                case ResourceTypeEnum.Embedded:
                    return "Embedded";
                case ResourceTypeEnum.Case:
                    return "Case";
                default:
                    return "File";
            }
        }

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
                default:
                    return "File";
            }
        }

        /// <summary>
        /// Returns a number of milliseconds converted into a duration string, such as "10 min 15 sec". Includes rounding to match the behaviour of the Azure Media Player.
        /// </summary>
        /// <param name="durationInMilliseconds">The number of milliseconds.</param>
        /// <returns>The duration string.</returns>
        public static string GetDurationText(int durationInMilliseconds)
        {
            if (durationInMilliseconds > 0)
            {
                // Azure media player rounds duration to nearest second. e.g. 8:59.88 becomes 9:00. LH needs to match.
                int nearestSecond = (int)Math.Round(((double)durationInMilliseconds) / 1000);
                var duration = new TimeSpan(0, 0, nearestSecond);
                string returnValue = string.Empty;

                // If duration greater than an hour, don't return the seconds part.
                if (duration.Hours > 0)
                {
                    returnValue = $"{duration.Hours} hr {duration.Minutes} min ";

                    // Exclude "0 min" from the return value.
                    if (returnValue.EndsWith(" 0 min "))
                    {
                        returnValue = returnValue.Replace("0 min ", string.Empty);
                    }
                }
                else
                {
                    returnValue = $"{duration.Minutes} min {duration.Seconds} sec ";

                    // Exclude "0 min" and "0 sec" from the return value.
                    if (returnValue.StartsWith("0 min "))
                    {
                        returnValue = returnValue.Replace("0 min ", string.Empty);
                    }

                    if (returnValue.EndsWith(" 0 sec "))
                    {
                        returnValue = returnValue.Replace("0 sec ", string.Empty);
                    }
                }

                return returnValue;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns a string containing either the authoredBy or organisation string, or a combination of both if present.
        /// </summary>
        /// <param name="authoredBy">The author name. Can be null.</param>
        /// <param name="organisation">The organisation name. Can be null.</param>
        /// <returns>A string containing the author and/or organisation, depending on the data.</returns>
        public static string GetFullAuthorText(string authoredBy, string organisation)
        {
            if (!string.IsNullOrEmpty(authoredBy) && !string.IsNullOrEmpty(organisation))
            {
                return "Authored by " + authoredBy + ", " + organisation;
            }
            else if (!string.IsNullOrEmpty(authoredBy) & string.IsNullOrEmpty(organisation))
            {
                return "Authored by " + authoredBy;
            }
            else if (string.IsNullOrEmpty(authoredBy) && !string.IsNullOrEmpty(organisation))
            {
                return "Authored by " + organisation;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get concat string.
        /// </summary>
        /// <param name="description">description.</param>
        /// <param name="length">length.</param>
        /// <returns>string.</returns>
        public static string ConcatString(string description, int length)
        {
            if (string.IsNullOrEmpty(description))
            {
                return string.Empty;
            }
            else if (description.Length <= length)
            {
                return description;
            }
            else
            {
                var subString = description.Substring(0, length - 1);
                var len = subString.LastIndexOf(' ');
                if (len == -1)
                {
                    return $"{subString}...";
                }

                return $"{subString.Substring(0, len)}...";
            }
        }

        /// <summary>
        /// The GetAttrition.
        /// </summary>
        /// <param name="authors">list of authors.</param>
        /// <returns>string.</returns>
        public static string GetAttribution(List<string> authors)
        {
            var attribution = string.Empty;

            if (authors != null && authors.Count > 0)
            {
                attribution += "Authored by: ";

                attribution += string.Join(", ", authors.ToArray());
            }

            if (attribution.Length > 40)
            {
                attribution = attribution.Substring(0, 40) + "...";
            }

            return attribution;
        }

        /// <summary>
        /// The GetInOn status.
        /// </summary>
        /// <param name="dateString">date string.</param>
        /// <returns>string.</returns>
        public static string GetInOn(string dateString)
        {
            string pattern = @"[0-9]{1,2} [A-Za-z]{3,4}";

            if (Regex.Match(dateString, pattern, RegexOptions.IgnoreCase).Success)
            {
                return "on";
            }
            else
            {
                return "in";
            }
        }

        /// <summary>
        /// To Enum.
        /// </summary>
        /// <typeparam name="T">object.</typeparam>
        /// <param name="value">value.</param>
        /// <param name="ignoreCase">ignore case.</param>
        /// <returns>enum object.</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        /// <summary>
        /// Returns the "authored date" formatted for display purposes, correctly constructed from the consituent parts, not all of which are mandatory.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <returns>The authored date.</returns>
        public static string GetAuthoredDate(int? day, int? month, int? year)
        {
            var authoredDate = string.Empty;
            if (year.HasValue)
            {
                authoredDate = year.Value.ToString();

                if (month.HasValue)
                {
                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Value);
                    authoredDate = $"{monthName} {authoredDate}";

                    if (day.HasValue)
                    {
                        authoredDate = $"{day} {authoredDate}";
                    }
                }
            }

            return authoredDate;
        }

        /// <summary>
        /// Gets the text to display on a generic file download button according to the file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>The text to display.</returns>
        public static string GetGenericFileButtonText(string extension)
        {
            var title = "View this file";

            if (extension.ToLower() == "zip")
            {
                title = "Download this file";
            }

            return title;
        }

        /// <summary>
        /// Gets the file extension from a filename. Returns "-" if none found.
        /// </summary>
        /// <param name="filename">The filename extract the file extension from.</param>
        /// <returns>The file extension.</returns>
        public static string GetFileExtension(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return string.Empty;
            }

            int extensionIndex = filename.LastIndexOf('.');
            if (extensionIndex == -1)
            {
                return string.Empty;
            }

            return filename.Substring(extensionIndex + 1);
        }

        /// <summary>
        /// Gets the pill colour CSS class name to suit a particular file extension.
        /// </summary>
        /// <param name="filename">The filename. The file extension will be extracted.</param>
        /// <returns>The pill colour CSS class name.</returns>
        public static string GetPillColour(string filename)
        {
            var extension = GetFileExtension(filename).ToUpper();

            switch (extension)
            {
                case "PDF":
                case "MP3":
                case "WAV":
                case "WMA":
                case "MP4":
                case "AVI":
                case "M4V":
                case "MOV":
                case "MKV":
                case "MPEG":
                case "MPG":
                case "WMV":
                    return "pill--red";

                case "DOC":
                case "DOCX":
                case "BMP":
                case "GIF":
                case "JPEG":
                case "JPG":
                case "PNG":
                case "PSD":
                case "TIFF":
                case "HTML":
                case "HTM":
                case "TXT":
                    return "pill--blue";

                case "XLS":
                case "XLSX":
                case "XLM":
                case "XLSM":
                case "XSL":
                case "XML":
                case "CSV":
                    return "pill-green";

                case "PPT":
                case "PPTX":
                case "ZIP":
                    return "pill-yellow";

                default:
                    return "pill--blue-dark";
            }
        }

        /// <summary>
        /// Returns breadcrumb tuple data ready to be passed into the _Breadcrumbs partial view when being used to display a folder path.
        /// </summary>
        /// <param name="nodes">The list of folder nodes to display.</param>
        /// <param name="catalogueUrl">The URL reference of the catalogue.</param>
        /// <returns>A list of tuples, composed of Title and Url.</returns>
        public static List<(string Title, string Url)> GetBreadcrumbsForFolderNodes(List<NodeViewModel> nodes, string catalogueUrl)
        {
            // Create breadcrumb tuple data
            var breadcrumbs = new List<(string Title, string Url)> { ("Home", "/") };

            for (int i = 0; i < nodes.Count; i++)
            {
                string nodeUrl = $"/catalogue/{catalogueUrl}";
                if (i > 0)
                {
                    nodeUrl += $"?nodeId={nodes[i].NodeId}";
                }

                breadcrumbs.Add((nodes[i].Name, nodeUrl));
            }

            return breadcrumbs;
        }
    }
}
