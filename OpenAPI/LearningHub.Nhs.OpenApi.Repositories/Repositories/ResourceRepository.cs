namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Resources;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Activity;//qqqq move to activity repository
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <inheritdoc />
    public class ResourceRepository : IResourceRepository
    {
        private LearningHubDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRepository"/> class.
        /// </summary>
        /// <param name="dbContext"><see cref="dbContext"/>.</param>
        public ResourceRepository(LearningHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceIds">.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds)
        {
            var resources = await this.dbContext.Resource
                                                    .AsNoTracking()
                                                    .Where(r => resourceIds.Contains(r.Id) && !r.Deleted)
                                                    .Include(r => r.ResourceReference)
                                                        .ThenInclude(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
                                                    .Include(r => r.CurrentResourceVersion.ResourceVersionRatingSummary)
                                                    .Include(r => r.ResourceReference)
                                                        .ThenInclude(rr => rr.NodePath.Node)
                                                    .ToListAsync();

            resources.ForEach(r =>
            {
                var nonExternalReferences = r.ResourceReference
                    .Where(rr => rr?.NodePath?.Node?.NodeTypeEnum != null && (int)rr.NodePath.Node.NodeTypeEnum != 4)
                    .ToList();

                r.ResourceReference = nonExternalReferences;
            });

            return resources;
        }

        ///// <inheritdoc />
        /////qqqq
        //public async Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds/*, int userId*/)
        //{

        //    var resources = await this.dbContext.Resource
        //                                            .AsNoTracking()
        //                                            .Where(r => resourceIds.Contains(r.Id) && !r.Deleted)
        //                                            .Include(r => r.ResourceVersion)
        //                                            .Include(r => r.ResourceReference)
        //                                                .ThenInclude(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
        //                                            .Include(r => r.CurrentResourceVersion.ResourceVersionRatingSummary)
        //                                            .Include(r => r.ResourceReference)
        //                                                .ThenInclude(rr => rr.NodePath.Node)
        //                                            //.Include(r => r.ResourceActivity.Where(ra => ra.UserId == userId && !ra.Deleted))
        //                                            .ToListAsync();
            
        //    // nodepath just for catalogues
        //    // has to have catalogue, but one catalogue community contribution, whether this is null or a specific catalogueid to catch it
        //    // qqqq what is this for - in proc can i just right join and NodeTypeEnum != 4 in where
        //    resources.ForEach(r =>
        //    {
        //        var nonExternalReferences = r.ResourceReference
        //            .Where(rr =>
        //            rr?.NodePath?.Node?.NodeTypeEnum != null &&
        //            (int)rr.NodePath.Node.NodeTypeEnum != 4)
        //            .ToList();

        //        r.ResourceReference = nonExternalReferences;
        //    });

        //    return resources;
        //}

        ///// <inheritdoc/>
        /////qqqq
        //public async Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
        //    IEnumerable<int> originalResourceReferenceIds, int userId)
        //{
        //        return await this.dbContext.ResourceReference
        //    .AsNoTracking()
        //    .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId) &&
        //                 !rr.Deleted &&
        //                 (int)rr.NodePath.Node.NodeTypeEnum != 4)
        //    .Include(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
        //    .Include(rr => rr.Resource.ResourceActivity
        //        .Where(ra => ra.UserId == userId && !ra.Deleted))
        //        .Include(rr => rr.Resource.CurrentResourceVersion.ResourceVersionRatingSummary)
        //        .ToListAsync();
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="originalResourceReferenceIds">.</param>
        /// <returns>A <see cref="Task{ResourceReference}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
          IEnumerable<int> originalResourceReferenceIds)
        {
            return await this.dbContext.ResourceReference
                .AsNoTracking()
                .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId) &&
                         !rr.Deleted &&
                         (int)rr.NodePath.Node.NodeTypeEnum != 4)
                .Include(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
                .Include(rr => rr.Resource.CurrentResourceVersion.ResourceVersionRatingSummary)
                .ToListAsync();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceReferenceIds"></param>
        /// <param name="userIds"></param>
        /// <param name="originalResourceReferenceIds">.</param>
        /// <returns>A <see cref="Task{ResourceReference}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<ResourceActivity>> GetResourceActivityPerResourceMajorVersion(
          IEnumerable<int>? resourceReferenceIds, IEnumerable<int>? userIds)
        { // qqqq compare with GetByUserIdWithResourceVersionId
            var resourceIdsParam = resourceReferenceIds != null
                ? string.Join(",", resourceReferenceIds)
                : null;

            var userIdsParam = userIds != null
                ? string.Join(",", userIds)
                : null;

            //var param1 = new SqlParameter("@searchText", SqlDbType.NVarChar) { Value = requestModel.SearchText == null ? DBNull.Value : requestModel.SearchText };
            var resourceIdsParameter = new SqlParameter("@p0", resourceIdsParam ?? (object)DBNull.Value);
            var userIdsParameter = new SqlParameter("@p1", userIdsParam ?? (object)DBNull.Value);

            //Test 
            // resourceIdsParameter = new SqlParameter("@p0", (object)DBNull.Value);
            // userIdsParameter = new SqlParameter("@p1",  (object)DBNull.Value);

            // var param8 = new SqlParameter("@totalcount", SqlDbType.Int) { Direction = ParameterDirection.Output };

            //var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
            //var param1 = new SqlParameter("@resourceVersionId", SqlDbType.Int) { Value = resourceVersionId };
            //var result = await this.DbContext.MyLearningActivity.FromSqlRaw("[activity].[GetUserLatestActivityCheck] @userId, @resourceVersionId ", param0, param1)
            //    .AsNoTracking().ToListAsync();

            //// Bind result to ResourceActivity model
            //this.BindNestedData(result);
            //List<ResourceActivity> listOfresourceActivities = result.Select(i => new ResourceActivity()
            //{
            //    ActivityEnd = i.ActivityEnd,
            //    ActivityStart = i.ActivityStart,
            //    ActivityStatusId = i.ActivityStatusId,
            //    AmendDate = i.AmendDate,
            //    AmendUserId = i.AmendUserId,
            //    AssessmentResourceActivity = i.AssessmentResourceActivity,
            //    CreateDate = i.CreateDate,
            //    CreateUserId = i.CreateUserId,
            //    Deleted = i.Deleted,
            //    DurationSeconds = i.DurationSeconds ?? 0,
            //    Id = i.Id,
            //    InverseLaunchResourceActivity = i.Resource_InverseLaunchResourceActivity,
            //    LaunchResourceActivityId = i.LaunchResourceActivityId,
            //    MajorVersion = i.MajorVersion,
            //    MediaResourceActivity = i.Resource_MediaResourceActivity,
            //    MinorVersion = i.MinorVersion,
            //    NodePath = i.NodePath,
            //    NodePathId = i.NodePathId,
            //    Resource = i.Resource,
            //    ResourceId = i.ResourceId,
            //    ResourceVersion = i.ResourceVersion,
            //    ResourceVersionId = i.ResourceVersionId,
            //    Score = i.Score,
            //    ScormActivity = i.ScormActivity,
            //    UserId = i.UserId,
            //}).ToList();
            //return listOfresourceActivities.OrderByDescending(r => r.ActivityStart).AsQueryable();

            try
            {
                //var result = await this.dbContext.MyLearningActivity.FromSqlRaw("[activity].[GetUserLatestActivityCheck] @userId, @resourceVersionId ", param0, param1)
                //    .AsNoTracking().ToListAsync();

                //var results = dbContext.ResourceActivity
                //.FromSqlRaw(
                //    "[activity].[GetResourceActivityPerResourceMajorVersion] @ResourceIds, @UserIds",
                //    resourceIdsParameter,
                //    userIdsParameter)
                //.AsNoTracking()
                //.AsEnumerable<ResourceActivity>();

                //var results = await this.dbContext.ResourceActivity
                //    .FromSqlRaw("[resource].[GetResourceActivityPerResourceMajorVersion]")
                //    .AsNoTracking()
                //    .ToListAsync();

                //var results = await dbContext.ResourceActivity
                //.FromSqlRaw("[resource].[GetResourceActivityPerResourceMajorVersion]")
                //.AsNoTracking()
                //.ToListAsync();

                //var results = await this.dbContext.ResourceActivity
                //.FromSqlRaw("[activity].[GetResourceActivity]")
                //.AsNoTracking()
                //.ToListAsync();

                //var results = await this.dbContext.ResourceActivity
                //.FromSqlRaw("[resource].[GetResourceActivity]")
                //.AsNoTracking()
                //.ToListAsync();

                var query = await this.dbContext.ResourceActivity.FromSqlRaw("[activity].[GetResourceActivity]")
                   .AsNoTracking()
                    .ToListAsync();
                //var ls = query.ToList();

                //var query = this.dbContext.ResourceActivity.FromSqlRaw("[activity].[GetResourceActivity]").AsEnumerable();

                // var queryResult2 =  query.AsEnumerable().Cast<dynamic>().ToList();
                //  var results2 = queryResult2.ToList();
                // Step 2: Execute the query and materialize the results using AsEnumerable()
                //var queryResult = query.AsEnumerable();

                // Step 3: Conver//t query result to a list
                // var results = queryResult.ToList();
                var results = query;
                // Step 4: Return the results
                return (IEnumerable<ResourceActivity>)results;


                //.ToListAsync();

                //.Select(ra => new ResourceActivity
                //{
                //    Id = ra.Id,
                //    UserId = ra.UserId,
                //    //LaunchResourceActivityId = ra.LaunchResourceActivityId,
                //    //ResourceId = ra.ResourceId,
                //    //ResourceVersionId = ra.ResourceVersionId,
                //    //MajorVersion = ra.MajorVersion,
                //    //MinorVersion = ra.MinorVersion,
                //    //NodePathId = ra.NodePathId,
                //    //ActivityStatusId = ra.ActivityStatusId,
                //    //ActivityStart = ra.ActivityStart,
                //    //ActivityEnd = ra.ActivityEnd,
                //    //DurationSeconds = ra.DurationSeconds,
                //    //Score = ra.Score,
                //    //Deleted = ra.Deleted,
                //    //CreateUserId = ra.CreateUserId,
                //    //CreateDate = ra.CreateDate,
                //    //AmendUserId = ra.AmendUserId,
                //    //AmendDate = ra.AmendDate,
                //    //////Resource = ra.Resource,
                //    //////ResourceVersion = ra.ResourceVersion,
                //    //////ActivityStatus = ra.ActivityStatus,
                //    //////LaunchResourceActivity = ra.LaunchResourceActivity,
                //    //////NodePath = ra.NodePath,
                //    //////InverseLaunchResourceActivity = ra.InverseLaunchResourceActivity,
                //    //////MediaResourceActivity = ra.MediaResourceActivity,
                //    //////AssessmentResourceActivity = ra.AssessmentResourceActivity,
                //    //////ScormActivity = ra.ScormActivity,
                //})


                //var resourceActivityForMajorVersions = results;
                //var qqqq = resourceActivityForMajorVersions.ToList();
                //return (IEnumerable<ResourceActivity>)resourceActivityForMajorVersions;
            }
            catch(Exception e)
            {
                //var query = this.dbContext.ResourceActivity.FromSqlRaw("[resources].[GetResourceActivity]").AsEnumerable();
                //var thing = query.ToList();
                throw e;
            }

        }

        /// <inheritdoc/>
        public void QqqqTest()
        {
            List<int> originalResourceReferenceIds = new List<int>() { 302 };
            List<int> resourceIds = new List<int>() { 303 };
            int userId = 57541;

            var getResourceReferencesByOriginalResourceReferenceIdsQuery = this.dbContext.ResourceReference //has
                                                                                                            //.AsNoTracking()
                 .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId) &&
                          !rr.Deleted &&
                          (int)rr.NodePath.Node.NodeTypeEnum != 4)
                 .Include(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion) //has
                 .Include(rr => rr.Resource.CurrentResourceVersion.ResourceVersionRatingSummary) //has
                 ;

            var GetResourcesFromIdsQuery =  this.dbContext.Resource //has
                                                    //.AsNoTracking()
                                                    .Where(r => resourceIds.Contains(r.Id) && !r.Deleted)
                                                    .Include(r => r.ResourceVersion) //not shared    -- is it used
                                                    .Include(r => r.ResourceReference) //has 
                                                        .ThenInclude(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion) //has
                                                    .Include(r => r.CurrentResourceVersion.ResourceVersionRatingSummary) //has
                                                    .Include(r => r.ResourceReference)
                                                        .ThenInclude(rr => rr.NodePath.Node) //hasnt except for the where
                                                    .Include(r => r.ResourceActivity.Where(ra => ra.UserId == userId && !ra.Deleted))
                                                    ;


            var query = getResourceReferencesByOriginalResourceReferenceIdsQuery;
            // Retrieve and print SQL before execution
            string sql = query.ToQueryString();
            Console.WriteLine(sql);

            // Execute the query asynchronously
            var resources = query.ToListAsync();
        }

    }
}
