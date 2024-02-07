// <copyright file="HtmlStringDeadLinkValidator.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using AngleSharp;
    using AngleSharp.Html.Dom;
    using AngleSharp.Html.Parser;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Migration.Validation.Helpers;

    /// <summary>
    /// This class checks whether a HTML string contains hyperlinks or images with href's that are not valid (i.e. alive and return a 200 response).
    /// </summary>
    public class HtmlStringDeadLinkValidator
    {
        private readonly UrlChecker urlChecker;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlStringDeadLinkValidator"/> class.
        /// </summary>
        /// <param name="urlChecker">The urlChecker.</param>
        public HtmlStringDeadLinkValidator(UrlChecker urlChecker)
        {
            this.urlChecker = urlChecker;
        }

        /// <summary>
        /// Performs the validation check.
        /// </summary>
        /// <param name="htmlText">The html text string to search.</param>
        /// <param name="requestTimeoutInSeconds">The timeout to use when sending requests to the URL.</param>
        /// <param name="result">The validation result to add any error to.</param>
        /// <param name="modelPropertyName">The input model property name to use in the validation error.</param>
        public void Validate(string htmlText, int requestTimeoutInSeconds, MigrationInputRecordValidationResult result, string modelPropertyName)
        {
            // Use AngleSharp to parse the HTML and use its LINQ extension to select the element types that we want to check.
            if (!string.IsNullOrEmpty(htmlText))
            {
                IConfiguration config = Configuration.Default;
                IBrowsingContext context = BrowsingContext.New(config);
                IHtmlParser parser = context.GetService<IHtmlParser>();
                IHtmlDocument document = parser.ParseDocument(htmlText);

                IEnumerable<IHtmlAnchorElement> links = document
                                .Links
                                .OfType<IHtmlAnchorElement>();

                foreach (var link in links)
                {
                    // Get the link href, minus the false "about:" scheme name if it's a relative url.
                    string linkHref = this.RemoveFalseAboutSchemeName(link.Href, link.OuterHtml);

                    if (!this.urlChecker.DoesUrlExist(linkHref, requestTimeoutInSeconds, out string error))
                    {
                        result.AddWarning(modelPropertyName, $"{modelPropertyName} contains an invalid link - \"{linkHref}\". URL response: {error}");
                    }
                }

                IEnumerable<IHtmlImageElement> imageSources = document
                                .Images
                                .OfType<IHtmlImageElement>();

                foreach (var src in imageSources)
                {
                    string srcUrl = this.RemoveFalseAboutSchemeName(src.Source, src.OuterHtml);

                    if (!this.urlChecker.DoesUrlExist(srcUrl, requestTimeoutInSeconds, out string error))
                    {
                        result.AddWarning(modelPropertyName, $"{modelPropertyName} contains an invalid image source - \"{srcUrl}\". URL response: {error}");
                    }
                }
            }
        }

        private string RemoveFalseAboutSchemeName(string url, string elementOuterHtml)
        {
            /* Anglesharp annoyingly adds the "about:" scheme name to the beginning of any relative urls. This looks misleading when displayed in the validation errors.
             * We strip it off if again if the original link didn't include it. */

            if (url.StartsWith("about://") && !elementOuterHtml.Contains(url))
            {
                return url.Substring(8);
            }
            else
            {
                return url;
            }
        }
    }
}
