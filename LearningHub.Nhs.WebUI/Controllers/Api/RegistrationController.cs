// <copyright file="RegistrationController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Extensions;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="RegistrationController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationController"/> class.
        /// </summary>
        /// <param name="userService">User service.</param>
        public RegistrationController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// The GetAsync.
        /// </summary>
        /// <param name="emailAddress">Email address.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("validate-email")]
        public async Task<ActionResult> GetAsync(string emailAddress)
        {
            var ipAddress = this.Request.GetUserIPAddress();

            var registrationStatus = await this.userService.GetEmailAddressRegistrationStatusAsync(emailAddress, ipAddress.ToString());
            return this.Ok(registrationStatus.ToString());
        }

        /// <summary>
        /// The PostAsync.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("{request}")]
        public async Task<ActionResult> PostAsync([FromBody] RegistrationRequestViewModel request)
        {
            string errorMessage = string.Empty;
            if (request.CountryId == 1 && request.RegionId == 0)
            {
                errorMessage = "A region is required";
            }
            else
            {
                request.SelfRegistration = true;
                var response = await this.userService.RegisterNewUser(request);
                if (!response.IsValid)
                {
                    errorMessage = string.Join(", ", response.Details);
                }
            }

            return this.Ok(errorMessage);
        }
    }
}
