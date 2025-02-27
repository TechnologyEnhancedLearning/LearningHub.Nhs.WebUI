namespace LearningHub.Nhs.AdminUI.ViewComponents
{
    using LearningHub.Nhs.AdminUI.Services;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewComponents;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionViewComponent"/> class.
    /// </summary>
    public class VersionViewComponent : ViewComponent
    {
        private readonly VersionService versionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionViewComponent"/> class.
        /// </summary>
        /// <param name="versionService">.</param>
        public VersionViewComponent(VersionService versionService)
        {
            this.versionService = versionService;
        }

        /// <summary>
        /// The Invoke.
        /// </summary>
        /// <returns>A representing the result of the synchronous operation.</returns>
        public IViewComponentResult Invoke()
        {
            var version = this.versionService.GetVersion();
            return new HtmlContentViewComponentResult(new HtmlString($"<!-- Version: {version} -->"));
        }
    }
}
