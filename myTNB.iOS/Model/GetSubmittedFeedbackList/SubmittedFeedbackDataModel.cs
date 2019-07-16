namespace myTNB.Model
{
    public class SubmittedFeedbackDataModel
    {
        private string _feedbackNameInListView = string.Empty;

        public string ServiceReqNo { set; get; }
        public string DateCreated { set; get; }
        public string FeedbackMessage { set; get; }
        public string FeedbackCategoryName { set; get; }
        public string FeedbackCategoryId { set; get; }
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