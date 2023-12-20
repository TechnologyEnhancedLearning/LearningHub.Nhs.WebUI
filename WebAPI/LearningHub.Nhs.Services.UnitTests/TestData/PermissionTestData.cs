// <copyright file="PermissionTestData.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests.TestData
{
    using System;
    using System.Linq;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The permission test data.
    /// </summary>
    internal class PermissionTestData
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private LearningHubDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionTestData"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        internal PermissionTestData(LearningHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// The setup.
        /// </summary>
        internal void Setup()
        {
            var role1 = new Role() { Name = "test role 1", AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now, };
            var permission11 = new Permission() { Code = "test1-1", Name = "test permission 1-1", AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now };
            var permission12 = new Permission() { Code = "test1-2", Name = "test permission 1-2", AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now };

            role1.PermissionRole.Add(new PermissionRole() { Role = role1, Permission = permission11 });
            this.dbContext.Role.Add(role1);
            //// permission12 as disconnected from Role
            this.dbContext.Permission.Add(permission12);
            this.dbContext.SaveChanges();

            var permission21 = new Permission() { Code = "test2-1", Name = "test permission 2-1", AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now };
            var permission22 = new Permission() { Code = "test2-2", Name = "test permission 2-2", AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now };

            this.dbContext.Permission.Add(permission21);
            this.dbContext.Permission.Add(permission22);
            this.dbContext.SaveChanges();

            var role2 = new Role() { Name = "test role 2", AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now };

            role2.PermissionRole.Add(new PermissionRole() { Role = role2, Permission = permission21 });
            role2.PermissionRole.Add(new PermissionRole() { Role = role2, Permission = permission22 });

            var userGroup1 = new UserGroup() { Name = "test user group 1", AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now };
            userGroup1.RoleUserGroup.Add(new RoleUserGroup() { RoleId = role1.Id, Role = role1, UserGroup = userGroup1, AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now });

            var userGroup2 = new UserGroup() { Name = "test user group 2", AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now };
            userGroup2.RoleUserGroup.Add(new RoleUserGroup() { RoleId = role1.Id, Role = role1, UserGroup = userGroup1, AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now });
            userGroup2.RoleUserGroup.Add(new RoleUserGroup() { RoleId = role2.Id, Role = role2, UserGroup = userGroup1, AmendUserId = 4, AmendDate = DateTimeOffset.Now, CreateUserId = 4, CreateDate = DateTimeOffset.Now });

            this.dbContext.UserGroup.Add(userGroup1);
            this.dbContext.UserGroup.Add(userGroup2);

            this.dbContext.SaveChanges();
        }

        /// <summary>
        /// The teardown.
        /// </summary>
        internal void Teardown()
        {
            var userGroups = this.dbContext.UserGroup.IgnoreQueryFilters().Where(ug1 => ug1.Name.StartsWith("test")).ToList();
            var roleUserGroups = this.dbContext.RoleUserGroup.IgnoreQueryFilters().Where(rug => rug.UserGroup.Name.StartsWith("test")).ToList();
            var roles = this.dbContext.Role.IgnoreQueryFilters().Where(r => r.Name.StartsWith("test")).ToList();
            var permissionRoles = this.dbContext.PermissionRole.IgnoreQueryFilters().Where(pr => pr.Permission.Name.StartsWith("test")).ToList();
            var permissions = this.dbContext.Permission.IgnoreQueryFilters().Where(p => p.Name.StartsWith("test")).ToList();

            this.dbContext.RoleUserGroup.RemoveRange(roleUserGroups);
            this.dbContext.PermissionRole.RemoveRange(permissionRoles);
            this.dbContext.UserGroup.RemoveRange(userGroups);
            this.dbContext.Role.RemoveRange(roles);
            this.dbContext.Permission.RemoveRange(permissions);

            this.dbContext.SaveChanges();
        }
    }
}
