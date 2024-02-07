// <copyright file="AuthorisationController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="AuthorisationController" />.
    /// </summary>
    public class AuthorisationController : Controller
    {
        /// <summary>
        /// The AccessDenied.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult AccessDenied()
        {
            return this.View();
        }
    }
}
