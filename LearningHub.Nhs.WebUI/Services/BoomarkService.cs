namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;
    using LearningHub.Nhs.Shared.Interfaces.Http;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="BoomarkService" />.
    /// </summary>
    public class BoomarkService : BaseService<BoomarkService>, IBookmarkService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoomarkService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="openApiHttpClient">The openApiHttpClient<see cref="IOpenApiHttpClient"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{MyLearningService}"/>.</param>
        public BoomarkService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<BoomarkService> logger)
            : base(learningHubHttpClient, openApiHttpClient, logger)
        {
        }

        /// <inheritdoc/>
        public async Task<int> Create(UserBookmarkViewModel bookmarkViewModel)
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = "bookmark/Create";
            var content = new StringContent(
                JsonConvert.SerializeObject(bookmarkViewModel),
                Encoding.UTF8,
                "application/json");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            var bookmarkId = bookmarkViewModel.Id;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                bookmarkId = Convert.ToInt32(result);
            }

            return bookmarkId;
        }

        /// <inheritdoc/>
        public async Task<int> Edit(UserBookmarkViewModel bookmarkViewModel)
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = "bookmark/Edit";
            var content = new StringContent(
                JsonConvert.SerializeObject(bookmarkViewModel),
                Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync(request, content).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            var bookmarkId = bookmarkViewModel.Id;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                bookmarkId = Convert.ToInt32(result);
            }

            return bookmarkId;
        }

        /// <inheritdoc/>
        public async Task DeleteFolder(int bookmarkId)
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = $"bookmark/deletefolder/{bookmarkId}";
            var response = await client.DeleteAsync(request).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent(int? parentId, bool? all = false)
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = $"bookmark/GetAllByParent/{parentId}?all={all}";
            var response = await client.GetAsync(request).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<IEnumerable<UserBookmarkViewModel>>(result);
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<int> Toggle(UserBookmarkViewModel bookmarkViewModel)
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = "bookmark/toggle";
            var content = new StringContent(
                JsonConvert.SerializeObject(bookmarkViewModel),
                Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync(request, content).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            var bookmarkId = bookmarkViewModel.Id;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                bookmarkId = Convert.ToInt32(result);
            }

            return bookmarkId;
        }
    }
}
