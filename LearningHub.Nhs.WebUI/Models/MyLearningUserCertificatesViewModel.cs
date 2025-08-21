namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;
    using System.Reflection;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.WebUI.Models.Learning;

    /// <summary>
    /// Defines the <see cref="MyLearningUserCertificatesViewModel" />.
    /// </summary>
    public class MyLearningUserCertificatesViewModel : MyLearningRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningUserCertificatesViewModel"/> class.
        /// </summary>
        public MyLearningUserCertificatesViewModel()
        {
            this.UserCertificates = new List<UserCertificateViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningUserCertificatesViewModel"/> class.
        /// </summary>
        /// <param name="requestModel">MyLearningRequestModel.</param>
        public MyLearningUserCertificatesViewModel(MyLearningRequestModel requestModel)
        {
            this.UserCertificates = new List<UserCertificateViewModel>();
            foreach (PropertyInfo prop in requestModel.GetType().GetProperties())
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(requestModel, null), null);
            }
        }

        /// <summary>
        /// Gets or sets the learning form event.
        /// </summary>
        public MyLearningFormActionTypeEnum MyLearningFormActionType { get; set; }

        /// <summary>
        /// Gets or sets the page item index.
        /// </summary>
        public int CurrentPageIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the TotalCount.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the Activities.
        /// </summary>
        public List<UserCertificateViewModel> UserCertificates { get; set; }

        /// <summary>
        /// Gets or sets the learning result paging.
        /// </summary>
        public PagingViewModel MyLearningPaging { get; set; }
    }
}
