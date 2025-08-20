namespace LearningHub.Nhs.WebUI.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using elfhHub.Nhs.Models.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// SelectListHelper.
    /// </summary>
    public static class SelectListHelper
    {
        /// <summary>
        /// MapOptionsToSelectListItems.
        /// </summary>
        /// <param name="options">options.</param>
        /// <param name="selectedId">selectedId.</param>
        /// <returns>SelectListItem.</returns>
        public static IEnumerable<SelectListItem> MapOptionsToSelectListItems(IEnumerable<GenericListViewModel> options, int? selectedId = null)
        {
            return options.Select(o => new SelectListItem(o.Name, o.Id.ToString(), o.Id == selectedId)).ToList();
        }

        /// <summary>
        /// MapSelectListWithSelection.
        /// </summary>
        /// <param name="options">options.</param>
        /// <param name="selectedId">selectedId.</param>
        /// <returns>SelectListItem.</returns>
        public static IEnumerable<SelectListItem> MapSelectListWithSelection(IEnumerable<SelectListItem> options, string selectedId = null)
        {
            return options.Select(o => new SelectListItem(o.Text, o.Value, o.Value == selectedId)).ToList();
        }
    }
}
