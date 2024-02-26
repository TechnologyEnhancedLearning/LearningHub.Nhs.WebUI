namespace LearningHub.Nhs.OpenApi.Services.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
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

        /// <summary>
        /// The get catalogue from resource reference method.
        /// </summary>
        /// <param name="resourceReference">The resourceReference.</param>
        /// <returns>The catalogue the resource reference is part of.</returns>
        public static CatalogueViewModel GetCatalogue(this ResourceReference resourceReference)
        {
            var catalogue = new CatalogueViewModel(0, NoCatalogueText, false);

            if (resourceReference.NodePath?.CatalogueNode?.CurrentNodeVersion?.CatalogueNodeVersion != null)
            {
                var catalogueNodeVersion = resourceReference.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion;
                catalogue = new CatalogueViewModel(catalogueNodeVersion);
            }

            return catalogue;
        }

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
    }
}
