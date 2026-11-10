namespace LearningHub.Nhs.Api.Authentication
{
    /// <summary>
    /// Single Sign On login object.
    /// </summary>
    public class LoginSso
    {
        /// <summary>
        /// Gets or sets external User id.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets external system id.
        /// </summary>
        public int ExternalSystemId { get; set; }

        /// <summary>
        /// Gets or sets client code.
        /// </summary>
        public string ClientCode { get; set; }
    }
}
