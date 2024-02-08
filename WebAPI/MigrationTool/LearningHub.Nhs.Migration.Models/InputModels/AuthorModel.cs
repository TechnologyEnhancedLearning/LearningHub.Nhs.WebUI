namespace LearningHub.Nhs.Migration.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data model class for representing an author, used by the StandardInputModel class.
    /// </summary>
    public class AuthorModel
    {
        /// <summary>
        /// Gets or sets the AuthorIndex.
        /// </summary>
        [JsonProperty(PropertyName = "Author Index")]
        public int? AuthorIndex { get; set; }

        /// <summary>
        /// Gets or sets the AuthorName.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the Organisation.
        /// </summary>
        public string Organisation { get; set; }

        /// <summary>
        /// Gets or sets the Role.
        /// </summary>
        public string Role { get; set; }
    }
}