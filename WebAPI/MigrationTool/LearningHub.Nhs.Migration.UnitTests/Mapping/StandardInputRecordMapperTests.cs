namespace LearningHub.Nhs.Migration.UnitTests.Mapping
{
    using System;
    using System.Linq;
    using LearningHub.Nhs.Migration.Interface.Mapping;
    using LearningHub.Nhs.Migration.Mapping;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Models.Enums;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// The StandardResourceMapper tests.
    /// </summary>
    public class StandardInputRecordMapperTests
    {
        /// <summary>
        /// The standard input record mapper.
        /// </summary>
        private readonly IInputRecordMapper standardInputRecordMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardInputRecordMapperTests"/> class.
        /// </summary>
        public StandardInputRecordMapperTests()
        {
            this.standardInputRecordMapper = new StandardInputRecordMapper();
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
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(inputRecord.Title, resourceParamsModel.Title);
            Assert.Equal(inputRecord.Description, resourceParamsModel.Description);
            Assert.Equal(inputRecord.CreateDate.Value, resourceParamsModel.CreateDate);
            Assert.Equal(int.Parse(inputRecord.ElfhUserId), resourceParamsModel.UserId);
            Assert.Equal(inputRecord.WebLinkUrl, resourceParamsModel.WebLinkUrl);
            Assert.Equal(inputRecord.ArticleBody, resourceParamsModel.ArticleBody);

            // Returns authors in AuthorIndex order and removes duplicate.
            Assert.Equal(2, resourceParamsModel.Authors.Count());
            Assert.Equal(inputRecord.Authors.ElementAt(1).Author, resourceParamsModel.Authors[0].Author);
            Assert.Equal(inputRecord.Authors.ElementAt(1).Organisation, resourceParamsModel.Authors[0].Organisation);
            Assert.Equal(inputRecord.Authors.ElementAt(1).Role, resourceParamsModel.Authors[0].Role);
            Assert.Equal(inputRecord.Authors.ElementAt(0).Author, resourceParamsModel.Authors[1].Author);
            Assert.Equal(inputRecord.Authors.ElementAt(0).Organisation, resourceParamsModel.Authors[1].Organisation);
            Assert.Equal(inputRecord.Authors.ElementAt(0).Role, resourceParamsModel.Authors[1].Role);

            Assert.Equal(3, resourceParamsModel.Keywords.Count());
            Assert.Equal(inputRecord.Keywords.ElementAt(0), resourceParamsModel.Keywords[0]);
            Assert.Equal(inputRecord.Keywords.ElementAt(1), resourceParamsModel.Keywords[1]);
            Assert.Equal(inputRecord.Keywords.ElementAt(2), resourceParamsModel.Keywords[2]);

            // Returns resource urls in ResourceIndex order.
            Assert.Equal(3, resourceParamsModel.ResourceFileUrls.Count());
            Assert.Equal(inputRecord.ResourceFiles.ElementAt(1).ResourceUrl, resourceParamsModel.ResourceFileUrls[0]);
            Assert.Equal(inputRecord.ResourceFiles.ElementAt(0).ResourceUrl, resourceParamsModel.ResourceFileUrls[1]);
            Assert.Equal(inputRecord.ResourceFiles.ElementAt(2).ResourceUrl, resourceParamsModel.ResourceFileUrls[2]);
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
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            inputRecord.ResourceType = "Audio";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.Audio, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsEmbeddedType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsEmbeddedType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Embedded";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.Embedded, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsEquipmentType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsEquipmentType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Equipment";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal((int)ResourceTypeEnum.Equipment, resourceParamsModel.ResourceTypeId);
        }

        /// <summary>
        /// GetResourceParamsModelReturnsImageType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsImageType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.ResourceType = "Image";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            inputRecord.ResourceType = "Video";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            inputRecord.ResourceType = "GenericFile";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            Assert.Throws<NotSupportedException>(() => this.standardInputRecordMapper.GetResourceParamsModel(jsonData));
        }

        /// <summary>
        /// GetResourceParamsModelReturnsGenericFileType.
        /// </summary>
        [Fact]
        public void GetResourceParamsModelReturnsNonCommercialLicenceType()
        {
            // Arrange
            var inputRecord = this.GetInputRecord();
            inputRecord.LicenceType = "creative commons (e&w) attribution, non-commercial";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            inputRecord.LicenceType = "creative commons (e&w) attribution, non-commercial, share-a-like";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            inputRecord.LicenceType = "creative commons (e&w) attribution, non-commercial, no derivatives";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            inputRecord.LicenceType = "all rights reserved";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

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
            inputRecord.LicenceType = "an unknown licence type";
            string jsonData = JsonConvert.SerializeObject(inputRecord);

            // Act
            var resourceParamsModel = this.standardInputRecordMapper.GetResourceParamsModel(jsonData);

            // Assert
            Assert.Equal(0, resourceParamsModel.ResourceLicenceId);
        }

        private StandardInputModel GetInputRecord()
        {
            var model = new StandardInputModel()
            {
                Title = "title",
                Authors = new AuthorModel[]
                {
                    new AuthorModel() { AuthorIndex = 1, Author = "author2", Organisation = "org2", Role = "role2" }, // out of order
                    new AuthorModel() { AuthorIndex = 0, Author = "author1", Organisation = "org1", Role = "role1" },
                },
                HasCost = false,
                Version = "1.0",
                Keywords = new string[] { "keyword1", "keyword2", "keyword3", "keyword3" }, // keyword3 is a duplicate
                CreateDate = DateTime.Now,
                ElfhUserId = "999",
                Description = "description",
                WebLinkUrl = "weblink url",
                ArticleBody = "article body",
                LicenceType = "All Rights Reserved",
                ResourceType = "SCORM",
                ResourceFiles = new ResourceFileModel[]
                {
                    new ResourceFileModel() { ResourceIndex = 1, ResourceUrl = "resource file name 1" }, // out of order
                    new ResourceFileModel() { ResourceIndex = 0, ResourceUrl = "resource file name 0" },
                    new ResourceFileModel() { ResourceIndex = 2, ResourceUrl = "resource file name 2" },
                },
            };

            return model;
        }
    }
}
