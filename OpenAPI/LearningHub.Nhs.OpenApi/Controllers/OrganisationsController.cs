namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Organisation;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Provides organisation and organisation membership operations.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("organisations")]
    public class OrganisationsController : OpenApiControllerBase
    {
        private readonly IOrganisationService organisationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationsController"/> class.
        /// </summary>
        /// <param name="organisationService">The organisation service.</param>
        public OrganisationsController(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }

        /// <summary>
        /// Searches organisations.
        /// </summary>
        /// <param name="request">Search and paging parameters.</param>
        /// <returns>Paged organisations.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultSet<OrganisationViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResultSet<OrganisationViewModel>>> SearchAsync([FromQuery] OrganisationSearchRequest request)
        {
            var result = await this.organisationService.SearchAsync(request);
            return this.Ok(result);
        }

        /// <summary>
        /// Gets an organisation by id.
        /// </summary>
        /// <param name="id">The organisation id.</param>
        /// <returns>The organisation.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OrganisationViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrganisationViewModel>> GetByIdAsync(int id)
        {
            var organisation = await this.organisationService.GetByIdAsync(id);
            return organisation == null ? this.NotFound() : this.Ok(organisation);
        }

        /// <summary>
        /// Gets the hierarchy for an organisation.
        /// </summary>
        /// <param name="id">The organisation id.</param>
        /// <returns>The organisation hierarchy.</returns>
        [HttpGet("{id:int}/hierarchy")]
        [ProducesResponseType(typeof(OrganisationHierarchyViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrganisationHierarchyViewModel>> GetHierarchyAsync(int id)
        {
            var hierarchy = await this.organisationService.GetHierarchyAsync(id);
            return hierarchy == null ? this.NotFound() : this.Ok(hierarchy);
        }

        /// <summary>
        /// Validates an organisation against ODS.
        /// </summary>
        /// <param name="odsCode">The ODS organisation code.</param>
        /// <returns>The validation result.</returns>
        [HttpGet("ods/{odsCode}/validation")]
        [ProducesResponseType(typeof(OdsOrganisationValidationViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<OdsOrganisationValidationViewModel>> ValidateOdsCodeAsync(string odsCode)
        {
            var result = await this.organisationService.ValidateOdsCodeAsync(odsCode);
            return this.Ok(result);
        }

        /// <summary>
        /// Soft deletes an organisation.
        /// </summary>
        /// <param name="id">The organisation id.</param>
        /// <returns>No content when successful.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await this.organisationService.SoftDeleteAsync(id, this.CurrentUserId.GetValueOrDefault());
            return deleted ? this.NoContent() : this.NotFound();
        }

        /// <summary>
        /// Gets organisations for the current user.
        /// </summary>
        /// <param name="includeEnded">Whether ended memberships should be returned.</param>
        /// <returns>The user's organisation memberships.</returns>
        [HttpGet("me/memberships")]
        [ProducesResponseType(typeof(IReadOnlyList<UserOrganisationViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<UserOrganisationViewModel>>> GetMyOrganisationsAsync([FromQuery] bool includeEnded = false)
        {
            var userId = this.CurrentUserId.GetValueOrDefault();
            var memberships = await this.organisationService.GetUserOrganisationsAsync(userId, includeEnded);
            return memberships == null ? this.NotFound() : this.Ok(memberships);
        }

        /// <summary>
        /// Gets organisations for a specific user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="includeEnded">Whether ended memberships should be returned.</param>
        /// <returns>The user's organisation memberships.</returns>
        [HttpGet("users/{userId:int}/memberships")]
        [ProducesResponseType(typeof(IReadOnlyList<UserOrganisationViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<UserOrganisationViewModel>>> GetUserOrganisationsAsync(int userId, [FromQuery] bool includeEnded = false)
        {
            var memberships = await this.organisationService.GetUserOrganisationsAsync(userId, includeEnded);
            return memberships == null ? this.NotFound() : this.Ok(memberships);
        }

        /// <summary>
        /// Adds a user membership to an organisation.
        /// </summary>
        /// <param name="organisationId">The organisation id.</param>
        /// <param name="request">The membership details.</param>
        /// <returns>The created membership.</returns>
        [HttpPost("{organisationId:int}/memberships")]
        [ProducesResponseType(typeof(UserOrganisationViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserOrganisationViewModel>> AddMembershipAsync(
            int organisationId,
            [FromBody] AddOrganisationMembershipRequest request)
        {
            var membership = await this.organisationService.AddMembershipAsync(
                organisationId,
                request,
                this.CurrentUserId.GetValueOrDefault());

            return this.StatusCode(StatusCodes.Status201Created, membership);
        }

        /// <summary>
        /// Updates an organisation membership and job role.
        /// </summary>
        /// <param name="membershipId">The membership id.</param>
        /// <param name="request">The updated membership details.</param>
        /// <returns>The updated membership.</returns>
        [HttpPut("memberships/{membershipId:int}")]
        [ProducesResponseType(typeof(UserOrganisationViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserOrganisationViewModel>> UpdateMembershipAsync(
            int membershipId,
            [FromBody] UpdateOrganisationMembershipRequest request)
        {
            var membership = await this.organisationService.UpdateMembershipAsync(
                membershipId,
                request,
                this.CurrentUserId.GetValueOrDefault());

            return membership == null ? this.NotFound() : this.Ok(membership);
        }

        /// <summary>
        /// Ends an organisation membership.
        /// </summary>
        /// <param name="membershipId">The membership id.</param>
        /// <param name="request">The membership end request.</param>
        /// <returns>No content when successful.</returns>
        [HttpPost("memberships/{membershipId:int}/end")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EndMembershipAsync(
            int membershipId,
            [FromBody] EndOrganisationMembershipRequest request)
        {
            var ended = await this.organisationService.EndMembershipAsync(
                membershipId,
                request.EndDate,
                this.CurrentUserId.GetValueOrDefault());

            return ended ? this.NoContent() : this.NotFound();
        }

        /// <summary>
        /// Gets available job role types.
        /// </summary>
        /// <returns>The available job role types.</returns>
        [HttpGet("job-role-types")]
        [ProducesResponseType(typeof(IReadOnlyList<JobRoleTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<JobRoleTypeViewModel>>> GetJobRoleTypesAsync()
        {
            var jobRoles = await this.organisationService.GetJobRoleTypesAsync();
            return this.Ok(jobRoles);
        }
    }
}
