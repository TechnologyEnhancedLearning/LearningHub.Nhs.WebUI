// <copyright file="TenantHelper.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Helpers
{
    using System.IO;
    using LearningHub.Nhs.WebUI.Models;

    /// <summary>
    /// Defines the <see cref="TenantHelper" />.
    /// </summary>
    public class TenantHelper
    {
        /// <summary>
        /// The GetLayoutPath.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="tenant">The tenant<see cref="TenantViewModel"/>.</param>
        /// <returns>The .</returns>
        public static string GetLayoutPath(string path, TenantViewModel tenant)
        {
            string basepath = Path.Combine(path, "/Views/Shared");
            return GetFilePathForTenant(basepath, "_Layout.cshtml", true, tenant);
        }

        /// <summary>
        /// The GetFilePathForTenant.
        /// </summary>
        /// <param name="basePath">The basePath.</param>
        /// <param name="filePath">The filePath.</param>
        /// <param name="includeTenantSubDirectory">The includeTenantSubDirectory<see cref="bool"/>.</param>
        /// <param name="tenant">The tenant<see cref="TenantViewModel"/>.</param>
        /// <returns>The .</returns>
        private static string GetFilePathForTenant(string basePath, string filePath, bool includeTenantSubDirectory, TenantViewModel tenant)
        {
            // If supplied filePath contains a path separator then separate the path component from the file name
            int pathSeparatorPosn = filePath.LastIndexOf('/');
            if (pathSeparatorPosn > 0)
            {
                basePath += filePath.Substring(0, pathSeparatorPosn);
                filePath = filePath.Substring(pathSeparatorPosn + 1, filePath.Length - pathSeparatorPosn - 1);
            }

            // Look for the file in a Tenant sub folder, if not found then return the default file path
            string tenantFilePath;
            if (includeTenantSubDirectory)
            {
                tenantFilePath = string.Format("{0}/Tenant/{1}/{2}", basePath, tenant.Code, filePath);
            }
            else
            {
                tenantFilePath = string.Format("{0}/{1}/{2}", basePath, tenant.Code, filePath);
            }

            return tenantFilePath;
        }
    }
}
