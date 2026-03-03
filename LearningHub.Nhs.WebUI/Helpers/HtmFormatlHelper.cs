namespace LearningHub.Nhs.WebUI.Helpers
{
    using HtmlAgilityPack;

    /// <summary>
    /// HtmFormatlHelper.
    /// </summary>
    public static class HtmFormatlHelper
    {
        /// <summary>
        /// StripHtmlTags.
        /// </summary>
        /// <param name="html">html.</param>
        /// <returns>string.</returns>
        public static string StripHtmlTags(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.InnerText;
        }
    }
}
