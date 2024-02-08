namespace LearningHub.Nhs.Migration.Mapping.Helpers
{
    using LearningHub.Nhs.Repository.Interface.Hierarchy;

    /// <summary>
    /// Helper class for looking up a user id from a user name.
    /// </summary>
    public class NodeIdLookup
    {
        private readonly ICatalogueNodeVersionRepository catalogueNodeVersionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeIdLookup"/> class.
        /// </summary>
        /// <param name="catalogueNodeVersionRepository">The catalogueNodeVersionRepository.</param>
        public NodeIdLookup(ICatalogueNodeVersionRepository catalogueNodeVersionRepository)
        {
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
        }

        /// <summary>
        /// Gets the node id for a particular catalogue name.
        /// </summary>
        /// <param name="catalogueName">The catalogue name.</param>
        /// <returns>The user id.</returns>
        public int GetNodeIdByCatalogueName(string catalogueName)
        {
            var nodeId = this.catalogueNodeVersionRepository.GetNodeIdByCatalogueName(catalogueName).Result;
            return nodeId;
        }
    }
}
