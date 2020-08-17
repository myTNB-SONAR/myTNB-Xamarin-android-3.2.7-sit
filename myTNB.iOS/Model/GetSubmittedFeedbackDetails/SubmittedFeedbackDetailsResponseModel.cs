﻿using System.Collections.Generic;

namespace myTNB.Model
{
    public class SubmittedFeedbackDetailsResponseModel
    {
        public SubmittedFeedbackDetailsModel d { set; get; }
    }

    public class SubmittedFeedbackDetailsModel : BaseModelV2
    {
        public SubmittedFeedbackDetailsDataModel data { set; get; }
    }

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

    public class FeedbackImageModel
    {
        public string imageHex { set; get; }
        public string fileName { set; get; }
        public string fileSize { set; get; }
    }
}