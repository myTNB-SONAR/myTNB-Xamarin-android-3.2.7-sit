namespace myTNB.Model
{
    public class SubmitFeedbackResponseModel
    {
        public SubmitFeedbackModel d { set; get; }
    }

    public class SubmitFeedbackModel : BaseModelV2
    {
        public SubmitFeedbackDataModel data { set; get; }
    }

    public class SubmitFeedbackDataModel
    {
        public string ServiceReqNo { set; get; } = string.Empty;
        public string DateCreated { set; get; } = string.Empty;
    }
}