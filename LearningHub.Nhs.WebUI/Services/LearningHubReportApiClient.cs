// <copyright file="LearningHubReportApiClient.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums.Report;
    using LearningHub.Nhs.Models.Report;
    using LearningHub.Nhs.Models.Report.ReportCreate;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// LearningHubReportApiClient.
    /// </summary>
    public class LearningHubReportApiClient : ILearningHubReportApiClient
    {
        private const string V = "'< div style = 'width: 100%; font-size: 10px; margin: 0 1cm; color: #bbb; height: 30px; text-align: right;' >< span class='pageNumber' style='font-size: 10px;'></span> / <span class='totalPages' style='font-size: 10px'></span></div>'";
        private readonly HttpClient client;
        private readonly ILogger<ILearningHubReportApiClient> logger;
        private readonly string leaningHubReportApiClientId;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubReportApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">httpClient.</param>
        /// <param name="logger">logger.</param>
        /// <param name="config">config.</param>
        public LearningHubReportApiClient(HttpClient httpClient, ILogger<ILearningHubReportApiClient> logger, IConfiguration config)
        {
            string learningHubReportAPIBaseUrl = "LearningHubReportAPIConfig:BaseUrl";
            string learningHubReportAPIClientId = "LearningHubReportAPIConfig:ClientId";
            string learningHubReportAPIClientIdentityKey = "LearningHubReportAPIConfig:ClientIdentityKey";
            string learningHubReportApiBaseUrl = config[learningHubReportAPIBaseUrl];
            this.leaningHubReportApiClientId = config[learningHubReportAPIClientId];
            string leaningHubReportApiClientIdentityKey = config[learningHubReportAPIClientIdentityKey];
            this.client = httpClient;
            this.client.BaseAddress = new Uri(learningHubReportApiBaseUrl);
            this.client.DefaultRequestHeaders.Accept.Clear();
            this.client.DefaultRequestHeaders.Add("ClientIdentityKey", leaningHubReportApiClientIdentityKey);
            this.logger = logger;
        }

        /// <summary>
        /// Call to generate Pdf.
        /// </summary>
        /// <param name="strHTML">strHTML.</param>
        /// <param name="userId">userId.</param>
        /// <returns>Api response.</returns>
        public async Task<ReportModel> PdfReport(string strHTML, int userId)
        {
            RequestModel requestModel = new RequestModel();
            ReportCreateModel reportCreateModel = new ReportCreateModel();
            List<ReportPageModel> reportPageModels = new List<ReportPageModel>();
            ReportPageModel reportPageModel = new ReportPageModel();
            ReportModel pdfReportResponse = new ReportModel();
            reportCreateModel.Name = "Report";
            reportCreateModel.ReportTypeId = (int)ReportType.LearningHubCertificate;
            reportPageModel.Html = strHTML;
            reportPageModel.ReportOrientationModeId = (int)OrientationMode.Landscape;
            reportPageModels.Add(reportPageModel);
            reportCreateModel.ReportPages = reportPageModels;
            requestModel.ClientId = new Guid(this.leaningHubReportApiClientId);
            requestModel.UserId = userId;
            requestModel.ReportCreateModel = reportCreateModel;
            var json = JsonConvert.SerializeObject(requestModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json-patch+json");
            var response = await this.client.PostAsync($"PdfReport", stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                pdfReportResponse = JsonConvert.DeserializeObject<ReportModel>(result);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized
                ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return pdfReportResponse;
        }

        /// <summary>
        /// Call to generate Pdf.
        /// </summary>
        /// <param name="lstHTML">lstHTML.</param>
        /// <param name="userId">userId.</param>
        /// <returns>Api response.</returns>
        public async Task<ReportModel> PdfReport(List<string> lstHTML, int userId)
        {
            RequestModel requestModel = new RequestModel();
            ReportCreateModel reportCreateModel = new ReportCreateModel();
            List<ReportPageModel> reportPageModels = new List<ReportPageModel>();
            ReportPageModel reportPageModel = null;
            ReportModel pdfReportResponse = new ReportModel();
            foreach (var strHTML in lstHTML)
            {
                reportPageModel = new ReportPageModel();
                reportCreateModel.Name = "Report";
                reportCreateModel.ReportTypeId = (int)ReportType.LearningHubCertificate;
                reportPageModel.Html = strHTML;
                reportPageModel.ReportOrientationModeId = (int)OrientationMode.Landscape;
                reportPageModels.Add(reportPageModel);
            }

            reportCreateModel.ReportPages = reportPageModels;
            requestModel.ClientId = new Guid(this.leaningHubReportApiClientId);
            requestModel.UserId = userId;
            requestModel.ReportCreateModel = reportCreateModel;
            var json = JsonConvert.SerializeObject(requestModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json-patch+json");
            var response = await this.client.PostAsync($"PdfReport", stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                pdfReportResponse = JsonConvert.DeserializeObject<ReportModel>(result);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized
                ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return pdfReportResponse;
        }

        /// <summary>
        /// Call to check if report is ready.
        /// </summary>
        /// <param name="pdfReportResponse">pdf Report Response.</param>
        /// <returns>Return PDF report status.</returns>
        public async Task<ReportStatusModel> PdfReportStatus(ReportModel pdfReportResponse)
        {
            ReportStatusModel pdfReportStatusResponse = new ReportStatusModel();
            var response = await this.client.GetAsync($"PdfReport/PdfReportStatus/{pdfReportResponse.FileName}/{pdfReportResponse.Hash}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                pdfReportStatusResponse = JsonConvert.DeserializeObject<ReportStatusModel>(result);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized
               ||
               response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return pdfReportStatusResponse;
        }

        /// <summary>
        /// GetPdfReportFile.
        /// </summary>
        /// <param name="pdfReportResponse">pdfReportResponse.</param>
        /// <returns>Returns PDF file.</returns>
        public async Task<byte[]> GetPdfReportFile(ReportModel pdfReportResponse)
        {
            ReportStatusModel pdfReportStatusResponse = new ReportStatusModel();
            var response = await this.client.GetAsync($"PdfReport/PdfReportFile/{pdfReportResponse.FileName}/{pdfReportResponse.Hash}").ConfigureAwait(false);
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
