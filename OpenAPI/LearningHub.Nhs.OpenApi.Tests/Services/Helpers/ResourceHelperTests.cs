// <copyright file="ResourceHelperTests.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Tests.Services.Helpers
{
    using System.Linq;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using Xunit;

    public class ResourceHelperTests
    {
        [Fact]
        public void OrderBySequenceOrdersResourcesCorrectly()
        {
            // Given
            var resources = Builder<ResourceMetadataViewModel>.CreateListOfSize(6).Build();
            var resourceIdSequence = new[] { 1, 5, 2, 3, 6, 4 };

            // When
            var orderedResources = resources.OrderBySequence(resourceIdSequence);

            // Then
            orderedResources.Select(r => r.ResourceId).Should().ContainInOrder(new[] { 1, 5, 2, 3, 6, 4 });
        }

        [Fact]
        public void OrderBySequenceHandlesExtraIdsInSequence()
        {
            // Given
            var resources = Builder<ResourceMetadataViewModel>.CreateListOfSize(6).Build();
            var resourceIdSequence = new[] { 7, 1, 5, 2, 3, 6, 4 };

            // When
            var orderedResources = resources.OrderBySequence(resourceIdSequence);

            // Then
            orderedResources.Select(r => r.ResourceId).Should().ContainInOrder(new[] { 1, 5, 2, 3, 6, 4 });
        }

        [Fact]
        public void OrderBySequenceHandlesMissingIdsInSequence()
        {
            // Given
            var resources = Builder<ResourceMetadataViewModel>.CreateListOfSize(6).Build();
            var resourceIdSequence = new[] { 1, 5, 2, 3, 4 };

            // When
            var orderedResources = resources.OrderBySequence(resourceIdSequence);

            // Then
            orderedResources.Select(r => r.ResourceId).Should().ContainInOrder(new[] { 1, 5, 2, 3, 4 });
        }

        [Theory]
        [InlineData(0, "Undefined")]
        [InlineData(10000, "")]
        public void GettingResourceTypeNameReturnsNameOrNull(int resourceTypeEnumInt, string expectedName)
        {
            // Given
            var resource = Builder<Resource>.CreateNew()
                .With(r => r.ResourceTypeEnum = (ResourceTypeEnum)resourceTypeEnumInt)
                .Build();

            // When
            var name = resource.GetResourceTypeNameOrEmpty();

            // Then
            name.Should().Be(expectedName);
        }
    }
}
