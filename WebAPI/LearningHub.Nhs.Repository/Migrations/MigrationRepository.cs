namespace LearningHub.Nhs.Repository.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Migration.Models.ResultModels;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Migrations;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// The migration repository.
    /// </summary>
    public class MigrationRepository : GenericRepository<Migration>, IMigrationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public MigrationRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Migration> GetByIdAsync(int id)
        {
            return await this.DbContext.Migration.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// Creates a new resource based on the supplied parameters.
        /// </summary>
        /// <param name="resourceParams">The resourceParams.</param>
        /// <param name="resourceFileParamsList">The list of files that have already been copied from the Azure migration blob container
        /// to the Azure resources file share.</param>
        /// <returns>The ResourceVersionId.</returns>
        public async Task<int> CreateResourceAsync(ResourceParamsModel resourceParams, List<ResourceFileParamsModel> resourceFileParamsList)
        {
            var stratergy = this.DbContext.Database.CreateExecutionStrategy();
            ResourceCreateProcResult resourceCreateResult = new ResourceCreateProcResult();
            await stratergy.Execute(
              async () =>
              {
                  using (IDbContextTransaction transaction = await this.DbContext.Database.BeginTransactionAsync())
                  {
                      if (resourceParams.ResourceTypeId == (int)ResourceTypeEnum.Article)
                      {
                          // If creating an Article, resource is created first, then the attachments.
                          resourceCreateResult = await this.CreateResource(resourceParams);
                          await this.CreateArticleResourceFiles(resourceCreateResult.ArticleResourceVersionId, resourceParams, resourceFileParamsList);
                      }
                      else
                      {
                          // For everything else, create the File first (if there is one!) then include its Id when creating the resource.
                          int fileId = 0;
                          if (resourceFileParamsList.Count() > 0)
                          {
                              fileId = await this.CreateResourceFile(resourceParams, resourceFileParamsList.Single());
                          }

                          resourceCreateResult = await this.CreateResource(resourceParams, fileId);
                      }

                      await this.CreateKeywords(resourceCreateResult.ResourceVersionId, resourceParams);
                      await this.CreateAuthors(resourceCreateResult.ResourceVersionId, resourceParams);

                      transaction.Commit();

                      return resourceCreateResult.ResourceVersionId;
                  }
              });
            return resourceCreateResult.ResourceVersionId;
        }

        private async Task<ResourceCreateProcResult> CreateResource(ResourceParamsModel resourceParams, int fileId = 0)
        {
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@DestinationNodeId", SqlDbType.Int) { Value = resourceParams.DestinationNodeId });
            sqlParams.Add(new SqlParameter("@MigrationInputRecordId", SqlDbType.Int) { Value = resourceParams.MigrationInputRecordId });
            sqlParams.Add(new SqlParameter("@ResourceTypeId", SqlDbType.Int) { Value = resourceParams.ResourceTypeId });
            sqlParams.Add(new SqlParameter("@Title", SqlDbType.NVarChar) { Value = resourceParams.Title });
            sqlParams.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = resourceParams.Description });
            sqlParams.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = resourceParams.UserId });
            sqlParams.Add(new SqlParameter("@SensitiveContentFlag", SqlDbType.Bit) { Value = resourceParams.SensitiveContentFlag });
            sqlParams.Add(new SqlParameter("@UserTimezoneOffset", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value });
            var resourceVersionIdOutput = new SqlParameter("@ResourceVersionId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            sqlParams.Add(resourceVersionIdOutput);
            string sql = "migrations.ResourceCreate @DestinationNodeId, @MigrationInputRecordId, @ResourceTypeId, @Title, @Description, @UserId, @SensitiveContentFlag, @ResourceVersionId output, @UserTimezoneOffset=@UserTimezoneOffset";

            if (fileId > 0)
            {
                sqlParams.Add(new SqlParameter("@FileId", SqlDbType.Int) { Value = fileId });
                sql += ", @FileId=@FileId";
            }

            if (resourceParams.ResourceLicenceId > 0)
            {
                sqlParams.Add(new SqlParameter("@ResourceLicenceId", SqlDbType.NVarChar) { Value = resourceParams.ResourceLicenceId });
                sql += ", @ResourceLicenceId=@ResourceLicenceId";
            }

            if (!string.IsNullOrEmpty(resourceParams.AdditionalInformation))
            {
                sqlParams.Add(new SqlParameter("@AdditionalInformation", SqlDbType.NVarChar) { Value = resourceParams.AdditionalInformation });
                sql += ", @AdditionalInformation=@AdditionalInformation";
            }

            SqlParameter articleResourceVersionIdOutput = null;
            if (resourceParams.ResourceTypeId == (int)ResourceTypeEnum.Article)
            {
                sqlParams.Add(new SqlParameter("@ArticleBody", SqlDbType.NVarChar) { Value = resourceParams.ArticleBody });
                sql += ", @ArticleBody=@ArticleBody";
                articleResourceVersionIdOutput = new SqlParameter("@ArticleResourceVersionId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                sqlParams.Add(articleResourceVersionIdOutput);
                sql += ", @ArticleResourceVersionId=@ArticleResourceVersionId output";
            }
            else if (resourceParams.ResourceTypeId == (int)ResourceTypeEnum.WebLink)
            {
                sqlParams.Add(new SqlParameter("@WebLinkURL", SqlDbType.NVarChar) { Value = resourceParams.WebLinkUrl });
                sqlParams.Add(new SqlParameter("@WebLinkDisplayText", SqlDbType.NVarChar) { Value = resourceParams.WebLinkDisplayText });
                sql += ", @WebLinkURL=@WebLinkURL, @WebLinkDisplayText=@WebLinkDisplayText";
            }
            else if (resourceParams.ResourceTypeId == (int)ResourceTypeEnum.Scorm)
            {
                if (resourceParams.EsrLinkTypeId > 0)
                {
                    sqlParams.Add(new SqlParameter("@ESRLinkTypeId", SqlDbType.Int) { Value = resourceParams.EsrLinkTypeId });
                    sql += ", @ESRLinkTypeId=@ESRLinkTypeId";
                }
            }
            else if (resourceParams.ResourceTypeId == (int)ResourceTypeEnum.GenericFile)
            {
                if (resourceParams.YearAuthored > 0)
                {
                    sqlParams.Add(new SqlParameter("@YearAuthored", SqlDbType.Int) { Value = resourceParams.YearAuthored });
                    sql += ", @YearAuthored=@YearAuthored";
                }

                if (resourceParams.MonthAuthored > 0)
                {
                    sqlParams.Add(new SqlParameter("@MonthAuthored", SqlDbType.Int) { Value = resourceParams.MonthAuthored });
                    sql += ", @MonthAuthored=@MonthAuthored";
                }

                if (resourceParams.DayAuthored > 0)
                {
                    sqlParams.Add(new SqlParameter("@DayAuthored", SqlDbType.Int) { Value = resourceParams.DayAuthored });
                    sql += ", @DayAuthored=@DayAuthored";
                }
            }

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);

            var result = new ResourceCreateProcResult
            {
                ResourceVersionId = (int)resourceVersionIdOutput.Value,
                ArticleResourceVersionId = (articleResourceVersionIdOutput != null) ? (int)articleResourceVersionIdOutput.Value : 0,
            };

            return result;
        }

        private async Task CreateKeywords(int resourceVersionId, ResourceParamsModel resourceParams)
        {
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@ResourceVersionId", SqlDbType.Int) { Value = resourceVersionId });
            var keywordParam = new SqlParameter("@Keyword", SqlDbType.NVarChar);
            sqlParams.Add(keywordParam);
            sqlParams.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = resourceParams.UserId });
            sqlParams.Add(new SqlParameter("@UserTimezoneOffset", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value });
            string sql = "migrations.ResourceVersionKeywordCreate @ResourceVersionId, @Keyword, @UserId, @UserTimezoneOffset";

            foreach (string keyword in resourceParams.Keywords)
            {
                keywordParam.Value = keyword;
                await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
            }
        }

        private async Task CreateAuthors(int resourceVersionId, ResourceParamsModel resourceParams)
        {
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@ResourceVersionId", SqlDbType.Int) { Value = resourceVersionId });

            var iAmTheAuthorParam = new SqlParameter("@IAmTheAuthor", SqlDbType.Bit);
            sqlParams.Add(iAmTheAuthorParam);

            var authorNameParam = new SqlParameter("@AuthorName", SqlDbType.NVarChar);
            sqlParams.Add(authorNameParam);

            var orgParam = new SqlParameter("@Organisation", SqlDbType.NVarChar);
            sqlParams.Add(orgParam);

            var roleParam = new SqlParameter("@Role", SqlDbType.NVarChar);
            sqlParams.Add(roleParam);

            sqlParams.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = resourceParams.UserId });

            sqlParams.Add(new SqlParameter("@UserTimezoneOffset", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value });

            string sql = "migrations.ResourceVersionAuthorCreate @ResourceVersionId, @IAmTheAuthor, @AuthorName, @Organisation, @Role, @UserId, @UserTimezoneOffset";

            foreach (AuthorParamsModel author in resourceParams.Authors)
            {
                iAmTheAuthorParam.Value = author.IAmTheAuthor;
                authorNameParam.Value = author.Author ?? string.Empty;
                orgParam.Value = author.Organisation ?? string.Empty;
                roleParam.Value = author.Role ?? string.Empty;
                await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
            }
        }

        private async Task<int> CreateResourceFile(ResourceParamsModel resourceParams, ResourceFileParamsModel resourceFileParams)
        {
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@FileTypeId", SqlDbType.Int) { Value = resourceFileParams.FileTypeId });
            sqlParams.Add(new SqlParameter("@FileName", SqlDbType.NVarChar) { Value = resourceFileParams.FileName });
            sqlParams.Add(new SqlParameter("@FilePath", SqlDbType.NVarChar) { Value = resourceFileParams.FilePath });
            sqlParams.Add(new SqlParameter("@FileSizeKb", SqlDbType.Int) { Value = resourceFileParams.FileSizeKb });
            sqlParams.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = resourceParams.UserId });
            sqlParams.Add(new SqlParameter("@UserTimezoneOffset", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value });
            var output = new SqlParameter("@FileId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            sqlParams.Add(output);
            string sql = "migrations.ResourceFileCreate @FileTypeId, @FileName, @FilePath, @FileSizeKb, @UserId, @UserTimezoneOffset, @FileId output";

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
            int fileId = (int)output.Value;
            return fileId;
        }

        private async Task CreateArticleResourceFiles(int articleResourceVersionId, ResourceParamsModel resourceParams, List<ResourceFileParamsModel> resourceFileParamsList)
        {
            foreach (ResourceFileParamsModel resourceFileParams in resourceFileParamsList)
            {
                int fileId = await this.CreateResourceFile(resourceParams, resourceFileParams);

                var sqlParams = new List<SqlParameter>();
                sqlParams.Add(new SqlParameter("@ArticleResourceVersionId", SqlDbType.Int) { Value = articleResourceVersionId });
                sqlParams.Add(new SqlParameter("@FileId", SqlDbType.Int) { Value = fileId });
                sqlParams.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = resourceParams.UserId });
                sqlParams.Add(new SqlParameter("@UserTimezoneOffset", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value });
                string sql = "migrations.ArticleResourceVersionFileCreate @ArticleResourceVersionId, @FileId, @UserId, @UserTimezoneOffset";

                await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
            }
        }
    }
}
