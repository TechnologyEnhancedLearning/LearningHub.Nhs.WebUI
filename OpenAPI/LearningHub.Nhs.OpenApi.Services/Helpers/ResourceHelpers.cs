namespace LearningHub.Nhs.OpenApi.Services.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Xml.Linq;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;

    /// <summary>
    /// Resource helpers.
    /// </summary>
    public static class ResourceHelpers
    {
        /// <summary>
        /// Text to return in ResourceMetaDataViewModel when there is no resource version.
        /// </summary>
        public const string NoResourceVersionText = "No current resource version";

        /// <summary>
        /// Text to return in CatalogueViewModel when there is no catalogue.
        /// </summary>
        public const string NoCatalogueText = "No catalogue for resource reference";

        /// qqqqq
        ///// <summary>
        ///// The get catalogue from resource reference method.
        ///// </summary>
        ///// <param name="resourceReference">The resourceReference.</param>
        ///// <returns>The catalogue the resource reference is part of.</returns>
        //public static CatalogueViewModel GetCatalogue(this ResourceReference resourceReference)
        //{
        //    var catalogue = new CatalogueViewModel(0, NoCatalogueText, false);

        //    if (resourceReference.NodePath?.CatalogueNode?.CurrentNodeVersion?.CatalogueNodeVersion != null)
        //    {
        //        var catalogueNodeVersion = resourceReference.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion;
        //        catalogue = new CatalogueViewModel(catalogueNodeVersion);
        //    }

        //    return catalogue;
        //}

        /// <summary>
        /// Orders the IEnumerable of resources according to the sequence of ids given.
        /// If any resources don't match the list of ids, they're placed at the start.
        /// </summary>
        /// <param name="resources"><see cref="resources"/>.</param>
        /// <param name="resourceIdSequence"><see cref="resourceIdSequence"/>.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="ResourceMetadataViewModel"/>.</returns>
        public static IEnumerable<ResourceMetadataViewModel> OrderBySequence(
            this IEnumerable<ResourceMetadataViewModel> resources,
            IList<int> resourceIdSequence)
        {
            return resources.OrderBy(resource => resourceIdSequence.IndexOf(resource.ResourceId));
        }

        /// <summary>
        /// Gets the string resource type name.
        /// </summary>
        /// <param name="resource"><see cref="Resource"/>.</param>
        /// <returns>The resource type name or an empty string if the resource isn't recognised.</returns>
        public static string GetResourceTypeNameOrEmpty(this Resource resource)
        {
            var resourceTypeName = Enum.GetName(typeof(ResourceTypeEnum), resource.ResourceTypeEnum);

            return resourceTypeName ?? string.Empty;
        }

        /// <summary>
        /// Gets the string resource type name.
        /// </summary>
        /// <param name="ResourceReferenceAndCatalogueDTO"><see cref="ResourceReferenceAndCatalogueDTO"/>.</param>
        /// <returns>The resource type name or an empty string if the resource isn't recognised.</returns>
        public static string GetResourceTypeNameOrEmpty(this ResourceReferenceAndCatalogueDTO resourceReferenceAndCatalogueDTO)
        {
            var resourceTypeName = Enum.GetName(typeof(ResourceTypeEnum), resourceReferenceAndCatalogueDTO.ResourceTypeEnum);

            return resourceTypeName ?? string.Empty;
        }

        /// <summary>
        /// The get catalogue from resource reference method.
        /// </summary>
        /// <param name="CatalogueDTO">The catalogueDTOs.</param>
        /// <returns>The catalogue the resource reference is part of.</returns>
        public static CatalogueViewModel GetCatalogue(this CatalogueDTO catalogueDTOs)
        {
            var catalogue = new CatalogueViewModel(0, NoCatalogueText, false);

            if (catalogueDTOs != null && catalogueDTOs.CatalogueNodeId != null)
            {
                catalogue = new CatalogueViewModel
                {
                    Id = catalogueDTOs.CatalogueNodeId,
                    Name = catalogueDTOs.CatalogueNodeName ?? string.Empty,
                    IsRestricted = catalogueDTOs.IsRestricted,
                };
            }

            return catalogue;
        }

        public static List<ResourceReferenceAndCatalogueDTO> FlattenResourceReferenceAndCatalogueDTOLS(List<ResourceReferenceAndCatalogueDTO> resourceReferenceAndCatalogueDTOs)
        {
            List<ResourceReferenceAndCatalogueDTO> resourceReferenceAndCatalogueDTOsFlattened = resourceReferenceAndCatalogueDTOs
                .SelectMany(rRACD => (rRACD.CatalogueDTOs ?? new List<CatalogueDTO>()).DefaultIfEmpty(), // handle null CatalogueDTOs for where we have nullified external catalogue data
                    (rRACD, cD) => new ResourceReferenceAndCatalogueDTO
                    {
                        ResourceId = rRACD.ResourceId,
                        Title = rRACD.Title,
                        Description = rRACD.Description,
                        ResourceTypeEnum = rRACD.ResourceTypeEnum,
                        MajorVersion = rRACD.MajorVersion,
                        Rating = rRACD.Rating,
                        CatalogueDTOs = cD == null ? new List<CatalogueDTO>() : new List<CatalogueDTO>
                        {
                            new CatalogueDTO
                            {
                                CatalogueNodeId = cD.CatalogueNodeId,
                                CatalogueNodeName = cD.CatalogueNodeName,
                                IsRestricted = cD.IsRestricted,
                                OriginalResourceReferenceId = cD.OriginalResourceReferenceId,
                            },
                        },
                    })
                .ToList();

            return resourceReferenceAndCatalogueDTOsFlattened;

        }
    }
}
