namespace LearningHub.Nhs.WebUI.Services
{
    /// <summary>
    /// Defines the <see cref="VersionService" />.
    /// </summary>
    public class VersionService
    {
        /// <summary>
        /// The GetVersion.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetVersion()
        {
            var version = typeof(Program).Assembly.GetName().Version;
            return $"{version?.Major}.{version?.Minor}.{version?.Build}";
        }
    }
}
