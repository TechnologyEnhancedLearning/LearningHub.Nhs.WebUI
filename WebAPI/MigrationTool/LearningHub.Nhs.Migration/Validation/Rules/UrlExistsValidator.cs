namespace LearningHub.Nhs.Migration.Validation.Rules
{
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Migration.Validation.Helpers;

    /// <summary>
    /// This class checks whether a URL exists valid (i.e. alive and return a 200 response).
    /// </summary>
    public class UrlExistsValidator
    {
        private readonly UrlChecker urlChecker;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlExistsValidator"/> class.
        /// </summary>
        /// <param name="urlChecker">The urlChecker.</param>
        public UrlExistsValidator(UrlChecker urlChecker)
        {
            this.urlChecker = urlChecker;
        }

        /// <summary>
        /// Performs the validation check.
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <param name="requestTimeoutInSeconds">The timeout to use when sending requests to the URL.</param>
        /// <param name="result">The validation result to add any error to.</param>
        /// <param name="modelPropertyName">The input model property name to use in the validation error.</param>
        public void Validate(string url, int requestTimeoutInSeconds, MigrationInputRecordValidationResult result, string modelPropertyName)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (!this.urlChecker.DoesUrlExist(url, requestTimeoutInSeconds, out string error))
                {
                    result.AddError(modelPropertyName, $"{modelPropertyName} contains an invalid link - \"{url}\". Error: {error}");
                }
            }
        }
    }
}
