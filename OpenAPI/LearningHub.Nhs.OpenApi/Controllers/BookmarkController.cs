﻿// <copyright file="BookmarkController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;

    /// <summary>
    /// Learning Hub Bookmark controller.
    /// </summary>
    [Authorize]
    [Route("Bookmark")]
    [ApiController]
    public class BookmarkController : Controller
    {
        private readonly IBookmarkService bookmarkService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkController"/> class.
        /// </summary>
        /// <param name="bookmarkService">The search service.</param>
        public BookmarkController(IBookmarkService bookmarkService)
        {
            this.bookmarkService = bookmarkService;
        }

        /// <summary>
        /// Gets all bookmarks by parent.
        /// </summary>
        /// <returns>Bookmarks.</returns>
        [HttpGet]
        [Route("GetAllByParent")]
        public async Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent()
        {
            var accessToken = await this.HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            return await this.bookmarkService.GetAllByParent(
                accessToken);
        }
    }
}
