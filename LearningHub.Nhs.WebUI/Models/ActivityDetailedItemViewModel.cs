namespace LearningHub.Nhs.WebUI.Models
{
    using System.Reflection;
    using LearningHub.Nhs.Models.MyLearning;

    /// <summary>
    /// Defines the <see cref="ActivityDetailedItemViewModel" />.
    /// </summary>
    public class ActivityDetailedItemViewModel : MyLearningDetailedItemViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDetailedItemViewModel"/> class.
        /// </summary>
        public ActivityDetailedItemViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDetailedItemViewModel"/> class.
        /// </summary>
        /// <param name="myLearningDetailedItemViewModel">myLearningDetailedItemViewModel.</param>
        public ActivityDetailedItemViewModel(MyLearningDetailedItemViewModel myLearningDetailedItemViewModel)
        {
            this.MyLearningDetailedItemViewModel = myLearningDetailedItemViewModel;

            foreach (PropertyInfo prop in myLearningDetailedItemViewModel.GetType().GetProperties())
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(myLearningDetailedItemViewModel, null), null);
            }
        }

        /// <summary>
        /// Gets or sets the MyLearningDetailedItemViewModel.
        /// </summary>
        public MyLearningDetailedItemViewModel MyLearningDetailedItemViewModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsMostRecent.
        /// </summary>
        public bool IsMostRecent { get; set; }

        /// <summary>
        /// Gets or sets the Score.
        /// </summary>
        public int Score { get; set; }
    }
}
