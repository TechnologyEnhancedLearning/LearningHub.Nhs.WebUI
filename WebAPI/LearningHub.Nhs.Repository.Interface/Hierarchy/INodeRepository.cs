namespace LearningHub.Nhs.Repository.Interface.Hierarchy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;

    /// <summary>
    /// The NodeRepository interface.
    /// </summary>
    public interface INodeRepository : IGenericRepository<Node>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Node> GetByIdAsync(int id);

        /////// <summary>
        /////// Gets the basic details of a node. Currently catalogues or folders.
        /////// </summary>
        /////// <param name="nodeId">The node id.</param>
        /////// <returns>The node details.</returns>
        ////NodeViewModel GetNodeDetails(int nodeId);

        /// <summary>
        /// Gets the contents of a node for the catalogue landing page - i.e. published folders and published resources only.
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="includeEmptyFolder">Include Empty Folder or not.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentBrowseViewModel>> GetNodeContentsForCatalogueBrowse(int nodeId, bool includeEmptyFolder);

        /// <summary>
        /// Gets the contents of a node for the My Contributions page - i.e. published folders only, and all resources (i.e. all statuses).
        /// Only returns the items found directly in the specified node, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentEditorViewModel>> GetNodeContentsForCatalogueEditor(int nodeId);

        /// <summary>
        /// Gets the contents of a node (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeContentAdminViewModel>> GetNodeContentsAdminAsync(int nodeId, bool readOnly);
    }
}
