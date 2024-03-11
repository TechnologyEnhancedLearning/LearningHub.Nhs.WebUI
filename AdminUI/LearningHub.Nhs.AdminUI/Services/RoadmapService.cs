namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Entities;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="RoadmapService" />.
    /// </summary>
    internal class RoadmapService : BaseService, IRoadmapService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoadmapService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        public RoadmapService(ILearningHubHttpClient learningHubHttpClient)
        : base(learningHubHttpClient)
        {
        }

        /// <summary>
        /// The AddRoadmap.
        /// </summary>
        /// <param name="roadmap">The roadmap<see cref="Roadmap"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<int> AddRoadmap(Roadmap roadmap)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = "Roadmap/Roadmaps";
            var content = new StringContent(
                JsonConvert.SerializeObject(roadmap),
                Encoding.UTF8,
                "application/json");
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return int.Parse(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return 0;
        }

        /// <summary>
        /// The DeleteRoadmapAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteRoadmapAsync(int id)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = "Roadmap/Roadmaps/" + id;

            var response = await client.DeleteAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The GetRoadmap.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{Roadmap}"/>.</returns>
        public async Task<Roadmap> GetRoadmap(int id)
        {
            var viewmodel = new Roadmap();
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Roadmap/Roadmaps/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<Roadmap>(result);
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
        /// The GetUpdates.
        /// </summary>
        /// <returns>The <see cref="List{Roadmap}"/>.</returns>
        public async Task<List<Roadmap>> GetUpdates()
        {
            var viewmodel = new List<Roadmap>();

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = "Roadmap/Updates";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<Roadmap>>(result);
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
        /// The UpdateRoadmap.
        /// </summary>
        /// <param name="roadmap">The roadmap<see cref="Roadmap"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateRoadmap(Roadmap roadmap)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = "Roadmap/Roadmaps";
            var content = new StringContent(
                JsonConvert.SerializeObject(roadmap),
                Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync(request, content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }
    }
}
