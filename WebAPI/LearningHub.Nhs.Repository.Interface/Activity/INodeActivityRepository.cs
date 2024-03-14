// <copyright file="INodeActivityRepository.cs" company="NHS England">
// Copyright (c) NHS England.
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