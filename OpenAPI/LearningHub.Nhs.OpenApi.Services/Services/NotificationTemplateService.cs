namespace LearningHub.Nhs.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Services.Services;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The NotificationTemplateService class.
    /// </summary>
    public class NotificationTemplateService : BaseService<NotificationTemplate>, INotificationTemplateService
    {
        private const string LeftBracket = "[";
        private const string RightBracket = "]";

        private readonly INotificationTemplateRepository notificationTemplateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationTemplateService"/> class.
        /// </summary>
        /// <param name="fwClient">The findwise client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="notificationTemplateRepository">The notification template repository.</param>
        public NotificationTemplateService(
            IFindwiseClient fwClient,
            ILogger<NotificationTemplate> logger,
            INotificationTemplateRepository notificationTemplateRepository)
            : base(fwClient, logger)
        {
            this.notificationTemplateRepository = notificationTemplateRepository;
        }

        /// <summary>
        /// The GetCatalogueAccessRequestAccepted.
        /// </summary>
        /// <param name="catalogueName">The catalogueName.</param>
        /// <param name="catalogueUrl">The catalogueUrl.</param>
        /// <returns>The notification with message and title set.</returns>
        public Notification GetCatalogueAccessRequestAccepted(string catalogueName, string catalogueUrl)
        {
            var template = this.notificationTemplateRepository.GetById(Models.Enums.NotificationTemplates.CatalogueAccessRequestSuccess);
            return new Notification
            {
                Message = this.Replace(template.Body, new Dictionary<string, string>
                {
                    ["CatalogueName"] = catalogueName,
                    ["CatalogueUrl"] = catalogueUrl,
                }),
                Title = template.Subject,
            };
        }

        /// <summary>
        /// The GetCatalogueAccessRequestRejected.
        /// </summary>
        /// <param name="catalogueName">The catalogueName.</param>
        /// <param name="catalogueUrl">The catalogueUrl.</param>
        /// <param name="rejectionReason">The rejectionReason.</param>
        /// <returns>The notification with message and title set.</returns>
        public Notification GetCatalogueAccessRequestFailure(string catalogueName, string catalogueUrl, string rejectionReason)
        {
            var template = this.notificationTemplateRepository.GetById(Models.Enums.NotificationTemplates.CatalogueAccessRequestFailure);
            return new Notification
            {
                Message = this.Replace(template.Body, new Dictionary<string, string>
                {
                    ["CatalogueName"] = catalogueName,
                    ["CatalogueUrl"] = catalogueUrl,
                    ["RejectionReason"] = rejectionReason,
                }),
                Title = template.Subject,
            };
        }

        /// <summary>
        /// The GetCatalogueEditorAdded.
        /// </summary>
        /// <param name="supportUrl">The support url.</param>
        /// <param name="catalogueName">The catalogue name.</param>
        /// <param name="catalogueUrl">The catalogue url.</param>
        /// <returns>The notification with message and title set.</returns>
        public Notification GetCatalogueEditorAdded(string supportUrl, string catalogueName, string catalogueUrl)
        {
            var template = this.notificationTemplateRepository.GetById(Models.Enums.NotificationTemplates.CatalogueEditorAdded);
            return new Notification
            {
                Message = this.Replace(template.Body, new Dictionary<string, string>
                {
                    ["CatalogueName"] = catalogueName,
                    ["CatalogueUrl"] = catalogueUrl,
                    ["SupportUrl"] = supportUrl,
                }),
                Title = template.Subject,
            };
        }

        /// <summary>
        /// The GetCatalogueEditorRemoved.
        /// </summary>
        /// <param name="supportUrl">The support url.</param>
        /// <param name="catalogueName">The catalogue name.</param>
        /// <param name="catalogueUrl">The catalogue url.</param>
        /// <returns>The notification with message and title set.</returns>
        public Notification GetCatalogueEditorRemoved(string supportUrl, string catalogueName, string catalogueUrl)
        {
            var template = this.notificationTemplateRepository.GetById(Models.Enums.NotificationTemplates.CatalogueEditorRemoved);
            return new Notification
            {
                Message = this.Replace(template.Body, new Dictionary<string, string>
                {
                    ["CatalogueName"] = catalogueName,
                    ["CatalogueUrl"] = catalogueUrl,
                    ["SupportUrl"] = supportUrl,
                }),
                Title = template.Subject,
            };
        }

        private string Replace(string str, Dictionary<string, string> replacements)
        {
            return replacements.Aggregate(
                str,
                (acc, val) =>
                    acc.Replace(LeftBracket + val.Key + RightBracket, val.Value));
        }
    }
}
