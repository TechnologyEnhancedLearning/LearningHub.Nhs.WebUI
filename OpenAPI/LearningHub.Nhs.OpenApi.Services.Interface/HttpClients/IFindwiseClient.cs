namespace LearningHub.Nhs.OpenApi.Services.Interface.HttpClients
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;

    /// <summary>
    /// The FindWise Http Client interface.
    /// </summary>
    public interface IFindwiseClient : IDisposable
    {
        /// <summary>
        /// Sends search request to Findwise and returns resources found.
        /// </summary>
        /// <param name="searchRequestModel"><see cref="LearningHub.Nhs.Models.Search.SearchRequestModel"/>.</param>
        /// <returns>The results from Findwise.</returns>
        Task<FindwiseResultModel> Search(ResourceSearchRequest searchRequestModel);
    }
}
