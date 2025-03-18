namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Search;

    /// <summary>
    /// The IFindwiseApiFacade.
    /// </summary>
    public interface IFindwiseApiFacade
    {
        /// <summary>
        /// Modifies the information Findwise has for the catalogues provided.
        /// Documents not in Findwise will be added.
        /// Documents that already exist in Findwise will be replaced.
        /// </summary>
        /// <param name="catalogues">The catalogues to add/replace in the index.</param>
        /// <returns>The task.</returns>
        Task AddOrReplaceAsync(List<SearchCatalogueRequestModel> catalogues);

        /// <summary>
        /// Modifies the information Findwise has for the resources provided.
        /// Documents not in Findwise will be added.
        /// Documents that already exist in Findwise will be replaced.
        /// </summary>
        /// <param name="resources">The resources to add/replace in the index.</param>
        /// <returns>The task.</returns>
        Task AddOrReplaceAsync(List<SearchResourceRequestModel> resources);

        /// <summary>
        /// Removes the documents from Findwise.
        /// </summary>
        /// <param name="resources">The resources to remove from Findwise.</param>
        /// <returns>The task.</returns>
        Task RemoveAsync(List<SearchResourceRequestModel> resources);
    }
}
