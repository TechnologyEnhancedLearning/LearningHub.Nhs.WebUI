// <copyright file="RoadmapService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.RoadMap;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The roadmap service.
    /// </summary>
    public class RoadmapService : IRoadmapService
    {
        private readonly IRoadmapRepository roadmapRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoadmapService"/> class.
        /// </summary>
        /// <param name="roadmapRepository">The roadmap repository.</param>
        /// <param name="mapper">mapper.</param>
        public RoadmapService(IRoadmapRepository roadmapRepository, IMapper mapper)
        {
            this.roadmapRepository = roadmapRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// The add roadmap async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roadmap">The roadmap.</param>
        /// <returns>The roadmap id.</returns>
        public async Task<int> AddRoadmapAsync(int userId, Roadmap roadmap)
        {
            var roadmapId = await this.roadmapRepository.CreateAsync(userId, roadmap);
            if (roadmap.RoadmapTypeId == 1)
            { // Updates
                var updates = this.GetUpdates();
                var orderedUpdates = updates.OrderByDescending(x => x.RoadmapDate.Value)
                    .Select(x => x.Id).ToList();
                var roadmapOrdering = new RoadmapOrdering { OrderedIds = orderedUpdates, RoadmapType = 1 };
                await this.UpdateOrderingAsync(userId, roadmapOrdering);
            }

            return roadmapId;
        }

        /// <summary>
        /// The update roadmap async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roadmap">The roadmap.</param>
        /// <returns>The task.</returns>
        public async Task UpdateRoadmapAsync(int userId, Roadmap roadmap)
        {
            await this.roadmapRepository.UpdateAsync(userId, roadmap);
            if (roadmap.RoadmapTypeId == 1)
            { // Updates
                var updates = this.GetUpdates();
                var orderedUpdates = updates.OrderByDescending(x => x.RoadmapDate.Value)
                    .Select(x => x.Id).ToList();
                var roadmapOrdering = new RoadmapOrdering { OrderedIds = orderedUpdates, RoadmapType = 1 };
                await this.UpdateOrderingAsync(userId, roadmapOrdering);
            }
        }

        /// <summary>
        /// The get updates.
        /// </summary>
        /// <returns>The updates.</returns>
        public List<Roadmap> GetUpdates()
        {
            return this.roadmapRepository.GetUpdates().OrderBy(x => x.OrderNumber).ToList();
        }

        /// <summary>
        /// The get updates.
        /// </summary>
        /// <param name="numberOfResults">numberOfResults.</param>
        /// <returns>The updates.</returns>
        public RoadMapResponseViewModel GetUpdates(int numberOfResults)
        {
            var roadMapItems = this.GetUpdates().Where(x => x.Published);
            var roadMapResponse = new RoadMapResponseViewModel
            {
                RoadMapItems = this.mapper.Map<List<RoadMapViewModel>>(roadMapItems.Take(numberOfResults)),
                TotalRecords = roadMapItems.Count(),
            };

            return roadMapResponse;
        }

        /// <summary>
        /// The get roadmap.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The roadmap.</returns>
        public async Task<Roadmap> GetRoadmap(int id)
        {
            return await this.roadmapRepository.GetAll().SingleAsync(x => x.Id == id);
        }

        /// <summary>
        /// The delete roadmap.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="id">The roadmap id.</param>
        /// <returns>The task.</returns>
        public async Task DeleteRoadmap(int userId, int id)
        {
            var roadmap = await this.GetRoadmap(id);
            roadmap.Deleted = true;
            await this.roadmapRepository.UpdateAsync(userId, roadmap);
        }

        /// <summary>
        /// The Update Ordering async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roadmapOrdering">The roadmap ordering.</param>
        /// <returns>The task.</returns>
        public async Task UpdateOrderingAsync(int userId, RoadmapOrdering roadmapOrdering)
        {
            var roadmaps = this.roadmapRepository.GetAll().Where(x => x.RoadmapTypeId == roadmapOrdering.RoadmapType);
            for (var i = 1; i <= roadmapOrdering.OrderedIds.Count; i++)
            {
                var id = roadmapOrdering.OrderedIds[i - 1];
                var roadmap = roadmaps.SingleOrDefault(x => x.Id == id);
                if (roadmap == null)
                {
                    throw new Exception($"Roadmap not found for id '{id}'");
                }

                if (i != roadmap.OrderNumber)
                {
                    roadmap.OrderNumber = i;
                    await this.roadmapRepository.UpdateAsync(userId, roadmap);
                }
            }
        }
    }
}
