using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class FeedbackQuestionRequestModel : BaseModel
    {
        public FeedbackQuestionRequestModel()
        {
        }
        public FeedbackQuestionRequestModel(BaseModel model) : this()
        {
            isError = model.isError;
            status = model.status;
            message = model.message;
            __type = model.__type;
            StatusCode = model.StatusCode;
        }

        [JsonProperty("data")]
        public List<FeedbackQuestionModel> FeedbackQuestions { get; set; }
    }
}
