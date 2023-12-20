// <copyright file="BookmarkService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Bookmark;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The BookmarkService.
    /// </summary>
    public class BookmarkService : IBookmarkService
    {
        private readonly IMapper mapper;
        private readonly IBookmarkRepository bookmarkRepository;
        private readonly IResourceReferenceRepository resourceReferenceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkService"/> class.
        /// </summary>
        /// <param name="mapper">The mapper<see cref="IMapper"/>.</param>
        /// <param name="bookmarkRepository">The bookmarkRepository<see cref="IBookmarkRepository"/>.</param>
        /// <param name="resourceReferenceRepository">.</param>
        public BookmarkService(IMapper mapper, IBookmarkRepository bookmarkRepository, IResourceReferenceRepository resourceReferenceRepository)
        {
            this.mapper = mapper;
            this.bookmarkRepository = bookmarkRepository;
            this.resourceReferenceRepository = resourceReferenceRepository;
        }

        /// <inheritdoc/>
        public async Task<int> Create(int currentUserId, UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkModel = this.mapper.Map<UserBookmark>(bookmarkViewModel);
            var maxPosition = (await this.bookmarkRepository.GetAll().Where(ub => ub.UserId == currentUserId).MaxAsync(x => (int?)x.Position)) ?? 0;
            bookmarkModel.UserId = currentUserId;
            bookmarkModel.Position = maxPosition + 1;

            return await this.bookmarkRepository.CreateAsync(currentUserId, bookmarkModel);
        }

        /// <inheritdoc/>
        public async Task<int> Edit(int currentUserId, UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkModel = await this.bookmarkRepository.GetById(bookmarkViewModel.Id);
            bookmarkModel.ParentId = bookmarkViewModel.ParentId;
            bookmarkModel.Position = bookmarkViewModel.Position;
            bookmarkModel.Title = bookmarkViewModel.Title;
            await this.bookmarkRepository.UpdateAsync(currentUserId, bookmarkModel);
            return bookmarkModel.Id;
        }

        /// <inheritdoc/>
        public async Task DeleteFolder(int bookmarkId, int userId)
        {
            await this.bookmarkRepository.DeleteFolder(bookmarkId, userId);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserBookmarkViewModel>> GetAllByParent(int currentUserId, int? parentId, bool? all = false)
        {
            var userBookmarks = this.bookmarkRepository.GetAll().Where(ub => ub.UserId == currentUserId && ub.Deleted == false).OrderBy(ub => ub.Position).ThenBy(ub => ub.AmendDate);

            var bookmarks = all == true ? userBookmarks : userBookmarks.Where(ub => ub.ParentId == parentId);

            var userBookmarkResult = this.mapper.Map<IEnumerable<UserBookmarkViewModel>>(bookmarks);

            foreach (var b in userBookmarkResult.Where(ub => ub.BookmarkTypeId == 1))
            {
                b.ChildrenCount = userBookmarks.Where(ub => ub.ParentId == b.Id).Count();
            }

            foreach (var b in userBookmarkResult.Where(ub => ub.ResourceReferenceId.HasValue))
            {
                var resourceRef = await this.resourceReferenceRepository.GetByOriginalResourceReferenceIdAsync(b.ResourceReferenceId.Value, false);
                b.ResourceTypeId = (int?)resourceRef.Resource.ResourceTypeEnum;
            }

            return userBookmarkResult;
        }

        /// <inheritdoc/>
        public async Task<int> Toggle(int currentUserId, UserBookmarkViewModel bookmarkViewModel)
        {
            var bookmarkId = bookmarkViewModel.Id;
            var maxPosition = (await this.bookmarkRepository.GetAll().Where(ub => ub.UserId == currentUserId && ub.ParentId == null).MaxAsync(x => (int?)x.Position)) ?? 0;
            if (bookmarkId == 0)
            {
                var bookmarkModel = this.mapper.Map<UserBookmark>(bookmarkViewModel);
                bookmarkModel.UserId = currentUserId;
                bookmarkModel.Position = maxPosition + 1;
                bookmarkId = await this.bookmarkRepository.CreateAsync(currentUserId, bookmarkModel);
            }
            else
            {
                var bookmarkModel = await this.bookmarkRepository.GetById(bookmarkId);
                bookmarkModel.Deleted = !bookmarkModel.Deleted;
                if (bookmarkModel.Deleted)
                {
                    bookmarkModel.ParentId = null;
                }
                else
                {
                    bookmarkModel.Link = bookmarkViewModel.Link;
                    bookmarkModel.Title = bookmarkViewModel.Title;
                    bookmarkModel.Position = maxPosition + 1;
                }

                await this.bookmarkRepository.UpdateAsync(currentUserId, bookmarkModel);
            }

            return bookmarkId;
        }
    }
}
