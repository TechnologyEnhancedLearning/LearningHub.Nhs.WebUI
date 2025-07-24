namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="NotificationService" />.
    /// </summary>
    public class NotificationService : BaseService<NotificationService>, INotificationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="logger">Logger.</param>
        public NotificationService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<NotificationService> logger)
          : base(learningHubHttpClient, openApiHttpClient, logger)
        {
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <param name="userNotificationId">The userNotificationId<see cref="int"/>.</param>
        /// <param name="notificationId">The notificationId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Delete(int userNotificationId, int notificationId)
        {
            var userNotification = await this.GetUserNotificationIdAsync(userNotificationId);
            if (userNotification == null)
            {
                var response = await this.PostAsync(new UserNotification
                {
                    NotificationId = notificationId,
                    Dismissed = true,
                });
                var result = response.Content.ReadAsStringAsync().Result;
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (!apiResponse.ValidationResult.IsValid)
                {
                    var userNotificationDetails = await this.GetUserNotificationDetailsAsync(notificationId);
                    var notification = await this.GetUserNotificationIdAsync(userNotificationDetails.Id);
                    notification.Dismissed = true;
                    await this.PutAsync(notification);
                }
            }
            else
            {
                userNotification.Dismissed = true;
                await this.PutAsync(userNotification);
            }
        }

        /// <summary>
        /// The Dismiss.
        /// </summary>
        /// <param name="userNotificationId">The userNotificationId<see cref="int"/>.</param>
        /// <param name="notificationId">The notificationId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task MarkAsRead(int userNotificationId, int notificationId)
        {
            var userNotification = await this.GetUserNotificationIdAsync(userNotificationId);
            if (userNotification == null)
            {
                await this.PostAsync(new UserNotification
                {
                    NotificationId = notificationId,
                    ReadOnDate = DateTime.UtcNow,
                });
            }
            else
            {
                userNotification.ReadOnDate = DateTime.UtcNow;
                await this.PutAsync(userNotification);
            }
        }

        /// <summary>
        /// The GetPagedAsync.
        /// </summary>
        /// <param name="pagingRequestModel">Paging request.</param>
        /// <param name="priorityType">Notification priority type.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<PagedResultSet<UserNotificationViewModel>> GetPagedAsync(PagingRequestModel pagingRequestModel, NotificationPriorityEnum priorityType)
        {
            PagedResultSet<UserNotificationViewModel> viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var content = new StringContent(JsonConvert.SerializeObject(pagingRequestModel), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"UserNotification/GetPage/{(int)priorityType}", content)
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<UserNotificationViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetUserNotificationIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserNotification}"/>.</returns>
        public async Task<UserNotification> GetUserNotificationIdAsync(int id)
        {
            UserNotification viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"UserNotification/GetById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<UserNotification>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetUserNotificationDetailsAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserNotification}"/>.</returns>
        public async Task<UserNotification> GetUserNotificationDetailsAsync(int id)
        {
            UserNotification viewmodel = null;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"UserNotification/GetByIdAndUserId/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<UserNotification>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetUserUnreadNotificationCountAsync.
        /// </summary>
        /// <param name="userid">The userid<see cref="int"/>.</param>
        /// <returns>The <see cref="T:Task{int}"/>.</returns>
        public async Task<int> GetUserUnreadNotificationCountAsync(int userid)
        {
            int count = 0;

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"UserNotification/GetUserUnreadNotificationCount/{userid}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                count = JsonConvert.DeserializeObject<int>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return count;
        }

        /// <summary>
        /// The PutAsync.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        private async Task<HttpResponseMessage> PutAsync(UserNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"UserNotification/PutAsync";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    response.Content = new StringContent("Error: unable to delete notification");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return response;
        }

        /// <summary>
        /// The PostAsync.
        /// </summary>
        /// <param name="notification">The notification<see cref="UserNotification"/>.</param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/>.</returns>
        private async Task<HttpResponseMessage> PostAsync(UserNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"UserNotification/PostAsync";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return response;
        }
    }
}
