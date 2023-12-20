// <copyright file="NotificationService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The notification service.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IMapper mapper;
        private readonly Settings settings;
        private INotificationRepository notificationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="notificationRepository">The notification repository.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="mapper">The mapper.</param>
        public NotificationService(
            INotificationRepository notificationRepository,
            IOptions<Settings> settings,
            IMapper mapper)
        {
            this.notificationRepository = notificationRepository;
            this.settings = settings.Value;
            this.mapper = mapper;
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<NotificationViewModel> GetByIdAsync(int id)
        {
            var notification = await this.notificationRepository.GetByIdAsync(id);

            return this.mapper.Map<NotificationViewModel>(notification);
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
        public async Task<PagedResultSet<NotificationViewModel>> GetPageAsync(int page, int pageSize, string sortColumn = "", string sortDirection = "", string filter = "")
        {
            var filterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(filter);

            PagedResultSet<NotificationViewModel> result = new PagedResultSet<NotificationViewModel>();

            var items = this.notificationRepository.GetAllFull()
                            .Where(n => new[] { NotificationTypeEnum.SystemUpdate, NotificationTypeEnum.SystemRelease }.Contains(n.NotificationTypeEnum));

            if (filterCriteria != null)
            {
                items = this.FilterItems(items, filterCriteria);
            }

            result.TotalItemCount = items.Count();

            items = this.OrderItems(items, sortColumn, sortDirection);

            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            result.Items = await this.mapper.ProjectTo<NotificationViewModel>(items).ToListAsync();

            return result;
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateAsync(int userId, NotificationViewModel notification)
        {
            var n = this.mapper.Map<Notification>(notification);

            var retVal = await this.ValidateAsync(n);

            if (retVal.IsValid)
            {
                retVal.CreatedId = await this.notificationRepository.CreateAsync(userId, n);
            }

            return retVal;
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateAsync(int userId, NotificationViewModel notification)
        {
            var n = this.mapper.Map<Notification>(notification);

            var retVal = await this.ValidateAsync(n);

            if (retVal.IsValid)
            {
                await this.notificationRepository.UpdateAsync(userId, n);
            }

            return retVal;
        }

        /// <summary>
        /// The validate async.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> ValidateAsync(Notification notification)
        {
            var notificationValidator = new NotificationValidator();
            var clientValidationResult = await notificationValidator.ValidateAsync(notification);

            var retVal = new LearningHubValidationResult(clientValidationResult);

            return retVal;
        }

        /// <summary>
        /// Creates resource access permisssion notification.
        /// </summary>
        /// <param name="userId">The current user id.</param>
        /// <param name="readOnlyUser">Is read only user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreatePermisssionNotificationAsync(int userId, bool readOnlyUser)
        {
            var title = this.settings.Notifications.ResourceAccessTitle;

            var message = (readOnlyUser ? this.settings.Notifications.ResourceReadonlyAccess : this.settings.Notifications.ResourceContributeAccess)
                                    .Replace("[SupportContact]", this.settings.SupportContact);

            var notification = await this.CreateAsync(userId, this.UserSpecificNotification(
                                    title, message, NotificationTypeEnum.UserPermission, NotificationPriorityEnum.Priority));

            if (notification.CreatedId.HasValue)
            {
                return notification.CreatedId.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Creates resource published notification.
        /// </summary>
        /// <param name="userId">The current user id.</param>
        /// <param name="resourceTitle">Notification title.</param>
        /// <param name="resourceReferenceId">Resource reference id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreateResourcePublishedNotificationAsync(int userId, string resourceTitle, int resourceReferenceId)
        {
            var title = this.settings.Notifications.ResourcePublishedTitle + resourceTitle;
            var message = this.settings.Notifications.ResourcePublished
                                    .Replace("[ResourceReferenceId]", resourceReferenceId.ToString());

            var notification = await this.CreateAsync(userId, this.UserSpecificNotification(
                                    title, message, NotificationTypeEnum.ResourcePublished, NotificationPriorityEnum.General));

            if (notification.CreatedId.HasValue)
            {
                return notification.CreatedId.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Creates resource publish failed notification.
        /// </summary>
        /// <param name="userId">The current user id.</param>
        /// <param name="resourceTitle">Resource title.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreatePublishFailedNotificationAsync(int userId, string resourceTitle, string errorMessage = "")
        {
            var title = this.settings.Notifications.ResourcePublishFailedTitle + resourceTitle;
            string message;
            if (errorMessage != string.Empty)
            {
                message = this.settings.Notifications.ResourcePublishFailedWithReason
                                            .Replace("[ResourceTitle]", resourceTitle)
                                            .Replace("[SupportContact]", this.settings.SupportContact)
                                            .Replace("[ErrorMessage]", errorMessage);
            }
            else
            {
                message = this.settings.Notifications.ResourcePublishFailed
                                            .Replace("[ResourceTitle]", resourceTitle)
                                            .Replace("[SupportContact]", this.settings.SupportContact);
            }

            var notification = await this.CreateAsync(userId, this.UserSpecificNotification(
                                    title, message, NotificationTypeEnum.PublishFailed, NotificationPriorityEnum.Priority));

            if (notification.CreatedId.HasValue)
            {
                return notification.CreatedId.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// The filter items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<Notification> FilterItems(IQueryable<Notification> items, List<PagingColumnFilter> filterCriteria)
        {
            if (filterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in filterCriteria)
            {
                switch (filter.Column)
                {
                    case "Title":
                        items = items.Where(l => l.Title.Contains(filter.Value));
                        break;
                    case "Message":
                        items = items.Where(l => l.Message.Contains(filter.Value));
                        break;
                    case "StartDate":
                        if (DateTime.TryParse(filter.Value, out var st))
                        {
                            items = items.Where(l => l.StartDate >= st && l.StartDate < st.AddDays(1));
                        }

                        break;
                    case "EndDate":
                        if (DateTime.TryParse(filter.Value, out var ed))
                        {
                            items = items.Where(l => l.EndDate >= ed && l.EndDate < ed.AddDays(1));
                        }

                        break;
                    case "CreatedBy":
                        items = items.Where(l => l.CreateUser.UserName.Contains(filter.Value));
                        break;
                    case "NotificationPriority":
                        if (int.TryParse(filter.Value, out var np))
                        {
                            items = items.Where(l => l.NotificationPriorityEnum == (NotificationPriorityEnum)np);
                        }

                        break;
                    case "NotificationType":
                        var types = filter.Value.Split(",")
                                        .Select(t => int.TryParse(t, out var nt) ? (NotificationTypeEnum)nt : default)
                                        .Where(t => t != default).ToList();
                        if (types.Count > 0)
                        {
                            items = items.Where(l => types.Contains(l.NotificationTypeEnum));
                        }

                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// The order items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        private IQueryable<Notification> OrderItems(IQueryable<Notification> items, string sortColumn, string sortDirection)
        {
            var descending = sortDirection == "D";

            switch (sortColumn)
            {
                case "Title":
                    items = descending ? items.OrderByDescending(l => l.Title) : items.OrderBy(l => l.Title);
                    break;
                case "StartDate":
                    items = descending ? items.OrderByDescending(l => l.StartDate) : items.OrderBy(l => l.StartDate);
                    break;
                case "EndDate":
                    items = descending ? items.OrderByDescending(l => l.EndDate) : items.OrderBy(l => l.EndDate);
                    break;
                case "CreatedBy":
                    items = descending ? items.OrderByDescending(l => l.CreateUser.UserName) : items.OrderBy(l => l.CreateUser.UserName);
                    break;
                case "NotificationPriority":
                    items = descending ? items.OrderByDescending(l => l.NotificationPriorityEnum) : items.OrderBy(l => l.NotificationPriorityEnum);
                    break;
                case "NotificationType":
                    items = descending ? items.OrderByDescending(l => l.NotificationTypeEnum) : items.OrderBy(l => l.NotificationTypeEnum);
                    break;
                default:
                    items = descending ? items.OrderByDescending(l => l.Id) : items.OrderBy(l => l.Id);
                    break;
            }

            return items;
        }

        private NotificationViewModel UserSpecificNotification(string title, string message, NotificationTypeEnum type, NotificationPriorityEnum priority)
        {
            return new NotificationViewModel
            {
                Title = title,
                Message = message,
                NotificationType = type,
                NotificationPriority = priority,
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.MaxValue,
                UserDismissable = true,
                IsUserSpecific = true,
            };
        }
    }
}
