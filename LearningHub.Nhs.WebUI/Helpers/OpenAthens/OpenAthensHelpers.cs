namespace LearningHub.Nhs.WebUI.Helpers.OpenAthens
{
    using System.Linq;
    using LearningHub.Nhs.Models.OpenAthens;

    /// <summary>
    /// Defines the <see cref="OpenAthensHelpers" />.
    /// </summary>
    internal static class OpenAthensHelpers
    {
        /// <summary>
        /// The ExtractOpenAthensProps.
        /// </summary>
        /// <param name="model">The model<see cref="BeginOpenAthensLinkToLearningHubUser"/>.</param>
        /// <param name="payload">The payload<see cref="OpenAthensAuthServerPayload"/>.</param>
        internal static void ExtractOpenAthensProps(this BeginOpenAthensLinkToLearningHubUser model, OpenAthensAuthServerPayload payload)
        {
            if (!payload.Claims.Any())
            {
                return;
            }

            model.OaUserId = payload.Claims["eduPersonTargetedID"];
            model.OaOrganisationId = payload.Claims["eduPersonScopedAffiliation"];
        }
    }
}
