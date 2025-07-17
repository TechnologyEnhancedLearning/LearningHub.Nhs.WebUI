namespace LearningHub.Nhs.WebUI.BlazorClient.Experiment
{
    using LearningHub.Nhs.WebUI.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class LearningHubWasmHttpClientExperiment : ILearningHubHttpClient
    {

        private readonly HttpClient _httpClient;

        public LearningHubWasmHttpClientExperiment(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string ApiUrl => throw new NotImplementedException();

        public Task<HttpClient> GetClientAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetDataAsync(string url)
        {
            return await _httpClient.GetStringAsync(url);
        }

    }
}
