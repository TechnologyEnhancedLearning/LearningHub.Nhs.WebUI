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
    [Route("Roles")]
    public class RolesController : OpenApiControllerBase
    {
        private readonly IRoleService service;

        public RolesController(IRoleService service) => this.service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var roles = await this.service.GetAllAsync(ct);
            return this.Ok(roles);
        }

        [HttpGet("{roleId:int}")]
        public async Task<IActionResult> GetById(int roleId, CancellationToken ct)
        {
            var role = await this.service.GetByIdAsync(roleId, ct);
            return this.Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleCreateViewModel request, CancellationToken ct)
        {
            var role = await this.service.CreateAsync(this.CurrentUserId.GetValueOrDefault(), request, ct);
            return this.CreatedAtAction(nameof(this.GetById), new { roleId = role.Id }, role);
        }

        [HttpPut("{roleId:int}")]
        public async Task<IActionResult> Update(
            int roleId, [FromBody] RoleUpdateViewModel request, CancellationToken ct)
        {
            var role = await this.service.UpdateAsync(this.CurrentUserId.GetValueOrDefault(), roleId, request, ct);
            return this.Ok(role);
        }

        [HttpDelete("{roleId:int}")]
        public async Task<IActionResult> Delete(int roleId, CancellationToken ct)
        {
            await this.service.DeleteAsync(this.CurrentUserId.GetValueOrDefault(), roleId, ct);
            return this.NoContent();
        }
    }
}
