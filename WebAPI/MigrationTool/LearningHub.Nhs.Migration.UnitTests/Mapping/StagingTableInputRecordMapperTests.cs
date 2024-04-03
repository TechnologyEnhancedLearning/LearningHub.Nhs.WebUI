namespace LearningHub.Nhs.Migration.UnitTests.Mapping
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Migration.Interface.Mapping;
    using LearningHub.Nhs.Migration.Mapping;
    using LearningHub.Nhs.Migration.Mapping.Helpers;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Services.Interface;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// The StagingTableInputRecordMapper tests.
    /// </summary>
    public class StagingTableInputRecordMapperTests
    {
        /// <summary>
        /// The mock migration repository.
        /// </summary>
        private readonly Mock<IUserProfileRepository> mockUserProfileRepository;

        /// <summary>
        /// The mock ICatalogueNodeVersionRepository.
        /// </summary>
        private readonly Mock<ICatalogueNodeVersionRepository> mockCatalogueNodeVersionRepository;

        /// <summary>
        /// The mock file type service.
        /// </summary>
        private readonly Mock<IFileTypeService> mockFileTypeService;

        /// <summary>
        /// The standard input record mapper.
        /// </summary>
        private readonly IInputRecordMapper stagingTableInputRecordMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="StagingTableInputRecordMapperTests"/> class.
        /// </summary>
        public StagingTableInputRecordMapperTests()
        {
            this.mockUserProfileRepository = new Mock<IUserProfileRepository>(MockBehavior.Strict);
            this.mockCatalogueNodeVersionRepository = new Mock<ICatalogueNodeVersionRepository>(MockBehavior.Strict);
            this.mockFileTypeService = new Mock<IFileTypeService>(MockBehavior.Strict);

            this.stagingTableInputRecordMapper = new StagingTableInputRecordMapper(
                new UserLookup(this.mockUserProfileRepository.Object),
                new NodeIdLookup(this.mockCatalogueNodeVersionRepository.Object),
                new FileTypeLookup(this.mockFileTypeService.Object));

            this.mockUserProfileRepository.Setup(r => r.GetByUsernameAsync("username1")).Returns(Task.FromResult(new UserProfile() { Id = 1234, FirstName = "firstname", LastName = "lastname" }));
            this.mockCatalogueNodeVersionRepository.Setup(x => x.GetNodeIdByCatalogueName("my catalogue")).Returns(Task.FromResult(99));
        }

        /// <summary>
        /// GetResourceParamsModelReturnsCorrectlyPopulatedModel.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsCorrectlyPopulatedModel()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(inputRecord.Title, resourceParamsModel.Title);
            Assert.Equal(inputRecord.Description, resourceParamsModel.Description);
            Assert.True(resourceParamsModel.SensitiveContentFlag);
            Assert.Equal(inputRecord.PublishedDate.Value, resourceParamsModel.CreateDate);
            Assert.Equal(1234, resourceParamsModel.UserId);
            Assert.Equal(99, resourceParamsModel.DestinationNodeId);

            Assert.Equal(inputRecord.WeblinkUrl, resourceParamsModel.WebLinkUrl);
            Assert.Equal(inputRecord.WeblinkText, resourceParamsModel.WebLinkDisplayText);
            Assert.Equal(inputRecord.ArticleBodyText, resourceParamsModel.ArticleBody);

            // Returns authors in AuthorIndex order and removes duplicate.
            Assert.Equal(2, resourceParamsModel.Authors.Count());
            Assert.Equal(inputRecord.AuthorName1, resourceParamsModel.Authors[0].Author);
            Assert.Equal(inputRecord.AuthorOrganisation1, resourceParamsModel.Authors[0].Organisation);
            Assert.Equal(inputRecord.AuthorRole1, resourceParamsModel.Authors[0].Role);
            Assert.Equal(inputRecord.AuthorName2, resourceParamsModel.Authors[1].Author);
            Assert.Equal(inputRecord.AuthorOrganisation2, resourceParamsModel.Authors[1].Organisation);
            Assert.Equal(inputRecord.AuthorRole2, resourceParamsModel.Authors[1].Role);

            Assert.Equal(3, resourceParamsModel.Keywords.Count());
            Assert.Equal("keyword1", resourceParamsModel.Keywords[0]);
            Assert.Equal("keyword2", resourceParamsModel.Keywords[1]);
            Assert.Equal("keyword3", resourceParamsModel.Keywords[2]);

            Assert.Equal(11, resourceParamsModel.ResourceFileUrls.Count());
            Assert.Equal(inputRecord.ServerFileName, resourceParamsModel.ResourceFileUrls[0]);
            Assert.Equal(inputRecord.ArticleFile1, resourceParamsModel.ResourceFileUrls[1]);
            Assert.Equal(inputRecord.ArticleFile2, resourceParamsModel.ResourceFileUrls[2]);
            Assert.Equal(inputRecord.ArticleFile3, resourceParamsModel.ResourceFileUrls[3]);
            Assert.Equal(inputRecord.ArticleFile4, resourceParamsModel.ResourceFileUrls[4]);
            Assert.Equal(inputRecord.ArticleFile5, resourceParamsModel.ResourceFileUrls[5]);
            Assert.Equal(inputRecord.ArticleFile6, resourceParamsModel.ResourceFileUrls[6]);
            Assert.Equal(inputRecord.ArticleFile7, resourceParamsModel.ResourceFileUrls[7]);
            Assert.Equal(inputRecord.ArticleFile8, resourceParamsModel.ResourceFileUrls[8]);
            Assert.Equal(inputRecord.ArticleFile9, resourceParamsModel.ResourceFileUrls[9]);
            Assert.Equal(inputRecord.ArticleFile10, resourceParamsModel.ResourceFileUrls[10]);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsArticleType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsArticleType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Article";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.Article, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsAudioType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsAudioType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "File";
            inputRecord.ServerFileName = "audio.mp3";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockFileTypeService.Setup(x => x.GetByFilename("audio.mp3")).Returns(Task.FromResult(new FileType() { DefaultResourceTypeId = (int)ResourceTypeEnum.Audio }));

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.Audio, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsImageType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsImageType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "File";
            inputRecord.ServerFileName = "image.jpg";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockFileTypeService.Setup(x => x.GetByFilename("image.jpg")).Returns(Task.FromResult(new FileType() { DefaultResourceTypeId = (int)ResourceTypeEnum.Image }));

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.Image, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsScormType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsScormType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Scorm";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.Scorm, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsVideoType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsVideoType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "File";
            inputRecord.ServerFileName = "video.mp4";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockFileTypeService.Setup(x => x.GetByFilename("video.mp4")).Returns(Task.FromResult(new FileType() { DefaultResourceTypeId = (int)ResourceTypeEnum.Video }));

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.Video, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsWebLinkType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsWebLinkType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "WebLink";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.WebLink, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsGenericFileType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsGenericFileType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "File";
            inputRecord.ServerFileName = "file.doc";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            this.mockFileTypeService.Setup(x => x.GetByFilename("file.doc")).Returns(Task.FromResult(new FileType() { DefaultResourceTypeId = (int)ResourceTypeEnum.GenericFile }));

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.GenericFile, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsGenericFileType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelThrowsExceptionIfNotRecognised()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "an unknown resource type";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData));
        }

        /// <summary>
        /// GetResourceParamsModelReturnsGenericFileType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsNonCommercialLicenceType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.Licence = "creative commons (e&w) attribution, non-commercial";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(1, resourceParamsModel.ResourceLicenceId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsGenericFileType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsShareALikeLicenceType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.Licence = "creative commons (e&w) attribution, non-commercial, share-a-like";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(2, resourceParamsModel.ResourceLicenceId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsGenericFileType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsNoDerivativesLicenceType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.Licence = "creative commons (e&w) attribution, non-commercial, no derivatives";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(3, resourceParamsModel.ResourceLicenceId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsGenericFileType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsAllRightsReservedLicenceType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.Licence = "all rights reserved";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(4, resourceParamsModel.ResourceLicenceId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsGenericFileType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsUnknownLicenceType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.Licence = "an unknown licence type";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(0, resourceParamsModel.ResourceLicenceId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsPartiallyPopulatedAuthorsCorrectly.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsPartiallyPopulatedAuthorsCorrectly()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.AuthorName1 = null;
            inputRecord.AuthorOrganisation1 = "org1";
            inputRecord.AuthorRole1 = "role1";
            inputRecord.AuthorName2 = null;
            inputRecord.AuthorOrganisation2 = "org2";
            inputRecord.AuthorRole2 = "role2";
            inputRecord.AuthorName3 = null;
            inputRecord.AuthorOrganisation3 = "org3";
            inputRecord.AuthorRole3 = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(3, resourceParamsModel.Authors.Count());
            Assert.Null(resourceParamsModel.Authors[0].Author);
            Assert.Equal("org1", resourceParamsModel.Authors[0].Organisation);
            Assert.Equal("role1", resourceParamsModel.Authors[0].Role);

            Assert.Null(resourceParamsModel.Authors[1].Author);
            Assert.Equal("org2", resourceParamsModel.Authors[1].Organisation);
            Assert.Equal("role2", resourceParamsModel.Authors[1].Role);

            Assert.Null(resourceParamsModel.Authors[2].Author);
            Assert.Equal("org3", resourceParamsModel.Authors[2].Organisation);
            Assert.Null(resourceParamsModel.Authors[2].Role);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsUsernameAsAuthorIfIAmTheAuthorIsTrue.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsUsernameAsAuthorIfIAmTheAuthorIsTrue()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.AuthorName1 = "replaced with username";
            inputRecord.AuthorOrganisation1 = "org1";
            inputRecord.AuthorRole1 = "role1";
            inputRecord.IAmTheAuthorFlag = "Yes";

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal("firstname lastname", resourceParamsModel.Authors[0].Author);
            Assert.Equal("org1", resourceParamsModel.Authors[0].Organisation);
            Assert.Equal("role1", resourceParamsModel.Authors[0].Role);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsEmptyStringAsWeblinkTextIfEmpty.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsEmptyStringAsWeblinkTextIfEmpty()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.WeblinkText = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(string.Empty, resourceParamsModel.WebLinkDisplayText);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsCommunityCatalogueNodeIdIfCatalogueIsNull.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsCommunityCatalogueNodeIdIfCatalogueIsNull()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.CatalogueName = null;

            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.stagingTableInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(1, resourceParamsModel.DestinationNodeId);
        }

        private StagingTableInputModel GetInputRecord()
        {
            var model = new StagingTableInputModel()
            {
                ResourceUniqueRef = "ref",
                Title = "title",
                Description = "description",
                SensitiveContentFlag = "Yes",
                Keywords = "keyword1, keyword2, keyword3, keyword3", // keyword3 is a duplicate
                CatalogueName = "my catalogue",
                Licence = "All Rights Reserved",
                AuthorName1 = "author1",
                AuthorOrganisation1 = "org1",
                AuthorRole1 = "role1",
                AuthorName2 = "author2",
                AuthorOrganisation2 = "org2",
                AuthorRole2 = "role2",
                ContributorLearningHubUserName = "username1",
                ResourceType = "Article",
                ArticleContentFilename = "articleBody.html",
                ArticleBodyText = "article body text",
                LMSLink = "Lms link",
                WeblinkUrl = "weblink url",
                WeblinkText = "weblink display text",
                ServerFileName = "server file name",
                ArticleFile1 = "article file 1",
                ArticleFile2 = "article file 2",
                ArticleFile3 = "article file 3",
                ArticleFile4 = "article file 4",
                ArticleFile5 = "article file 5",
                ArticleFile6 = "article file 6",
                ArticleFile7 = "article file 7",
                ArticleFile8 = "article file 8",
                ArticleFile9 = "article file 9",
                ArticleFile10 = "article file 10",
                PublishedDate = DateTime.Now,
            };

            return model;
        }
    }
}
