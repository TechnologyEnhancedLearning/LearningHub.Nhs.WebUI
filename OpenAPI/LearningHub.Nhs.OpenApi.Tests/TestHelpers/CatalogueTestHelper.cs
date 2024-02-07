// <copyright file="CatalogueTestHelper.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Tests.TestHelpers
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    public static class CatalogueTestHelper
    {
        private const string DefaultCatalogueNodeVersionName = "default catalogue node version name";

        public static CatalogueNodeVersion CreateCatalogueNodeVersion(
            int id = 0,
            string name = DefaultCatalogueNodeVersionName,
            int nodeId = 0,
            bool isRestricted = false)
        {
            return new CatalogueNodeVersion
            {
                Id = id,
                Name = name,
                RestrictedAccess = isRestricted,
                NodeVersion = new NodeVersion { NodeId = nodeId },
            };
        }
    }
}