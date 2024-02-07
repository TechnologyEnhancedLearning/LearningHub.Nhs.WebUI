// <copyright file="NotificationDetailModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using System;

    /// <summary>
    /// Defines the NotificationModalType.
    /// </summary>
    public enum NotificationModalType
    {
        /// <summary>
        /// Defines the Delete.
        /// </summary>
        Delete,

        /// <summary>
        /// Defines the View.
        /// </summary>
        View,
    }

    /// <summary>
    /// Defines the <see cref="NotificationDetailModel" />.
    /// </summary>
    public class NotificationDetailModel
    {
        /// <summary>
        /// Gets or sets the Body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the Date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ModalType.
        /// </summary>
        public NotificationModalType ModalType { get; set; }

        /// <summary>
        /// Gets or sets the NotificationId.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; }
    }
}
