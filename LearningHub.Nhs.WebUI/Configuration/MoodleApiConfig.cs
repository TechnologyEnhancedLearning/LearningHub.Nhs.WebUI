namespace LearningHub.Nhs.WebUI.Configuration
{
    /// <summary>
    /// The Moodle Settings.
    /// </summary>
    public class MoodleApiConfig
    {
        /// <summary>
        /// Gets or sets the base url for the Moodle service.
        /// </summary>
        public string BaseUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Web service Rest Format.
        /// </summary>
        public string MoodleWSRestFormat { get; set; } = null!;

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string WSToken { get; set; } = null!;

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string ApiPath { get; set; } = "webservice/rest/server.php";

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string CoursePath { get; set; } = "course/view.php";
    }
}
