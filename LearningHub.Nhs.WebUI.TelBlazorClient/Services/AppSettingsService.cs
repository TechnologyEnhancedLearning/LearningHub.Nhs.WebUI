using LearningHub.Nhs.WebUI.TelBlazorClient.Configuration;
using LearningHub.Nhs.Models.Shared;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LearningHub.Nhs.WebUI.TelBlazorClient.Services
{
    public class AppSettingsService
    {
        private readonly HttpClient _httpClient;
        private readonly Settings _settings;

        public AppSettingsService(HttpClient httpClient, Settings settings)
        {
            _httpClient = httpClient;
            _settings = settings;

            // Configure HttpClient defaults
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _settings.ApiKey);
        }

        public async Task<List<string>> GetAutoSuggestionsAsync(string term)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Search/GetAutoSuggestionResult/{Uri.EscapeDataString(term)}");

                //var viewModel = new AutoSuggestionModel();
                //if (response.IsSuccessStatusCode)
                //{
                //    var result1 = response.Content.ReadAsStringAsync().Result;
                //    viewModel = JsonConvert.DeserializeObject<AutoSuggestionModel>(result1);
                //}

                var result = await response.Content.ReadFromJsonAsync<List<string>>();
                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ApiService] Error fetching suggestions: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
