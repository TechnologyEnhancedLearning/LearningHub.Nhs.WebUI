// <copyright file="ITransactionManager.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// The ITransactionManager.
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Begins a transaction, for use in a using block.
        /// </summary>
        /// <returns>A DbContextTransaction.</returns>
        IDbContextTransaction BeginTransaction();
    }
}
