namespace LearningHub.Nhs.WebUI.Models.Learning
{
    /// <summary>
    /// UserCertificateDetailsRequestViewModel.
    /// </summary>
    public class UserCertificateDetailsRequestViewModel
    {
        /// <summary>
        /// Gets or sets Page.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets PageSize.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets Filter.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets ResourceTypeId.
        /// </summary>
        public int? ResourceTypeId { get; set; }
    }
}
