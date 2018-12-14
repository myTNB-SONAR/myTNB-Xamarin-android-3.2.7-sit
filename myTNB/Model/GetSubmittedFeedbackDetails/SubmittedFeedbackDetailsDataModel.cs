using System.Collections.Generic;

namespace myTNB.Model
{
    public class SubmittedFeedbackDetailsDataModel
    {
        public string ServiceReqNo { set; get; }
        public string AccountNum { set; get; }
        public string StateId { set; get; }
        public string StateName { set; get; }
        public string Location { set; get; }
        public string PoleNum { set; get; }
        public string FeedbackTypeId { set; get; }
        public string FeedbackTypeName { set; get; }
        public string DateCreated { set; get; }
        public string StatusCode { set; get; }
        public string StatusDesc { set; get; }
        public List<FeedbackImageModel> FeedbackImage { set; get; }
        //Custom
        public string FeedbackCategoryId { set; get; }
        public string FeedbackMessage { set; get; }
        public string FeedbackCategoryName { set; get; }
    }
}
