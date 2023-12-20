// <copyright file="LogService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
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
    using LearningHub.Nhs.Models.Log;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    /// <summary>
    /// The log service.
    /// </summary>
    public class LogService : ILogService
    {
        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The log repository.
        /// </summary>
        private ILogRepository logRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogService"/> class.
        /// </summary>
        /// <param name="logRepository">The log repository.</param>
        /// <param name="mapper">The mapper.</param>
        public LogService(
            ILogRepository logRepository,
            IMapper mapper)
        {
            this.logRepository = logRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LogViewModel> GetByIdAsync(int id)
        {
            var log = await this.logRepository.GetByIdAsync(id);

            return this.mapper.Map<LogViewModel>(log);
        }

        /// <summary>
        /// The get page async.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PagedResultSet<LogBasicViewModel>> GetPageAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string filter = "")
        {
            var filterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(filter);

            PagedResultSet<LogBasicViewModel> result = new PagedResultSet<LogBasicViewModel>();

            var items = this.logRepository.GetAll();

            items = this.FilterItems(items, filterCriteria);

            result.TotalItemCount = items.Count();

            items = this.OrderItemsItems(items, sortColumn, sortDirection);

            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            var test = items.ToList();

            result.Items = await this.mapper.ProjectTo<LogBasicViewModel>(items).ToListAsync();

            return result;
        }

        /// <summary>
        /// The filter items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<Log> FilterItems(IQueryable<Log> items, List<PagingColumnFilter> filterCriteria)
        {
            if (filterCriteria == null || filterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in filterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "id":
                        int enteredId = 0;
                        int.TryParse(filter.Value, out enteredId);
                        items = items.Where(l => l.Id == enteredId);
                        break;
                    case "application":
                        items = items.Where(x => x.Application.Contains(filter.Value));
                        break;
                    case "logged":
                        DateTime enteredDate = DateTime.MinValue;
                        DateTime.TryParse(filter.Value, out enteredDate);

                        if (enteredDate != DateTime.MinValue)
                        {
                            items = items.Where(l => l.Logged >= enteredDate && l.Logged < enteredDate.AddDays(1));
                        }

                        break;
                    case "level":
                        items = items.Where(l => l.Level.Contains(filter.Value));
                        break;
                    case "message":
                        items = items.Where(l => l.Message.Contains(filter.Value));
                        break;
                    case "logger":
                        items = items.Where(l => l.Logger.Contains(filter.Value));
                        break;
                    case "callsite":
                        items = items.Where(l => l.Callsite.Contains(filter.Value));
                        break;
                    case "exception":
                        items = items.Where(l => l.Exception.Contains(filter.Value));
                        break;
                    case "username":
                        items = items.Where(l => l.UserName.Contains(filter.Value));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// The order items items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<Log> OrderItemsItems(IQueryable<Log> items, string sortColumn, string sortDirection)
        {
            switch (sortColumn.ToLower())
            {
                case "application":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Application);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Application);
                    }

                    break;
                case "logged":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Logged);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Logged);
                    }

                    break;
                case "level":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Level);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Level);
                    }

                    break;
                case "message":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Message);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Message);
                    }

                    break;
                case "logger":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Logger);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Logger);
                    }

                    break;
                case "callsite":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Callsite);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Callsite);
                    }

                    break;
                case "exception":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Exception);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Exception);
                    }

                    break;
                case "username":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.UserName);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.UserName);
                    }

                    break;
                default:
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(l => l.Id);
                    }
                    else
                    {
                        items = items.OrderBy(l => l.Id);
                    }

                    break;
            }

            return items;
        }
    }
}
