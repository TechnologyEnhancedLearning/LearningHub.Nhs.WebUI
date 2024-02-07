// <copyright file="BaseController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="BaseController" />.
    /// </summary>
    [Authorize(Policy = "RequireAdministratorRole")]
    public class BaseController : Controller
    {
        /// <summary>
        /// Defines the hostingEnvironment.
        /// </summary>
        private IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">Hosting env.</param>
        protected BaseController(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Gets the HostingEnvironment.
        /// </summary>
        protected IWebHostEnvironment HostingEnvironment => this.hostingEnvironment;

        /// <summary>
        /// Gets the ContentRootPath.
        /// </summary>
        protected string ContentRootPath
        {
            get
            {
                return this.HostingEnvironment.ContentRootPath;
            }
        }

        /// <summary>
        /// Gets the WebRootPath.
        /// </summary>
        protected string WebRootPath
        {
            get
            {
                return this.HostingEnvironment.WebRootPath;
            }
        }

        /// <summary>
        /// The OnActionExecuting.
        /// </summary>
        /// <param name="context">The context<see cref="Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext"/>.</param>
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            this.Initialise();
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// The Initialise.
        /// </summary>
        private void Initialise()
        {
        }
    }
}
