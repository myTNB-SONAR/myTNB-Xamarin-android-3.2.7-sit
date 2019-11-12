using System.Collections.Generic;

namespace myTNB.Model
{
    public class SubmittedFeedbackResponseModel
    {
        public SubmittedFeedbackModel d { set; get; }
    }

    public class SubmittedFeedbackModel : BaseModelV2
    {
        public List<SubmittedFeedbackDataModel> data { set; get; }
    }

    public class SubmittedFeedbackDataModel
    {
        private string _feedbackNameInListView = string.Empty;

        public string ServiceReqNo { set; get; } = string.Empty;
        public string DateCreated { set; get; } = string.Empty;
        public string FeedbackMessage { set; get; } = string.Empty;
        public string FeedbackCategoryName { set; get; } = string.Empty;
        public string FeedbackCategoryId { set; get; } = string.Empty;
        public string FeedbackNameInListView
        {
            set
            {
                _feedbackNameInListView = value;
            }
            get
            {
                return _feedbackNameInListView ?? string.Empty;
            }
        }
    }
}