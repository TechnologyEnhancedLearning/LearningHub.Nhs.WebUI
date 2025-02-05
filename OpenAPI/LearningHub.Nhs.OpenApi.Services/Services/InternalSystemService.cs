namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Entities.Maintenance;
    using LearningHub.Nhs.Models.Maintenance;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Maintenance;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;

    /// <summary>
    /// The Internal System Service.
    /// </summary>
    public class InternalSystemService : IInternalSystemService
    {
        private const string CacheKey = "InternalSystems";

        private readonly IMapper mapper;

        private readonly ICachingService cachingService;

        private readonly IInternalSystemRepository internalSystemRepository;

        private readonly IQueueCommunicatorService queueCommunicatorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSystemService"/> class.
        /// </summary>
        /// <param name="internalSystemRepository">The internal system repository.</param>
        /// <param name="queueCommunicatorService">The queueCommunicatorService.</param>
        /// <param name="cachingService">The caching service.</param>
        /// <param name="mapper">The mapper.</param>
        public InternalSystemService(
            IInternalSystemRepository internalSystemRepository,
            IQueueCommunicatorService queueCommunicatorService,
            ICachingService cachingService,
            IMapper mapper)
        {
            this.internalSystemRepository = internalSystemRepository;
            this.queueCommunicatorService = queueCommunicatorService;
            this.cachingService = cachingService;
            this.mapper = mapper;
        }

        /// <summary>
        /// The get all async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<List<InternalSystemViewModel>> GetAllAsync()
        {
            var cachedInternalSystems = await cachingService.GetAsync<List<InternalSystem>>(CacheKey);
            if (cachedInternalSystems != null && cachedInternalSystems.Item != null)
            {
                return mapper.Map<List<InternalSystemViewModel>>(cachedInternalSystems.Item);
            }

            var internalSystems = internalSystemRepository.GetAll().ToList();
            await cachingService.SetAsync(CacheKey, internalSystems);

            return mapper.Map<List<InternalSystemViewModel>>(internalSystems);
        }

        /// <summary>
        /// Gets InternalSystem by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task<InternalSystemViewModel> GetByIdAsync(int id)
        {
            var allItems = await GetAllAsync();
            return allItems.Single(x => x.Id == id);
        }

        /// <summary>
        /// Toggles the internalSystem offline status.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task<InternalSystemViewModel> ToggleOfflineStatusAsync(int id, int userId)
        {
            await cachingService.RemoveAsync(CacheKey);
            var internalSystem = internalSystemRepository.GetAll().Single(x => x.Id == id);
            internalSystem.IsOffline = !internalSystem.IsOffline;

            if (!internalSystem.IsOffline && internalSystem.Name.ToLower().Contains("queue"))
            {
                await queueCommunicatorService.TransferMessagesAsync($"{internalSystem.Name}-temp", internalSystem.Name);
            }

            await internalSystemRepository.UpdateAsync(userId, internalSystem);
            return mapper.Map<InternalSystemViewModel>(internalSystem);
        }
    }
}
