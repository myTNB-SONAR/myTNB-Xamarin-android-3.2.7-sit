using System.Collections.Generic;

namespace myTNB.Model
{
    public class SubmittedFeedbackModel : BaseModel
    {
        public List<SubmittedFeedbackDataModel> data { set; get; }
    }
}