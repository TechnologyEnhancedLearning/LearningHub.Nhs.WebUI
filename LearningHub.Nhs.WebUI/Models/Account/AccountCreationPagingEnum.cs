// <copyright file="AccountCreationPagingEnum.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Account
{
    /// <summary>
    /// Defines the AccountCreationPagingEnum.
    /// </summary>
    public enum AccountCreationPagingEnum
    {
        /// <summary>
        /// Default
        /// </summary>
        Default = 0,

        /// <summary>
        /// Previoous page change.
        /// </summary>
        PreviousPageChange = 1,

        /// <summary>
        /// Next page change.
        /// </summary>
        NextPageChange = 2,
    }
}
