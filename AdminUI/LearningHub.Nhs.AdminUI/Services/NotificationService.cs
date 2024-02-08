namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Paging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="NotificationService" />.
    /// </summary>
    public class NotificationService : BaseService, INotificationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        public NotificationService(ILearningHubHttpClient learningHubHttpClient)
        : base(learningHubHttpClient)
        {
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="notification">The notification<see cref="NotificationViewModel"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<int> Create(NotificationViewModel notification)
        {
            int createId = 0;
            var json = JsonConvert.SerializeObject(notification);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Notification/PostAsync";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }

                createId = apiResponse.ValidationResult.CreatedId ?? 0;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return createId;
        }

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="notification">The notification<see cref="NotificationViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Edit(NotificationViewModel notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Notification/PutAsync";
            var response = await client.PutAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The GetIdAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{NotificationViewModel}"/>.</returns>
        public async Task<NotificationViewModel> GetIdAsync(int id)
        {
            NotificationViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Notification/GetById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<NotificationViewModel>(result);
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
        /// Copy existing notification.
        /// </summary>
        /// <param name="id">The notification id.</param>
        /// <returns>New niotification id.</returns>
        public async Task<int> CopyAsync(int id)
        {
            var notification = await this.GetIdAsync(id);
            notification.Id = 0;

            return await this.Create(notification);
        }

        /// <summary>
        /// Deletes a notification.
        /// </summary>
        /// <param name="id">The notification id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int id)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var response = await client.DeleteAsync($"Notification/delete/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("delete failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The GetPagedAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{NotificationViewModel}"/>.</returns>
        public async Task<PagedResultSet<NotificationViewModel>> GetPagedAsync(PagingRequestModel pagingRequestModel)
        {
            PagedResultSet<NotificationViewModel> viewmodel = null;

            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Notification/GetFilteredPage"
                + $"/{pagingRequestModel.Page}"
                + $"/{pagingRequestModel.PageSize}"
                + $"/{pagingRequestModel.SortColumn}"
                + $"/{pagingRequestModel.SortDirection}"
                + $"/{Uri.EscapeUriString(filter)}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<NotificationViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }
    }
}
