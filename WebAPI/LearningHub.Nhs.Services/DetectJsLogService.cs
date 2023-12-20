// <copyright file="DetectJsLogService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;

    /// <summary>
    /// DetectJsLogService.
    /// </summary>
    public class DetectJsLogService : IDetectJsLogService
    {
        private readonly IDetectJsLogRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetectJsLogService"/> class.
        /// </summary>
        /// <param name="repository">DetectJsLogRepository.</param>
        public DetectJsLogService(IDetectJsLogRepository repository)
        {
            this.repository = repository;
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(long jsEnabled, long jsDisabled)
        {
            await this.repository.UpdateAsync(jsEnabled, jsDisabled);
        }
    }
}