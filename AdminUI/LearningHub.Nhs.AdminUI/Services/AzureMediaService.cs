// <copyright file="AzureMediaService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Security.Claims;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.Management.Media;
    using Microsoft.Azure.Management.Media.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure.Authentication;

    /// <summary>
    /// Defines the <see cref="AzureMediaService" />.
    /// </summary>
    public class AzureMediaService : IAzureMediaService
    {
        private readonly ILogger logger;
        private WebSettings settings;
        private IAzureMediaServicesClient azureMediaServicesClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureMediaService"/> class.
        /// </summary>
        /// <param name="settings">Settings.</param>
        /// <param name="logger">Logger.</param>
        public AzureMediaService(IOptions<WebSettings> settings, ILogger<AzureMediaService> logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Create AzureMedia InputAsset from file upload.
        /// </summary>
        /// <param name="file">File.</param>
        /// <returns>.</returns>
        public async Task<string> CreateMediaInputAsset(IFormFile file)
        {
            string uniqueness = Guid.NewGuid().ToString().Substring(0, 13);
            string inputAssetName = $"input-{uniqueness}";

            IAzureMediaServicesClient client = await this.CreateMediaServicesClientAsync();

            Asset asset = await client.Assets.CreateOrUpdateAsync(this.settings.AzureMediaResourceGroup, this.settings.AzureMediaAccountName, inputAssetName, new Asset());

            ListContainerSasInput input = new ListContainerSasInput()
            {
                Permissions = AssetContainerPermission.ReadWrite,
                ExpiryTime = DateTime.Now.AddHours(2).ToUniversalTime(),
            };

            var response = client.Assets.ListContainerSasAsync(this.settings.AzureMediaResourceGroup, this.settings.AzureMediaAccountName, inputAssetName, input.Permissions, input.ExpiryTime).Result;

            string uploadSasUrl = response.AssetContainerSasUrls.First();
            string filename = Regex.Replace(file.FileName, "[^a-zA-Z0-9.]", string.Empty);

            var blobServiceClient = new BlobContainerClient(new Uri(uploadSasUrl));
            var blobClient = blobServiceClient.GetBlockBlobClient(filename);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }

            return asset.Name;
        }

        /// <summary>
        /// Download the input media file from Azure Media Services.
        /// </summary>
        /// <param name="inputAssetName">The name of the input asset.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>.</returns>
        public async Task<BlobDownloadResult> DownloadMediaInputAsset(string inputAssetName, string fileName)
        {
            IAzureMediaServicesClient client = await this.CreateMediaServicesClientAsync();

            AssetContainerSas assetContainerSas = await client.Assets.ListContainerSasAsync(
                this.settings.AzureMediaResourceGroup,
                this.settings.AzureMediaAccountName,
                inputAssetName,
                permissions: AssetContainerPermission.Read,
                expiryTime: DateTime.UtcNow.AddHours(1).ToUniversalTime());

            string sasUri = assetContainerSas.AssetContainerSasUrls.First();

            var blobServiceClient = new BlobContainerClient(new Uri(sasUri));
            var blobClient = blobServiceClient.GetBlockBlobClient(fileName);
            var fileContent = await blobClient.DownloadContentAsync();

            return fileContent;
        }

        /// <summary>
        /// The GetContentAuthenticationTokenAsync.
        /// </summary>
        /// <param name="encodedAssetId">The encodedAssetId<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public async Task<string> GetContentAuthenticationTokenAsync(string encodedAssetId)
        {
            byte[] tokenSigningKey = Convert.FromBase64String(this.settings.AzureMediaJWTPrimaryKeySecret);

            var keyidentifier = await this.GetContentKeyIdentifier(encodedAssetId);

            return GetJWTToken(this.settings.AzureMediaIssuer, this.settings.AzureMediaAudience, keyidentifier, tokenSigningKey, this.settings.AzureMediaJWTTokenExpiryMinutes);
        }

        /// <summary>
        /// The GetTopLevelManifestForToken.
        /// </summary>
        /// <param name="manifestProxyUrl">The manifestProxyUrl<see cref="string"/>.</param>
        /// <param name="topLeveLManifestUrl">The topLeveLManifestUrl<see cref="string"/>.</param>
        /// <param name="token">The token<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetTopLevelManifestForToken(string manifestProxyUrl, string topLeveLManifestUrl, string token)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            this.logger.LogDebug($"topLeveLManifestUrl={topLeveLManifestUrl}");
            var httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(topLeveLManifestUrl));
            httpRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            httpRequest.Timeout = 30000;

            this.logger.LogDebug($"Calling httpRequest.GetResponse()");
            var httpResponse = httpRequest.GetResponse();

            try
            {
                this.logger.LogDebug($"Calling httpResponse.GetResponseStream()");
                var stream = httpResponse.GetResponseStream();

                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        const string qualityLevelRegex = @"(QualityLevels\(\d+\)/Manifest\(.+\))";

                        var toplevelmanifestcontent = reader.ReadToEnd();

                        var topLevelManifestBaseUrl = topLeveLManifestUrl.Substring(0, topLeveLManifestUrl.IndexOf(".ism", System.StringComparison.OrdinalIgnoreCase)) + ".ism";
                        this.logger.LogDebug($"topLevelManifestBaseUrl={topLevelManifestBaseUrl}");
                        var urlEncodedTopLeveLManifestBaseUrl = HttpUtility.UrlEncode(topLevelManifestBaseUrl);
                        var urlEncodedToken = HttpUtility.UrlEncode(token);

                        var newContent = Regex.Replace(
                            toplevelmanifestcontent,
                            qualityLevelRegex,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "{0}?playBackUrl={1}/$1&token={2}",
                                manifestProxyUrl,
                                urlEncodedTopLeveLManifestBaseUrl,
                                urlEncodedToken));

                        this.logger.LogDebug($"newContent={newContent}");

                        return newContent;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogDebug($"Exception: {ex.Message}");
            }
            finally
            {
                httpResponse.Close();
            }

            return null;
        }

        /// <summary>
        /// Creates the AzureMediaServicesClient object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <returns>.</returns>
        public async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync()
        {
            if (this.azureMediaServicesClient != null)
            {
                return this.azureMediaServicesClient;
            }

            var credentials = await this.GetCredentialsAsync();

            this.azureMediaServicesClient = new AzureMediaServicesClient(this.settings.AzureMediaArmEndpoint, credentials)
            {
                SubscriptionId = this.settings.AzureMediaSubscriptionId,
            };

            return this.azureMediaServicesClient;
        }

        /// <summary>
        /// The GetJWTToken.
        /// </summary>
        /// <param name="issuer">The issuer<see cref="string"/>.</param>
        /// <param name="audience">The audience<see cref="string"/>.</param>
        /// <param name="keyIdentifier">The keyIdentifier<see cref="string"/>.</param>
        /// <param name="tokenVerificationKey">The tokenVerificationKey.</param>
        /// <param name="expiryMinutes">The ExpiryMinutes<see cref="int"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string GetJWTToken(string issuer, string audience, string keyIdentifier, byte[] tokenVerificationKey, int expiryMinutes)
        {
            var tokenSigningKey = new SymmetricSecurityKey(tokenVerificationKey);

            SigningCredentials cred = new SigningCredentials(
                tokenSigningKey,
                SecurityAlgorithms.HmacSha256, // Use the  HmacSha256 and not the HmacSha256Signature option, or the token will not work!
                SecurityAlgorithms.Sha256Digest);

            Claim[] claims = new Claim[]
            {
                new Claim(ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim.ClaimType, keyIdentifier),
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.Now.AddMinutes(-5),
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: cred);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }

        /// <summary>
        /// Get AzureMedia Credentials.
        /// </summary>
        /// <returns>.</returns>
        private async Task<ServiceClientCredentials> GetCredentialsAsync()
        {
            ClientCredential clientCredential = new ClientCredential(this.settings.AzureMediaAadClientId, this.settings.AzureMediaAadSecret);
            return await ApplicationTokenProvider.LoginSilentAsync(this.settings.AzureMediaAadTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure);
        }

        /// <summary>
        /// The GetContentKeyIdentifier.
        /// </summary>
        /// <param name="encodedAssetId">The encodedAssetId<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private async Task<string> GetContentKeyIdentifier(string encodedAssetId)
        {
            var client = await this.CreateMediaServicesClientAsync();

            var streamingLocatorsResponse = await client.Assets.ListStreamingLocatorsAsync(this.settings.AzureMediaResourceGroup, this.settings.AzureMediaAccountName, encodedAssetId);
            var contentKeyResponse = await client.StreamingLocators.ListContentKeysAsync(this.settings.AzureMediaResourceGroup, this.settings.AzureMediaAccountName, streamingLocatorsResponse.StreamingLocators.First().Name);
            string keyIdentifier = contentKeyResponse.ContentKeys.First().Id.ToString();

            return keyIdentifier;
        }
    }
}
