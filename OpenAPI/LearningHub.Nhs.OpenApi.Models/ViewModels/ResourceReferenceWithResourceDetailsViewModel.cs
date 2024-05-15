namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    /// <summary>
    /// Class.
    /// </summary>
    public class ResourceReferenceWithResourceDetailsViewModel 
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceReferenceWithResourceDetailsViewModel"/> class.
        /// </summary>
        /// <param name="resourceId"><see cref="ResourceId"/>.</param>
        /// <param name="refId"><see cref="RefId"/>.</param>
        /// <param name="title"><see cref="Title"/>.</param>
        /// <param name="description"><see cref="Description"/>.</param>
        /// <param name="catalogueViewModel"><see cref="Catalogue"/>.</param>
        /// <param name="resourceType"><see cref="ResourceType"/>.</param>
        /// <param name="rating"><see cref="Rating"/>.</param>
        /// <param name="link"><see cref="Link"/>.</param>
        /// <param name="userSummaryActvityStatus"><see cref="UserSummaryActvityStatus"/>.</param>
        public ResourceReferenceWithResourceDetailsViewModel( 
            int resourceId,
            int refId,
            string title,
            string description,
            CatalogueViewModel catalogueViewModel,
            string resourceType,
            decimal rating,
            string link,
            string userSummaryActvityStatus
            )
        {
            this.ResourceId = resourceId;
            this.RefId = refId;
            this.Title = title;
            this.Description = description;
            this.Catalogue = catalogueViewModel;
            this.ResourceType = resourceType;
            this.Rating = rating;
            this.Link = link;
            this.UserSummaryActvityStatus = userSummaryActvityStatus;
        }

        /// <summary>
        /// Gets <see cref="ResourceId"/>.
        /// </summary>
        public int ResourceId { get; }

        /// <summary>
        /// Gets <see cref="RefId"/>.
        /// </summary>
        public int RefId { get; }

        /// <summary>
        /// Gets <see cref="Title"/>.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets <see cref="Description"/>.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets <see cref="Catalogue"/>.
        /// </summary>
        public CatalogueViewModel Catalogue { get; }

        /// <summary>
        /// Gets <see cref="ResourceType"/>.
        /// </summary>
        public string ResourceType { get; }

        /// <summary>
        /// Gets <see cref="Rating"/>.
        /// </summary>
        public decimal Rating { get; }

        /// <summary>
        /// Gets <see cref="Link"/>.
        /// </summary>
        public string Link { get; }

        /// <summary>
        /// Gets <see cref="UserSummaryActvityStatus"/>.
        /// </summary>
        public string UserSummaryActvityStatus { get;  }
    }
}
