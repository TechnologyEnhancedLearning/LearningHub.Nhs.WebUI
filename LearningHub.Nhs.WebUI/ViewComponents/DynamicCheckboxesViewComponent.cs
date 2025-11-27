namespace LearningHub.Nhs.WebUI.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.WebUI.Models.DynamicCheckbox;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="DynamicCheckboxesViewComponent" />.
    /// </summary>
    public class DynamicCheckboxesViewComponent : ViewComponent
    {
        /// <summary>
        /// The Invoke.
        /// </summary>
        /// <param name="label">label.</param>
        /// <param name="checkboxes">checkboxes.</param>
        /// <param name="required">required.</param>
        /// <param name="errorMessage">errorMessage.</param>
        /// <param name="hintText">hintText.</param>
        /// <param name="cssClass">cssClass.</param>
        /// <param name="selectedValues">selectedValues.</param>
        /// <param name="propertyName">propertyName.</param>
        /// <returns>A representing the result of the synchronous operation.</returns>
        public IViewComponentResult Invoke(
            string label,
            IEnumerable<DynamicCheckboxItemViewModel> checkboxes,
            bool required = false,
            string? errorMessage = null,
            string? hintText = null,
            string? cssClass = null,
            IEnumerable<string>? selectedValues = null,
            string propertyName = "SelectedValues")
        {
            var selectedList = selectedValues?.ToList() ?? new List<string>();

            var viewModel = new DynamicCheckboxesViewModel
            {
                Label = label,
                HintText = string.IsNullOrWhiteSpace(hintText) ? null : hintText,
                ErrorMessage = errorMessage,
                Required = required,
                CssClass = string.IsNullOrWhiteSpace(cssClass) ? null : cssClass,
                SelectedValues = selectedList,
                Checkboxes = checkboxes.Select(cb => new DynamicCheckboxItemViewModel
                {
                    Value = cb.Value,
                    Label = cb.Label,
                    HintText = cb.HintText,
                    Selected = selectedList.Contains(cb.Value),
                }).ToList(),
            };

            this.ViewData["PropertyName"] = propertyName;
            return this.View(viewModel);
        }
    }
}
