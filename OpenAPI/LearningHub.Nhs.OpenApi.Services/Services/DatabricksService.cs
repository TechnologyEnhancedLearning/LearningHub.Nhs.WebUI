using LearningHub.Nhs.OpenApi.Models.Configuration;
using LearningHub.Nhs.OpenApi.Services.HttpClients;
using LearningHub.Nhs.OpenApi.Services.Interface.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LearningHub.Nhs.Models.Databricks;
using System.Linq;
using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
using LearningHub.Nhs.Models.Entities.DatabricksReport;
using AutoMapper;
using LearningHub.Nhs.Models.Common;
using Microsoft.EntityFrameworkCore;
using LearningHub.Nhs.OpenApi.Models.ViewModels;
using LearningHub.Nhs.Models.Enums;
using System.Text.Json;
using LearningHub.Nhs.Models.Entities;

namespace LearningHub.Nhs.OpenApi.Services.Services
{
    /// <summary>
    /// DatabricksService
    /// </summary>
    public class DatabricksService : IDatabricksService
    {
        private const string CacheKey = "DatabricksReporter";
        private readonly IOptions<DatabricksConfig> databricksConfig;
        private readonly IOptions<LearningHubConfig> learningHubConfig;
        private readonly IReportHistoryRepository reportHistoryRepository;
        private readonly IQueueCommunicatorService queueCommunicatorService;
        private readonly ICachingService cachingService;
        private readonly INotificationService notificationService;
        private readonly IUserNotificationService userNotificationService;
        private readonly IMoodleApiService moodleApiService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabricksService"/> class.
        /// </summary>
        /// <param name="databricksConfig">databricksConfig.</param>
        /// <param name="learningHubConfig">learningHubConfig.</param>
        /// <param name="reportHistoryRepository">reportHistoryRepository.</param>
        /// <param name="mapper">mapper.</param>
        /// <param name="queueCommunicatorService">queueCommunicatorService.</param>
        /// <param name="cachingService">cachingService.</param>
        /// <param name="notificationService">notificationService.</param>
        /// <param name="userNotificationService">userNotificationService.</param>
        /// <param name="moodleApiService">moodleApiService.</param>
        public DatabricksService(IOptions<DatabricksConfig> databricksConfig,IOptions<LearningHubConfig> learningHubConfig, IReportHistoryRepository reportHistoryRepository, IMapper mapper, IQueueCommunicatorService queueCommunicatorService, ICachingService cachingService, INotificationService notificationService, IUserNotificationService userNotificationService, IMoodleApiService moodleApiService)
        {
            this.databricksConfig = databricksConfig;
            this.learningHubConfig = learningHubConfig;
            this.reportHistoryRepository = reportHistoryRepository;
            this.mapper = mapper;
            this.queueCommunicatorService = queueCommunicatorService;
            this.cachingService = cachingService;
            this.notificationService = notificationService;
            this.userNotificationService = userNotificationService;
            this.moodleApiService = moodleApiService;
        }

        /// <inheritdoc/>
        public async Task<bool> IsUserReporter(int userId)
        {
            bool isReporter = false;
            string cacheKey = $"{userId}:{CacheKey}";
            try
            {
                var userReportPermission = await this.cachingService.GetAsync<bool>(cacheKey);
                if (userReportPermission.ResponseEnum == CacheReadResponseEnum.Found)
                {
                    return userReportPermission.Item;
                }


                DatabricksApiHttpClient databricksInstance = new DatabricksApiHttpClient(this.databricksConfig);

                var sqlText = $"CALL {this.databricksConfig.Value.UserPermissionEndpoint}({userId});";
                const string requestUrl = "/api/2.0/sql/statements";

                var requestPayload = new
                {
                    warehouse_id = this.databricksConfig.Value.WarehouseId,
                    statement = sqlText,
                    wait_timeout = "30s",
                    on_wait_timeout = "CANCEL"
                };

                var jsonBody = JsonConvert.SerializeObject(requestPayload);
                using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await databricksInstance.GetClient().PostAsync(requestUrl, content);

                var databricksResponse = await databricksInstance.GetClient().PostAsync(requestUrl, content);
                if (databricksResponse.StatusCode is not HttpStatusCode.OK)
                {
                    //log failure
                    return false;
                }
                var responseResult = await databricksResponse.Content.ReadAsStringAsync();

                responseResult = responseResult.Trim();
                var root = JsonDocument.Parse(responseResult).RootElement;
                string data = root.GetProperty("result").GetProperty("data_array")[0][0].GetString();
                isReporter = data == "1";

                await this.cachingService.SetAsync(cacheKey, isReporter);
                return isReporter;

            }
            catch
            {
                await this.cachingService.SetAsync(cacheKey, isReporter);
                return isReporter;
            }
        }

        /// <inheritdoc/>
        public async Task<DatabricksDetailedViewModel> CourseCompletionReport(int userId, DatabricksRequestModel model)
        {
        newEntry:
            if (model.ReportHistoryId == 0 && model.Take > 1)
            {
            
                bool timePeriodCheck = int.TryParse(model.TimePeriod, out int timePeriod);
                var reportHistory = new ReportHistory { CourseFilter = string.Join(",", model.Courses) ,StartDate = model.StartDate,EndDate =model.EndDate, PeriodDays = timePeriodCheck ? timePeriod : 0 ,
                FirstRun = DateTimeOffset.Now, LastRun = DateTimeOffset.Now, ReportStatusId = 2};
                model.ReportHistoryId = await AddReportHistory(userId, reportHistory);
            }
            else if(model.ReportHistoryId > 0 && model.Take > 1)
            {
                //get the existing values and compare
                var reportChecker = await GetPagedReportHistoryById(userId, model.ReportHistoryId);
                if (reportChecker != null) 
                {
                    if(reportChecker.CourseFilter == "all") { reportChecker.CourseFilter = string.Empty; }
                    if(reportChecker.CourseFilter != string.Join(",", model.Courses) || reportChecker.StartDate.GetValueOrDefault().Date != model.StartDate.GetValueOrDefault().Date || reportChecker.EndDate.GetValueOrDefault().Date != model.EndDate.GetValueOrDefault().Date)
                    {
                        model.ReportHistoryId = 0;
                        goto newEntry;
                    }
                }
                await UpdateReportLastRunTime(userId, model.ReportHistoryId);
            }

            DatabricksApiHttpClient databricksInstance = new DatabricksApiHttpClient(this.databricksConfig);

            const string requestUrl = "/api/2.0/sql/statements";

            var sql = $@"CALL {this.databricksConfig.Value.CourseCompletionEndpoint}(:par_adminId, :par_completionFlag, :par_locationId, :par_catalogueId, :par_learnerId, :par_courseId, :par_PageSize, :par_PageNumber, :par_Date_from, :par_Date_to);";


            var parameters = new List<KeyValuePair<string, object>>
            {
                new("par_adminId", userId),
                new("par_completionFlag", -1),
                new("par_locationId", -1),
                new("par_catalogueId", -1),
                new("par_learnerId", -1),
                new("par_courseId", model.Courses.Count < 1 ? string.Empty : string.Join(",",  model.Courses)),
                new("par_PageSize", model.Take),
                new("par_PageNumber", model.Skip),
                new("par_Date_from", model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd"): string.Empty),
                new("par_Date_to", model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd"): string.Empty),
            };

            var formattedParams = parameters.Select(p => new { name = p.Key, value = p.Value });

            var body = new
            {
                warehouse_id = this.databricksConfig.Value.WarehouseId,
                statement = sql,
                parameters = formattedParams,
                wait_timeout = "30s",
                on_wait_timeout = "CANCEL"
            };

            var json = JsonConvert.SerializeObject(body);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await databricksInstance.GetClient().PostAsync(requestUrl, content);

            var databricksResponse = await databricksInstance.GetClient().PostAsync(requestUrl, content);
            if (databricksResponse.StatusCode is not HttpStatusCode.OK)
            {
                //log failure
                return new DatabricksDetailedViewModel { ReportHistoryId = model.ReportHistoryId };
            }
            var responseResult = await databricksResponse.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<DatabricksResponse>(responseResult);
            if (result != null && result.Result.DataArray != null)
            {
                var records = MapDataArrayToCourseCompletionRecords(result.Result.DataArray);
                return new DatabricksDetailedViewModel { CourseCompletionRecords = records, ReportHistoryId = model.ReportHistoryId };

            }

            return new DatabricksDetailedViewModel { CourseCompletionRecords= new List<DatabricksDetailedItemViewModel>(), ReportHistoryId = model.ReportHistoryId };
        }

        /// <inheritdoc/>
        public async Task<PagedResultSet<ReportHistoryModel>> GetPagedReportHistory(int userId,int  page, int pageSize)
        {
            var result = new PagedResultSet<ReportHistoryModel>();
            var query = this.reportHistoryRepository.GetByUserIdAsync(userId);

            // Execute async count
            result.TotalItemCount = await query.CountAsync();
            try
            {
                // Execute async paging
                var pagedItems = await query
                    .OrderByDescending(x => x.LastRun)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                result.Items = mapper.Map<List<ReportHistoryModel>>(pagedItems);
            }
            catch(Exception e)
            {

            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ReportHistoryModel> GetPagedReportHistoryById(int userId, int reportHistoryId)
        {
            var result = new ReportHistoryModel();

            var reportHistory = await this.reportHistoryRepository.GetByIdAsync(reportHistoryId);
            if(reportHistory != null)
            {
                if(reportHistory.CreateUserId != userId)
                {
                    throw new Exception("Invalid Id");
                }
            }
            result = mapper.Map<ReportHistoryModel>(reportHistory);
            if(result != null && string.IsNullOrWhiteSpace(result.CourseFilter))
            {
                result.CourseFilter = "all";
            }

            return result;
            
        }

        /// <inheritdoc/>
        public async Task<bool> QueueReportDownload(int userId, int reportHistoryId)
        {
            var result = new ReportHistoryModel();

            var reportHistory = await this.reportHistoryRepository.GetByIdAsync(reportHistoryId);
            if (reportHistory != null && reportHistory.DownloadRequest == null)
            {
                if (reportHistory.CreateUserId != userId)
                {
                    throw new Exception("Invalid Id");
                }
                reportHistory.DownloadRequest = true;
                reportHistory.DownloadRequested = DateTimeOffset.Now;
            }
            else
            {
                throw new Exception("Invalid Id");
            }
            //call the job
            DatabricksApiHttpClient databricksInstance = new DatabricksApiHttpClient(this.databricksConfig);

            const string requestUrl = "/api/2.1/jobs/run-now";

            var body = new
            {
                job_id = this.databricksConfig.Value.JobId,
                notebook_params = new
                {
                    par_adminId = userId,
                    par_completionFlag = -1,
                    par_locationId = -1,
                    par_catalogueId = -1,
                    par_learnerId = -1,
                    par_courseId = reportHistory.CourseFilter,
                    par_PageSize = 0,
                    par_PageNumber = 0,
                    par_Date_from = reportHistory.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd"),
                    par_Date_to = reportHistory.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd"),
                    par_reportId = reportHistoryId
                }
            };

            var json = JsonConvert.SerializeObject(body);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var databricksResponse = await databricksInstance.GetClient().PostAsync(requestUrl, content);
            if (databricksResponse.StatusCode is not HttpStatusCode.OK)
            {
                reportHistory.ProcessingMessage = databricksResponse.ReasonPhrase;
                reportHistory.ReportStatusId = (int)Nhs.Models.Enums.Report.Status.Failed;
                await reportHistoryRepository.UpdateAsync(userId, reportHistory);
                return false;
            }
            var responseResult = await databricksResponse.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<dynamic>(responseResult);
            if (responseData != null) 
            {
                reportHistory.ReportStatusId = (int)Nhs.Models.Enums.Report.Status.Pending;
                reportHistory.ParentJobRunId = (long)responseData.run_id;
                await reportHistoryRepository.UpdateAsync(userId, reportHistory);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public async Task<ReportHistoryModel> DownloadReport(int userId, int reportHistoryId)
        {
            var response = new ReportHistoryModel();

            var reportHistory = await this.reportHistoryRepository.GetByIdAsync(reportHistoryId);
            if (reportHistory != null)
            {
                if (reportHistory.CreateUserId != userId)
                {
                    throw new Exception("Invalid Id");
                }
                reportHistory.DownloadedDate = DateTimeOffset.Now;
               await reportHistoryRepository.UpdateAsync(userId, reportHistory);
               response = mapper.Map<ReportHistoryModel>(reportHistory);
            }
            else
            {
                throw new Exception("Invalid Id");
            }
          
            return response;
        }

        /// <summary>
        /// DatabricksJobUpdate.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="databricksNotification"></param>
        /// <returns></returns>
        public async Task DatabricksJobUpdate(int userId, DatabricksNotification databricksNotification)
        {
            var reportHistory = await this.reportHistoryRepository.GetAll().FirstOrDefaultAsync(x=>x.ParentJobRunId == databricksNotification.Run.ParentRunId);
            if (reportHistory == null) { return; }
            reportHistory.JobRunId = databricksNotification.Run.RunId;
            if (!databricksNotification.EventType.Contains("success"))
            {
                reportHistory.ReportStatusId = (int)Nhs.Models.Enums.Report.Status.Failed;
                reportHistoryRepository.Update(userId, reportHistory);
                return;
            }
            reportHistoryRepository.Update(userId, reportHistory);
            
            await this.queueCommunicatorService.SendAsync(this.learningHubConfig.Value.DatabricksProcessingQueueName, databricksNotification.Run.RunId);
            return;
        }


        /// <summary>
        /// DatabricksJobUpdate.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="databricksUpdateRequest">databricksUpdateRequest.</param>
        /// <returns></returns>
        public async Task UpdateDatabricksReport(int userId, DatabricksUpdateRequest databricksUpdateRequest)
        {
            var reportHistory = await this.reportHistoryRepository.GetAll().FirstOrDefaultAsync(x => x.JobRunId == databricksUpdateRequest.RunId);
            if (reportHistory == null) { return; }
            if(string.IsNullOrWhiteSpace(databricksUpdateRequest.ProcessingMessage))
            {
                reportHistory.DownloadReady = DateTimeOffset.Now;
                reportHistory.FilePath = databricksUpdateRequest.FilePath;
                reportHistory.ReportStatusId = (int)Nhs.Models.Enums.Report.Status.Ready;
                //send notification
                string firstCourse = string.Empty;

                var courses = await moodleApiService.GetCoursesByCategoryIdAsync(learningHubConfig.Value.StatMandId);

                firstCourse = string.IsNullOrWhiteSpace(reportHistory.CourseFilter)
                    ? courses.Courses.Select(c => c.Displayname).FirstOrDefault()
                    : courses.Courses
                        .Where(c => reportHistory.CourseFilter.Contains(c.Id.ToString()))
                        .Select(c => c.Displayname)
                        .FirstOrDefault();


                var notificationId = await this.notificationService.CreateReportNotificationAsync(userId, "Course Completion", firstCourse);

                if (notificationId > 0)
                {
                    await this.userNotificationService.CreateAsync(userId, new UserNotification { UserId = reportHistory.CreateUserId, NotificationId = notificationId });
                }
            }
            else
            {
                reportHistory.ProcessingMessage = databricksUpdateRequest.ProcessingMessage;
                reportHistory.ReportStatusId = (int)Nhs.Models.Enums.Report.Status.Failed;
            }

            reportHistoryRepository.Update(userId, reportHistory);
            return;
        }


        private async Task<int> AddReportHistory(int userId,ReportHistory model)
        {
            return await reportHistoryRepository.CreateAsync(userId, model);
        }

        private async Task UpdateReportLastRunTime(int userId, int reportHistoryId)
        {
            var entry = await reportHistoryRepository.GetByIdAsync(reportHistoryId);
            entry.LastRun = DateTime.Now;
            await reportHistoryRepository.UpdateAsync(userId, entry);
        }

        /// <summary>
        /// MapDataArrayToCourseCompletionRecords.
        /// </summary>
        /// <param name="dataArray"></param>
        /// <returns></returns>
        public static List<DatabricksDetailedItemViewModel> MapDataArrayToCourseCompletionRecords(List<List<object>> dataArray)
        {
            var records = new List<DatabricksDetailedItemViewModel>();

            foreach (var row in dataArray)
            {
                if (row == null || row.Count < 19) continue;

                var record = new DatabricksDetailedItemViewModel
                {
                    UserName = row[0]?.ToString(),
                    FirstName = row[1]?.ToString(),
                    LastName = row[2]?.ToString(),
                    Email = row[3]?.ToString(),
                    Programme = row[4]?.ToString(),
                    Course = row[5]?.ToString(),
                    CourseStatus = row[6]?.ToString(),
                    Location = row[7]?.ToString(),
                    Role = row[8]?.ToString(),
                    Grade = row[9]?.ToString(),
                    MedicalCouncilNo = row[10]?.ToString(),
                    MedicalCouncilName = row[11]?.ToString(),
                    LastAccess = row[12]?.ToString(),
                    CourseCompletionDate = row[13]?.ToString(),
                    ReferenceType = row[14]?.ToString(),
                    ReferenceValue = row[15]?.ToString(),
                    PermissionType = row[16]?.ToString(),
                    MinValidDate = row[17]?.ToString(),
                    TotalRows = row[18] != null && int.TryParse(row[18].ToString(), out int totalRows) ? totalRows : 0
                };

                records.Add(record);
            }

            return records;
        }

    }

}
