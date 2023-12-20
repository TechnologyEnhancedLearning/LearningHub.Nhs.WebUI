// <copyright file="IBookmarkService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Bookmark;

    /// <summary>
    /// The Bookmark service interface.
    /// </summary>
    public interface IBookmarkService
    {
        /// <summary>
        /// get all bookmarks.
        /// </summary>
        /// <param name="authHeader">The authentication header value.</param>
        /// <returns>IEnumerable BookmarkViewModel.</returns>
        Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent(string authHeader);
    }
}
