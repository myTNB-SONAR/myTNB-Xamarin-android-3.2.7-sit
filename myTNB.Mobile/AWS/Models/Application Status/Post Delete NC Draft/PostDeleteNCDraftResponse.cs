using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models
{
    public class PostDeleteNCDraftResponse : BaseResponse<PostDeleteNCDraftModel>
    {

    }
    public class PostDeleteNCDraftModel
    {
        [JsonProperty("result")]
        public bool Result { set; get; }
    }
}