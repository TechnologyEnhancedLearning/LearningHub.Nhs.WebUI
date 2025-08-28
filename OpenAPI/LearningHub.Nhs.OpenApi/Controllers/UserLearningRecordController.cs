namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.NHS.OpenAPI.Controllers;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// UserLearningRecord operations.
    /// </summary>
    [Route("UserLearningRecord")]
    [ApiController]
    [Authorize]
    public class UserLearningRecordController : OpenApiControllerBase
    {
    /// <summary>
    /// The MyLearning service.
    /// </summary>
    private readonly IUserLearningRecordService userLearningRecordService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserLearningRecordController"/> class.
    /// </summary>
    /// <param name="userLearningRecordService">
    /// The myLearning service.
    /// </param>
    public UserLearningRecordController(IUserLearningRecordService userLearningRecordService)
    {
      this.userLearningRecordService = userLearningRecordService;
    }

    /// <summary>
    /// Get UserHistory record by id.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="sortColumn">
    /// The sort column.
    /// </param>
    /// <param name="sortDirection">
    /// The sort direction.
    /// </param>
    /// <param name="presetFilter">
    /// The preset filter.
    /// </param>
    /// <param name="filter">
    /// The filter.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    [HttpGet]
    [Route("GetUserLearningRecords/{page}/{pageSize}/{sortColumn}/{sortDirection}/{presetFilter}/{filter}")]
    public async Task<IActionResult> GetUserLearningRecordsAsync(int page, int pageSize, string sortColumn, string sortDirection, string presetFilter, string filter)
    {
      PagedResultSet<MyLearningDetailedItemViewModel> pagedResultSet = await this.userLearningRecordService.GetUserLearningRecordsAsync(page, pageSize, sortColumn, sortDirection, presetFilter, filter);
      return this.Ok(pagedResultSet);
    }
  }
}
