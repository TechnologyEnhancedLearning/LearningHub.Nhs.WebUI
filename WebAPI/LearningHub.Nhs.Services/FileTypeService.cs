// <copyright file="FileTypeService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The file type service.
    /// </summary>
    public class FileTypeService : IFileTypeService
    {
        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<FileTypeService> logger;

        /// <summary>
        /// The file type repository.
        /// </summary>
        private IFileTypeRepository fileTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTypeService"/> class.
        /// </summary>
        /// <param name="fileTypeRepository">The file type repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        public FileTypeService(
            IFileTypeRepository fileTypeRepository,
            ILogger<FileTypeService> logger,
            IMapper mapper)
        {
            this.fileTypeRepository = fileTypeRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<FileType> GetByIdAsync(int id)
        {
            return await this.fileTypeRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// The get all async.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<FileTypeViewModel>> GetAllAsync()
        {
            var fileTypes = await this.fileTypeRepository.GetAll()
                .ToListAsync();

            return this.mapper.Map<List<FileTypeViewModel>>(fileTypes);
        }

        /// <summary>
        /// Gets the FileType corresponding to a filename.
        /// </summary>
        /// <param name="filename">The filename to return the FileType for.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<FileType> GetByFilename(string filename)
        {
            var extension = Path.GetExtension(filename).Replace(".", string.Empty);
            if (!string.IsNullOrEmpty(extension))
            {
                return await this.fileTypeRepository.GetAll().FirstOrDefaultAsync(x => x.Extension == extension);
            }

            return null;
        }

        /// <summary>
        /// Gets a list of all disallowed file extensions.
        /// </summary>
        /// <returns>All Disallowed File Extensions.</returns>
        public List<string> GetAllDisallowedFileExtensions()
        {
            return this.fileTypeRepository.GetAll().Where(x => x.NotAllowed).Select(x => x.Extension).ToList();
        }
    }
}
