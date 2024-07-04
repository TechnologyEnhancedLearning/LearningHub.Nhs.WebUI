using LearningHub.Nhs.Models.Entities.Activity;
using LearningHub.Nhs.Models.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Tests.TestMockData
{
    
    public static class MockDataForOpenApiResourceTests
    {
        public static List<ResourceActivityDTO> GetResourceActivityDTOList => new List<ResourceActivityDTO>()
{ new ResourceActivityDTO
            {
                Id = 8324,
                UserId = 57541,
                LaunchResourceActivityId = null,
                ResourceId = 2,
                ResourceVersionId = 404,
                MajorVersion = 1,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 3, // Completed
                ActivityStart = DateTime.Parse("2023-08-01T13:52:27+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
                         new ResourceActivityDTO
            {
                Id = 8324,
                UserId = 57541,
                LaunchResourceActivityId = null,
                ResourceId = 3,
                ResourceVersionId = 404,
                MajorVersion = 1,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 3, // Completed
                ActivityStart = DateTime.Parse("2023-08-01T13:52:27+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
                                               new ResourceActivityDTO
            {
                Id = 8324,
                UserId = 57541,
                LaunchResourceActivityId = null,
                ResourceId = 4, // this is for the external catalogue resource
                ResourceVersionId = 404,
                MajorVersion = 1,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 3, // Completed
                ActivityStart = DateTime.Parse("2023-08-01T13:52:27+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
                         new ResourceActivityDTO
            {
                Id = 642,
                UserId = 57541,
                LaunchResourceActivityId = null,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 1,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 3, // completed
                ActivityStart = DateTime.Parse("2020-10-07T08:32:42+00:00"),
                ActivityEnd = DateTime.Parse("2020-10-07T08:32:42+00:00"),
                DurationSeconds = 0,
                Score = null,
            },
            new ResourceActivityDTO
            {
                Id = 8322,
                UserId = 57541,
                LaunchResourceActivityId = 8321,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 2,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 7, // In complete -> Inprogress
                ActivityStart = DateTime.Parse("2023-08-01T13:52:01+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
            new ResourceActivityDTO
            {
                Id = 8324,
                UserId = 57541,
                LaunchResourceActivityId = null,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 3,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 3, // Completed
                ActivityStart = DateTime.Parse("2023-08-01T13:52:27+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
            new ResourceActivityDTO
            {
                Id = 8326,
                UserId = 57541,
                LaunchResourceActivityId = 8325,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 4,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 4, // Failed
                ActivityStart = DateTime.Parse("2023-08-01T13:53:41+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
            new ResourceActivityDTO
            {
                Id = 8329,
                UserId = 57541,
                LaunchResourceActivityId = null,
                ResourceId = 303,
                ResourceVersionId = 404,
                MajorVersion = 5,
                MinorVersion = 0,
                NodePathId = 1,
                ActivityStatusId = 5, // passed
                ActivityStart = DateTime.Parse("2023-08-01T13:57:03+01:00"),
                ActivityEnd = null,
                DurationSeconds = 0,
                Score = null,
            },
        };
        private static List<CatalogueDTO> CatalogueDTOList => new List<CatalogueDTO>()
        {
            new CatalogueDTO(originalResourceReferenceId: 100, catalogueNodeId: 1, catalogueNodeName: "catalogue1", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 101, catalogueNodeId: 2, catalogueNodeName: "catalogue2", isRestricted: true),
            new CatalogueDTO(originalResourceReferenceId: 102, catalogueNodeId: 3, catalogueNodeName: "catalogue3Restricted", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 103, catalogueNodeId: 4, catalogueNodeName: "catalogue4", isRestricted: true),
            new CatalogueDTO(originalResourceReferenceId: 302, catalogueNodeId: 5, catalogueNodeName: "catalogue5", isRestricted: false),
            new CatalogueDTO(originalResourceReferenceId: 303, catalogueNodeId: 6, catalogueNodeName: "catalogue6", isRestricted: false),
            
        };

        public static List<ResourceReferenceAndCatalogueDTO> GetResourceReferenceAndCatalogueDTOList => new List<ResourceReferenceAndCatalogueDTO>()
        {
            // Entry for originalResourceId 100
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 1,
                title: "title1AudioNoActivitySummaryData",
                description: "description1AudioActivitySummaryData",
                resourceTypeId: 11,  // ResourceTypeEnum.Assessment corresponds to 11
                majorVersion: 1,
                rating: 3m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[0] }
            ),

            // Entry for originalResourceId 101
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 2,
                title: "title2Article",
                description: "description2Article",
                resourceTypeId: 1,  // Article
                majorVersion: 1,
                rating: 0m,  // Defaulting rating to 0 instead of null
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[1] }  // Using the second item in CatalogueDTOList
            ),

            // Entry for originalResourceId 102, 103
            /*
                Warning because searching the by originalResourceId would return a single record and catalogue
                but by resourceId could return two calendars and two originalResourceIds
                Then this would mock originalResourceIds 102, 103 but not mock calls for either well individually
            */
             
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 3,
                title: "title3Image",
                description: "description3Image",
                resourceTypeId: 5,  // ResourceTypeEnum.Image corresponds to 5
                majorVersion: 1,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[2], CatalogueDTOList[3] }
            ),
            //  No catalogue can occur only by nullified external data aas currently the stored procedure does not left join on currentVersion. This data should be used with includeExternalResources true
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 4,
                title: "title4NullifiedExternalCatalogue",
                description: "description4NullifiedExternalCatalogue",
                resourceTypeId: 8, // Weblink -> Launched
                majorVersion: 1,
                rating: 0m,
                catalogueDTOs: new List<CatalogueDTO>() {}
            ),
            // Entry for originalResourceId 302
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 303,
                title: "titleHasSummaryDataGenericFile",
                description: "descriptionHasSummaryDataGenericFile",
                resourceTypeId: 9,  // ResourceTypeEnum.GenericFile corresponds to 9
                majorVersion: 99,
                rating: 3m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[4] }
            ),
            // Entry for originalResourceId 303 twice which shouldn't happen, this is for testing an error
            new ResourceReferenceAndCatalogueDTO(
                resourceId: 304,
                title: "titleHasSummaryDataGenericFile",
                description: "descriptionHasSummaryDataGenericFile",
                resourceTypeId: 9,  // ResourceTypeEnum.GenericFile corresponds to 9
                majorVersion: 99,
                rating: 3m,
                catalogueDTOs: new List<CatalogueDTO> { CatalogueDTOList[5], CatalogueDTOList[5] }
            ),

        };
    }
}
