// <copyright file="INodeActivityRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>
namespace LearningHub.Nhs.Repository.Interface.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Repository.Interface;

    /// <summary>
    /// INodeActivityRepository.
    /// </summary>
    public interface INodeActivityRepository : IGenericRepository<NodeActivity>
    {
    }
}