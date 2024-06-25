using LearningHub.Nhs.Models.Entities.Activity;
using LearningHub.Nhs.Models.Resource;
using System.Collections.Generic;

namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    /// <summary>
    /// Class.
    /// </summary>
    public class ResourceReferenceWithResourceDetailsViewModel 
    {
        public ResourceReferenceWithResourceDetailsViewModel(ResourceReferenceAndCatalogueDTO resourceReferenceAndCatalogueDTO, List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescriptionLS)
        {


        }


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
        /// <param name="userSummaryActivityStatus"><see cref="UserSummaryActivityStatus"/>.</param>
        public ResourceReferenceWithResourceDetailsViewModel(
            int resourceId,
            int refId,
            string title,
            string description,
            CatalogueViewModel catalogueViewModel,
            string resourceType,
            int? majorVersion,
            decimal rating,
            string link,
            List<MajorVersionIdActivityStatusDescription>? userSummaryActivityStatus
            )
        {
            this.ResourceId = resourceId;
            this.RefId = refId;
            this.Title = title;
            this.Description = description;
            this.Catalogue = catalogueViewModel;
            this.ResourceType = resourceType;
            this.MajorVersion = majorVersion;
            this.Rating = rating;
            this.Link = link;
            this.UserSummaryActivityStatus = userSummaryActivityStatus;
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
        /// Gets <see cref="MajorVersion"/>.
        /// </summary>
        public int? MajorVersion { get; }


        /// <summary>
        /// Gets <see cref="Rating"/>.
        /// </summary>
        /// 
        public decimal Rating { get; }

        /// <summary>
        /// Gets <see cref="Link"/>.
        /// </summary>
        public string Link { get; }

        /// <summary>
        /// Gets <see cref="UserSummaryActivityStatus"/>.
        /// </summary>
        public List<MajorVersionIdActivityStatusDescription>? UserSummaryActivityStatus { get;  }
    }
}
