namespace LearningHub.Nhs.AdminUI.Helpers
{
    using LearningHub.Nhs.Models.Moodle;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="TreeBuilderHelper" />.
    /// </summary>
    public static class TreeBuilderHelper
    {
        /// <summary>
        /// BuildSelectListFromCategories.
        /// </summary>
        /// <param name="selectList">The selectList.</param>
        /// <param name="tree">The tree.</param>
        /// <param name="parentId">The parentId.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="instance">The instance.</param>
        public static void BuildTree(
     List<SelectListItem> selectList,
     Dictionary<int, List<MoodleCategory>> tree,
     int parentId,
     int depth,
     string instance)
        {
            if (!tree.ContainsKey(parentId))
                return;

            foreach (var category in tree[parentId])
            {
                string indent = new string('\u00A0', depth * 2);

                selectList.Add(new SelectListItem
                {
                    Value = $"{instance}:{category.Id}",
                    Text = $"{indent}{category.Name}"
                });

                BuildTree(selectList, tree, category.Id, depth + 1, instance);
            }
        }
    }
}
