namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Role;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Authorize]
    [Route("users/{userId:int}/roles")]
    public class UserRolesController  : OpenApiControllerBase
    {
        private readonly IUserRoleService service;

        public UserRolesController(IUserRoleService service) => this.service = service;

        [HttpGet]
        public async Task<IActionResult> GetForUser(int userId, CancellationToken ct)
        {
            var roles = await this.service.GetForUserAsync(userId, ct);
            return this.Ok(roles);
        }

        [HttpGet("~/roles/{roleId:int}/users")]
        public async Task<IActionResult> GetForRole(int roleId, CancellationToken ct)
        {
            var roles = await this.service.GetForRoleAsync(roleId, ct);
            return this.Ok(roles);
        }

        [HttpGet("{assignmentId:int}")]
        public async Task<IActionResult> Get(int userId, int assignmentId, CancellationToken ct)
        {
            var assignment = await this.service.GetAsync(userId, assignmentId, ct);
            return this.Ok(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(
            int userId, [FromBody] UserRoleCreateViewModel request, CancellationToken ct)
        {
            var assignment = await this.service.AssignAsync(this.CurrentUserId.GetValueOrDefault(), userId, request, ct);
            return this.CreatedAtAction(nameof(this.Get), new { userId, assignmentId = assignment.Id }, assignment);
        }

        [HttpPut("{assignmentId:int}/scope")]
        public async Task<IActionResult> UpdateScope(
            int userId, int assignmentId, [FromBody] UserRoleScopeUpdateViewModel request, CancellationToken ct)
        {
            var assignment = await this.service.UpdateScopeAsync(this.CurrentUserId.GetValueOrDefault(), userId, assignmentId, request, ct);
            return this.Ok(assignment);
        }

        [HttpDelete("{assignmentId:int}")]
        public async Task<IActionResult> Unassign(int userId, int assignmentId, CancellationToken ct)
        {
            await this.service.UnassignAsync(this.CurrentUserId.GetValueOrDefault(), userId, assignmentId, ct);
            return this.NoContent();
        }
    }
}