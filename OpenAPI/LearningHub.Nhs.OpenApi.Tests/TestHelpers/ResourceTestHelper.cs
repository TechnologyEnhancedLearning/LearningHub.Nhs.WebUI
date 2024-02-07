// <copyright file="ResourceTestHelper.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;

    public static class ResourceTestHelper
    {
        private const string DefaultCatalogueName = "default catalogue name";

        public static Resource CreateResourceWithDetails(
            int id = 1,
            int resourceReferenceId = 1,
            bool deleted = false,
            string catalogueName = DefaultCatalogueName,
            DateTimeOffset? amendDate = null,
            int amendUserId = 1,
            DateTimeOffset? createDate = null,
            int createUserId = 1,
            string title = "title",
            string description = "description",
            bool hasNodePath = true,
            bool hasCurrentResourceVersion = true,
            bool hasRatingSummary = true,
            decimal rating = 0m,
            ResourceTypeEnum? resourceType = ResourceTypeEnum.Article)
        {
            var resourceReference = new List<ResourceReference>()
            {
                CreateResourceReferenceWithDetails(
                    id: resourceReferenceId,
                    resourceId: id,
                    amendDate: amendDate,
                    amendUserId: amendUserId,
                    createDate: createDate,
                    createUserId: 1,
                    deleted: false,
                    catalogueName: catalogueName,
                    hasNodePath: hasNodePath),
            };

            return new Resource
            {
                Id = id,
                AmendDate = amendDate ?? default,
                AmendUserId = amendUserId,
                CreateDate = createDate ?? default,
                CreateUserId = createUserId,
                CurrentResourceVersion = hasCurrentResourceVersion
                    ? CreateResourceVersion(title, description, hasRatingSummary, rating)
                    : null,
                ResourceReference = resourceReference,
                Deleted = deleted,
                ResourceTypeEnum = resourceType ?? default,
            };
        }

        public static ResourceReference CreateResourceReferenceWithDetails(
            int id = 1,
            int originalResourceReferenceId = 2,
            int resourceId = 1,
            DateTimeOffset? amendDate = null,
            int amendUserId = 1,
            DateTimeOffset? createDate = null,
            int createUserId = 1,
            bool deleted = false,
            string catalogueName = DefaultCatalogueName,
            bool hasNodePath = true,
            bool hasCatalogueNodeVersion = true,
            bool hasNodeVersion = true,
            Resource? resource = null,
            bool catalogueNodeVersionIsRestricted = false)
        {
            amendDate ??= default;
            return new ResourceReference
            {
                AmendDate = amendDate ?? default,
                OriginalResourceReferenceId = originalResourceReferenceId,
                ResourceId = resourceId,
                NodePathId = 1,
                CreateDate = createDate ?? default,
                AmendUserId = amendUserId,
                CreateUserId = createUserId,
                Deleted = deleted,
                Id = id,
                NodePath = hasNodePath ? CreateNodePath(hasCatalogueNodeVersion, catalogueName, hasNodeVersion, catalogueNodeVersionIsRestricted) : null,
                Resource = resource,
            };
        }

        private static ResourceVersion CreateResourceVersion(
            string title = "title",
            string description = "description",
            bool hasRatingSummary = true,
            decimal rating = 0)
        {
            return new ResourceVersion
            {
                Id = 0,
                Deleted = false,
                CreateUserId = 0,
                CreateDate = default,
                AmendUserId = 0,
                AmendDate = default,
                ResourceId = 0,
                PublicationId = null,
                MajorVersion = null,
                MinorVersion = null,
                Title = title,
                Description = description,
                AdditionalInformation = null,
                ReviewDate = null,
                Resource = null,
                ResourceWhereCurrent = null,
                VersionStatusEnum = VersionStatusEnum.Published,
                ArticleResourceVersion = null,
                EmbeddedResourceVersion = null,
                EquipmentResourceVersion = null,
                WebLinkResourceVersion = null,
                GenericFileResourceVersion = null,
                ImageResourceVersion = null,
                VideoResourceVersion = null,
                AudioResourceVersion = null,
                ResourceVersionAuthor = null,
                ResourceVersionKeyword = null,
                ResourceVersionEvent = null,
                ResourceVersionFlag = null,
                Publication = null,
                HasCost = false,
                Cost = null,
                ResourceLicenceId = null,
                ResourceLicence = null,
                SensitiveContent = false,
                CreateUser = null,
                ResourceVersionRatings = null,
                ResourceVersionRatingSummary = hasRatingSummary ? CreateTestRatingSummary(rating: rating) : null,
                FileChunkDetail = null,
                ResourceVersionUserAcceptance = null,
            };
        }

        private static ResourceVersionRatingSummary CreateTestRatingSummary(decimal rating)
        {
            return new ResourceVersionRatingSummary
            {
                Id = 0,
                Deleted = false,
                CreateUserId = 0,
                CreateDate = default,
                AmendUserId = 0,
                AmendDate = default,
                ResourceVersionId = 0,
                AverageRating = rating,
                RatingCount = 0,
                Rating1StarCount = 0,
                Rating2StarCount = 0,
                Rating3StarCount = 0,
                Rating4StarCount = 0,
                Rating5StarCount = 0,
                ResourceVersion = null,
            };
        }

        private static CatalogueNodeVersion CreateCatalogueNodeVersion(NodeVersion nodeVersion, string catalogueName = DefaultCatalogueName, bool isRestricted = false)
        {
            return new CatalogueNodeVersion
            {
                Id = 0,
                Deleted = false,
                CreateUserId = 0,
                CreateDate = default,
                AmendUserId = 0,
                AmendDate = default,
                NodeVersionId = 0,
                Name = catalogueName,
                Url = null,
                BadgeUrl = null,
                BannerUrl = null,
                Description = null,
                OwnerName = null,
                OwnerEmailAddress = null,
                Notes = null,
                Order = CatalogueOrder.AlphabeticalAscending,
                Keywords = null,
                NodeVersion = nodeVersion,
                RestrictedAccess = isRestricted,
            };
        }

        private static NodeVersion CreateNodeVersion(
            bool hasCatalogueNodeVersion = true,
            string catalogueName = DefaultCatalogueName,
            bool catalogueIsRestricted = false)
        {
            var nodeVersion = new NodeVersion
            {
                Id = 0,
                Deleted = false,
                CreateUserId = 0,
                CreateDate = default,
                AmendUserId = 0,
                AmendDate = default,
                NodeId = 0,
                VersionStatusEnum = VersionStatusEnum.Published,
                MajorVersion = null,
                MinorVersion = null,
                Publication = null,
                Node = null,
                NodeWhereCurrent = null,
            };
            nodeVersion.CatalogueNodeVersion = hasCatalogueNodeVersion ? CreateCatalogueNodeVersion(nodeVersion, catalogueName, catalogueIsRestricted) : null;
            return nodeVersion;
        }

        private static NodePath CreateNodePath(
            bool hasCatalogueNodeVersion,
            string catalogueName = DefaultCatalogueName,
            bool hasNodeVersion = true,
            bool catalogueNodeVersionIsRestricted = false)
        {
            return new NodePath
            {
                Id = 0,
                Deleted = false,
                CreateUserId = 0,
                CreateDate = default,
                AmendUserId = 0,
                AmendDate = default,
                NodeId = 0,
                NodePathString = null,
                CatalogueNodeId = 0,
                NodePathNode = null,
                CatalogueNode = hasNodeVersion
                    ? CreateNode(hasCatalogueNodeVersion: hasCatalogueNodeVersion, catalogueName: catalogueName, catalogueNodeVersionIsRestricted: catalogueNodeVersionIsRestricted)
                    : null,
                Node = null,
                NodePathDisplay = null,
            };
        }

        private static Node CreateNode(
            int id = 0,
            string catalogueName = DefaultCatalogueName,
            bool hasNodeVersion = true,
            bool hasCatalogueNodeVersion = true,
            bool catalogueNodeVersionIsRestricted = false)
        {
            return new Node
            {
                Id = id,
                Deleted = false,
                CreateUserId = 0,
                CreateDate = default,
                AmendUserId = 0,
                AmendDate = default,
                CurrentNodeVersion = hasNodeVersion
                    ? CreateNodeVersion(hasCatalogueNodeVersion, catalogueName, catalogueNodeVersionIsRestricted)
                    : null,
                Name = null,
                Description = null,
                Hidden = false,
                CurrentNodeVersionId = 0,
                NodeTypeEnum = NodeTypeEnum.Catalogue,
                NodeVersion = null,
            };
        }
    }
}