namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Class.
    /// </summary>
    public class ResourceMetadataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMetadataViewModel"/> class.
        /// </summary>
        public ResourceMetadataViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMetadataViewModel"/> class.
        /// </summary>
        /// <param name="resourceId"><see cref="ResourceId"/>.</param>
        /// <param name="title"><see cref="Title"/>.</param>
        /// <param name="description"><see cref="Description"/>.</param>
        /// <param name="references"><see cref="References"/>.</param>
        /// <param name="resourceType"><see cref="ResourceType"/>.</param>
        /// <param name="rating"><see cref="Rating"/>.</param>
        /// <param name="userSummaryActvityStatus"><see cref="UserSummaryActvityStatus"/>.</param>
        public ResourceMetadataViewModel(
            int resourceId,
            string title,
            string description,
            List<ResourceReferenceViewModel> references,
            string resourceType,
            decimal rating,
            string userSummaryActvityStatus)
        {
            this.ResourceId = resourceId;
            this.Title = title;
            this.Description = description;
            this.References = references;
            this.ResourceType = resourceType;
            this.Rating = rating;
            this.UserSummaryActvityStatus = userSummaryActvityStatus;
        }

        /// <summary>
        /// Gets or sets <see cref="ResourceId"/>.
        /// </summary>
        public int ResourceId { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Title"/>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Description"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets <see cref="References"/>.
        /// </summary>
        public List<ResourceReferenceViewModel> References { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ResourceType"/>.
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Rating"/>.
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Gets or sets <see cref="UserSummaryActvityStatus"/>.
        /// </summary>
        public string UserSummaryActvityStatus { get; set; }
    }
}
