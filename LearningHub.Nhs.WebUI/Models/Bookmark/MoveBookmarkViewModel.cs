namespace LearningHub.Nhs.WebUI.Models.Bookmark
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using LearningHub.Nhs.Models.Bookmark;
    using NHSUKFrontendRazor.ViewModels;

    /// <summary>
    /// MoveBookmarkViewModel.
    /// </summary>
    public class MoveBookmarkViewModel
    {
        /// <summary>
        /// Gets or sets the Bookmark.
        /// </summary>
        public UserBookmarkViewModel Bookmark { get; set; }

        /// <summary>
        /// Gets or sets the Folders.
        /// </summary>
        public Dictionary<int, string> Folders { get; set; }

        /// <summary>
        /// Gets or sets the SelectedFolderId.
        /// </summary>
        [Required(ErrorMessage = "You must select a folder to move your bookmark to")]
        public int? SelectedFolderId { get; set; }

        /// <summary>
        /// Gets or sets the new folder name, if SelectedFolderId is 0 (new folder).
        /// </summary>
        [MinLength(2, ErrorMessage = "Folder name must be at least 2 characters")]
        [MaxLength(60, ErrorMessage = "Folder name must be no more than 60 characters")]
        public string NewFolderName { get; set; }

        /// <summary>
        /// sets the list of radio Folder.
        /// </summary>
        /// <returns>The <see cref="List{RadiosItemViewModel}"/>.</returns>
        public List<RadiosItemViewModel> FolderRadio()
        {
            var radio = new List<RadiosItemViewModel>();
            if (this.Bookmark.ParentId.HasValue)
            {
                radio.Add(new RadiosItemViewModel("-1", "Move your bookmark to the top-level", false, null));
            }

            if (!this.Folders.Any())
            {
                return radio;
            }

            radio.AddRange(from k in this.Folders
                           let newRadio = new RadiosItemViewModel(k.Key.ToString(), k.Value, false, null)
                           select newRadio);

            return radio;
        }
    }
}