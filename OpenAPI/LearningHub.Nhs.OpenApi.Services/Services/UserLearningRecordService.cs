namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The rating service.
    /// </summary>
    public class UserLearningRecordService : IUserLearningRecordService
    {
        /// <summary>
        /// The resourceActivityRepository.
        /// </summary>
        private readonly IResourceActivityRepository resourceActivityRepository;
        private readonly ICatalogueNodeVersionRepository catalogueNodeVersionRepository;

        /// <summary>
        /// The mediaResourcePlayedSegmentRepository.
        /// </summary>
        private readonly IMediaResourcePlayedSegmentRepository mediaResourcePlayedSegmentRepository;

        /// <summary>
        /// The assessment resource activity repository.
        /// </summary>
        private readonly IAssessmentResourceActivityRepository assessmentResourceActivityRepository;

        /// <summary>
        /// The assessment resource activity interaction repository.
        /// </summary>
        private readonly IAssessmentResourceActivityInteractionRepository assessmentResourceActivityInteractionRepository;
        private readonly IMyLearningService myLearningService;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly IOptions<LearningHubConfig> settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLearningRecordService"/> class.
        /// </summary>
        /// <param name="resourceActivityRepository">The resource activity repository.</param>
        /// <param name="catalogueNodeVersionRepository">The catalogue node repository.</param>
        /// <param name="mediaResourcePlayedSegmentRepository">The mediaResourcePlayedSegmentRepository.</param>
        /// <param name="assessmentResourceActivityRepository">The assessmentResourceActivityRepository.</param>
        /// <param name="assessmentResourceActivityInteractionRepository">The assessmentResourceActivityInteractionRepository.</param>
        /// <param name="myLearningService">The myLearningService.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="settings">The settings.</param>
        public UserLearningRecordService(
            IResourceActivityRepository resourceActivityRepository,
            IMediaResourcePlayedSegmentRepository mediaResourcePlayedSegmentRepository,
            IAssessmentResourceActivityRepository assessmentResourceActivityRepository,
            IAssessmentResourceActivityInteractionRepository assessmentResourceActivityInteractionRepository,
            ICatalogueNodeVersionRepository catalogueNodeVersionRepository,
            IMyLearningService myLearningService,
            IMapper mapper,
            IOptions<LearningHubConfig> settings)
        {
            this.resourceActivityRepository = resourceActivityRepository;
            this.mediaResourcePlayedSegmentRepository = mediaResourcePlayedSegmentRepository;
            this.assessmentResourceActivityRepository = assessmentResourceActivityRepository;
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
            this.assessmentResourceActivityInteractionRepository = assessmentResourceActivityInteractionRepository;
            this.myLearningService = myLearningService;
            this.mapper = mapper;
            this.settings = settings;
        }

        /// <summary>
        /// GetUserLearningRecordsAsync.
        /// </summary>
        /// <param name="page">page.</param>
        /// <param name="pageSize">pageSize.</param>
        /// <param name="sortColumn">sortColumn.</param>
        /// <param name="sortDirection">sortDirection.</param>
        /// <param name="presetFilter">presetFilter.</param>
        /// <param name="filter">filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PagedResultSet<MyLearningDetailedItemViewModel>> GetUserLearningRecordsAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string presetFilter = "", string filter = "")
        {
            try
            {
                PagedResultSet<MyLearningDetailedItemViewModel> result = new PagedResultSet<MyLearningDetailedItemViewModel>();
                var presetFilterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(presetFilter);
                var userIdFilter = presetFilterCriteria.Where(f => f.Column == "userid").FirstOrDefault();

                MyLearningRequestModel requestModel = new MyLearningRequestModel();
                requestModel.Skip = (page - 1) * pageSize;
                requestModel.Take = pageSize;
                var activityQuery = resourceActivityRepository.GetByUserIdFromSP(int.Parse(userIdFilter.Value), requestModel, settings.Value.DetailedMediaActivityRecordingStartDate).Result.OrderByDescending(r => r.ActivityStart).DistinctBy(l => l.Id);
                MyLearningDetailedViewModel viewModel = new MyLearningDetailedViewModel()
                {
                    TotalCount = resourceActivityRepository.GetTotalCount(int.Parse(userIdFilter.Value), requestModel, settings.Value.DetailedMediaActivityRecordingStartDate),
                };

                var activityEntities = activityQuery.ToList();

                viewModel.Activities = await myLearningService.PopulateMyLearningDetailedItemViewModels(activityEntities, int.Parse(userIdFilter.Value));
                if (userIdFilter != null)
                {
                    result.Items = viewModel.Activities; /*this.mapper.Map<List<UserLearningRecordViewModel>>(viewModel.Activities);*/
                    result.TotalItemCount = viewModel.TotalCount;
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}