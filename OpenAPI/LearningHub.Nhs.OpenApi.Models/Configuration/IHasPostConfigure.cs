namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// IHasPostConfigure.
    /// </summary>
    public interface IHasPostConfigure
    {
        /// <summary>
        /// The method which should be run in the OptionsBuilder's PostConfigure method after adding.
        /// </summary>
        public void PostConfigure();
    }
}
