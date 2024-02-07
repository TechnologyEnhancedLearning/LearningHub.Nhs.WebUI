// <copyright file="UserLearningRecordController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
  using System.Threading.Tasks;
  using elfhHub.Nhs.Models.Common;
  using LearningHub.Nhs.Models.Common;
  using LearningHub.Nhs.Models.MyLearning;
  using LearningHub.Nhs.Services.Interface;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// UserLearningRecord operations.
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  //// [Authorize]
  public class UserLearningRecordController : ApiControllerBase
  {
    /// <summary>
    /// The MyLearning service.
    /// </summary>
    private readonly IUserLearningRecordService userLearningRecordService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserLearningRecordController"/> class.
    /// </summary>
    /// <param name="userService">
    /// The user service.
    /// </param>
    /// <param name="userLearningRecordService">
    /// The myLearning service.
    /// </param>
    /// <param name="logger">The logger.</param>
    public UserLearningRecordController(
        IUserService userService,
        IUserLearningRecordService userLearningRecordService,
        ILogger<MyLearningController> logger)
        : base(userService, logger)
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
